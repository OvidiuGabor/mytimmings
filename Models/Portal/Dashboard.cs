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

        public List<Leave.ILeave> userLeaves;
        public List<Alert> alerts;

        public List<PublicHoliday> publicHolidays;



        public Dashboard(Security.User user, Dictionary<string, double> dailyTotalHours, UserStatus userStats, List<Leave.ILeave> leaves, List<Alert> alerts, List<PublicHoliday> holidays)
        {
            this.user = user;
            this.dailyTotalHours = dailyTotalHours;
            this.userStats = userStats;
            this.userLeaves = leaves;
            this.alerts = alerts;
            this.publicHolidays = holidays;

        }

    }
}