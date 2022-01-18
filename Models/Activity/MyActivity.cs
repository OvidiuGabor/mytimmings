using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Activity
{
    public class MyActivity
    {
        public List<Portal.WorkRecord> workLogs = new List<Portal.WorkRecord>();
        public List<string> projects = new List<string>();
        public List<Models.Portal.Leave.ILeave> leaves = new List<Portal.Leave.ILeave>();
    }
}