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
        public DateTime currentDate { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string status { get; set; }
        public int projectId { get; set; }
        public string projectName { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public List<string> tags { get; set; }



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
            projectId = (int)record.Project_ID;
            projectName = record.Project_Name;
            title = record.Title;
            description = record.Description;
            CreateTagsList(record.Tags);


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

                foreach(string item in tempTags)
                {
                    if (!item.Contains('#') && !String.IsNullOrWhiteSpace(item))
                    {
                        tags.Add( "#" + item);
                    }
                }
            }
           
          
            
        }
    }
}