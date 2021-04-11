using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Request
{
    public class Request
    {
        public string Type { get; set; }
        public string Requestor { get; set; }
        private string Approver { get; set; }
        private DateTime Date { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RequestorComments { get; set; }
        public string ApproverComments { get; set; }
        private bool Approved { get; set; }


        public Request()
        {

        }

        public Request(string type, string requestor, DateTime startDate, DateTime endDate, string comments)
        {
            Type = type;
            Requestor = requestor;
            StartDate = startDate;
            EndDate = endDate;
            RequestorComments = comments;
            Date = DateTime.Now;
        }

    }
}