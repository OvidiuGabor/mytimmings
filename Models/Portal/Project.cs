using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class Project
    {
        public int id { get; set; }
        public string  name { get; set; }
        public string userId { get; set; }
        public bool active { get; set; }
        public double workedHours { get; set; }

        public Project(DBContext.User_Assigned_Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project argument cannot be null.");

            id = (int)project.ProjectId;
            name = project.ProjectName;
            userId = project.UserID;
            active = project.Active ?? false;
            workedHours = Convert.ToDouble(project.WorkedHours);

        }

        public Project(int id, string name, string userId)
        {
            this.id = id;
            this.name = name;
            this.userId = userId;

        }
        public Project()
        {

        }
    }
}