using System;

namespace mytimmings.Models.Portal.Leave
{
    public interface ILeave
    {
        DateTime requestDate { get; set; }
        DateTime end { get; set; }
        int numberofDays { get; set; }
        DateTime start { get; set; }
        string type { get; set; }
        string status { get; set; }

        int CalculateNumberOfBusinessDays(DateTime start, DateTime end, params DateTime[] bankHolidays);
    }
}