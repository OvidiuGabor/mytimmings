using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal.Leave
{
    public class Leave : ILeave
    {
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public int numberofDays { get; set; }
        public DateTime requestDate { get; set; }
        public string status { get; set; }

        public Leave(DBContext.Leave leave, DateTime[] publicHolidays)
        {

            type = leave.Type;
            start = leave.StartDate.Value;
            end = leave.EndDate.Value;
            if (leave.TotalDays == 0)
            {
                numberofDays = Utilities.Helper.CalculateBusinessDays(start, end, publicHolidays);
            }
            else
            {
                numberofDays = (int)leave.TotalDays;
            }

            status = "Approved";
            requestDate = leave.RequestedDate.Value.Date;


        }

        public int CalculateNumberOfBusinessDays(DateTime start, DateTime end, params DateTime[] bankHolidays)
        {
            return Utilities.Helper.CalculateBusinessDays(start, end, bankHolidays);
        }
    }

}