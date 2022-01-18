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
        public ActionResult MyTimeSheet()
        {
            Models.Security.User user = (Models.Security.User)Session["User"];
            Models.Security.AuthState userState = (Models.Security.AuthState)Session["AuthState"];

            if (user == null || userState == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var worklogsDb = db.Main_Data.Where(x => x.User_ID == user.ID).OrderByDescending(x => x.Status_Start_Time).Take(10);

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










            Models.Activity.MyActivity myActivity = new Models.Activity.MyActivity();
            myActivity.workLogs = workLogs;
            myActivity.leaves = leaves;


            return View(myActivity);
        }

        







        private Models.Security.User GetUserFromSession()
        {
            return (Models.Security.User)Session["User"];
        }
    }
}