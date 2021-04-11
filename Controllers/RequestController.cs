using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mytimmings.Controllers
{
    public class RequestController : Controller
    {
        private DBContext.DBModel db = new DBContext.DBModel();
        // GET: Request
        public ActionResult Index()
        {

            //Instantiate the Container
            Models.Security.User user = GetUserSession();
            Models.Security.UserSettings userSettings = GetUserSettingsSession();

            if (user ==  null)
                 return RedirectToAction("index", "home");
            try
            {
                Models.Request.RequestContainerForView model = new Models.Request.RequestContainerForView(user, userSettings);
                return View(model);
            }
            catch (Exception)
            {
                //Log the error maybe?!!
                return RedirectToAction("index", "home");
            }
            

           
        }





        #region get the Session Details
        private Models.Security.UserSettings GetUserSettingsSession()
        {
            return Session["UserSettings"] as Models.Security.UserSettings;
        }

        private Models.Security.User GetUserSession()
        {
            return Session["User"] as Models.Security.User;
        }
        #endregion
    }
}