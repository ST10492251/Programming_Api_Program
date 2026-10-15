namespace RaceDayAPI.Controllers.Models
{
    public class whetherRecord
    {
            public int WeatherRecordID { get; set; }

            public int EventID { get; set; }

            public decimal? Temperature { get; set; }

            public string? WeatherCondition { get; set; }

            public decimal? WindSpeed { get; set; }

            public DateTime RecordedAt { get; set; }

            // Relationship
            public Event Event { get; set; } = null!;
        }
    }
