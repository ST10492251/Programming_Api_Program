using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Controllers.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        public int OrganiserId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Status { get; set; }

        // Events can have multiple enrollments
        public ICollection<Enrollment> Enrollments { get; set; }
            = new List<Enrollment>();
    }
}
