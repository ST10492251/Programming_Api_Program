namespace RaceDayAPI.Controllers.Models
{
    public class Route
    {
        public int RouteID { get; set; }

        public int EventID { get; set; }

        public string RouteName { get; set; } = string.Empty;

        public decimal DistanceKm { get; set; }

        public string? RouteDescription { get; set; }

        public Event EventName { get; set; } = null!;


    }
}