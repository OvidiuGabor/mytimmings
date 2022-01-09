using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class UserStatus
    {

        private Models.Security.UserSettings userSettings;
        private List<WorkRecord> records;
        private List<Leave> leaves;

        public DateTime loginDateTime { get; set; }
        public TimeSpan breakLength { get; set; }
        public TimeSpan workLength { get; set; }
        public TimeSpan totalWorkedHoursPerMonth { get; set; }
        public TimeSpan requiredMonthlyWorkingHours { get; set;}
        public int totalVacationDaysUsed { get; set; }
        public int totalVacationDaysRemaning{ get; set; }
        public int totalMedicalDaysUsed { get; set; }

        Dictionary<string, int> leavesUsed { get; set; }
        Dictionary<string, int> leavesRemaning { get; set; }


        public UserStatus(Security.UserSettings userSettings, List<WorkRecord> records, List<Leave> leaves)
        {

            this.userSettings = userSettings;
            this.records = records;
            this.leaves = leaves;
        }


        public void CalculateStats()
        {
            setloginTime();
            calculateBreakTime();
            calculateWorkTimeForToday();
        }
        


        private void setloginTime()
        {
            if(records != null)
            {

                var first = records.OrderBy(x => x.startDate).FirstOrDefault();

                loginDateTime = first.startDate;
            }
        }

        private void calculateBreakTime()
        {
            TimeSpan breakTime = new TimeSpan();

            foreach(var record in records)
            {
                if (record.status == "Break")
                {
                    breakTime += record.endDate - record.startDate;
                }
            }

            breakLength = breakTime;

        }

        private void calculateWorkTimeForToday()
        {
            TimeSpan workTime = new TimeSpan();
            foreach(var record in records)
            {
                if(record.status != "Break")
                {
                    workTime += record.endDate - record.startDate;
                }
            }

            workLength = workTime;
        }
    }
}