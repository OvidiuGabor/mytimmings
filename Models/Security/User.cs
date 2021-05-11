using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace mytimmings.Models.Security
{
    public class User
    {
        #region Define user properties
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ID { get; set; }
        public string Country { get; set; }
        public string Company { get; set; }
        public string ManagerID { get; set; }
        public string Password { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }
        public string PhotoPath { get; set;}
        public bool isUser { get; set; }
        public bool isManager { get; set; }
        public bool isAdmin { get; set; }
        public bool isDeveloper { get; set; }

        #endregion

        #region Define user methods
        public User() { }

        public static User CreateUser(DBContext.User vmUser)
        {
            User newUser = new User();

            
            foreach(PropertyInfo key in newUser.GetType().GetProperties())
            {
                string propertyname = key.Name;

                if (vmUser.GetType().GetProperty(propertyname) != null)
                {
                    string propertyType = vmUser.GetType().GetProperty(propertyname).PropertyType.Name;

                    if (propertyType == "Boolean")
                    {
                        if(vmUser.GetType().GetProperty(propertyname).GetValue(vmUser) == null)
                        {
                            key.SetValue(newUser, false);
                        }
                        else
                        {
                            key.SetValue(newUser, vmUser.GetType().GetProperty(propertyname).GetValue(vmUser));
                        }
                    }else if (propertyType == "Nullable`1")
                    {
                        if (vmUser.GetType().GetProperty(propertyname).GetValue(vmUser) == null)
                        {
                            key.SetValue(newUser, DateTime.Now);
                        }
                        else
                        {
                            key.SetValue(newUser, vmUser.GetType().GetProperty(propertyname).GetValue(vmUser));
                        }
                    }
                    else
                    {
                        if (vmUser.GetType().GetProperty(propertyname).GetValue(vmUser) == null)
                        {
                            key.SetValue(newUser, "");
                        }
                        else
                        {
                            key.SetValue(newUser, vmUser.GetType().GetProperty(propertyname).GetValue(vmUser));
                        }
                    }

                   
                    
                }

            }

            return newUser;
        }
        #endregion
    }
}