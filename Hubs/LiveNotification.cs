using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace mytimmings.Hubs
{
    public class LiveNotification
    {
        public DateTime DateReceived { get; set; }
        public DateTime? DateClosed { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }
        public string Type { get; set; }

        private readonly string UserId;
        private readonly string Connstring;
        SqlDependency Dependecy = new SqlDependency();
        public LiveNotification(string userId)
        {
            UserId = userId;
            Connstring = getConnectionString();
        }

        private string getConnectionString()
        {
            return PasswordProtect.Decrypt.DecryptText(GetConnectionString("DBModel"), true);

        }
        private static string GetConnectionString(string connectionStringName)
        {
            return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

        }

        public LiveNotification(string dateReceived, string dateClosed, string message, string sender, string type)
        {
           

            DateReceived = DateTime.Parse(dateReceived);
            if(!String.IsNullOrEmpty(dateClosed))
            {
                DateClosed = DateTime.Parse(dateClosed);
            }
         
            Message = message;
            Sender = GetNameFromId(sender);
            Type = type;

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

        public static List<LiveNotification> GetOpenNotification(List<DBContext.LiveNotification> notifList)
        {
            List<LiveNotification> returnList = new List<LiveNotification>();
            if (notifList.Count > 0)
            {
                foreach (var item in notifList)
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


        public List<LiveNotification> GetOpenNotificationByID()
        {
            List<LiveNotification> notifList = new List<LiveNotification>();

            //SqlDependency.Start(Connstring);
            SqlConnection conn = new SqlConnection(Connstring);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"SELECT [ID]
                              ,[UserId]
                              ,[DateReceived]
                              ,[DateClosed]
                              ,[Message]
                              ,[Sender]
                              ,[Type]
                          FROM [mytimngs_admin].[LiveNotification]";
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            Dependecy = new SqlDependency(cmd);
            Dependecy.OnChange += new OnChangeEventHandler(dependecy_OnChage);

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            var dt = new DataTable();

            var reader = cmd.ExecuteReader();
            dt.Load(reader);
            
            for(int i = 0; i  < dt.Rows.Count; i++)
            {
                var curRow = dt.Rows[i];
                notifList.Add(new LiveNotification(curRow["DateReceived"].ToString(), curRow["DateClosed"].ToString(), curRow["Message"].ToString(), curRow["Sender"].ToString(), curRow["Type"].ToString()));
            }


            return notifList;

        }

        private void dependecy_OnChage(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                GetOpenNotificationByID();
                NotificationHub.SendNotification();
                Dependecy.OnChange -= new OnChangeEventHandler(dependecy_OnChage);
            }
           
        }

        public static void  InsertNotification(string userId, DateTime receiveDate, string title, string message, string sender, string type)
        {
            var db = new DBContext.DBModel();
            var record = new DBContext.LiveNotification();
            record.UserId = userId;
            record.DateReceived = receiveDate;
            record.Title = title;
            record.Message = message;
            record.Sender = sender;
            record.Type = type;

            try
            {
                db.LiveNotifications.Add(record);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

    }
}