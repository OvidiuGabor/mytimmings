using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class UserLeaveStatus
    {
        public string leaveType { get; set; }
        public int daysEntitled { get; set; }
        public int daysAccrued { get; set; }
        public int daysCarriedOver { get; set; }
        public int totalDaysAvailable { get; set; }

        public UserLeaveStatus(DBContext.User_Leaves_Status userLeaveStatus)
        {
            if (userLeaveStatus == null)
                throw new ArgumentNullException("Parameter cannot be null!");

            leaveType = userLeaveStatus.Leave_type;
            daysEntitled = (int)userLeaveStatus.Entitled;
            daysAccrued = (int)userLeaveStatus.Accrued;
            daysCarriedOver = (int)userLeaveStatus.Carried_Over;

        }
    }
}