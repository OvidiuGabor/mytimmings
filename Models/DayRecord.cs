using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models
{
    public class DayRecord
    {
        private readonly Models.Security.User user;
        private readonly List<DBContext.Main_Data> records;
        public DateTime date { get; set; }
        //Represents the total worktime for a the  day (excluding break)
        public double workTimeInSeconds { get; set; }
        //Represents total Break time for the day
        public double breakTimeInSeconds { get; set; }

        //represents the first entry into the system
        public DateTime startTime { get; set; }
        //represents the last entry into the system.
        public DateTime endTime { get; set; }


        public DayRecord(List<DBContext.Main_Data> records, Models.Security.User user)
        {
            if (records.Count == 0 || records == null)
                throw new ArgumentNullException("The argument cannot be null, or have 0 records");

            this.records = records;
            this.user = user;


        }

        public void ProcessData()
        {
            CalculateData();
            GetFirstTime();
            GetlastTime();
            SetDayDate();
        }
        private void CalculateData()
        {
            List<string> productiveItems = new List<string>();
            List<string> nonProductiveItems = new List<string>();

            Utilities.Helper.getStatusTypes(user.Company).TryGetValue("Productive", out productiveItems); 
            Utilities.Helper.getStatusTypes(user.Company).TryGetValue("Non Productive", out productiveItems);

            foreach (var record in records)
            {
                if (productiveItems.Contains(record.Status))
                {
                    workTimeInSeconds += CalculateDuration(record.Status_Start_Time, record.Status_End_Time);
                }
                else
                {
                    breakTimeInSeconds += CalculateDuration(record.Status_Start_Time, record.Status_End_Time);

                }
            }
        }

        private double CalculateDuration(DateTime startTime, DateTime endTime)
        {
            if (startTime == null || endTime == null)
                throw new ArgumentNullException("Start tine or End time should not be null!");


            double durationInSeconds = 0;
            if (startTime.Ticks > endTime.Ticks)
                throw new InvalidCastException("Start Date cannot be greater then End Date.");


            TimeSpan duration = new TimeSpan(endTime.Ticks - startTime.Ticks);
            durationInSeconds = duration.TotalSeconds;


            return durationInSeconds;
        }

        private void GetFirstTime()
        {
            var sortedList = records.OrderBy(x => x.Status_Start_Time);
            startTime = sortedList.Select(x => x.Status_Start_Time).FirstOrDefault();
        }
        private void GetlastTime()
        {
            var sortedList = records.OrderByDescending(x => x.Status_End_Time);
            startTime = sortedList.Select(x => x.Status_End_Time).FirstOrDefault();
        }

        private void SetDayDate()
        {
            int dayNumber = records.Select(x => x.Status_Start_Time.Day).FirstOrDefault();

            foreach(var record in records)
            {
                int tempDate = record.Status_Start_Time.Date.Day;
                if (record.Status_Start_Time.Date.Day == record.Status_End_Time.Date.Day)
                {
                    if(dayNumber != tempDate)
                    {
                        throw new InvalidOperationException("There are 2 different days into the list of records!");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Start Date and End Day cannot be on different days!");
                }



            }

            date = records.Select(x => x.Status_Start_Time).FirstOrDefault();
        }

    }
}