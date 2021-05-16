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


        public Leave(DBContext.Leave leaves)
        {

            Type = leaves.Type;
            Start = leaves.StartDate.Value;
            End = leaves.EndDate.Value;
            CalculateNbDays();

        }

        private void CalculateNbDays()
        {
            if (Start.Ticks > 0 && End.Ticks > 0)
                NumberofDays = (End - Start).Days;
        }

    }

}