using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Security
{
    public class UserSettings
    {
        public string ID { get; private set; }
        public int ShiftTime { get; private set; }
        public int BreakTime { get; private set; }
        public int TotalTime { get; private set; }

        public UserSettings()
        {

        }
        public UserSettings(DBContext.User_Settings setting)
        {
            ID = setting.ID;
            ShiftTime = Convert.ToInt32(setting.ShiftTime);
            BreakTime = Convert.ToInt32(setting.BreakTime);
            TotalTime = Convert.ToInt32(setting.TotalTime);
        }



    }
}