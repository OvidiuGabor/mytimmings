using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mytimmings.Models.Portal
{
    public class Portal
    {

        DayRecord currentStatus = new DayRecord();
        public DayOverview CurrentDayOverview = new DayOverview();

        public List<DayRecord> daysList = new List<DayRecord>();
        public List<SelectListItem> projectList = new List<SelectListItem>();
        public List<SelectListItem> statusList = new List<SelectListItem>();


        public string project { get; set; }
        public string status { get; set; }

        public string projectName { get; set; }


        public Portal()
        {
            projectList = Utilities.Helper.getProjectList();
            statusList = Utilities.Helper.getStatuslist();

        }
    
    }
}