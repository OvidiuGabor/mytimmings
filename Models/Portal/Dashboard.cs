using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class Dashboard
    {

        public Models.Security.User user { get; }

        public Dictionary<int, double> dailyTotalHours { get; set; }


        public Dashboard(Security.User user, Dictionary<int, double> dailyTotalHours)
        {
            this.user = user;
            this.dailyTotalHours = dailyTotalHours;
        }

    }
}