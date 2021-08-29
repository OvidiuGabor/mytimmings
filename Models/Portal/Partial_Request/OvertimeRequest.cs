using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal.Partial_Request
{
    public class OvertimeRequest : IPartialRequest
    {
        public DateTime StartDate { get; set; }
        public string Type { get; set; }
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan Duration { get; set; }
        public int ProjectId { get; set; }
        public string Comments { get; set; }
        private Security.User User { get; set; }
        private readonly DBContext.DBModel db = new DBContext.DBModel();

        public OvertimeRequest()
        {

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


        public void AddUser(Security.User user)
        {
            if (user == null)throw new ArgumentNullException("User cannot be null.");

            User = user;
        }

        public void AddRequest() {

            //Add the record to the Main_Data table
            DateTime endDate = StartTime.Add(Duration);
            Action rec = new Action(User.ID, StartTime, endDate, Type, ProjectId, Comments);
            rec.AddRecord();
        }

        public void DeleteRequest() { }

        public void UpdateRequest() { }




    }
}