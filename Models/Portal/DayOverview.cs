using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class DayOverview
    {

        List<Models.Portal.DayRecord> daysRecordsList { get; set; }

        public string totalTime { get; set; }

        public DateTime finishDate { get; set; }
        public DateTime startDate { get; set; }
        public DateTime lastStatusStartime { get; set; }

        public DateTime actualCurrentTime { get; set; }

        public string lastStatus { get; set; }

        public int maxWorkingTime { get; set; }
        public int totalTimeMilisec { get; set; }






        public DayOverview()
        {
            daysRecordsList = new List<DayRecord>();
            actualCurrentTime = DateTime.Now;

        }



        public static DayOverview CreateModel (List<DayRecord> records, int maxWorkingHours)
        {
            return new DayOverview
            {
                daysRecordsList = records,
                totalTime = CalculateTotalTime(records),
                finishDate = CalculateMaxTime(GetStartDate(records), maxWorkingHours),
                startDate = GetStartDate(records),
                lastStatusStartime = GetCurrentTime(records),
                lastStatus = GetLastStatus(records),
                maxWorkingTime = maxWorkingHours * 3600,
                totalTimeMilisec = CalculateTotalTimeInt(records)

            };
        }

        private static string CalculateTotalTime(List<DayRecord> Records)
        {
            string totalTime = "";
            int tempCalculate = 0;

            foreach (DayRecord record in Records)
            {
                tempCalculate += record.totalTime;
            }

            TimeSpan t = TimeSpan.FromSeconds(tempCalculate);
            totalTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);

            return totalTime;
        }
        private static int CalculateTotalTimeInt(List<DayRecord> Records)
        {
            int tempCalculate = 0;

            foreach (DayRecord record in Records)
            {
                tempCalculate += record.totalTime;
            }

 
          
            return tempCalculate;
        }

        private static DateTime CalculateMaxTime (DateTime startTime, int maxHours)
        {
            DateTime setDate = startTime;
            return setDate.AddHours(maxHours);


        }
        public static DateTime GetStartDate(List<DayRecord> records)
        {
            List<DateTime> datesList = new List<DateTime>();

            foreach(var item in records)
            {
                datesList.Add(item.startDate);
            }

            return datesList.OrderBy(x => x.Hour).FirstOrDefault();
        }
        public static DateTime GetCurrentTime(List<DayRecord> records)
        {
            List<DateTime> datesList = new List<DateTime>();

            foreach (var item in records)
            {
                datesList.Add(item.startDate);
            }

            return datesList.OrderByDescending(x => x.Hour).FirstOrDefault();
        }

        public static string GetLastStatus(List<DayRecord> records)
        {
            string status = "";
            DateTime datetoCompare = DateTime.Now.AddDays(-1);
            foreach (var item in records)
            {
                if(item.startDate > datetoCompare)
                {
                    datetoCompare = item.startDate;
                    status = item.status;
                }
            }

            return status;
        }
    }



}