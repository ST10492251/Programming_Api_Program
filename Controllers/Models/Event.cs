
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Controllers.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        
        
        // Events can have multiple enrollments
        public ICollection<Enrollment> Enrollments { get; set; }
            = new List<Enrollment>();

        // Stores the ID of the organiser who created the event
        public string? OrganiserId { get; set; }
    }
}