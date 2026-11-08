using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Controllers.Models
{
    [Table("Enrolments")]
    public class Enrollment
    {
        [Key]
        public int EnrolmentID { get; set; }

        public int EventCategoryID { get; set; }

        public int ParticipantID { get; set; }

        public DateTime? EnrolmentDate { get; set; }

        public string? Status { get; set; }

        [ForeignKey(nameof(EventCategoryID))]
        public EventCategory EventCategory { get; set; } = null!;

        [ForeignKey(nameof(ParticipantID))]
        public User Participant { get; set; } = null!;
    }
}