using Microsoft.Extensions.Logging;

namespace RaceDayAPI.Controllers.Models
{
    // Models/User.cs
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } // "Organiser" or "Participant"
    }

}