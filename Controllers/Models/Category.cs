namespace RaceDayAPI.Controllers.Models
{
    public class Category
    {
            public int CategoryID { get; set; }

            public string CategoryName { get; set; } = string.Empty;

            public string? Description { get; set; }

            // Relationships
            public ICollection<EventCategory> EventCategories { get; set; }
                = new List<EventCategory>();
        }
    }

