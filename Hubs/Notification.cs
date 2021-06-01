using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace mytimmings.Hubs
{
    [HubName("Notification")]
    public class Notification : Hub
    {

        public List<LiveNotification> NotificationList = new List<LiveNotification>();


        [HubMethodName("SendNotification")]
        public void SendNotification()
        {
            Clients.All.SendNotification(NotificationList);
        }




    }
}