using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Security
{
    public class UserSettings
    {
        public string UserId { get; set; }
        public TimeSpan ShiftTime { get; set; }
        public TimeSpan BreakTime { get; set; }
        public TimeSpan TotalTime { get; set; }

        public UserSettings()
        {

        }

        public UserSettings(DBContext.User_Settings record)
        {

            if (record == null)
                throw new ArgumentNullException("Db records cannot be null.");

            UserId = record.ID;
            ShiftTime = record.ShiftTime.Value;
            BreakTime = record.BreakTime.Value;
            if (!record.TotalTime.HasValue || record.TotalTime.Value.Ticks == 0)
                TotalTime = (record.ShiftTime.Value + record.BreakTime.Value);
            else
                TotalTime = record.TotalTime.Value;
        }
    }
}