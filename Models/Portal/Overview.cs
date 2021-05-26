using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mytimmings.Models.Portal
{
      public class Overview
      {
        public List<Action> Statuses { get; set; }

        public WorkingHours WorkingHours { get; set; }
        //List of the actions that have been performed today 
        public List<Action> TodayActions{ get; set; }
        public List<LeaveStatus> LeaveStatus { get; set; }

        public TimeTracker TimeTracker = new TimeTracker();

        public List<TimeTracker> DailyLogins = new List<TimeTracker>(); //This will be a list that will be used for Daily Logins Widged

        public string ActionName { get;set; }
        public string ProjectId { get; set; }
        public List<SelectListItem> ActionsDropDown = new List<SelectListItem>(); //Dropdown for Action

        public List<SelectListItem> ProjectDropDown = new List<SelectListItem>(); //project Dropdown
        public PartialStatus Partial { get; set; }
        public Overview(List<Action> statuses, TimeTracker timeTracker, List<TimeTracker> logins, List<LeaveStatus> leaveStatuses, Models.Security.User user)
        {
            TimeTracker = timeTracker;
            Statuses = statuses;
            LeaveStatus = leaveStatuses;
            ActionsDropDown = Utilities.Helper.getStatuslist(user.Company);
            ProjectDropDown = Utilities.Helper.getProjectList(user.ID);
            DailyLogins = logins;
            GetTodayActions(user.ID);
            GenerateWorkinghours();
            SetSelectedDropDowns();
            Partial = new PartialStatus(user.Company);

        }


        private void GenerateWorkinghours()
        {
            WorkingHours = new WorkingHours(Statuses);
            WorkingHours.CalculateHours();
        }

        private void GetTodayActions(string userId)
        {
            DBContext.DBModel db = new DBContext.DBModel();
            if(TimeTracker.StartTime != null && TimeTracker.StartTime.Value.Ticks > 0)
            {
                if (Statuses.Count > 0)
                {
                    TodayActions = Statuses.Where(x => x.StartTime >= TimeTracker.StartTime).ToList();

                }
                else
                {
                    var dbData = db.Main_Data.Where(x => x.userID == userId && x.Status_Start_Time.CompareTo(TimeTracker.StartTime) >= 0).ToList();
                    foreach(var item in dbData)
                    {
                        TodayActions.Add(new Action(item));
                    }
                }
            }

           
        }

        private void SetSelectedDropDowns()
        {
            if(TodayActions != null && TodayActions.Count > 0)
            {
                var temp = TodayActions.OrderByDescending(x => x.StartTime);
                ProjectId = temp.First().ProjectId.ToString();
                ActionName = temp.First().Name;
            }
        }

    }
}