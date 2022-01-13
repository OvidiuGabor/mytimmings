using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal.Leave
{
    public class SavedLeave: ILeave
    {
        private readonly int id;
        public DateTime requestDate { get; set; }
        public DateTime end { get; set; }
        public int numberofDays { get; set; }
        public DateTime start { get; set; }
        public string type { get; set; }
        public string status { get; set ; }
        public string comments { get; set; }
        public string userId { get; set; }

        public SavedLeave(DBContext.Leaves_Saved_Not_Submited leave, DateTime[] publicHolidays)
        {
            if (leave == null)
                throw new ArgumentNullException("Parameter leave cannot be null");


            id = (int)leave.Id;
            userId = leave.UserID;
            type = leave.LeaveType;
            requestDate = leave.RequestedDate.Value.Date;
            start = leave.StartDate.Value.Date;
            end = leave.EndDate.Value.Date;
            status = leave.Status;
            comments = leave.Comments;
            numberofDays = CalculateNumberOfBusinessDays(start, end, publicHolidays);

        }

        public int CalculateNumberOfBusinessDays(DateTime start, DateTime end, params DateTime[] bankHolidays)
        {

            return Utilities.Helper.CalculateBusinessDays(start, end);
        }
    }
}