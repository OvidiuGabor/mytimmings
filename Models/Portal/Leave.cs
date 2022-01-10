using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class Leave
    {
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int NumberofDays { get; set; }


        public Leave(DBContext.Leave leave)
        {

            Type = leave.Type;
            Start = leave.StartDate.Value;
            End = leave.EndDate.Value;
            if (leave.TotalDays == 0)
            {
               NumberofDays =  CalculateBusinessDays(Start, End);
            }
            else
            {
                NumberofDays = (int)leave.TotalDays;
            }
           

        }

        public int CalculateNumberOfBusinessDays(DateTime start, DateTime end, params DateTime[] bankHolidays)
        {
            return CalculateBusinessDays(start, end, bankHolidays);
        }

        private int CalculateBusinessDays(DateTime start, DateTime end, params DateTime[] BankHolidays)
        {
            var firstDay = start.Date;
            var lastDay = end.Date;

            if (firstDay > lastDay)
                throw new ArgumentException("Incorect last day, cannot be lower then the start! " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekDays = businessDays / 7;

            if(businessDays > fullWeekDays * 7)
            {
                int firstDayOfWeek = firstDay.DayOfWeek ==DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;

                if(lastDayOfWeek < firstDayOfWeek)
                {
                    lastDayOfWeek += 7;
                }

                if (firstDayOfWeek <= 6) {
                    if (lastDayOfWeek >= 7)
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)
                        businessDays -= 1;

                }else if(firstDayOfWeek <= 7 && lastDayOfWeek >= 7)
                {
                    businessDays -= 1;
                }
            }

            businessDays -= fullWeekDays + fullWeekDays;


            foreach(var holiday in BankHolidays)
            {
                DateTime bh = holiday.Date;
                if(firstDay <= bh && bh <= lastDay)
                {
                    businessDays--;
                }
            }

            return businessDays;

        }
    }

}