using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Notification
{
    public class Notification_Comm
    {
        private int Id { get; set; }
        public string NotificationID { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Message { get; set; }
        public string SubmitedBy { get; set; }


        public Notification_Comm()
        {

        }
        public Notification_Comm(string notificationID, DateTime updatedDate, string message, string submitedBy)
        {
            NotificationID = notificationID;
            UpdatedDate = updatedDate;
            Message = message;
            SubmitedBy = submitedBy;
        }

        public Notification_Comm(DBContext.Notification_Comunication_History db)
        {
            if (db == null)
                throw new ArgumentNullException("Paramter db supplied is null!");

            Id = (int)db.Id;
            NotificationID = db.NotificationId;
            UpdatedDate = db.UpdatedDate;
            Message = db.Message;
            SubmitedBy = db.SubmitedBy;

        }



        public void AddNewRecord(Notification_Comm notifMessage)
        {
            if (notifMessage == null)
                throw new ArgumentNullException("Paramter supplied cannot be null!");

            DBContext.DBModel db = new DBContext.DBModel();

            DBContext.Notification_Comunication_History message = new DBContext.Notification_Comunication_History
            {
                NotificationId = notifMessage.NotificationID,
                Message = notifMessage.Message,
                UpdatedDate = notifMessage.UpdatedDate,
                SubmitedBy = notifMessage.SubmitedBy
            };

            try
            {
                db.Notification_Comunication_History.Add(message);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public List<Notification_Comm> GeCommListByNotificationId(string notifId)
        {

            if (String.IsNullOrEmpty(notifId))
                throw new ArgumentNullException("Parameter notifId cannot be null or empry!");

            List<Notification_Comm> lst = new List<Notification_Comm>();
            DBContext.DBModel db = new DBContext.DBModel();

            List<DBContext.Notification_Comunication_History> recordList = db.Notification_Comunication_History.Where(x => x.NotificationId == notifId).ToList();

            if(recordList.Count > 0)
            {
                foreach(var record in recordList)
                {
                    lst.Add(new Notification_Comm(record));
                }
            }



            return lst;
        }


    }
}