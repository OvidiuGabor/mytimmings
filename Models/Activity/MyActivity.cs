using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Activity
{
    public class MyActivity : ICloneable
    {
        public List<Portal.WorkRecord> workLogs = new List<Portal.WorkRecord>();
        public List<Portal.Project> projects = new List<Portal.Project>();
        public List<Models.Portal.Leave.ILeave> leaves = new List<Portal.Leave.ILeave>();
        public int numberOfItems { get; set; }


        public MyActivity(List<Portal.WorkRecord> worklogs, List<Portal.Leave.ILeave> leaves, List<Models.Portal.Project> projects)
        {
            this.workLogs = worklogs;
            this.leaves = leaves;
            this.projects = projects;
            numberOfItems = 5;
        }

        protected MyActivity(Models.Activity.MyActivity myActivity)
        {
            this.workLogs = myActivity.workLogs;
            this.projects = myActivity.projects;
            this.leaves = myActivity.leaves;
            this.numberOfItems = myActivity.numberOfItems;
        }

        public MyActivity()
        {

        }

        public object Clone()
        {
            return new MyActivity(this);
        }
    }
}