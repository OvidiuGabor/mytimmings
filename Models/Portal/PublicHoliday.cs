using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class PublicHoliday
    {
        public DateTime holidayDate { get; set; }
        public string holidayName { get; set; }
        public string holidayCountry { get; set; }
        public string holidayCountryRegion { get; set; }

        public PublicHoliday(DBContext.Public_Holidays holiday)
        {
            if (holiday == null)
                throw new ArgumentNullException("Paramter cannot be null!");

            holidayDate = holiday.Holiday_date.Value.Date;
            holidayName = holiday.Holiday_name;
            holidayCountry = holiday.Holiday_country;
            holidayCountryRegion = holidayCountryRegion;
        }
    }
}