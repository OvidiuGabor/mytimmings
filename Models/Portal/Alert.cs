using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class Alert
    {
        private readonly int id;

        private string managerId;
        private string userId;
        private bool isForAll;
        public string message { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public bool important { get; set; }

        public bool dismissButton { get; set; }
        public bool applyOnDutyBtn { get; set; }
        public bool applyOnLeaveBtn { get; set; }

        public Alert(DBContext.Alert alert)
        {
            if (alert == null)
                throw new ArgumentNullException("The parameter provided cannot be null!");

            id = (int)alert.Id;
            userId = alert.UserID;
            managerId = alert.ManagerId;
            isForAll = alert.IsForEveryone ?? false;
            message = alert.Message;
            startDate = alert.StartDate;
            endDate = alert.EndDate;
            dismissButton = true;

        }

        public Alert(string userId, string message, bool important)
        {
            if (String.IsNullOrEmpty(userId))
                throw new ArgumentNullException("The parameter userId cannot be null or empty!");

            this.userId = userId;
            this.message = message;
            this.important = important;
        }




       
    }
}