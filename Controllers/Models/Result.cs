using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Controllers.Models
{
    [Table("Results")]
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        [Required]
        public string ResultStatus { get; set; } = string.Empty;
    }
}

