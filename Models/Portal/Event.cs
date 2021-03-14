using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class Event
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public int ProjectId { get; set; }

        //public Event(int projectId)
        //{
        //    ProjectId = projectId;
        //}

        public Event()
        {
                
        }
    }
}