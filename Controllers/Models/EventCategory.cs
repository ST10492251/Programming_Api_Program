namespace RaceDayAPI.Controllers.Models
{
    public class EventCategory
    {
      
            public int EventCategoryID { get; set; }

            public int EventID { get; set; }

            public int CategoryID { get; set; }

            public decimal EntryFee { get; set; }

            public int MaximumParticipants { get; set; }

            // Relationships
            public Event Event { get; set; } = null!;

            public Category Category { get; set; } = null!;

            public ICollection<Enrollment> Enrolments { get; set; }
                = new List<Enrollment>();
        }
    }
