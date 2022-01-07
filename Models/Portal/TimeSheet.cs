using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class TimeSheet
    {
        private static Dictionary<int, double> dailyTotalHours;

        private static int daysToPursue = 0;
        private static DateTime beginDate;




        public static Dictionary<int, double> CalculateTotalHoursForPeriod(List<WorkRecord> records, int daystoCalculate, DateTime dateToCalculate)
        {
            daysToPursue = daystoCalculate;
            beginDate = dateToCalculate;
            SetUpDictionary();
            CalculateTotalHours(records);

            return dailyTotalHours;
        }



        public static Dictionary<int, double> CalculateTotalHours(List<WorkRecord> records)
        {

            if (records.Count == 0 || records == null)
                throw new ArgumentNullException("The recods count cannot be 0 or the argument cannot be null!");

            if(dailyTotalHours == null)
                dailyTotalHours = new Dictionary<int, double>();

            foreach(var record in records)
            {
                if(record.startDate.Day == record.startDate.Day)
                {
                    int day = record.startDate.Day;
                    if (dailyTotalHours.ContainsKey(day))
                    {
                        dailyTotalHours[day] += Utilities.Helper.ConvertSecondsToHours(CalculateDuration(record.startDate, record.endDate));
                    }
                    else
                    {
                        dailyTotalHours.Add(day, Utilities.Helper.ConvertSecondsToHours(CalculateDuration(record.startDate, record.endDate)));
                    }
                    
                }
            }



            return dailyTotalHours;
        }


        private static double CalculateDuration(DateTime startTime, DateTime endTime)
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


        private static void SetUpDictionary()
        {
            dailyTotalHours = new Dictionary<int, double>();
           


            for(int i  = daysToPursue; i < 0; i++)
            {
                DateTime tempDate = beginDate.AddDays(i);
                int day = tempDate.Date.Day;
                dailyTotalHours.Add(day, 0);

            }

          
        }

    }
}