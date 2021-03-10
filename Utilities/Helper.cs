using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mytimmings.Utilities
{
    public class Helper
    {
        public static DBContext.DBModel db = new DBContext.DBModel();

        public static string getConnectionString()
        {
            string connstring = System.Configuration.ConfigurationManager.ConnectionStrings["DBModel"].ConnectionString;
            return connstring;
        }

        #region Methods for Portal
        public static List<SelectListItem> getProjectList() {

            List<SelectListItem> projectList = new List<SelectListItem>();

            List<DBContext.Project> projectsListdb = db.Projects.Where(x => x.isActive == true).ToList(); 
            if(projectsListdb != null)
            {
                SelectListItem defaultItem = new SelectListItem
                {
                    Text = "Please Select a Project",
                    Value = "0"

                };
                projectList.Add(defaultItem);

                foreach(var item in projectsListdb)
                {
                    SelectListItem lstItem = new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()

                    };
                    projectList.Add(lstItem);
                }
            }
            return projectList;
            
        }

        public static List<SelectListItem> getStatuslist()
        {

            List<SelectListItem> statusList = new List<SelectListItem>();

            List<DBContext.Params> paramsListdb = db.Params.Where(x => x.Identifier == "Status" && x.Param2 == "True").ToList();
            if (paramsListdb != null)
            {
                SelectListItem defaultItem = new SelectListItem
                {
                    Text = "Please Select a Status",
                    Value = "0"

                };
                statusList.Add(defaultItem);

                foreach (var item in paramsListdb)
                {
                    SelectListItem lstItem = new SelectListItem
                    {
                        Text = item.Param1,
                        Value = item.Param1

                    };
                    statusList.Add(lstItem);
                }
            }
            return statusList;

        }

        public static bool isTotalTimeHigherThenMaxTime(List<Models.Portal.DayRecord> records, int maxTime)
        {
            int currentTime = 0;
            if (records.Count() < 1)
                throw new ArgumentOutOfRangeException("There should be at lease one record!");

            foreach(var record in records)
            {
                currentTime += record.totalTime;
            }

            if (currentTime > maxTime)
                return false;
            else
                return true;

        }

        #endregion

        public static DateTime convertToUTC(DateTime dateTimeToConvert)
        {
            if (dateTimeToConvert == null)
                throw new ArgumentNullException("Date/Time supplied is null!");

            return dateTimeToConvert.ToUniversalTime();
        }

        public static DateTime removeMiliseconds(DateTime date)
        {
            if(date == null)
                throw new ArgumentNullException("Date/Time supplied is null!");

            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
        }


        public static Dictionary<string, string> getColorCoding(Models.Security.User user)
        {
            Dictionary<string, string> colorList = new Dictionary<string, string>();

            List<DBContext.Params> colorsDb = db.Params.Where(x => x.Identifier == "ColorCoding" & x.Active == true & x.Company == user.Company).Distinct().ToList();

            foreach(var item in colorsDb)
            {
                colorList.Add(item.Param1, item.Param2);
            }




            return colorList;
        }


    }
}