using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal.Partial_Request
{
    public class OvertimeRequest : IPartialRequest
    {
        public DateTime RequestDate { get; set; }
        public string Type { get; set; } = "Overtime";
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan Duration { get; set; }
        public int ProjectId { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public string Approver { get; set; }
        public  Security.User User { get; set; }
        private readonly DBContext.DBModel db = new DBContext.DBModel();

        private string RequestId;

        public OvertimeRequest()
        {
            RequestId = Utilities.Helper.generateRequestId(Type);
        }

        //public OvertimeRequest(DateTime startDate, DateTime startTime, TimeSpan duration, string type, string comments, int projectId)
        //{
        //    StartDate = startDate;
        //    StartTime = startTime;
        //    Duration = duration;
        //    Type = type;
        //    Comments = comments;
        //    ProjectId = projectId;
        //}

        public OvertimeRequest(string requestId)
        {
            if (String.IsNullOrEmpty(requestId))
                throw new ArgumentNullException("The argument notificationId cannot be null or empty string!");

            getPartialRequestFromId(requestId);
        }
        public OvertimeRequest(DBContext.PartialTime_Requests record)
        {
            if (record == null)
                throw new ArgumentNullException("The argument record cannot be null!");

            RequestId = record.RequestId;
            User.ID = record.UserId;
            RequestDate = record.RequestDate;
            StartTime = record.StartTime;
            Duration = record.Duration;
            Comments = record.Comment;
            ProjectId = (int)record.ProjectId;
            Approver = record.Approver;
            Status = record.Status;
            
        }
        public void AddUser(Security.User user)
        {
            if (user == null)throw new ArgumentNullException("User cannot be null.");

            User = user;
        }

        public void AddRequest() {

            //Add the record to the Partial Time Request Table
            DBContext.PartialTime_Requests request = new DBContext.PartialTime_Requests();
            request.RequestId = RequestId;
            request.UserId = User.ID;
            request.SubmitedDate = DateTime.UtcNow;
            request.RequestDate = RequestDate;
            request.StartTime = Utilities.Helper.convertToUTC(StartTime);
            request.EndTime = StartTime.Add(Duration);
            request.Duration = Duration;
            request.Comment = Comments;
            request.ProjectId = ProjectId;
            request.Approver = Approver;
            request.Status = Status;

            try
            {
                db.PartialTime_Requests.Add(request);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void UpdateRequest() {

            var request = db.PartialTime_Requests.Where(x => x.RequestId == RequestId).FirstOrDefault();

            if (request == null)
                throw new InvalidOperationException("The Request with the id " + RequestId + " cannot be found in the database!");

            request.RequestDate = RequestDate;
            request.StartTime = Utilities.Helper.convertToUTC(StartTime);
            request.EndTime = StartTime.Add(Duration);
            request.Duration = Duration;
            request.Comment = Comments;
            request.ProjectId = ProjectId;
            request.Approver = Approver;
            request.Status = Status;
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public string getNotificationId()
        {
            return RequestId;
        }

        private void getPartialRequestFromId(string requestId)
        {
            var record = db.PartialTime_Requests.Where(x => x.RequestId == requestId).FirstOrDefault();

            if (record == null)
                throw new InvalidOperationException("Cannot find any record for the Requestid: " + requestId + ".");

            new OvertimeRequest(record);

        }

        public void ApproveRequest() {
            var request = db.PartialTime_Requests.Where(x => x.RequestId == RequestId).FirstOrDefault();

            if (request == null)
                throw new InvalidOperationException("The Request with the id " + RequestId + " cannot be found in the database!");

            request.Status = "Approved";
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void DeclineRequest()
        {
            var request = db.PartialTime_Requests.Where(x => x.RequestId == RequestId).FirstOrDefault();

            if (request == null)
                throw new InvalidOperationException("The Request with the id " + RequestId + " cannot be found in the database!");

            request.Status = "Declined";
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}