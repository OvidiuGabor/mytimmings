using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mytimmings.Models.Request
{
    //based on this class will create the view
    // the class will contain one request, the user, and the dropdows for the form.
    // i am doing this, so that i can return multiple information to the view!
    public class RequestContainerForView
    {
        private DBContext.DBModel db = new DBContext.DBModel();
        // Create a new enpty request, so i can instantiate the fields that i need in the fiew
        public Request request = new Request();
        // get the settings for the user
        public Security.UserSettings UserSettings { get; set; }
        // get the user and set it as private, because will only use it inside this class
        private Security.User User { get; set; }
        public string ManagerName { get; set; }

        public List<SelectListItem> LeaveList = new List<SelectListItem>();

        public RequestContainerForView(Security.User user, Security.UserSettings userSettings)
        {
            if (user == null)
                throw new ArgumentNullException("The User cannot be null.");

            if (userSettings == null)
                throw new ArgumentNullException("The User Settings object cannot be null.");

            UserSettings = userSettings;
            User = user;
            LeaveList = getLeaveTypes();
            ManagerName = getMangerName();
        }


        //generate the leave list from the db
        private List<SelectListItem> getLeaveTypes()
        {

            List<SelectListItem> lst = new List<SelectListItem>();
            if (User == null)
            {
                lst.Add(new SelectListItem { Text = "N/A", Value = "Invalid" });
                return lst;
            }
            List<DBContext.Params> leavesLstFromDb = db.Params.Where(x => x.Identifier == "Request" && x.Param2 == "Leave" && x.Active == true && x.Company == User.Company).ToList();

            //Add a default item
            lst.Add(new SelectListItem { Text = "Select Item", Value = "Invalid" });
            if (leavesLstFromDb.Count > 0)
            {
                foreach(var record in leavesLstFromDb)
                {
                    lst.Add(new SelectListItem { Text = record.Param3, Value = record.Id.ToString() });
                }
            }



            return lst;

        }

        //set the manager name that will be show on the view;
        private string getMangerName()
        {
            string managerName = "";
            if (User == null)
            {
                managerName = "Unknow";
                return managerName;
            }

            DBContext.User manager = db.Users.Find(User.ManagerID);
            if(manager == null)
            {
                managerName = "Unknow";
                return managerName;
            }

            managerName = manager.LastName + " " + manager.FirstName;

            return managerName;
        }
    }
}