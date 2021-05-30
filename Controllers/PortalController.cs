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
            startDate = startDate.AddDays(-40).Date;
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



        private Models.Security.User GetUserFromSession()
        {
            return (Models.Security.User)Session["User"];
        }

    }
}