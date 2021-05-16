using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class LeaveStatus
    {
        public string Type { get; set; }
        public int DaysRemaining { get; set; }

        public LeaveStatus()
        {

        }
        public LeaveStatus(string type, int daysRemaining)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException("Type of leave cannot be null or empty");

            Type = type;
            DaysRemaining = daysRemaining;
        }
    }
}