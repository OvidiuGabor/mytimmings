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
            DateTime currentDate = DateTime.UtcNow; //get the currentdate
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
            //Listof records
            List<Models.Portal.Status> statusLiost = Models.Portal.Status.CreateFromDbList(dbRecords);
            var OverViewModel = new Models.Portal.Overview(statusLiost);



            return View();
        }
    }
}