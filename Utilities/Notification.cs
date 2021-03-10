using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Utilities
{
    public class Notification
    {
        public string Type { get; set; }
        public int Id { get; set; }
        public string Message { get; set; }
        public bool ShowToUser { get; set; }

        public Notification(string type, int id, string message, bool showToUser)
        {
            Type = type;
            Id = id;
            Message = message;
            ShowToUser = showToUser;
            
        }


    }
}