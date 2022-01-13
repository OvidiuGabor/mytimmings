using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal.Leave
{
    public class LeaveRequest: ILeave
    {
        public string requestor { get; set; }
        public string approver { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public bool approved { get; set; }
        public string  requestorComments { get; set; }
        public string approverComments { get; set; }
        public DateTime requestDate { get; set; }
        public int numberofDays { get; set; }
        public string type { get; set; }
        public string status { get; set; }

        public LeaveRequest(DBContext.Leave_Requests request, DateTime[] publicHoliday)
        {
            if(request == null)
            {
                throw new ArgumentNullException("Parameter request cannot be null!");
            }

            type = request.RequestType;
            requestor = request.Requestor;
            approver = request.Approver;
            requestDate = request.RequestDate.Value.Date;
            start = request.RequestStartDate.Value.Date;
            end = request.RequestEndDate.Value.Date;
            approved = request.Approved ?? false;
            requestorComments = request.RequestorComments;
            approverComments = request.ApproverComments;
            status = request.Approved.Value ? "Approved" : "Pending";
            numberofDays = CalculateNumberOfBusinessDays(start, end, publicHoliday);


        }

        public int CalculateNumberOfBusinessDays(DateTime start, DateTime end, params DateTime[] bankHolidays)
        {
            return Utilities.Helper.CalculateBusinessDays(start, end, bankHolidays);   
        }
    }
}