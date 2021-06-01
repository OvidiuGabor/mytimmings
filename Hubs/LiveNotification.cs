using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Hubs
{
    public class LiveNotification
    {


        private static DBContext.DBModel db = new DBContext.DBModel();

        public DateTime DateReceived { get; set; }
        public DateTime? DateClosed { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }
        public string Type { get; set; }


        public LiveNotification()
        {

        }
        public LiveNotification(DBContext.LiveNotification db)
        {
            if (db == null)
                throw new ArgumentNullException("Constructor argument cannot be null.");

            DateReceived = db.DateReceived.Value;
            DateClosed = db.DateClosed;
            Message = db.Message;
            Sender = GetNameFromId(db.Sender);
            Type = db.Type;

        }

        public static List<LiveNotification> GetOpenNotification(string userID)
        {
            List<LiveNotification> returnList = new List<LiveNotification>();
            var notifList = db.LiveNotifications.Where(x => x.UserId == userID && x.DateClosed == null).ToList();
            if(notifList.Count > 0)
            {
                foreach(var item in notifList)
                {
                    returnList.Add(new LiveNotification(item));
                }
            }

            return returnList;
        }

        private string GetNameFromId(string sender)
        {
            return "Ovidiu Gabor";
        }
    }
}