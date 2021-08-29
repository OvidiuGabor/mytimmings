using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class Action
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public String Comment { get; set; }
        public string Type { get; set; }
        public int ProjectId { get; set; }

        private readonly string UserId;

        public Action()
        {

        }
        public Action(string userId, DateTime start, DateTime end, string status, int projectId, string comment)
        {
            if (userId == null || status == null)
                throw new ArgumentNullException("The arguments for this Method Cannot be null!");

            UserId = userId;
            StartTime = start;
            EndTime = end;
            Name = status;
            ProjectId = projectId;
            Comment = comment;

        }
        public Action(DBContext.Main_Data data)
        {
            if (data == null)
                throw new ArgumentNullException("Db Object cannot be null");

            Name = data.Current_Status;
            StartTime = data.Status_Start_Time;
            EndTime = data.Status_End_Time;
            Comment = Comment;
            ProjectId = data.ProjectID.HasValue ? (int)data.ProjectID.Value : 0;
            Duration = CalculateDuration(StartTime, EndTime);
            UserId = data.userID;
            Type = GetType(data.Current_Status);
          
        }

        public static List<Action> CreateFromDbList( List<DBContext.Main_Data> lst)
        {
            List<Action> result = new List<Action>();
            foreach(var item in lst)
            {
                result.Add(new Action(item));
            }
            return result;
        }

        private TimeSpan CalculateDuration(DateTime startTime, DateTime? endTime)
        {
            //Dont need to convert to UTC, as the time is recorded in UTC Directly 
            //DateTime startUtcTime = Utilities.Helper.convertToUTC(startTime);
            DateTime startUtcTime = startTime;
            DateTime endUtcTime = DateTime.UtcNow;
            if (endTime != null)
                endUtcTime = endTime.Value;

            if (startTime == null)
                throw new ArgumentNullException("Cannot calclate duration when the Start Time is null.");

            if (DateTime.Compare(startUtcTime, endUtcTime) > 0)
                throw new InvalidOperationException("Cannot calculate duration when start time is lower then End time");

            TimeSpan result = endUtcTime - startUtcTime;


            return result;
        }

        private string GetType(string statusName)
        {
            string result = "Other";
            string companyId = Utilities.Helper.getCompanyId(UserId);
            Dictionary<string, List<string>> listToCheck = Utilities.Helper.getStatusTypes(companyId); //this should be the list of actions that we need to compare

            foreach(var item in listToCheck)
            {
                if (item.Value.Contains(statusName))
                    return item.Key;
            }


            return result;


        }


        public void AddRecord()
        {
            InsertRecord();
        }

        private void InsertRecord()
        {
            var db = new DBContext.DBModel();
            var newRec = new DBContext.Main_Data();
            newRec.userID = UserId;
            newRec.CurrentDate = StartTime;
            newRec.Status_End_Time = EndTime;
            newRec.Status_Start_Time = StartTime;
            newRec.Current_Status = Name;
            newRec.ProjectID = ProjectId;
            newRec.Comments = Comment;

            try
            {
                db.Main_Data.Add(newRec);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
}