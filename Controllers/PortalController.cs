using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mytimmings.Controllers
{
    public class PortalController : Controller
    {
        private DBContext.DBModel db = new DBContext.DBModel();
        // GET: Portal
        public ActionResult Overview()
        {
            //as default we get the data for the last week from the database
            //starting monday, untill today.
            Models.Security.User user = (Models.Security.User)Session["User"];
            Models.Security.AuthState userState = (Models.Security.AuthState)Session["AuthState"];
            Models.Portal.TimeTracker tm = new Models.Portal.TimeTracker(userState.UserSettings);
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
            List<Models.Portal.Status> statusList = Models.Portal.Status.CreateFromDbList(dbRecords);

            //Create the timetracker Object
            //get the record from the Login table, if exists
            #region CurrentTimeTracker
            var loginDbRecord = db.User_Login_Logout.Where(x => x.UserId == user.ID && ((x.Date.Value.Year == currentDate.Year &&
                                x.Date.Value.Month == currentDate.Month && x.Date.Value.Day == currentDate.Day) ||
                                (x.Date.Value.Year == previousDay.Year && x.Date.Value.Month == previousDay.Month &&
                                x.Date.Value.Day == previousDay.Day && x.LogoutTime.HasValue == false))).FirstOrDefault();

            if (loginDbRecord != null)
                tm = new Models.Portal.TimeTracker(loginDbRecord, userState);
            #endregion

            //Get a list of all the daily loginsfor the x days
            #region loginsList
            var dailyLoginsDb = db.User_Login_Logout.Where(x => x.UserId == user.ID && x.Date.Value.CompareTo(startDate) >= 0);

            foreach(var item in dailyLoginsDb)
            {
                DailyLoginsList.Add(new Models.Portal.TimeTracker(item,userState));
            }
            #endregion

            var OverViewModel = new Models.Portal.Overview(statusList, tm, DailyLoginsList, user);



            return View();
        }
    }
}