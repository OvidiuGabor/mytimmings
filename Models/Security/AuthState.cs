using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Security
{
    public class AuthState
    {
        Models.Security.User User = new User();
        private bool IsLogedIn { get; set; }
        private bool IsLogedOut { get; set; }

        public string TimeZone { get; set; }

        public UserSettings UserSettings = new UserSettings();
        public AuthState(User user, string timezone)
        {
            User = user;
            TimeZone = timezone;
            GetUserSettings();
        }


        public void LogIn()
        {
            IsLogedIn = true;
            IsLogedOut = false;
        }
        public void LogOut()
        {
            IsLogedOut = true;
            IsLogedIn = false;
            User = null;
        }

        public bool IsUserLogged()
        {
            if (IsLogedIn)
                return true;


            return false;
        }

        private void GetUserSettings()
        {
            DBContext.DBModel db = new DBContext.DBModel();
            var settings = db.User_Settings.Find(User.ID);
            if (settings != null)
                UserSettings = new UserSettings(settings);
        }
    }
}