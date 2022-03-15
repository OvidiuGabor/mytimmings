using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class WorkRecord
    {
        private int recordId { get; }
        private string userId { get; }
        public string timeZone { get; set; }
        public DateTime currentDate { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string status { get; set; }
        public int projectId { get; set; }
        public string projectName { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string tagsforDb { get; set; }
        public string tagsColorforDb { get; set; }
        public List<string> tags { get; set; }
        public List<string> tagsColors { get; set; }



        public WorkRecord(DBContext.Main_Data record)
        {
            if (record == null)
                throw new ArgumentNullException("The provided record cannot be null!");

            recordId = (int)record.ID;
            userId = userId;
            currentDate = record.CurrentDate;
            startDate = record.Status_Start_Time;
            endDate = record.Status_End_Time;
            status = record.Status;
            projectId = record.Project_ID.HasValue == true ? (int)record.Project_ID.Value : 0;
            projectName = record.Project_Name == null ? getProjectName(projectId) : record.Project_Name;
            title = record.Title == null ? "No Title" : record.Title;
            description = record.Description;

            CreateTagsList(record.Tags);
            CreateTagsColorList(record.TagsColors);
            checkForTagsAndColors();

        }

        public WorkRecord()
        {

        }
        public WorkRecord(DateTime recordDate, DateTime startTime, DateTime endTime, string status, int projectId, string title, string description, string tags, string tagsColor, string timeZone)
        {
            currentDate = recordDate;
            startDate = startTime;
            endDate = endTime;
            this.status = status;
            this.projectId = projectId;
            this.title = title;
            this.description = description;
            tagsforDb = tags;
            tagsColorforDb = tagsColor;
            this.timeZone = timeZone;
            getProjectNameFromId();

            CreateTagsList(tags);
            CreateTagsColorList(tagsColor);
            checkForTagsAndColors();
        }



        public int InsertRecordIntoDb(Models.Security.User user)
        {

            if(user != null)
            {
               return  InsertRecord(GenerateDbRecord(user));
            }
            else
            {
                throw new ArgumentNullException("User argument is null!");
            }
        }

        private DBContext.Main_Data GenerateDbRecord(Models.Security.User user)
        {
            return new DBContext.Main_Data
            {
                User_ID = user.ID,
                CurrentDate = this.currentDate,
                Status_Start_Time = startDate,
                Status_End_Time = endDate,
                Status = status,
                Project_ID = projectId,
                Project_Name = projectName,
                Title = title,
                Description = description,
                Tags = tagsforDb,
                TagsColors = tagsColorforDb,
                User_Time_Zone = timeZone
                

            };
        }
        private int InsertRecord(DBContext.Main_Data record)
        {
       
            DBContext.DBModel db = new DBContext.DBModel();
           
            db.Main_Data.Add(record);
            db.SaveChanges();
            int id = (int)record.ID;
            return id;
        }

        private void CreateTagsList(string tag)
        {
            tags = new List<string>();
            List<string> tempTags = new List<string>();

            if (!String.IsNullOrEmpty(tag))
            {
                if (tag.Length > 0)
                {
                    tempTags = tag.Split('#').ToList();
                }

                foreach (string item in tempTags)
                {
                    if (!item.Contains('#') && !String.IsNullOrWhiteSpace(item))
                    {
                        tags.Add("#" + item);
                    }
                }
            }



        }

        private void CreateTagsColorList(string colors)
        {
            tagsColors = new List<string>();

            if (!String.IsNullOrEmpty(colors))
            {
                List<string> tempList = new List<string>();
                if (colors.Contains(";"))
                {
                    tempList = colors.Split(';').ToList();
                }
                else
                {
                    tempList.Add(colors);
                }
                    
                tagsColors.AddRange(tempList);
            }

        }

        private string getProjectName(int projectId)
        {
            string projectName = "";
            DBContext.DBModel db = new DBContext.DBModel();
            projectName = db.Projects.Where(x => x.Id == projectId).Select(x => x.Name).FirstOrDefault();
            if (projectName == null)
                projectName = "Other";



            return projectName;

        }

        //here we check that the number of items in tags list and in tagColors list to be fine
        //if they are the same we leave them 
        //if there are less tags rather the colors, it is ok also
        //if there are more tags then colors, then we get the differece of tags from the params table
        private void checkForTagsAndColors()
        {
            int tagsCount = tags.Count();
            int colorsCount = tagsColors.Count();

            if (tagsCount >  colorsCount)
            {
                int limit = tagsCount - colorsCount;
                DBContext.DBModel db = new DBContext.DBModel();
                string defaultColors = db.Params.Where(x => x.Identifier == "Colors").Select(x => x.Param1).FirstOrDefault();
                if (!String.IsNullOrEmpty(defaultColors))
                {
                    var colorList = defaultColors.Split(';');
                    int counter = 0;
                    foreach(var color in colorList)
                    {
                        if (counter >= limit)
                            break;

                        if (!tagsColors.Contains(color))
                        {
                            tagsColors.Add(color);
                        }
                        counter++;
                    }
                }

            }




        }   
        private void getProjectNameFromId()
        {
            DBContext.DBModel db = new DBContext.DBModel();
            var projectName = db.User_Assigned_Projects.Where(x => x.ProjectId == this.projectId).Select(x => x.ProjectName).FirstOrDefault();
            if (string.IsNullOrEmpty(projectName))
                throw new ArgumentException("You are not assign to the project!");


            this.projectName = projectName;
        }
    }
}