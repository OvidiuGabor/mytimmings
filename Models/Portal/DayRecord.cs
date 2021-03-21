using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class DayRecord
    {
        public string userid { get; set; }
        public DateTime currentDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }
        public DateTime? endDate { get; set; }

        public string status { get; set; }
        public string projectId { get; set; }
        public string projectName { get; set; }
        public bool isRunning { get; set; }

        [StringLength(1000)]
        public string comments { get; set; }

        public int totalTime { get; set; }

        public decimal id { get; set; } 


        #region define Methods of the Class
        public DayRecord()
        {
            startDate = Utilities.Helper.convertToUTC(DateTime.Now);
            currentDate = Utilities.Helper.convertToUTC(DateTime.Now);
        }

        public DayRecord(Models.Portal.Event ev, Models.Security.User user)
        {
            userid = user.ID;
            currentDate = Utilities.Helper.convertToUTC(DateTime.Now);
            status = ev.Status;
            projectId = ev.ProjectId.ToString();
            isRunning = false;
            comments = ev.Comments;
            startDate = Utilities.Helper.CalculateDateFromString(ev.StartTime);
            endDate = Utilities.Helper.CalculateDateFromString(ev.EndTime);
        }

        public static DayRecord CreateReocrd(DBContext.Main_Data data)
        {
            return new DayRecord
            {
                id = data.ID,
                userid = data.userID,
                currentDate = data.CurrentDate,
                startDate = data.Status_Start_Time,
                endDate = data.Status_End_Time,
                status = data.Current_Status,
                projectId = data.ProjectID.ToString(),
                isRunning = data.IsRunning,
                comments = data.Comments,
                totalTime = calculateTotalTime(data.Status_Start_Time, data.Status_End_Time ?? Utilities.Helper.convertToUTC(DateTime.Now)),
                projectName = getProjectName(data.ProjectID.ToString())

            };
        }

        public static DBContext.Main_Data InsertNewRecord (DayRecord record)
        {
            return new DBContext.Main_Data
            {
                userID = record.userid,
                CurrentDate = record.currentDate,
                Status_Start_Time = record.startDate,
                Status_End_Time = record.endDate,
                Current_Status = record.status,
                ProjectID = Int32.Parse(record.projectId),
                IsRunning = record.isRunning,
                Comments = record.comments,
                
            };
        }

        public static DayRecord CreateModel (string userID, string projectID, string status, string comments)
        {
            return new DayRecord
            {
                userid = userID,
                status = status,
                projectId = projectID,
                comments = comments,
                isRunning = true
            };
        }

        //for refactoring
        public List<DayRecord> getAllRecordsForCurrentDay(DayRecord todayRecord)
        {
            DBContext.DBModel db = new DBContext.DBModel();
            List<Models.Portal.DayRecord> recordsList = new List<Models.Portal.DayRecord>();
            List<DBContext.Main_Data> recordsListDB = db.Main_Data.OrderBy(y => y.Status_Start_Time).Where(x => DbFunctions.TruncateTime(x.CurrentDate) == DbFunctions.TruncateTime(todayRecord.currentDate) && x.userID == todayRecord.userid).ToList();
            if (recordsListDB.Count > 0)
            {
                foreach (var item in recordsListDB)
                {
                    Models.Portal.DayRecord createRecord = CreateReocrd(item);
                    recordsList.Add(createRecord);
                }
            }




            return recordsList;

        }
        private static int calculateTotalTime(DateTime startDate, DateTime endDate)
        {
            int totalTime = 0;

            if(endDate == null) { endDate = Utilities.Helper.convertToUTC(DateTime.Now); };
            if(startDate == endDate) { 
                totalTime = 0;
            }
            else
            {
                totalTime = (int)endDate.Subtract(startDate).TotalSeconds;

            }



            return totalTime;
        }

        private static string getProjectName (string projectId)
        {
            string projectName = "";
            DBContext.DBModel db = new DBContext.DBModel();
            decimal id = Convert.ToDecimal(projectId);
            projectName = db.Projects.Find(id).Name;
            

            return projectName;
        }

        #endregion
    }
}