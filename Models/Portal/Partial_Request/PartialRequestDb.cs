using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal.Partial_Request
{
    public class PartialRequestDb
    {
        public string UserId { get; set; }
        public TimeSpan RequestTime { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string  Comment { get; set; }
        public int ProjectId { get; set; }
        public string Approver { get; set; }
        public string Status { get; set; }
        public PartialRequestDb()
        {

        }

        public PartialRequestDb(DBContext.PartialTime_Requests db)
        {
            if (db == null)
                throw new ArgumentNullException("Argument from DB cannot be null.");

            UserId = db.UserId;
            RequestTime = db.RequestTime;
            RequestDate = db.RequestDate;
            StarTime = db.StartTime;
            EndTime = db.EndTime;
            Duration = db.Duration;
            ProjectId = (int)db.ProjectId;
            Approver = db.Approver;
            Status = db.Status;
            Comment = db.Comment;

        }

        public static DBContext.PartialTime_Requests CreateDbRecord(PartialRequestDb rec)
        {
            if (rec == null)
                throw new ArgumentNullException("Record cannot be null.");

            return new DBContext.PartialTime_Requests
            {
                UserId = rec.UserId,
                RequestTime = rec.RequestTime,
                RequestDate = rec.RequestDate,
                StartTime = rec.StarTime,
                EndTime = CalculateEnd(rec.StarTime, rec.Duration),
                Duration = rec.Duration,
                ProjectId = rec.ProjectId,
                Approver = rec.Approver,
                Status = rec.Status,
                Comment = rec.Comment
            };


        }

        public static void AddRecord(DBContext.PartialTime_Requests rec)
        {
            if (rec == null)
                throw new ArgumentNullException("Argument cannot be null.");

            var db = new DBContext.DBModel();
            try
            {
                db.PartialTime_Requests.Add(rec);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                //log error
                throw ex;
            }

        }


        public static void UpdateStatus(DBContext.PartialTime_Requests rec, string newStatus)
        {
            if (rec == null)
                throw new ArgumentNullException("Record from Db cannot be null");

            if (String.IsNullOrEmpty(newStatus))
                throw new ArgumentNullException("Status Argument cannot be null or Empty");

            var db = new DBContext.DBModel();
            rec.Status = newStatus;
            try
            {
                db.PartialTime_Requests.Attach(rec);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                //log the error;
                throw ex;
            }
        }


        private static DateTime CalculateEnd(DateTime startDate, TimeSpan duration)
        {
            return startDate.Add(duration);
        }

        private void InsertRecordIntoDb()
        {

        }
            

    }
}