namespace RaceDayAPI.Controllers.Models
{
    public class Result
    {
            public int ResultID { get; set; }

            public int EnrolmentID { get; set; }

            public TimeSpan? FinishTime { get; set; }

            public int? Position { get; set; }

            public string ResultStatus { get; set; } = string.Empty;

            // Relationship
            public Enrollment Enrolment { get; set; } = null!;
        }
    }

