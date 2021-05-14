using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mytimmings.Models.Portal
{
    class Overview
    {
        public List<Status> Statuses { get; set; }

        public WorkingHours WorkingHours { get; set; }
        //List of the actions that have been performed today 
        public List<Status> TodayActions{ get; set; }

        public TimeTracker TimeTracker = new TimeTracker();
        public List<TimeTracker> DailyLogins = new List<TimeTracker>(); //This will be a list that will be used for Daily Logins Widged


        public List<SelectListItem> StatusDropDown = new List<SelectListItem>();

        public List<SelectListItem> ProjectDropDown = new List<SelectListItem>();



        public Overview(List<Status> statuses, TimeTracker timeTracker, List<TimeTracker> logins, Models.Security.User user)
        {
            TimeTracker = timeTracker;
            Statuses = statuses;
            GenerateWorkinghours();
            StatusDropDown = Utilities.Helper.getStatuslist(user.Company);
            ProjectDropDown = Utilities.Helper.getProjectList(user.ID);
            DailyLogins = logins;
        }


        private void GenerateWorkinghours()
        {
            WorkingHours = new WorkingHours(Statuses);
            WorkingHours.CalculateHours();
        }

    }
}