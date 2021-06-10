using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace mytimmings.Hubs
{
    [HubName("notificationHub")]
    public class NotificationHub : Hub
    {


        [HubMethodName("sendNotification")]
        public static void SendNotification()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.receiveNotification();
        }

    }
}