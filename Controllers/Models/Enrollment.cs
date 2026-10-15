namespace RaceDayAPI.Controllers.Models
{
    // Models/Enrollment.cs
    using System.ComponentModel.DataAnnotations;

    public class Enrollment
    {
        [Key]
        public int ID{ get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public int EventID{ get; set; }
        public Event Event { get; set; }

        public string CategoryName { get; set; } // e.g., "Under 20", "10km", "Senior"
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
