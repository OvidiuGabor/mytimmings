using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class CalendarRecord
    {
        public string Title { get; set; }
        public string  Start { get; set; }
        public string End { get; set; }
        public bool AllDay { get; set; }
        public bool Overlap { get; set; }
        public string Color { get; set; }
        public string Comment { get; set; }
        public int ProjectId { get; set; }

        private Dictionary<string, string> ColorCoding { get; set; }
        public CalendarRecord(DBContext.Main_Data record, string timezone, Dictionary<string, string> colorCoding)
        {
            if (record == null)
                throw new ArgumentNullException("The record cannot be null");

            AllDay = false;
            Title = record.Current_Status;
            Start = ConvertDataToString(ConvertToUserTimeZone(record.Status_Start_Time, timezone));
            End = ConvertDataToString(ConvertToUserTimeZone(record.Status_End_Time, timezone));
            Overlap = false;

            if (record.Current_Status == "End Day")
                AllDay = true;


            ColorCoding = colorCoding;
            Color = getEventColor();
            Comment = record.Comments;
            ProjectId = (int)record.ProjectID;
             
        }

        private string ConvertDataToString(DateTime? value)
        {
            string convertedDate = null;
            if(value != null && value.HasValue)
                convertedDate = value.Value.Year + "-" + value.Value.ToString("MM") + "-" + value.Value.ToString("dd") + "T" + value.Value.ToString("HH") + ":" + value.Value.ToString("mm") + ":" + value.Value.ToString("ss");

            return convertedDate;

        }

        private DateTime? ConvertToUserTimeZone(DateTime? validDate, string timeZoneOffset)
        {

            int timeZone = 0;
            if (!String.IsNullOrEmpty(timeZoneOffset))
                int.TryParse(timeZoneOffset, out timeZone);


            if (validDate == null)
                return null;

            DateTime dateTime = validDate.Value - new TimeSpan(timeZone / 60, timeZone % 60, 0);

            return dateTime;
        }

        private string getEventColor()
        {
            string color = null;

            if (ColorCoding.Count > 1)
                color = ColorCoding.Where(x => x.Key == Title).Select(x => x.Value).FirstOrDefault();
            //if there is no color defined into the database we get the default onwe
            if(color == null)
                color = ColorCoding.Where(x => x.Key == "Default").Select(x => x.Value).FirstOrDefault();
            //if we cannot find a default into the database we set it as Purple
            if (color == null)
                color = "purple";

            return color;

        }



        public void setEndDayInfo(List<DBContext.Main_Data> todayRecords)
        {

            string output="";
            var totalProductiveMiliseconds = 0;
            var totalNonProcutiveMiliseconds = 0;
            var totalWorkMiliseconds = 0;

            //get a list of the statuses
            DBContext.DBModel db = new DBContext.DBModel();

            List<DBContext.Params> dbStatus = db.Params.Where(x => x.Identifier == "Status" && x.Active == true).ToList();
            //Loop within each Record
            //check which type of activity is
            //get total time in miliseconts format
            //if the activity cannot be foud, by default we set is as non Productive
            foreach(var record in todayRecords.Where(x =>x.Current_Status != "End Day"))
            {
                
                var activityType = dbStatus.Where(x => x.Param1 == record.Current_Status).Select(x => x.Param3).FirstOrDefault();
                if (activityType == null)
                    activityType = "Non Productive";


                if(activityType == "Productive")
                {
                    //get time difference between start and end
                    if(record.Status_End_Time != null && record.Status_Start_Time != null)
                    {
                        TimeSpan timediff = record.Status_End_Time.Value - record.Status_Start_Time;
                        totalProductiveMiliseconds += (int)timediff.TotalMilliseconds;
                        totalWorkMiliseconds += (int)timediff.TotalMilliseconds;
                    }
                    else
                    {
                        throw new ArgumentNullException("Time canot be calculated");
                    }
                    

                }
                if (activityType == "Non Productive")
                {
                    //get time difference between start and end
                    if (record.Status_End_Time != null && record.Status_Start_Time != null)
                    {
                        TimeSpan timediff = record.Status_End_Time.Value - record.Status_Start_Time;
                        totalNonProcutiveMiliseconds += (int)timediff.TotalMilliseconds;
                        totalWorkMiliseconds += (int)timediff.TotalMilliseconds;
                    }
                    else
                    {
                        throw new ArgumentNullException("Time canot be calculated");
                    }


                }


            }
            TimeSpan productiveTime = TimeSpan.FromMilliseconds(totalWorkMiliseconds);
            output = @"Prod: " + string.Format("{0:D2}:{1:D2}:{2:D2}", TimeSpan.FromMilliseconds(totalProductiveMiliseconds).Hours, TimeSpan.FromMilliseconds(totalProductiveMiliseconds).Minutes, TimeSpan.FromMilliseconds(totalProductiveMiliseconds).Seconds)
                 + " Non Prod: " + string.Format("{0:D2}:{1:D2}:{2:D2}", TimeSpan.FromMilliseconds(totalNonProcutiveMiliseconds).Hours, TimeSpan.FromMilliseconds(totalNonProcutiveMiliseconds).Minutes, TimeSpan.FromMilliseconds(totalNonProcutiveMiliseconds).Seconds)
                 + " Total : " + string.Format("{0:D2}:{1:D2}:{2:D2}", TimeSpan.FromMilliseconds(totalWorkMiliseconds).Hours, TimeSpan.FromMilliseconds(totalWorkMiliseconds).Minutes, TimeSpan.FromMilliseconds(totalWorkMiliseconds).Seconds);



            Title = output;

        }


    }
}