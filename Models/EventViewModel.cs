

using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.Models
{
    public class EventViewModel
    {
        public int EventID { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [Display(Name = "Event Name")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required.")]
        [Display(Name = "Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select an event status.")]
        public string Status { get; set; } = "Draft";

        public int OrganiserId { get; set; }
    }
}
