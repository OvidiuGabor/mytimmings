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
        private List<Leave.ILeave> leaves;
        private List<UserLeaveStatus> leavesStatus;

        public DateTime loginDateTime { get; set; }
        public TimeSpan breakLength { get; set; }
        public TimeSpan workLength { get; set; }
        public TimeSpan totalWorkedHoursPerMonth { get; set; }
        public TimeSpan requiredMonthlyWorkingHours { get; set;}
        public int totalVacationDaysUsed { get; set; }
        public int totalVacationDaysRemaning{ get; set; }
        public int totalMedicalDaysUsed { get; set; }

        public Dictionary<string, int> leavesUsed { get; set; }
        public Dictionary<string, int> leavesRemaning { get; set; }


        public UserStatus(Security.UserSettings userSettings, List<WorkRecord> records, List<Leave.ILeave> leaves, List<UserLeaveStatus> leavesStatus)
        {

            this.userSettings = userSettings;
            this.records = records;
            this.leaves = leaves;
            this.leavesStatus = leavesStatus;
            
        }


        public void CalculateStats()
        {
            setloginTime();
            calculateBreakTime();
            calculateWorkTimeForToday();
            leavesUsed = splitLeavesByType();
            leavesRemaning = splitLeavesStatusByType();
        }

        public TimeSpan CalculateWorkTimeForGivenList(List<WorkRecord> records)
        {
            TimeSpan workTime = new TimeSpan();

            foreach (var record in records)
            {
                if(record.status != "Break")
                {
                    if(record.startDate.Ticks < record.endDate.Ticks)
                    {
                        workTime += record.endDate - record.startDate;
                    }
                }
            }
            return workTime;
        }



        private void setloginTime()
        {
            if(records.Count > 0)
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

        private Dictionary<string, int> splitLeavesByType()
        {
            Dictionary<string, int> leavesSplit = new Dictionary<string, int>();
            foreach (var leave in leaves)
            {
                if (leavesSplit.ContainsKey(leave.type))
                {
                    leavesSplit[leave.type] += leave.numberofDays;
                }
                else
                {
                    leavesSplit.Add(leave.type, leave.numberofDays);
                }
            }

            return leavesSplit;
        }

        private Dictionary<string, int> splitLeavesStatusByType()
        {
            Dictionary<string, int> leavesSplit = new Dictionary<string, int>();
            foreach (var leave in leavesStatus)
            {
                if (leavesSplit.ContainsKey(leave.leaveType))
                {
                    leavesSplit[leave.leaveType] += (leave.daysEntitled + leave.daysCarriedOver - leave.daysAccrued);
                }
                else
                {
                    leavesSplit.Add(leave.leaveType, (leave.daysEntitled + leave.daysCarriedOver - leave.daysAccrued));
                }
            }

            return leavesSplit;
        }

    }
}