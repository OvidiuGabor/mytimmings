using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace mytimmings.Controllers
{
    public class PortalController : Controller
    {
        private readonly DBContext.DBModel db = new DBContext.DBModel();
        // GET: Portal
        public ActionResult Overview()
        {
            //as default we get the data for the last week from the database
            //starting monday, untill today.
            Models.Security.User user = (Models.Security.User)Session["User"];
            Models.Security.AuthState userState = (Models.Security.AuthState)Session["AuthState"];
            if(user == null|| userState == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Models.Portal.TimeTracker timeTracker = new Models.Portal.TimeTracker(userState.UserSettings);
            List<Models.Portal.TimeTracker> DailyLoginsList = new List<Models.Portal.TimeTracker>();//create an empty daily login list
            DateTime currentDate = DateTime.UtcNow; //get the currentdate
            DateTime previousDay = DateTime.UtcNow.AddDays(-1); //set the previous day also as we need it for the login query
            DateTime startDate = DateTime.UtcNow; //get the currentdate

            int dayOfWeek = (int)DateTime.Today.DayOfWeek; //get the current day of the week (Tueday = 2, thursday = 3...)

            if (dayOfWeek > 1 )
                startDate = startDate.AddDays(-(dayOfWeek -1));
            if (dayOfWeek == 0)
                startDate = startDate.AddDays(-6);


            //test
            //startDate = startDate.AddDays(-40).Date;
            //Get the records from the database
            List<DBContext.Main_Data> dbRecords = db.Main_Data.Where(x => x.userID == user.ID && x.CurrentDate.CompareTo(startDate) >= 0).ToList();
            //List of records
            List<Models.Portal.Action> statusList = Models.Portal.Action.CreateFromDbList(dbRecords);


            //Create the timetracker Object
            //get the record from the Login table, if exists
            #region CurrentTimeTracker
            var loginDbRecord = db.User_Login_Logout.Where(x => x.UserId == user.ID && ((x.Date.Value.Year == currentDate.Year &&
                                x.Date.Value.Month == currentDate.Month && x.Date.Value.Day == currentDate.Day) ||
                                (x.Date.Value.Year == previousDay.Year && x.Date.Value.Month == previousDay.Month &&
                                x.Date.Value.Day == previousDay.Day && x.LogoutTime.HasValue == false))).FirstOrDefault();

            if (loginDbRecord != null)
                timeTracker = new Models.Portal.TimeTracker(loginDbRecord, userState, user);
            #endregion

            //Get a list of all the daily loginsfor the x days
            #region loginsList
            var dailyLoginsDb = db.User_Login_Logout.Where(x => x.UserId == user.ID && x.Date.Value.CompareTo(startDate) >= 0).OrderByDescending(x =>x.LoginTime);

            foreach(var item in dailyLoginsDb)
            {
                DailyLoginsList.Add(new Models.Portal.TimeTracker(item));
            }
            #endregion

            //Get used Leaves days.
            #region calculate the leave stats
            var leavesDb = db.Leaves.Where(x => x.UserId == user.ID && x.StartDate.Value.Year == currentDate.Year)
                .GroupBy(x => x.Type).Select(cl => new
                {
                    type = cl.FirstOrDefault().Type,
                    total = cl.Sum(x => x.TotalDays)

                }).ToList();
            //need to loop through each item then omapre with the user entitlement and then calculate the remaining.
            List<Models.Portal.LeaveStatus> leaveStatus = new List<Models.Portal.LeaveStatus>();
            var userLeaveStatus = db.User_Leaves_Status.Where(x => x.UserId == user.ID && x.Year == currentDate.Year).ToList();
            foreach(var item in leavesDb)
            {
                //get the total number of days for the current year for the
                var currentLeaveStatus = userLeaveStatus.Where(x => x.Leave_type == item.type).FirstOrDefault();
                if(currentLeaveStatus != null)
                {
                    int remainingDays = (int)((currentLeaveStatus.Entitled + currentLeaveStatus.Carried_Over) - item.total);
                    leaveStatus.Add(new Models.Portal.LeaveStatus(item.type, remainingDays));

                }
                    

            }
            leaveStatus = leaveStatus.OrderBy(x => x.DaysRemaining).ToList();
            #endregion

            var overviewModel = new Models.Portal.Overview(statusList, timeTracker, DailyLoginsList,leaveStatus, user);


            return View(overviewModel);
        }
        public ActionResult CheckIn(Models.Portal.Action data)
        {
            //Get the date in UTC Format;
            DateTime CurrentTime = DateTime.UtcNow;
            DateTime PreviousTime = DateTime.UtcNow.AddDays(-1);

            //get user from the sessiopn
            Models.Security.User user = GetUserFromSession();
            if (user != null)
            {
                //IF the user didnt select any status and any project then we create a record in login table and another one in main data with Available as default
                //check if the user has  clock out already
                var clockedOut = db.User_Login_Logout.Where(x => x.UserId == user.ID && x.LoginTime.Value > PreviousTime && x.LogoutTime != null).OrderByDescending(x => x.LogoutTime).FirstOrDefault();
                if (clockedOut != null)
                    return Json(new { result = false, message = "Already Clocked Out for today!" }, JsonRequestBehavior.AllowGet);
               
                //check if the user has  clock In already
                var clockedIn = db.User_Login_Logout.Where(x => x.UserId == user.ID && x.LoginTime.Value > PreviousTime && x.LogoutTime == null).OrderByDescending(x => x.LogoutTime).FirstOrDefault();
                if (clockedOut != null)
                    return Json(new { result = false, message = "Already Clocked In for today!" }, JsonRequestBehavior.AllowGet);

                //Create a login object
                DBContext.User_Login_Logout login = new DBContext.User_Login_Logout
                {
                    UserId = user.ID,
                    Date = CurrentTime,
                    LoginTime = CurrentTime,
                };
                db.User_Login_Logout.Add(login);


                //create a main data record with projectID as 0 and Status as Available as default!;
                DBContext.Main_Data mainData = new DBContext.Main_Data
                {
                    userID = user.ID,
                    CurrentDate = CurrentTime,
                    Status_Start_Time = CurrentTime,
                    ProjectID = 0,
                    Current_Status = "Available",
                    Comments = data.Comment
                    
                };


                if (data.ProjectId >= 0)
                    mainData.ProjectID = data.ProjectId;
                
                if (data.Type != "0")
                    mainData.Current_Status = data.Type;


                //if(data.Type != "0" && data.ProjectId ==0)
                //    return Json(new { result = false, message = "Need to select a project!" }, JsonRequestBehavior.AllowGet);

                try
                {
                    db.Main_Data.Add(mainData);
                    db.SaveChanges();
                    
                    return Json(new { result = true, message = "Clocked In!" , location = Url.Action("Overview", "Portal")}, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    //Log the exception

                    return Json(new { result = false, message = "Something happen! Please try again!" }, JsonRequestBehavior.AllowGet);
                }


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            
        }

        public ActionResult UpdateStatus(Models.Portal.Action data)
        {
            //Get the date in UTC Format;
            DateTime CurrentTime = DateTime.UtcNow;
            DateTime PreviousTime = DateTime.UtcNow.AddDays(-1);

            Models.Security.User user = GetUserFromSession();
            if (user != null)
            {
                if (data.Type == "0")
                    return Json(new { result = false, message = "Please select a status!" }, JsonRequestBehavior.AllowGet);

                if(data.ProjectId == 0)
                    return Json(new { result = false, message = "Please select a Project!" }, JsonRequestBehavior.AllowGet);



                //check if the user has clock in and if not clock out for today
                var clockedOut = db.User_Login_Logout.Where(x => x.UserId == user.ID && x.LoginTime.Value > PreviousTime && x.LogoutTime != null).OrderByDescending(x => x.LoginTime).FirstOrDefault();
                if (clockedOut != null)
                    return Json(new { result = false, message = "Already Clocked, cannot change status!" }, JsonRequestBehavior.AllowGet);


                var clockedIn = db.User_Login_Logout.Where(x => x.UserId == user.ID && x.LoginTime.Value > PreviousTime && x.LogoutTime == null).OrderByDescending(x => x.LoginTime).FirstOrDefault();
                if (clockedIn == null)
                    return Json(new { result = false, message = "Please Clock In First!" }, JsonRequestBehavior.AllowGet);


                //get the last records and set the EndStatus to now
                var addFinishTimelst = db.Main_Data.Where(x => x.userID == user.ID && x.Status_End_Time == null).OrderByDescending(x => x.Status_Start_Time).FirstOrDefault();
                if(addFinishTimelst != null)
                {
                    try
                    {
                        addFinishTimelst.Status_End_Time = CurrentTime;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        //Log the exception

                        return Json(new { result = false, message = "Something happen! Please try again!" }, JsonRequestBehavior.AllowGet);
                    }
                   
                }



                //create a main data record with projectID as 0 and Status as Available as default!;
                DBContext.Main_Data mainData = new DBContext.Main_Data
                {
                    userID = user.ID,
                    CurrentDate = CurrentTime,
                    Status_Start_Time = CurrentTime,
                    ProjectID = data.ProjectId,
                    Current_Status = data.Type,
                    Comments = Utilities.Helper.StripDangerousCharacters(data.Comment)

                };

                try
                {
                    db.Main_Data.Add(mainData);
                    db.SaveChanges();
                    return Json(new { result = true, message = "Status Updated!", newobj = mainData }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    //Log the exception

                    return Json(new { result = false, message = "Something happen! Please try again!" }, JsonRequestBehavior.AllowGet);
                }


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

          
        }

        public ActionResult ClockOut()
        {

            DateTime CurrentTime = DateTime.UtcNow;
            DateTime PreviousTime = DateTime.UtcNow.AddDays(-1);

            Models.Security.User user = GetUserFromSession();
            if (user != null)
            {
                //get the last Login Record
                var checkRecord = db.User_Login_Logout.Where(x => x.UserId == user.ID && x.LoginTime > PreviousTime && x.LogoutTime != null).OrderByDescending(x => x.LogoutTime).FirstOrDefault();
                if(checkRecord != null){
                    return Json(new { result = false, message = "Already Clock Out Today!" }, JsonRequestBehavior.AllowGet);
                }

                var loginRecord = db.User_Login_Logout.Where(x => x.UserId == user.ID && x.LoginTime > PreviousTime && x.LogoutTime == null).OrderByDescending(x => x.LoginTime).FirstOrDefault();
                if(loginRecord != null)
                {
                    loginRecord.LogoutTime = CurrentTime;
                }
                else
                {
                    return Json(new { result = false, message = "You didn't Clock In for today!" }, JsonRequestBehavior.AllowGet);
                }

                //get the last records and set the EndStatus to now
                var addFinishTimelst = db.Main_Data.Where(x => x.userID == user.ID && x.Status_End_Time == null).OrderByDescending(x => x.Status_Start_Time).FirstOrDefault();
                if (addFinishTimelst != null)
                {
                    addFinishTimelst.Status_End_Time = CurrentTime;

                }


                try
                {
                    db.SaveChanges();
                    //return Json(new { result = true, message = "Clock Out Successfully!" }, JsonRequestBehavior.AllowGet);
                    return Json(new { result = true, message = "Clocked Out!", location = Url.Action("Overview", "Portal") }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    //Log the exception

                    return Json(new { result = false, message = "Something happen! Please try again!" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        #region Partial Requests

        public ActionResult OvertimeRequest(Models.Portal.Partial_Request.OvertimeRequest req)
        {
            //when a request is send
            //1. add a notification to the liveNotification table where we inform the manager that a new overtime request has been sent for appoval if the request is set for ManaualApprove
            //2. if the request is set for AutoApprove, then we just inform the manager that someone has requested for Overtime.

            //get ther user details from session
            var user = GetUserFromSession();
            if (user == null)
                return RedirectToAction("Overview");

            //add the current user as the User who made the request.
            req.AddUser(user);


            //Perform some checkes about the notification.
            //1. The request cannot be in the past;
            //2. The Request cannot be during shift time, so need to check for the starting time of the shift, then calculate the end time.
            //3. End time of the request cannot be higher then 4 hours per day. Maybe in here we can add some seetings on how much time an overtime can have.
            //4. Check for the duration to be higher then 0 minutes
            //5. check for the project, to be different then 0
            //6. check for the start time to be higher then 0
            DateTime currentTime = DateTime.UtcNow;

            //check for the start and end time
            if(DateTime.Compare(req.StartDate , req.StartDate.Add(req.Duration)) > 0)
            {
                return Json(new { result = false, message = "Start time cannot be later then End Time!" }, JsonRequestBehavior.AllowGet);
            }
            //check duration
            if(req.Duration.Ticks == 0)
            {
                return Json(new { result = false, message = "Duration need to be higher then 0!" }, JsonRequestBehavior.AllowGet);
            }
            if (req.StartTime.Ticks == 0)
            {
                return Json(new { result = false, message = "Start Time need to be higher then 0!" }, JsonRequestBehavior.AllowGet);
            }
            if (req.ProjectId == 0)
            {
                return Json(new { result = false, message = "Need to select a project!" }, JsonRequestBehavior.AllowGet);
            }
            //if the user didnt select a date, we cab take the date from the time, as it is basically the same!
            //Technically in the view we should check for this situation and not allow a empty date field
            if (req.StartDate.Ticks == 0)
                req.StartDate = req.StartTime;



            //Check if there is another request raised for the specific period and tell the user that cannot raise a new one.
            //if the request is already approve, need to contact the manager to request it
            //if the request is not approved you can cancel it, and then request a new one




            //check for the start time
            if (DateTime.Compare(req.StartDate.Date, currentTime.Date) < 0)
            {
                return Json(new { result = false, message = "Cannot add Overtime for a past day!" }, JsonRequestBehavior.AllowGet);
            }
            else if(DateTime.Compare(req.StartDate.Date, currentTime.Date) == 0)//current day
            {
                //check for the shift time.
                var shiftStart = db.User_Login_Logout.Where(x => x.UserId == user.ID && x.LoginTime.Value.Year == currentTime.Year && x.LoginTime.Value.Month == currentTime.Month && x.LoginTime.Value.Day == currentTime.Day).FirstOrDefault();
                if(shiftStart != null)
                {
                    var userSettings = db.User_Settings.Find(user.ID);
                    if(userSettings != null)
                    {
                        var supposeEnd = shiftStart.LoginTime.Value.Add(userSettings.ShiftTime.Value);
                        if (DateTime.Compare(req.StartDate, supposeEnd) < 0)
                        {
                            return Json(new { result = false, message = "Cannot request Overtime during shift period!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { result = false, message = "There has been an error. Contact the Administrator!" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            //in the future
            else
            {

            }


          



            //Get the Approve status from the compnay settings
            var companySettings = db.Companies_Assigned_Items.Where(x => x.Company_ID == user.Company).FirstOrDefault();
            if(companySettings == null)
            {
                //log the error
                //send error to the Dev team
                return Json(new { result = false, message = "There has been an error when processing your request!" }, JsonRequestBehavior.AllowGet);
            }

            List<string> AutoApproveLst = Utilities.Helper.convertStringtoList(companySettings.AutoApprove, ';');
            List<string> ManualApprove = Utilities.Helper.convertStringtoList(companySettings.ManualApprove, ';');

            if (AutoApproveLst.Contains(req.Type)) //if the request type is set to manual
            {
                //send notif tot the manager saying that the request has been autoApprove
                //autoApprove the request
                //since the request is set to autoApprove we can update directly the mai_data table with the request.
                //Add also a record into the partial Time Request table with the status of approved!
                try
                {
                    Approve(req);
                    //Create the notitication and send it to the Manager.
                    string title = req.Type + " request!";
                    string message = user.FirstName + " " + user.LastName + " has submited an " + req.Type + " request for the date of " + req.StartDate.ToString("dd-MM-yyyy") + " with the starting time at " + req.StartTime.ToString("HH:mm") +
                    " and a total duration of " + req.Duration + ". This request is on pending state as of now. Please take an action from below and resolve it.";

                    //Add record into the partial Tine Requests

                    //Add record into the main data


                    Hubs.LiveNotification.InsertNotification(user.ManagerID, DateTime.Now, title, message, user.ID, "Approve");

                }
                catch (Exception ex)
                {

                    //log the error
                    return Json(new { result = false, message = "There has been an error when processing your request!" }, JsonRequestBehavior.AllowGet);
                }
              
            }
            else if (ManualApprove.Contains(req.Type)) //if the request type is set to auto
            {

                //Add record into the PartialTime_request db




                //send notification to the manager that there is a pending request
                string title = req.Type + " request pending for Approval!";
                
                string message = user.FirstName + " " + user.LastName + " has submited an " + req.Type + " request for the date of " + req.StartDate.ToString("dd-MM-yyyy") + " with the starting time at " + req.StartTime.ToString("HH:mm") +
                        " and a total duration of " + req.Duration + ". This request has been Auto Approved according to the process in place.";

                Hubs.LiveNotification.InsertNotification(user.ManagerID, DateTime.Now, title, message, user.ID, "Info");
            }
            else
            {
                //log the error
                //send error to the Dev team
                return Json(new { result = false, message = "There has been an error when processing your request!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = true, message = "Request has been submited!" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult ApproveRequest(string id)
        {



            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeclineReqeust(string id)
        {



            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        private void Approve(Models.Portal.Partial_Request.IPartialRequest request)
        {

            if (request == null)
            {
                //log the request
                throw new ArgumentNullException("The requst argument cannot be null.");
            }

            request.AddRequest();
            
        }
        //Approve by ID
        private void Approve(string id)
        {

        }

        private void Decline(Models.Portal.Partial_Request.IPartialRequest request)
        {
          
            if(request == null)
            {
                //log the request
                throw new ArgumentNullException("The requst argument cannot be null.");
            }

            request.DeleteRequest();
            
        }


        #region SignalR


        public JsonResult GeLiveNotification()
        {
            Models.Security.User user = GetUserFromSession();

            List<Hubs.LiveNotification> notif = new List<Hubs.LiveNotification>();
            var liveNotif = new Hubs.LiveNotification(user.ID);
            notif = liveNotif.GetOpenNotificationByID();

            return Json(notif, JsonRequestBehavior.AllowGet);

        }
       
        #endregion


        private Models.Security.User GetUserFromSession()
        {
            return (Models.Security.User)Session["User"];
        }

    }
}