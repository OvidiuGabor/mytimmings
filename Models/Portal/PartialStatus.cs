//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace mytimmings.Models.Portal
//{
//    public class PartialStatus : Action
//    {

//        //This list will hold the partial statuses that the company has active
//        //based on this list then we can extract from the database the required items
//        public List<string> PartialStatusesAvailable { get; set; }

//        private string CompanyId { get; set; }

//        public PartialStatus(string companyId)
//        {
//            CompanyId = companyId;
//            getStatusesAv();
//        }

//        //This method will return the list of statuses that a company has available.
//        private void getStatusesAv()
//        {
//            DBContext.DBModel db = new DBContext.DBModel();
//            if (CompanyId != null)
//            {
//                var status = db.Companies_Assigned_Items.Where(x => x.Company_ID == CompanyId).Select(x => x.Partial).FirstOrDefault();
//                if (!String.IsNullOrEmpty(status))
//                {
//                    PartialStatusesAvailable = Utilities.Helper.convertStringtoList(status, ';');
//                }
//            }
//            else {
//                PartialStatusesAvailable = new List<string>();
//            }


//        }
//    }
//}