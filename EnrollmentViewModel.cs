using System;

namespace RaceDayMVC.Models
{
    public class EnrollmentViewModel
    {
        public int ID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public DateTime EnrolledAt { get; set; }

        // Event details
        public string EventName { get; set; } = string.Empty;

        public ParticipantViewModel Participant { get; set; }
            = new ParticipantViewModel();
    }

    public class ParticipantViewModel
    {
        public int UserID { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public DateTime? EnrolledAt { get; set; }
    }
}
