using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class Overview
    {
        public List<Status> Statuses { get; set; }

        public WorkingHours WorkingHours { get; set; }


        public Overview(List<Status> statuses)
        {
            Statuses = statuses;
            GenerateWorkinghours();
        }


        private void GenerateWorkinghours()
        {
            WorkingHours = new WorkingHours(Statuses);
            WorkingHours.CalculateHours();
        }

    }
}