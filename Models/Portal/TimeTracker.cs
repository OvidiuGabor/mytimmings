using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace mytimmings.Models.Portal
{
    class TimeTracker
    {
        //this will hold the current time passed since the clock in has been pressed!
        public TimeSpan CurrentTimePointer { get; set; }
        public DateTime StartTime { get; set; } //thjis will hold the start time of the clock
        public DateTime? EndTime { get; set; }
        public DateTime MaxFinishTime { get; set; } //this will hold the max duration, based on the user settings

            
        private Models.Security.UserSettings UserSettings = new Security.UserSettings();

        public TimeTracker()
        {

        }
        public TimeTracker(Models.Security.UserSettings userSettings)
        {

            UserSettings = userSettings;
            MaxFinishTime = CalculateMaxDuration();
            CurrentTimePointer = CalculateCurrentTimePointer();
        }

        public TimeTracker(DBContext.User_Login_Logout db, Models.Security.AuthState userState)
        {
            if (db == null)
                throw new ArgumentNullException("Db Object cannot be null");

            StartTime = db.LoginTime.Value;
            EndTime = db.LogoutTime;
            UserSettings = userState.UserSettings;
            MaxFinishTime = CalculateMaxDuration();
            CurrentTimePointer = CalculateCurrentTimePointer();
        }

        private DateTime CalculateMaxDuration()
        {
            TimeSpan defaultTime = new TimeSpan(9, 0, 0);
            //if the user doenst have the seetings configurated or missing, then we set it to default of 9H?
            if (UserSettings.TotalTime.Ticks == 0)
                return StartTime.Add(defaultTime);

            return StartTime.Add(UserSettings.TotalTime);

        }

        private TimeSpan CalculateCurrentTimePointer()
        {
            DateTime currentTime = DateTime.UtcNow;
            if (StartTime.Ticks == 0)
                return new TimeSpan(0, 0, 0); //since there is no start time, then we return 0 time.

            if (EndTime.HasValue)
            {
                if (EndTime.Value.Ticks > StartTime.Ticks)
                    return EndTime.Value - StartTime;
                else
                    if (currentTime.Ticks > StartTime.Ticks)
                        return currentTime - StartTime;
            }
            else
            {
                if (currentTime.Ticks > StartTime.Ticks)
                    return (currentTime - StartTime);
            }
                    

            return new TimeSpan(0, 0, 0);

        }



    }
}