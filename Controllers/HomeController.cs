using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace mytimmings.Controllers
{
    public class HomeController : Controller
    {

        public DBContext.DBModel dbmodel = new DBContext.DBModel();
        //public DBContext.DBModel dbmodel = null;

        public ActionResult Index()
        {
            Session["errorMessage"] = null;
            if (TempData["loginError"] is ErrorManagement.LoginError error)
            {
                Session["errorMessage"] = error.errorMessage;
            }

            return View();
        }
        [HttpPost]
        public ActionResult UserLogin(Models.Security.LoginModel vm)
        {
            bool isMailvalid = false;
            if(vm != null)
            {
                if(vm.loginID == null || vm.loginID.Length < 1 || vm.loginPassword == null || vm.loginPassword.Length < 1)
                {
                    ErrorManagement.LoginError loginError = ErrorManagement.LoginError.CreateModel("EmailIssue", true, "The Login Credential are not correct!");
                    TempData["loginError"] = loginError;
                    return RedirectToAction("index", "Home");
                }
                //since the user can connect using the email address and the id, we need to validate first if the email address is correct
                //if it is, then we search based on the email addreess, if not, we try to search based on the id, if the id is not valid also, then we failed and return tu user
                //validate the Email Address by using the MailAddress class from .Net Framework.
                //if the email Address received from the view is accurate, then it will create a instance of the MailAddress class. If not will fail    
                //MailAddress mailAddress = new MailAddress(vm.loginID);
                //isMailvalid = (mailAddress.Address == vm.loginID);
                if (vm.loginID.Contains("@")) { isMailvalid = true; };

                if (isMailvalid)
                {

                    if (ModelState.IsValid)
                    {
                        //try to get the details from the database
                        DBContext.User user = dbmodel.Users.Where(x => x.EmailAddress == vm.loginID && x.Password == vm.loginPassword).FirstOrDefault();
                        if(user != null)
                        {
                            //Get the settings for the useer
                           // Models.Security.UserSettings userSettings = new Models.Security.UserSettings(dbmodel.User_Settings.Where(x => x.ID == user.ID).FirstOrDefault());
                            Models.Security.User userModel = Models.Security.User.CreateUser(user);
                            Models.Security.AuthState userState = new Models.Security.AuthState(userModel, vm.timezone);
                            userState.LogIn();
                            SetUserSession(userModel, userState);

                            return RedirectToAction("Overview", "Portal");
                        }
                        else
                        {
                            //Create Error Model
                            ErrorManagement.LoginError loginError = ErrorManagement.LoginError.CreateModel("EmailIssue", true, "The Login Credential are not correct!");
                            TempData["loginError"] = loginError;
                            return RedirectToAction("index", "Home");
                        }
                    }
                    //if the model is not valid, return back to user
                    else
                    {
                        ErrorManagement.LoginError loginError = ErrorManagement.LoginError.CreateModel("EmailIssue", true, "The Login Credential are not correct!");
                        TempData["loginError"] = loginError;
                        return RedirectToAction("index","Home");
                    }
                }
                else //get user by the ID since the email failed.
                {
                    DBContext.User user = dbmodel.Users.Where(x => x.ID == vm.loginID && x.Password == vm.loginPassword).FirstOrDefault();
                    if (user != null)
                    {
                        //Get the settings for the useer
                        //Models.Security.UserSettings userSettings = new Models.Security.UserSettings(dbmodel.User_Settings.Where(x => x.ID == user.ID).FirstOrDefault());
                        Models.Security.User userModel = Models.Security.User.CreateUser(user);
                        Models.Security.AuthState userState = new Models.Security.AuthState(userModel, vm.timezone);
                        userState.LogIn();

                        SetUserSession(userModel, userState);

                        return RedirectToAction("Overview", "Portal");
                    }
                    else
                    {
                        ErrorManagement.LoginError loginError = ErrorManagement.LoginError.CreateModel("IDIssue", true, "The Login Credential are not correct!");
                        TempData["loginError"] = loginError;
                        return RedirectToAction("index", "Home");
                    }
                }

            }
            else
            {
                ErrorManagement.LoginError loginError = ErrorManagement.LoginError.CreateModel("Datasend", true, "The Login Credential are not correct!");
                TempData["loginError"] = loginError;
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }
        #region Set Session Variables
        private void SetUserSession(Models.Security.User user, Models.Security.AuthState userState)
        {
            Session["User"] = user;
            Session["DisplayName"] = user.FirstName;
            Session["AuthState"] = userState;
        }
        public  Models.Security.User GetUserSession()
        {
            return Session["User"] as Models.Security.User;
        }

        public void ClearSession()
        {
            Session["User"] = null;
        }
        #endregion
      
    }
}