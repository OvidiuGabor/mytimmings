﻿using System;
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
            //Decrypt the connection String
            string  decryptedString = PasswordProtect.Decrypt.DecryptText(connstring, true);

            return decryptedString;
        }

        #region Methods for Portal
        public static List<SelectListItem> getProjectList(string userID) {

            List<SelectListItem> projectList = new List<SelectListItem>();

            List<DBContext.User_Assigned_Project> projectsListdb = db.User_Assigned_Projects.Where(x => x.Active == true && x.UserID == userID).ToList(); 
            if(projectsListdb != null)
            {
                SelectListItem defaultItem = new SelectListItem
                {
                    Text = "Choose Project",
                    Value = "0"

                };
                projectList.Add(defaultItem);

                foreach(var item in projectsListdb)
                {
                    SelectListItem lstItem = new SelectListItem
                    {
                        Text = item.ProjectName,
                        Value = item.Id.ToString()

                    };
                    projectList.Add(lstItem);
                }
            } 
            return projectList;
            
        }

        public static List<SelectListItem> getStatuslist(string companyId)
        {

            List<SelectListItem> statusList = new List<SelectListItem>();
            SelectListItem defaultItem = new SelectListItem
            {
                Text = "Choose Status",
                Value = "0"
            };
            statusList.Add(defaultItem);
            Dictionary<string, List<string>> statuses = getStatusTypes(companyId);
            foreach(var item in statuses)
            {
                if(item.Value.Count > 0)
                {
                    foreach(string el in item.Value)
                    {
                        statusList.Add(new SelectListItem
                        {
                            Text = el,
                            Value = el,
                           
                        });
                    }
                }
            }


            

            return statusList;

        }

        public static Dictionary<string, List<string>> getStatusTypes(string companyId)
        {
            Dictionary<string, List<string>> statuses = new Dictionary<string, List<string>>();
            var companyItem = db.Companies_Assigned_Items.Where(x => x.Company_ID == companyId).FirstOrDefault();

            if(companyItem != null)
            {
                List<string> temp = new List<string>();
                temp = (convertStringtoList(companyItem.Productive_Actions, ';'));
                statuses.Add("Productive", temp);
                temp = (convertStringtoList(companyItem.Non_Productive_Actions, ';'));
                statuses.Add("Non Productive", temp);
                temp = (convertStringtoList(companyItem.Other, ';'));
                statuses.Add("Other", temp);

            }
            else
            {

                var dbList = db.Params.Where(x => x.Identifier == "Action" && x.Active == true).ToList();
                foreach(var item in dbList)
                {
                    if(item.Param2 == "Productive")
                        if (statuses.ContainsKey("Productive"))
                        {
                            statuses["Productive"].Add(item.Param1);
                        }
                        else
                        {
                            statuses.Add("Productive", new List<string> { item.Param1 });
                        }
                    if (item.Param2 == "Non Productive")
                        if (statuses.ContainsKey("Non Productive"))
                        {
                            statuses["Non Productive"].Add(item.Param1);
                        }
                        else
                        {
                            statuses.Add("Non Productive", new List<string> { item.Param1 });
                        }
                    if (item.Param2 == "Other")
                        if (statuses.ContainsKey("Other"))
                        {
                            statuses["Other"].Add(item.Param1);
                        }
                        else
                        {
                            statuses.Add("Other", new List<string> { item.Param1 });
                        }

                }
            }

            return statuses;
          
        }
        public static string getCompanyId(string userID)
        {
            if (String.IsNullOrEmpty(userID))
                throw new ArgumentNullException("String cannot be null or Empty.");

            return db.Users.Where(x => x.ID == userID).Select(x => x.Company).FirstOrDefault();
        }

        public static List<string> convertStringtoList(string stringToconvert, char delimiter) 
        {
            List<string> newList = new List<string>();

            if (String.IsNullOrEmpty(stringToconvert) || Char.IsWhiteSpace(delimiter))
                return newList;


            string[] newString = stringToconvert.Split(delimiter);
            newList = newString.ToList();



            return newList;
        
        
        }

        //Generate an requestId when the user submits a request
        public static string generateRequestId(string requestType)
        {
            string requestId = "REQ";
            int retry = 0;
            try
            {
                requestId += requestType.Substring(0, 1);
                requestId = createRequestString(requestId);
                retry++;

                //in order to check if the id exists, we are going to make a request inot the db, and if the result is null, then the id is good.
                //if we get a result, then the id is not good and we start again, so that the number will increase.
                //after 5 retry, we can send it as a error;
                var checkId = db.PartialTime_Requests.Where(x => x.RequestId == requestId).FirstOrDefault();

                if(checkId != null)
                {
                    if(retry > 5)
                    {
                        throw new InvalidOperationException("Cannot generate a new Id for the partial time request");
                    }
                    generateRequestId(requestType);
                }
            }
            catch (Exception)
            {
                if (retry > 5)
                {
                    throw new InvalidOperationException("Cannot generate a new Id for the partial time request");
                }
                retry++;
                    
                generateRequestId(requestType);
               
            }


            return requestId;
        }


        private static string createRequestString(string partialID)
        {
            int countRec = db.PartialTime_Requests.Count() +1;

            string convertedInt = countRec.ToString();
            int characterscount = convertedInt.Length;
            string finalId = partialID;
            for (var i = 0; i < (6 -characterscount); i++)
            {
                finalId += "0";
            }
            finalId += convertedInt;
            return finalId;
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

            List<DBContext.Params> colorsDb = db.Params.Where(x => x.Identifier == "ColorCoding" & x.Active == true).Distinct().ToList();

            foreach(var item in colorsDb)
            {
                colorList.Add(item.Param1, item.Param2);
            }




            return colorList;
        }

        //This method will convert thwe datestring (yyyy-mm-ddThh:mm) into a date format
        public static DateTime CalculateDateFromString(string date)
        {
            if (String.IsNullOrEmpty(date))
                throw new ArgumentNullException("String Date is null!");
            try
            {
                DateTime convertedDate = DateTime.ParseExact(date.Substring(0, (date.Length - 6)), "yyyy-MM-ddTHH:mm:ss", null);
                return convertToUTC(convertedDate);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static string StripDangerousCharacters(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                var temp = str.Trim();
                temp.Replace("  ", "");
                return temp;
            }


            return "";
        }

        public static double ConvertSecondsToHours(double seconds)
        {
            if (seconds <= 0)
                return 0;


            return Math.Round((seconds / 3600), 2);
        }



        public static int CalculateBusinessDays(DateTime start, DateTime end, params DateTime[] BankHolidays)
        {
            var firstDay = start.Date;
            var lastDay = end.Date;

            if (firstDay > lastDay)
                throw new ArgumentException("Incorect last day, cannot be lower then the start! " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekDays = businessDays / 7;

            if (businessDays > fullWeekDays * 7)
            {
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;

                if (lastDayOfWeek < firstDayOfWeek)
                {
                    lastDayOfWeek += 7;
                }

                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)
                        businessDays -= 1;

                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)
                {
                    businessDays -= 1;
                }
            }

            businessDays -= fullWeekDays + fullWeekDays;


            foreach (var holiday in BankHolidays)
            {
                DateTime bh = holiday.Date;
                if (firstDay <= bh && bh <= lastDay)
                {
                    businessDays--;
                }
            }

            return businessDays;

        }

    }
}