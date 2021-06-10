using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Interfaces
{
    public class Notification : Hub
    {
        string message = String.Empty;
        public void Hello()
        {
            message = "Hello";
            Clients.All.hello(message);
        }
    }
}