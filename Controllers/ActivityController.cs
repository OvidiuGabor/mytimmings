using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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