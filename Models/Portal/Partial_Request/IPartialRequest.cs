using System;

namespace mytimmings.Models.Portal.Partial_Request
{
    public interface IPartialRequest
    {
        string Comments { get; set; }
        TimeSpan Duration { get; set; }
        int ProjectId { get; set; }
        DateTime StartDate { get; set; }
        DateTime StartTime { get; set; }
        string Type { get; set; }


        void AddRequest();

        void DeleteRequest();

        void UpdateRequest();
    }
}