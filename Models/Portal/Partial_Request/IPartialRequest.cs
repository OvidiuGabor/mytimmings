using System;

namespace mytimmings.Models.Portal.Partial_Request
{
    public interface IPartialRequest
    {
        string Comments { get; set; }
        TimeSpan Duration { get; set; }
        int ProjectId { get; set; }
        DateTime RequestDate { get; set; }
        DateTime StartTime { get; set; }
        string Type { get; set; }
        string Status { get; set; }
        string Approver { get; set; }
        Security.User User { get; set; }


        void AddRequest();

        void UpdateRequest();

        void ApproveRequest();
        void DeclineRequest();
    }
}