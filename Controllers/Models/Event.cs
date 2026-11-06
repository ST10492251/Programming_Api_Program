<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
=======
﻿
using System.ComponentModel.DataAnnotations;
>>>>>>> 1ca75e54721b6f6e0cb19dc8518f6ba6ae7ee484

namespace RaceDayAPI.Controllers.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

<<<<<<< HEAD
        public int OrganiserId { get; set; }

=======
>>>>>>> 1ca75e54721b6f6e0cb19dc8518f6ba6ae7ee484
        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

<<<<<<< HEAD
        public string? Description { get; set; }

        public string? Status { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
            = new List<Enrollment>();
=======
        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        
        
        // Events can have multiple enrollments
        public ICollection<Enrollment> Enrollments { get; set; }
            = new List<Enrollment>();

        // Stores the ID of the organiser who created the event
        public string? OrganiserId { get; set; }
>>>>>>> 1ca75e54721b6f6e0cb19dc8518f6ba6ae7ee484
    }
}