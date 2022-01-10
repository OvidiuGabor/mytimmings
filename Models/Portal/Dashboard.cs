using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class Dashboard
    {

        public Models.Security.User user { get; }

        public Dictionary<string, double> dailyTotalHours { get; set; }
        public UserStatus userStats;

        public List<Leave> userLeaves;
        public List<Alert> alerts;



        public Dashboard(Security.User user, Dictionary<string, double> dailyTotalHours, UserStatus userStats, List<Leave> leaves, List<Alert> alerts)
        {
            this.user = user;
            this.dailyTotalHours = dailyTotalHours;
            this.userStats = userStats;
            this.userLeaves = leaves;
            this.alerts = alerts;
        }

    }
}