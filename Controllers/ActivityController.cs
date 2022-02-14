using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mytimmings.Controllers
{
    
    public class ActivityController : Controller
    {
        // GET: Activity
        private readonly DBContext.DBModel db = new DBContext.DBModel();
        [HttpGet]
        public ActionResult MyTimeSheet()
        {
            Models.Security.User user = (Models.Security.User)Session["User"];
            Models.Security.AuthState userState = (Models.Security.AuthState)Session["AuthState"];

            if (user == null || userState == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var worklogsDb = db.Main_Data.Where(x => x.User_ID == user.ID).OrderByDescending(x => x.Status_Start_Time).Take(100);

            List<Models.Portal.WorkRecord> workLogs = new List<Models.Portal.WorkRecord>();

            foreach(var worklog in worklogsDb)
            {
                var newWorkLog = new Models.Portal.WorkRecord(worklog);
                workLogs.Add(newWorkLog);
            }
            //Get today date in UTC Format
            DateTime todayDate = DateTime.UtcNow.Date;

            //Get the public holidays
            var publicHolidaysDb = db.Public_Holidays.Where(x => x.Holiday_country == user.Country && x.Holiday_date >= todayDate.Date).ToList();
            List<Models.Portal.PublicHoliday> publicHolidays = new List<Models.Portal.PublicHoliday>();
            DateTime[] publicHolidaysAsArray = new DateTime[publicHolidaysDb.Count];
            int counter = 0;
            foreach (var holiday in publicHolidaysDb)
            {
                publicHolidaysAsArray[counter] = holiday.Holiday_date.Value.Date;
                publicHolidays.Add(new Models.Portal.PublicHoliday(holiday));
                counter++;
            }


            List<DBContext.Leave> thisYearleaves = db.Leaves.Where(x => x.StartDate.Value.Year == todayDate.Year && x.UserId == user.ID && x.EndDate.Value.Year == todayDate.Year).ToList();
            List<Models.Portal.Leave.ILeave> leaves = new List<Models.Portal.Leave.ILeave>();
            foreach (var leave in thisYearleaves)
            {
                var newleave = new Models.Portal.Leave.Leave(leave, publicHolidaysAsArray);
                leaves.Add(newleave);
            }


            //Get the requested leaves
            List<DBContext.Leave_Requests> requestedLeavesDb = db.Leave_Requests.Where(x => x.Requestor == user.ID && x.RequestStartDate.Value >= todayDate.Date).ToList();
            foreach (var leave in requestedLeavesDb)
            {
                var newLeave = new Models.Portal.Leave.LeaveRequest(leave, publicHolidaysAsArray);
                leaves.Add(newLeave);
            }

            //Get the requested leaves
            List<DBContext.Leaves_Saved_Not_Submited> savedLeavesDb = db.Leaves_Saved_Not_Submited.Where(x => x.UserID == user.ID && x.StartDate.Value >= todayDate.Date).ToList();
            foreach (var leave in savedLeavesDb)
            {
                var newLeave = new Models.Portal.Leave.SavedLeave(leave, publicHolidaysAsArray);
                leaves.Add(newLeave);
            }



            //get a list of projects that the user is assign to
            var projectsDb = db.User_Assigned_Projects.Where(x => x.UserID == user.ID).ToList();
            List<Models.Portal.Project> projectsAssignedToUser = new List<Models.Portal.Project>();
            //add one item as default All projects with the value 0
            projectsAssignedToUser.Add(new Models.Portal.Project(0, "All projects", user.ID));

            foreach (var project in projectsDb)
            {
                projectsAssignedToUser.Add(
                    new Models.Portal.Project(project)
                    );
            }
            


            //Implement Dependency injection
            //Mostlikly is not needed
            var cb = new ContainerBuilder();
            //cb.RegisterType<List<Models.Portal.WorkRecord>>();
            //cb.RegisterType<List<Models.Portal.Leave.ILeave>>();

            cb.RegisterInstance(workLogs).As<List<Models.Portal.WorkRecord>>();
            cb.RegisterInstance(leaves).As<List<Models.Portal.Leave.ILeave>>();
            cb.RegisterInstance(projectsAssignedToUser).As<List<Models.Portal.Project>>();

            cb.RegisterType<Models.Activity.MyActivity>();

            var builder = cb.Build();
            var myActivity = builder.Resolve<Models.Activity.MyActivity>();


            //Models.Activity.MyActivity myActivity = new Models.Activity.MyActivity();
            //myActivity.workLogs = workLogs;
            //myActivity.leaves = leaves;

            //Add values to session for further use when the user change the number of records to show
            SetMyStatSession(myActivity);
            Session.Timeout = 120;

            return View(myActivity);
        }

        
        public JsonResult TimeSheet(string queryItems)
        {
            if (String.IsNullOrEmpty(queryItems))
            {
                return Json(new { status = "error", message = "No query data provided!" }, JsonRequestBehavior.AllowGet);
            }

            //get the myActivity from the Session
            var myActivity = getMyActivityFromSession();
            Models.Activity.MyActivity tempData = new Models.Activity.MyActivity();
            tempData = (Models.Activity.MyActivity)myActivity.Clone();

            const int numberOfItems = 5;
            var custom = new
            {
                selectedItems = new List<string>(),
                numberOfItems = ""

            };
            var convertedJson = custom;
            //Deserialze de json accordin to the custom settings!
            try
            {
                convertedJson = JsonConvert.DeserializeAnonymousType(queryItems, custom);
            }
            catch (Exception ex)
            {
                //log the exception
                return Json(new { status = "error", message = "Wrong Json provided in query. Contact System Administrator!" }, JsonRequestBehavior.AllowGet);
            }

            //Set a default if no value provided in the Json
            if (String.IsNullOrEmpty(convertedJson.numberOfItems))
                tempData.numberOfItems = numberOfItems;
            else
                tempData.numberOfItems = Int32.Parse(convertedJson.numberOfItems);


            if (!convertedJson.selectedItems.Contains("0"))
            {
                List<Models.Portal.WorkRecord> workLogsFiltered = new List<Models.Portal.WorkRecord>();

                foreach(var projectId in convertedJson.selectedItems)
                {
                    int projectIdAsInt = Int32.Parse(projectId);
                    workLogsFiltered.AddRange(tempData.workLogs.Where(x => x.projectId == projectIdAsInt).ToList());
                }
                tempData.workLogs = workLogsFiltered.OrderByDescending(x => x.startDate).ToList();
            }



            IncreaseSessionTimout();
            return Json(new { status = "success", data = tempData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddTask(FormCollection formCollection)
        {
            Dictionary<string, string> erroredItems = new Dictionary<string, string>();
            bool errored = false;
            Models.Portal.WorkRecord record = new Models.Portal.WorkRecord();
            Models.Portal.WorkRecord insertedRecord = null;
            Models.Security.User user = GetUserFromSession();


            //get the records fromt the session
            var myActivityFromSession = getMyActivityFromSession();
            var records = myActivityFromSession.workLogs;

            //get data from the formCollection
            var recordDateFm = formCollection["currentDate"]; //2022/02/09
            var startTimeFm = formCollection["startDate"]; //20:00
            var endTimeFm = formCollection["endDate"]; //21:00
            var statusFm = formCollection["status"];
            var projectIdFm = formCollection["projectId"];
            var descriptionFm = formCollection["description"];
            var titleFm = formCollection["title"];
            var tagsFm = formCollection["tags"];
            var tagsColorFm = formCollection["tagsColors"];

            DateTime recordDate = DateTime.UtcNow;
            DateTime startTime = DateTime.UtcNow;
            DateTime endTime = DateTime.UtcNow;
            string tags = null;
            string tagsColor = null;


            //validate Form Data
            //Validate recordDateFm
            if (!String.IsNullOrEmpty(recordDateFm))
            {
                string[] dateFormats = new[] { "yyyy/MM/dd", "yyyy-MM-dd" };
                CultureInfo provider = new CultureInfo("en-US");
                var success = DateTime.TryParseExact(recordDateFm, dateFormats, provider, DateTimeStyles.AssumeUniversal, out recordDate);
                if (!success)
                {
                    errored = true;
                    erroredItems.Add("currentDate", "Task Date is not valid!");
                }
            }
            else
            {
                errored = true;
                erroredItems.Add("currentDate", "Task Date is not valid!");
            }

            //validate startTimeFm
            if(!String.IsNullOrEmpty(startTimeFm))
            {
                string[] dateFormats = new[] { "HH:mm", "HH:mm:ss" };
                CultureInfo provider = new CultureInfo("en-US");
                try
                {
                    var tempStartTime = DateTime.ParseExact(startTimeFm, "HH:mm", CultureInfo.InvariantCulture);
                    startTime = (recordDate.Date + tempStartTime.TimeOfDay).ToUniversalTime();
                }
                catch (Exception ex)
                {

                    errored = true;
                    erroredItems.Add("startDate", "Start Time is not valid!");
                }
              
            }
            else
            {
                errored = true;
                erroredItems.Add("startDate", "Start Time is not valid!");
            }

            //validate endTimeFm
            if (!String.IsNullOrEmpty(endTimeFm))
            {
                string[] dateFormats = new[] { "HH:mm", "HH:mm:ss" };
                CultureInfo provider = new CultureInfo("en-US");
                try
                {
                   var  tempEndTime = DateTime.ParseExact(endTimeFm, "HH:mm", CultureInfo.InvariantCulture);
                    endTime = (recordDate.Date + tempEndTime.TimeOfDay).ToUniversalTime();
                }
                catch (Exception ex)
                {

                    errored = true;
                    erroredItems.Add("endDate", "End Time is not valid!");
                }

            }
            else
            {
                errored = true;
                erroredItems.Add("endDate", "End Time is not valid!");
            }

            //Validate Status
            if (String.IsNullOrEmpty(statusFm))
            {
                errored = true;
                erroredItems.Add("status", "Task Status is not valid!");
            }
            //Validate project Id
            if (String.IsNullOrEmpty(projectIdFm))
            {
                errored = true;
                erroredItems.Add("projectId", "Selected Project is not valid!");
            }
            //Validate Description
            if (String.IsNullOrEmpty(descriptionFm))
            {
                errored = true;
                erroredItems.Add("description", "Please add a short description!");
            }
            //Validate Title
            if (String.IsNullOrEmpty(titleFm))
            {
                errored = true;
                erroredItems.Add("title", "Please add a title to the task!");
            }

            if (!string.IsNullOrEmpty(tagsFm))
            {
                tags = tagsFm.Replace(',', ' ');
            }
            if (!string.IsNullOrEmpty(tagsColorFm))
            {
                tagsColor = tagsColorFm.Replace("),", ");").Replace("0.1)", "1)").Replace(" ", "");
            }


            record = new Models.Portal.WorkRecord(recordDate, startTime, endTime, statusFm, Int32.Parse(projectIdFm), titleFm, descriptionFm, tags, tagsColor);
            int position = 1;
            Dictionary<string,string> validationErrors = CheckTaskForPotentialIssues(record, user);
         
            if (!errored)
            {
                if (validationErrors.Count > 0)
                {
                    foreach (var error in validationErrors)
                    {
                        erroredItems.Add(error.Key, error.Value);
                    }
                    return Json(new { status = "failed", errors = erroredItems }, JsonRequestBehavior.AllowGet);
                }

                try
                {
                   
                    //here we get the ID of the new record inserted, in  order to get the record from the database
                    //we need to do this because we need the date in UTC format
                    //and cannot figure a way to do it without extracting the new record from the database
                    //apperently when you return the JSOn with a datetime that is createad in the controller it gets the date in pc timezone, and we need it in UTC Time.
                    int id =  record.InsertRecordIntoDb(user);
                    insertedRecord = new Models.Portal.WorkRecord(db.Main_Data.Find(id));
                    
                    //checking the position of the new task so that we can place correctly in the view.
                    foreach(var item in records)
                    {
                        if (item.startDate < insertedRecord.startDate)
                        {
                            records.Insert(position-1, insertedRecord);
                            myActivityFromSession.workLogs = records;
                            SetMyStatSession(myActivityFromSession);
                            break;
                        }
                            

                        position++;
                    }


                }
                catch (Exception ex)
                {
                    //log exception
                    erroredItems.Add("Insert", "There is an issue with the data. Try again or contact Administrator!");
                    return Json(new { status = "failed", errors = erroredItems }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = "failed", errors = erroredItems }, JsonRequestBehavior.AllowGet);
            }




            return Json(new { status = "success", recordAdded = insertedRecord, position = position }, JsonRequestBehavior.AllowGet);

        }




        public JsonResult GetColors()
        {
            List<string> colors = new List<string>();

            var colorsFromDb = db.Params.Where(x => x.Identifier == "Colors" && x.Active == true).FirstOrDefault();
            if(colorsFromDb != null)
            {
                var tempColors = colorsFromDb.Param1.Split(';').ToList();
                if(tempColors.Count == 0)
                    return Json(new { status = "error", data = colors }, JsonRequestBehavior.AllowGet);

                colors.AddRange(tempColors);
            }



            return Json(new { status = "success", data = colors }, JsonRequestBehavior.AllowGet);
        }






        private Dictionary<string,string> CheckTaskForPotentialIssues(Models.Portal.WorkRecord task, Models.Security.User user)
        {
            Dictionary<string, string> issues = new Dictionary<string, string>();
            DBContext.DBModel db = new DBContext.DBModel();

            if (user == null)
                user = GetUserFromSession();



            if(task == null)
            {
                issues.Add("Missing Data","The Task was not submited successfully");
            }
            else
            {

                //Check that the start time should not be  grater then the end time
                if (task.startDate > task.endDate)
                    issues.Add("Invalid Time","Start time cannot be higher then end time!");

                //Check if it is Sunday or Saturday
                if(task.currentDate.DayOfWeek == DayOfWeek.Saturday || task.currentDate.DayOfWeek == DayOfWeek.Sunday)
                    issues.Add("Wrong Day", "Cannot add task on: " + task.currentDate.DayOfWeek);

                //check if it is bank holiday
                var isBankHoliday = db.Public_Holidays.Where(x => x.Holiday_date == task.currentDate.Date).FirstOrDefault();
                if(isBankHoliday != null)
                    issues.Add("Bank Holiday", "On: " + task.currentDate.ToString("dd/MM/yyyy") + " is Public Holyday: " + isBankHoliday.Holiday_name);
                //check if it is on leave already
                var leaves = db.Leaves.Where(x => x.StartDate <= task.currentDate.Date && x.EndDate >= task.currentDate.Date).FirstOrDefault();
                if (leaves != null)
                    issues.Add("Leave", "Cannot add Task on Leave Day!");

                var requestedLeaves = db.Leave_Requests.Where(x => x.RequestStartDate <= task.currentDate.Date && x.RequestEndDate >= task.currentDate.Date).FirstOrDefault();
                if (requestedLeaves != null)
                    issues.Add("Requested Leave", "Leave requested for the day!");


                //get a list of records for the task day.
                var taskDate = task.currentDate.Date;
                var existingRecords = db.Main_Data.Where(x => x.CurrentDate.Day == taskDate.Day && x.CurrentDate.Month == taskDate.Month && x.CurrentDate.Year == taskDate.Year && x.User_ID == user.ID).ToList();

                var overlapingRecord = existingRecords.Where(x => x.Status_Start_Time <= task.startDate && x.Status_End_Time >= task.startDate).FirstOrDefault();
                if (overlapingRecord != null)
                    issues.Add("Wrong Start Time", "Interval already existing with Title: " + overlapingRecord.Title);

                overlapingRecord = db.Main_Data.Where(x => x.Status_Start_Time <= task.endDate && x.Status_Start_Time >= task.startDate).FirstOrDefault();
                if (overlapingRecord != null)
                    issues.Add("Wrong End Time", "Interval already existing with Title: " + overlapingRecord.Title);

                
            }


            return issues;
        }


        private Models.Security.User GetUserFromSession()
        {
            return (Models.Security.User)Session["User"];
        }


        private void SetMyStatSession(Models.Activity.MyActivity myActivity)
        {
            Session["myActivity"] = myActivity;
            IncreaseSessionTimout();
        }

        private Models.Activity.MyActivity getMyActivityFromSession()
        {
            return (Models.Activity.MyActivity)Session["myActivity"];
        }

        private void IncreaseSessionTimout()
        {
            Session.Timeout = 60;
        }


    }
}