using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace mytimmings.Controllers
{
    public class PortalController : Controller
    {
        // GET: Portal
        private DBContext.DBModel db = new DBContext.DBModel();
        public ActionResult Index()
        {
            if (Session["User"] != null)
            {
                Models.Portal.Portal portal = new Models.Portal.Portal();
                Models.Portal.DayOverview currentDayOverView = new Models.Portal.DayOverview();
                List<Models.Portal.DayRecord> recordsForToday = new List<Models.Portal.DayRecord>();
                //Need to extract the user settings in order to get the value for the max date.
                //Mostlikely this will be done to the home Controller, when we get the user and save in into Session.
                #region GetuserSettings
                int maxhours = 9;
                #endregion
                DateTime currentDate = Utilities.Helper.convertToUTC(DateTime.Now);
                //Get a list of all records that are in the database for today
                //Exclude the records with isrunning = false, and status End Day, because that means that the day has ended
                List<DBContext.Main_Data> currentRecords = db.Main_Data.Where(x => DbFunctions.TruncateTime(x.CurrentDate) == DbFunctions.TruncateTime(currentDate) && x.Current_Status != "End Day").ToList();
                if (currentRecords.Count != 0)
                {
                    foreach (var item in currentRecords)
                    {
                        Models.Portal.DayRecord day = Models.Portal.DayRecord.CreateReocrd(item);
                        recordsForToday.Add(day);
                    }


                    currentDayOverView = Models.Portal.DayOverview.CreateModel(recordsForToday, maxhours);
                    DBContext.Main_Data lastRecord = currentRecords.OrderByDescending(x => x.Status_Start_Time).FirstOrDefault();
                    decimal projectId = lastRecord.ProjectID ?? 0;
                    string projectIdString = projectId.ToString();
                    portal.project = projectIdString;
                    portal.projectName = db.Projects.Find(projectId).Name;
                    portal.status = lastRecord.Current_Status;
                }
                portal.CurrentDayOverview = currentDayOverView;
                portal.daysList = recordsForToday;
                return View(portal);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }


        #region Clocks Start and End

        public JsonResult StartClock(Models.Portal.Portal vm)
        {

            DateTime currentDate = Utilities.Helper.convertToUTC(DateTime.Now);
            //Check first if the day has been Closed. IF so then show error message that is closed, and need to contact the manager to reset it!
            DBContext.Main_Data endDay = db.Main_Data.Where(x => DbFunctions.TruncateTime(x.CurrentDate) == DbFunctions.TruncateTime(currentDate) && x.Current_Status == "End Day").FirstOrDefault();
            if (endDay != null)
            {
                return Json(new { result = "Error", message = "Clock is stopped for today. Contact your Manager!" }, JsonRequestBehavior.AllowGet);
            }
            //Before starting we need to check if there has been added a new record for today. Return an error message if there is
            List<DBContext.Main_Data> currentRecords = db.Main_Data.Where(x => DbFunctions.TruncateTime(x.CurrentDate) == DbFunctions.TruncateTime(currentDate)).ToList();
            if (currentRecords.Count == 0)
            {
                //Check if the data from the model is accurate, and it is different from 0- which is the default model
                if (vm.project == "0" || vm.status == "0")
                {
                    string projectError = "";
                    string statusError = "";

                    if (vm.project == "0" || vm.project == null || vm.project == "")
                    {
                        projectError = "Please select a project.";
                    }

                    if (vm.status == "0" || vm.status == null || vm.status == "")
                    {
                        statusError = "Please select a status.";
                    }

                    return Json(new { result = "Error", message = projectError + " "  + statusError }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    //Get the user from the session
                    Models.Security.User user = Session["User"] as Models.Security.User;
                    //create an instance of the object Day Record that will hold the new record
                    Models.Portal.DayRecord newRecord = Models.Portal.DayRecord.CreateModel(user.ID, vm.project, vm.status, "");

                    DBContext.Main_Data record = Models.Portal.DayRecord.InsertNewRecord(newRecord);
                    db.Main_Data.Add(record);
                    db.SaveChanges();


                }
            }
            else
            {
                return Json(new { result = "Error", message = "Clock already started for today!" }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { result = "Success", message = "Start Clock!" }, JsonRequestBehavior.AllowGet);
        }

        //Controller for Stoping the clock for today
        public JsonResult StopClock()
        {
            //check first if the day has started
            //cannot stop a day if it doesnt started yet
            DateTime currentDate = Utilities.Helper.convertToUTC(DateTime.Now);
            List<DBContext.Main_Data> currentRecords = db.Main_Data.Where(x => DbFunctions.TruncateTime(x.CurrentDate) == DbFunctions.TruncateTime(currentDate)).ToList();
            DBContext.Main_Data DayClosed = db.Main_Data.Where(x => DbFunctions.TruncateTime(x.CurrentDate) == DbFunctions.TruncateTime(currentDate) && x.Current_Status == "End Day").FirstOrDefault();
            if (currentRecords.Count == 0)
            {
                return Json(new { result = "Error", message = "The clock has not been started for today!" }, JsonRequestBehavior.AllowGet);
            }
            else if (DayClosed != null)
            {
                return Json(new { result = "Error", message = "Clock has already been closed for today!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Models.Security.User user = Session["User"] as Models.Security.User;

                //Select first the last record and update the End time with the current datetime
                //need to do this to make sure all records for today has been ended
                try
                {
                    DBContext.Main_Data lastData = currentRecords.OrderByDescending(x => x.Status_Start_Time).FirstOrDefault();
                    lastData.Status_End_Time = currentDate;
                    lastData.IsRunning = false;
                    db.SaveChanges();



                    //we need to select all records for today, and mark them as isrunning = false
                    //Add an record that say the day has been eneded
                    Models.Portal.DayRecord finalRecord = Models.Portal.DayRecord.CreateModel(lastData.userID, lastData.ProjectID.ToString(), "End Day", "");
                    finalRecord.endDate = DateTime.Now;

                    DBContext.Main_Data finalRecordDB = Models.Portal.DayRecord.InsertNewRecord(finalRecord);
                    finalRecordDB.IsRunning = false;
                    db.Main_Data.Add(finalRecordDB);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return Json(new { result = "Errir", message = "Something Happen!" }, JsonRequestBehavior.AllowGet);
                }
               

            }
            //update the user Settings with the isDayopen to False
            Models.Security.UserSettings userSettings = GetUserSettingsSession();
            userSettings.SetDayStatus(false);
            SetUserSettingsSession(userSettings);

            return Json(new { result = "Success", message = "Stop Clock!" }, JsonRequestBehavior.AllowGet);
        }

        //this handles the Update Status button, for when a user is changing the project or the status of the day
        public JsonResult UpdateStatus(Models.Portal.Portal vm)
        {
            //Need to check first if the day has been started, and also if the day has not been closed.
            //Cannot updated a status if the day is not started, or if the day has been closed.
            DateTime currentDate = Utilities.Helper.convertToUTC(DateTime.Now);
            Models.Security.User user = Session["User"] as Models.Security.User;
            if (user != null)
            {
                List<DBContext.Main_Data> currentRecords = db.Main_Data.Where(x => DbFunctions.TruncateTime(x.CurrentDate) == DbFunctions.TruncateTime(currentDate)).ToList();
                if (currentRecords.Count != 0)
                {
                    foreach (var item in currentRecords)
                    {
                        if (item.Current_Status == "End Day")
                        {
                            return Json(new { result = "Error", message = "Clock is stopped for today. Contact your Manager!" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (vm.status != "0" && vm.project != "0")
                    {
                        //Get last record and se the end time
                        //insert new record withe the new status and project if changed.
                        DBContext.Main_Data lastRecord = currentRecords.OrderByDescending(x => x.Status_Start_Time).FirstOrDefault();
                        if (lastRecord.IsRunning == true)
                        {
                            lastRecord.Status_End_Time = currentDate;
                            db.SaveChanges();
                        }


                        Models.Portal.DayRecord newRecord = Models.Portal.DayRecord.CreateModel(user.ID, vm.project, vm.status, "");
                        DBContext.Main_Data addRecord = Models.Portal.DayRecord.InsertNewRecord(newRecord);
                        db.Main_Data.Add(addRecord);
                        db.SaveChanges();


                    }
                    else
                    {
                        string projectError = "";
                        string statusError = "";

                        if (vm.project == "0" || vm.project == null || vm.project == "")
                        {
                            projectError = "Please select a project.";
                        }

                        if (vm.status == "0" || vm.status == null || vm.status == "")
                        {
                            statusError = "Please select a status.";
                        }

                        return Json(new { result = "Error", message = projectError +  " " + statusError }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { result = "Error", message = "Need to start the clock first!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { redirectUrl = Url.Action("Index, Home"), isRedirect = true });
            }

            return Json(new { result = "Success", message = "Updated!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddAditionalHours(Models.Portal.Portal vm)
        {

            string projectError = "";
            string statusError = "";
            bool error = false;
            DateTime currentDate = Utilities.Helper.convertToUTC(DateTime.Now);
            if (vm.project == "0" || vm.project == null || vm.project == "")
            {
                projectError = "Please select a project.";
                error = true;
            }

            if (vm.status == "0" || vm.status == null || vm.status == "")
            {
                statusError = "Please select a status.";
                error = true;
            }
            if (error)
            {
                return Json(new { result = "Error", projectError, statusError }, JsonRequestBehavior.AllowGet);
            }


            Models.Security.User user = Session["User"] as Models.Security.User;
            //Create day model

            //Get the last record and change hthe status to nor running and update the End Time
            //check also if the record is not already added
            DBContext.Main_Data lastRecord = db.Main_Data.Where(x => DbFunctions.TruncateTime(x.CurrentDate) == DbFunctions.TruncateTime(currentDate)).OrderByDescending(x => x.Status_Start_Time).FirstOrDefault();
            if (lastRecord.IsRunning == true && lastRecord.Current_Status == "Extra Time")
            {
                return Json(new { result = "Success", message = "Record Allready exist." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                lastRecord.Status_End_Time = currentDate;
                lastRecord.IsRunning = false;
                db.SaveChanges();

                //Add a new record
                Models.Portal.DayRecord newRecord = Models.Portal.DayRecord.CreateModel(lastRecord.userID, vm.project, vm.status, "Automatically Added Extra Time");
                DBContext.Main_Data addRecord = Models.Portal.DayRecord.InsertNewRecord(newRecord);
                db.Main_Data.Add(addRecord);
                db.SaveChanges();
            }



            return Json(new { result = "Success", message = "Extra Time Added" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region EditRecord
        [HttpPost]
        public ActionResult EditRecord(string id)
        {
            //need to select all records for today
            //Edit the one that has the Id send from the view/
            //show a list off all other records for today, that can be edited
            List<Models.Portal.DayRecord> recordsList = new List<Models.Portal.DayRecord>();
            if (id != null || id != "")
            {
                Int32.TryParse(id, out int recordId);
                DBContext.Main_Data currentRecordDB = db.Main_Data.Find(recordId);
                if (currentRecordDB != null)
                {

                    Models.Portal.DayRecord currentRecord = Models.Portal.DayRecord.CreateReocrd(currentRecordDB);
                    recordsList.Add(currentRecord);

                    //We need to extract all other recrds for the current day, and display them 

                    List<DBContext.Main_Data> recordsListDB = db.Main_Data.OrderBy(y => y.Status_Start_Time).Where(x => DbFunctions.TruncateTime(x.CurrentDate) == DbFunctions.TruncateTime(currentRecord.currentDate) && x.ID != currentRecordDB.ID).ToList();
                    if (recordsListDB.Count > 0)
                    {
                        foreach (var item in recordsListDB)
                        {
                            Models.Portal.DayRecord createRecord = Models.Portal.DayRecord.CreateReocrd(item);
                            recordsList.Add(createRecord);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                return RedirectToAction("index", "ErrorHandler");
            }

            return View(recordsList);
        }
        [HttpGet]
        public ActionResult EditRecord()
        {
            if (Session["User"] != null)
            {
                List<Models.Portal.CalendarRecord> records = new List<Models.Portal.CalendarRecord>();

                Models.Security.User user = GetUserSession();
                //Get user Time Zone
                string UserTimeZone = Session["TimeZone"].ToString();
                DateTime currentDate = DateTime.Now;
                Dictionary<string, string> colorCoding = Utilities.Helper.getColorCoding(user);

                //Get user data
                List<DBContext.Main_Data> data = db.Main_Data.Where(x => x.userID == user.ID
                                            && x.CurrentDate.Year == currentDate.Year && x.CurrentDate.Month == currentDate.Month && x.CurrentDate.Day == currentDate.Day
                                            && x.Current_Status != "End Day")
                                            .OrderBy(x => x.Status_Start_Time).ToList();
                if (data != null)
                {
                    foreach (var item in data)
                    {

                        var record = new Models.Portal.CalendarRecord(item, UserTimeZone, colorCoding);

                        //if (item.Current_Status == "End Day")
                        //{
                        //    record.setEndDayInfo(data.Where(x => x.Status_Start_Time.Year == item.Status_Start_Time.Year && x.Status_Start_Time.Month == item.Status_Start_Time.Month && x.Status_Start_Time.Day == item.Status_Start_Time.Day).ToList());
                        //}

                        records.Add(record);
                    }
                }

                //Get the Status from the Params table
                Dictionary<string, string> statuses = new Dictionary<string, string>();

                List<string> statusFromDb = db.Params.Where(x => x.Identifier == "Status" && x.Company == user.Company).Select(x => x.Param1).ToList();
                foreach(var item in statusFromDb)
                {
                    var color = db.Params.Where(x => x.Identifier == "ColorCoding" && x.Param1 == item && x.Company == user.Company).Select(x => x.Param2).FirstOrDefault();
                    statuses.Add(item, color);
                }


                TempData["status"] = statuses;
                return View(records);
            }
            else
            {
                return RedirectToAction("index", "home");
            }
            
        }

        //public ActionResult SaveRecord(Models.Portal.DayRecord recordFromTheView)
        //{
        //    if (recordFromTheView != null)
        //    {
        //        bool startTimeOK = true;
        //        bool endTimeOK = true;
        //        var dayRecord = new Models.Portal.DayRecord();
        //        List<Models.Portal.DayRecord> recordsList = dayRecord.getAllRecordsForCurrentDay(recordFromTheView);
        //        DateTime currentDate = DateTime.UtcNow;
        //        Models.Security.UserSettings userSettings = Session["UserSettings"] as Models.Security.UserSettings;

        //        //Format the dates received to UTC time zone
        //        recordFromTheView.startDate = Utilities.Helper.convertToUTC(recordFromTheView.startDate);
        //        if(recordFromTheView.endDate != null)
        //            recordFromTheView.endDate = Utilities.Helper.convertToUTC(recordFromTheView.endDate.Value);

        //        //Check if all the data exists, if not return error
        //        if (recordFromTheView.startDate != null)
        //        {

        //            if (recordFromTheView.startDate > currentDate)
        //                ModelState.AddModelError("startDate", "Cannot add a time in the future!");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("startDate", "The date cannot be Empty");
        //        }
                


        //        if (ModelState.IsValid)
        //        {
        //            //check first if there are any records before of after this record
        //            //if there is a record before: 
        //            //if there is previous record, then modify the end time of that record to mach the start time of this record
        //            //if there is next record and mofidy the end time, then modity also the start time of the next record, to match this time.
        //            //do not allow, if the previous or next record is not finished(doesn't have end time)!!!

        //            //get following record

        //            var recordFromtheDb = db.Main_Data.Where(x => x.ID == recordFromTheView.id).FirstOrDefault();
        //            // check if start time has been modify
        //            if (DateTime.Equals(Utilities.Helper.removeMiliseconds(recordFromtheDb.Status_Start_Time), recordFromTheView.startDate) == false)
        //            {

        //                //Get previous record
        //                var previousRecord = recordsList.Where(x => x.id < recordFromTheView.id).FirstOrDefault();
        //                if (previousRecord != null)
        //                {
        //                    //get the record fromt he DB
        //                    DBContext.Main_Data prevRecordDB = db.Main_Data.Where(x => x.ID == previousRecord.id).FirstOrDefault();

        //                    if(prevRecordDB.Status_Start_Time >= recordFromTheView.startDate)
        //                    {
        //                        //Add error saying that the record cannot be modify, because the end time, is higher then the next item end time!!

        //                        startTimeOK = false;
        //                    }
        //                    else
        //                    {
                               
        //                        prevRecordDB.Status_End_Time = recordFromTheView.startDate;
        //                        //db.SaveChanges();
        //                        //add nottidications
        //                        recordFromtheDb.Status_Start_Time = recordFromTheView.startDate;


        //                        startTimeOK = true;

        //                    }
        //                }
        //                else
        //                {
                            
        //                    recordFromtheDb.Status_Start_Time = recordFromTheView.startDate;
        //                    startTimeOK = true;
        //                }

        //                Models.Portal.DayRecord newDay = Models.Portal.DayRecord.CreateReocrd(recordFromtheDb);
        //                recordsList.RemoveAll(x =>x.id == recordFromTheView.id);
        //                recordsList.Add(newDay);
        //                //Check Here if the new total time will be higher then the user max allowed time, and decline the changes if it is higher!
        //                bool checkTotalTime = Utilities.Helper.isTotalTimeHigherThenMaxTime(recordsList, (userSettings.TotalTime * 3600));
        //                if (checkTotalTime)
        //                    startTimeOK = true;
        //                else
        //                    startTimeOK = false;
        //            }

        //            // check if End time has been modify
        //            if (DateTime.Equals(Utilities.Helper.removeMiliseconds(recordFromtheDb.Status_End_Time.Value), recordFromTheView.endDate) == false)
        //            {

        //                //Get Next record
        //                var followRecord = recordsList.Where(x => x.id > recordFromTheView.id).FirstOrDefault();
        //                //Check Here if the new total time will be higher then the user max allowed time, and decline the changes if it is higher!




        //                if (followRecord != null)
        //                {
        //                    DBContext.Main_Data followRecordFromDb = db.Main_Data.Where(x => x.ID == followRecord.id ).FirstOrDefault();
        //                    if (followRecordFromDb.Status_End_Time == null)
        //                    {
        //                        //Cannot cahnge the End Time because you have an activity that is not closed!
        //                        endTimeOK = false;
        //                    }
        //                    else if(followRecordFromDb.Status_End_Time > recordFromTheView.endDate)
        //                    {
                              
        //                        followRecordFromDb.Status_Start_Time = recordFromTheView.endDate.Value;
        //                        recordFromtheDb.Status_End_Time = recordFromTheView.endDate;
        //                        //db.SaveChanges();
        //                        //add notifications
        //                        endTimeOK = true;

        //                    }
        //                    else
        //                    {
        //                        //Add error message saying that the end time cannot be changed, because the next activity end time is lower. and will make record invalid
        //                        endTimeOK = false;
        //                    }

        //                }
        //                else
        //                {
        //                    recordFromtheDb.Status_End_Time = recordFromTheView.endDate;
        //                    endTimeOK = true;
        //                }

        //                Models.Portal.DayRecord newDay = Models.Portal.DayRecord.CreateReocrd(recordFromtheDb);
        //                recordsList.RemoveAll(x =>x.id == recordFromTheView.id);
        //                recordsList.Add(newDay);
        //                //Check Here if the new total time will be higher then the user max allowed time, and decline the changes if it is higher!
        //                bool checkTotalTime = Utilities.Helper.isTotalTimeHigherThenMaxTime(recordsList, (userSettings.TotalTime * 3600));
        //                if (checkTotalTime)
        //                    endTimeOK = true;
        //                else
        //                    endTimeOK = false;

        //            }


        //            if (endTimeOK && startTimeOK)
        //            {


        //                db.SaveChanges();
        //                recordsList = dayRecord.getAllRecordsForCurrentDay(recordFromTheView);
        //            }

        //            return RedirectToAction("EditRecord", new RouteValueDictionary(new {controller = "Portal", action = "EditRecord", id = recordFromTheView.id.ToString() }));
        //        }
        //        else
        //        {
        //            return View(recordsList);
        //        }

        //    }
        //    else
        //    {
        //        //Redirect To Error page
        //    }

        //    return View();
        //}

        public JsonResult UpdateRecords( List<Models.Portal.Event> data)
        {
            bool errorsFound = false;
            if(data.Count < 1 || data == null)
                return Json(new { result = "Error", message = "No records Received!" }, JsonRequestBehavior.AllowGet);

            //get user from session!
            Models.Security.User user = GetUserSession();
            if (user == null)
                return Json(new { result = "Error", message = "Session Expired" }, JsonRequestBehavior.AllowGet);

            //create a list where we will store the recrods for updating the database!
            List<DBContext.Main_Data> records = new List<DBContext.Main_Data>();
            List<Models.Portal.Event> eventsErrored = new List<Models.Portal.Event>();
         
            foreach(var item in data)
            {
                //if we encounter eny error while we are trying to process the events from the view, those events get storred into an errored array, and we send it back to the user to check them
                //do now allow saving if there are any errors into the data!
                #region Exclude record that doesant have the correct status
                var statusExist = db.Params.Where(x => x.Identifier == "Status" && x.Company == user.Company && x.Param1 == item.Status && x.Param2 == "True").FirstOrDefault();
                if(statusExist != null)
                {
                    try
                    {
                        var record = new Models.Portal.DayRecord(item, user);
                        var recordFordb = Models.Portal.DayRecord.InsertNewRecord(record);
                        records.Add(recordFordb);
                    }
                    catch (Exception)
                    {
                        eventsErrored.Add(item);
                        errorsFound = true;


                    }
                }

                #endregion
               

            }
            //if we didnt find any errors, then we try to save the data
            //else if there are errors we return the errored listback to the user!
            if (!errorsFound)
            {
                try
                {
                    //First delete all data for the day
                    //then add the new records
                    List<DBContext.Main_Data> todayRecords = db.Main_Data.Where(x => x.userID == user.ID && x.CurrentDate.Year == DateTime.Now.Year && x.CurrentDate.Month == DateTime.Now.Month && x.CurrentDate.Day == DateTime.Now.Day
                                        && x.Current_Status != "End Day").ToList();
                    db.Main_Data.RemoveRange(todayRecords);

                    db.Main_Data.AddRange(records);
                    db.SaveChanges();

                    return Json(new { result = "Success", message = "Records Saved!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { result = "Error", message = ex }, JsonRequestBehavior.AllowGet);

                }

            }
            else
            {
                return Json(new { result = "Error", message = eventsErrored }, JsonRequestBehavior.AllowGet);
            }


        }
        //This Action will delete all records from the DB for the current day!
        //if the user click clear calendar, there is no way back, all records will be deleted!
        public JsonResult ClearEvents()
        {
            DateTime currentDate = DateTime.Now;
            List<DBContext.Main_Data> currentDayRecords = db.Main_Data.Where(x => x.CurrentDate.Year == currentDate.Year && x.CurrentDate.Month == currentDate.Month && x.CurrentDate.Day == currentDate.Day).ToList();
            if(currentDayRecords.Count  == 0)
                return Json(new { result = "Error", message = "No Record Found!" }, JsonRequestBehavior.AllowGet);

            try
            {
                db.Main_Data.RemoveRange(currentDayRecords);
                db.SaveChanges();
                return Json(new { result = "Success", message = "Day Updated!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { result = "Error", message = "Something Happen! Try Again!" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        public ActionResult History()
        {
            //we check if the user session is still available, if not redirect ro the login page
            if (Session["User"] == null)
                return RedirectToAction("Index", "Home");

            //get the user id, then we extract all the data for the user from the database
            //then we format it in order to be able to pass it to the Calendar and display it.
            List<Models.Portal.CalendarRecord> records = new List<Models.Portal.CalendarRecord>();
            
            Models.Security.User user = GetUserSession();
            //Get user Time Zone
            string UserTimeZone = Session["TimeZone"].ToString();
            //Get Color Coding
            Dictionary<string, string> colorCoding= Utilities.Helper.getColorCoding(user);

            //Get user data
            List<DBContext.Main_Data> data = db.Main_Data.Where(x => x.userID == user.ID).OrderBy(x => x.Status_Start_Time).ToList();
            if(data != null)
            {
                foreach(var item in data)
                {

                    var record = new Models.Portal.CalendarRecord(item, UserTimeZone, colorCoding);

                    if(item.Current_Status == "End Day")
                    {
                        record.setEndDayInfo(data.Where(x => x.Status_Start_Time.Year == item.Status_Start_Time.Year && x.Status_Start_Time.Month == item.Status_Start_Time.Month && x.Status_Start_Time.Day == item.Status_Start_Time.Day).ToList());
                    }



                    records.Add(record);
                }
            }
    




            return View(records);
        }

        public Models.Security.User GetUserSession()
        {
            return Session["User"] as Models.Security.User;
        }
        public Models.Security.UserSettings GetUserSettingsSession()
        {
            return Session["UserSettings"] as Models.Security.UserSettings;
        }
        private void SetUserSettingsSession( Models.Security.UserSettings settings)
        {
            Session["UserSettings"] = settings;
        }
    }
}