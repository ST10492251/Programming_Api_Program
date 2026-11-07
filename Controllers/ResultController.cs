using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Controllers.Models;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET PARTICIPANT RESULTS
        // GET: api/Results/mine
        // ==========================================

        [HttpGet("mine")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Please log in again."
                });
            }

            var results = await (
                from r in _context.Results
                join e in _context.Enrollments
                    on r.EnrolmentID equals e.EnrolmentID
                join ec in _context.EventCategories
                    on e.EventCategoryID equals ec.EventCategoryID
                join ev in _context.Events
                    on ec.EventID equals ev.EventID
                join c in _context.Categories
                    on ec.CategoryID equals c.CategoryID
                where e.ParticipantID == userId
                select new
                {
                    ResultID = r.ResultID,
                    EnrolmentID = e.EnrolmentID,
                    EventID = ev.EventID,
                    EventName = ev.EventName,
                    EventDate = ev.EventDate,
                    Location = ev.Location,
                    CategoryName = c.CategoryName,
                    FinishTime = r.FinishTime,
                    Position = r.Position,
                    ResultStatus = r.ResultStatus
                })
                .OrderBy(r => r.EventDate)
                .ToListAsync();

            return Ok(results);
        }

        // ==========================================
        // ADD OR UPDATE PARTICIPANT RESULT
        // POST: api/Results
        // ==========================================

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> SaveResult(
            [FromBody] SaveResultDto request)
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int organiserId))
            {
                return Unauthorized(new
                {
                    message = "Please log in again."
                });
            }

            if (request.EnrolmentID <= 0)
            {
                return BadRequest(new
                {
                    message = "A valid enrolment ID is required."
                });
            }

            if (request.Position.HasValue && request.Position <= 0)
            {
                return BadRequest(new
                {
                    message = "Position must be greater than zero."
                });
            }

            if (string.IsNullOrWhiteSpace(request.ResultStatus))
            {
                return BadRequest(new
                {
                    message = "Result status is required."
                });
            }

            // Check enrolment and event ownership.
            var enrolment = await _context.Enrollments
                .Where(e => e.EnrolmentID == request.EnrolmentID)
                .Select(e => new
                {
                    e.EnrolmentID,
                    OrganiserID = e.EventCategory.Event.OrganiserId
                })
                .FirstOrDefaultAsync();

            if (enrolment == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            if (enrolment.OrganiserID != organiserId)
            {
                return Forbid();
            }

            // Find an existing result.
            var result = await _context.Results
                .FirstOrDefaultAsync(r =>
                    r.EnrolmentID == request.EnrolmentID);

            bool isNewResult = result == null;

            if (isNewResult)
            {
                result = new Result
                {
                    EnrolmentID = request.EnrolmentID
                };

                _context.Results.Add(result);
            }

            // Save result information.
            result.FinishTime = request.FinishTime;
            result.Position = request.Position;
            result.ResultStatus = request.ResultStatus.Trim();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = isNewResult
                    ? "Result added successfully."
                    : "Result updated successfully.",

                result.ResultID,
                result.EnrolmentID,
                result.FinishTime,
                result.Position,
                result.ResultStatus
            });
        }
    }

    // ==========================================
    // RESULT REQUEST MODEL
    // ==========================================

    public class SaveResultDto
    {
        public int EnrolmentID { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        public string ResultStatus { get; set; } = string.Empty;
    }
}
