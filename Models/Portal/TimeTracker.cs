using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace mytimmings.Models.Portal
{
    public class TimeTracker
    {
        //this will hold the current time passed since the clock in has been pressed!
        public TimeSpan CurrentTimePointer { get; set; }
        public DateTime? StartTime { get; set; } //this will hold the start time of the clock
        public DateTime? EndTime { get; set; }
        public DateTime MaxFinishTime { get; set; } //this will hold the max duration, based on the user settings

            
        public Security.UserSettings UserSettings = new Security.UserSettings();
        public Security.User User { get; set; }
        public TimeTracker()
        {

        }
        public TimeTracker(Models.Security.UserSettings userSettings)
        {

            UserSettings = userSettings;
            MaxFinishTime = CalculateMaxDuration();
            CurrentTimePointer = CalculateCurrentTimePointer();
        }

        public TimeTracker(DBContext.User_Login_Logout db, Models.Security.AuthState userState, Security.User user)
        {
            if (db == null)
                throw new ArgumentNullException("Db Object cannot be null");

            StartTime = db.LoginTime;
            EndTime = db.LogoutTime;
            UserSettings = userState.UserSettings;
            User = user;
            MaxFinishTime = CalculateMaxDuration();
            CurrentTimePointer = CalculateCurrentTimePointer();
            PerformTimeCheck();
        }

        public TimeTracker(DBContext.User_Login_Logout db)
        {
            if (db == null)
                throw new ArgumentNullException("Db Object cannot be null");

            StartTime = db.LoginTime.Value;
            EndTime = db.LogoutTime;
        }
        private DateTime CalculateMaxDuration()
        {
            TimeSpan defaultTime = new TimeSpan(9, 0, 0);
            //if the user doenst have the seetings configurated or missing, then we set it to default of 9H?
            if (UserSettings.TotalTime.Ticks == 0)
                return StartTime.Value.Add(defaultTime);

            if (StartTime == null)
                return DateTime.UtcNow.Add(UserSettings.TotalTime);

            return StartTime.Value.Add(UserSettings.TotalTime);

        }

        private TimeSpan CalculateCurrentTimePointer()
        {
            DateTime currentTime = DateTime.UtcNow;
            if (StartTime == null)
                return new TimeSpan(0, 0, 0); //since there is no start time, then we return 0 time.

            if (EndTime.HasValue)
            {
                if (EndTime.Value.Ticks > StartTime.Value.Ticks)
                    return EndTime.Value - StartTime.Value;
                else
                    if (currentTime.Ticks > StartTime.Value.Ticks)
                        return currentTime - StartTime.Value;
            }
            else
            {
                if (currentTime.Ticks > StartTime.Value.Ticks)
                    return (currentTime - StartTime.Value);
            }
                    

            return new TimeSpan(0, 0, 0);

        }

        //perform some check in order to determine if the previous day has logout or not, then reset the start time to 0;
        private void PerformTimeCheck()
        {
            DBContext.DBModel db = new DBContext.DBModel();
            TimeSpan additionalhours = new TimeSpan(0,0,0);
            DateTime currentTime = DateTime.UtcNow;
            var companyPartialRequests = db.Companies_Assigned_Items.Where(x => x.Company_ID == User.Company).Select(x => x.Partial).FirstOrDefault();
            //List of items to check
            List<string> partialItems = new List<string>();
            if (!String.IsNullOrEmpty(companyPartialRequests))
                partialItems = Utilities.Helper.convertStringtoList(companyPartialRequests, ';');

            //if there is no end time
            if(EndTime == null && StartTime != null)
            {
                //need to check if there is overtime/recovery/partial day/or any other request requested
                //get the company available request
                    if (partialItems.Count > 0)
                    {
                        var additional = db.Main_Data.Where(x => x.Status_Start_Time.CompareTo(StartTime.Value) >= 0 && partialItems.Contains(x.Current_Status)).ToList();
                        foreach (var item in additional)
                        {
                            if(item.Status_End_Time != null)
                                additionalhours += item.Status_End_Time.Value - item.Status_Start_Time;
                        }
                    }

                if (additionalhours.TotalMinutes > 0)
                {
                    MaxFinishTime.Add(additionalhours);
                }

                //reset everything to 0
                TimeSpan datediff = MaxFinishTime - StartTime.Value;

                if (datediff.TotalMinutes < CurrentTimePointer.Minutes)
                {
                    StartTime = null;
                    EndTime = null;
                    CurrentTimePointer = new TimeSpan(0, 0, 0);
                    MaxFinishTime = CalculateMaxDuration();
                }

            }

        }

    }
}