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
        //This Property will keep track of the user status, if he ended the day or not
        //true -> day is not closed or not started
        //false -> day is closed
        public bool isDayOpen { get; set; }

        public Dictionary<int, string> ProjectsAssigned = new Dictionary<int, string>();

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

        public void AddProject(int projectId, string projectName)
        {
            if (projectId == 0)
                throw new ArgumentNullException("Argument projectId cannot be equal to 0.");

            if (String.IsNullOrEmpty(projectName))
                throw new ArgumentNullException("Argument projectName cannot be null or empty");

            ProjectsAssigned.Add(projectId, projectName);

        }

        public void SetDayStatus(bool status)
        {
            isDayOpen = status;

        }

    }
}