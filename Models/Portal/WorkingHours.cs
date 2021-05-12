using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    //Calculated in seconds
    public class WorkingHours
    {
        public TimeSpan TotalTime { get; set; }
        public TimeSpan Productive { get; set; }
        public TimeSpan NonProductive { get; set; }
        public TimeSpan Other { get; set; }

        private readonly List<Status> StatusesList = new List<Status>();


        public WorkingHours(List<Status> statuses)
        {
            if (statuses.Count < 1)
                throw new ArgumentOutOfRangeException("The list must have at least 1 element.");
            StatusesList = statuses;

        }


        public void CalculateHours()
        {
            CalculateTimes();
        }


        private void CalculateTimes()
        {
            
            TotalTime = new TimeSpan(0, 0, 0);
            Productive = new TimeSpan(0, 0, 0);
            NonProductive = new TimeSpan(0, 0, 0);
            Other = new TimeSpan(0, 0, 0);
            
            foreach(var status in StatusesList)
            {
                if (status.Type == "Productive")
                    Productive += status.Duration;
                if (status.Type == "Non Productive")
                    NonProductive += status.Duration;
                if (status.Type == "Other")
                    Other += status.Duration;

                TotalTime = status.Duration;
            }


        }


    }
}