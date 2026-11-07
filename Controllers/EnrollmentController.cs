using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers.Models;
using RaceDayAPI.Data;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrollmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/enrollments
        // Participants can enrol in an event category.
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> Enroll(
            [FromBody] EnrollmentDto dto)
        {
            var userIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Please log in again."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                return BadRequest(new
                {
                    message = "Please select an event category."
                });
            }

            var eventCategory = await _context.EventCategories
                .Include(ec => ec.Category)
                .FirstOrDefaultAsync(ec =>
                    ec.EventID == dto.EventId &&
                    ec.Category.CategoryName == dto.CategoryName);

            if (eventCategory == null)
            {
                return NotFound(new
                {
                    message = "The selected event category was not found."
                });
            }

            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e =>
                    e.ParticipantID == userId &&
                    e.EventCategoryID == eventCategory.EventCategoryID);

            if (alreadyEnrolled)
            {
                return Conflict(new
                {
                    message = "You are already enrolled in this category."
                });
            }

            var enrolledCount = await _context.Enrollments
                .CountAsync(e =>
                    e.EventCategoryID == eventCategory.EventCategoryID);

            if (enrolledCount >= eventCategory.MaximumParticipants)
            {
                return Conflict(new
                {
                    message = "Sorry, this event category is fully booked."
                });
            }

            var enrollment = new Enrollment
            {
                ParticipantID = userId,
                EventCategoryID = eventCategory.EventCategoryID,
                EnrolmentDate = DateTime.UtcNow,
                Status = "Confirmed"
            };

            _context.Enrollments.Add(enrollment);

            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Enrollment successful.",
                enrollment.EnrolmentID,
                enrollment.ParticipantID,
                enrollment.EventCategoryID,
                CategoryName = eventCategory.Category.CategoryName,
                enrollment.EnrolmentDate,
                enrollment.Status
            });
        }

        // GET: api/enrollments/mine
        // Participants can view their own enrolments.
        [HttpGet("mine")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Please log in again."
                });
            }

            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.ParticipantID == userId)
                .Select(e => new
                {
                    EnrolmentID = e.EnrolmentID,
                    EventID = e.EventCategory.EventID,
                    EventName = e.EventCategory.Event.EventName,
                    EventDate = e.EventCategory.Event.EventDate,
                    Location = e.EventCategory.Event.Location,
                    CategoryName = e.EventCategory.Category.CategoryName,
                    EntryFee = e.EventCategory.EntryFee,
                    EnrolmentDate = e.EnrolmentDate,
                    Status = e.Status
                })
                .OrderByDescending(e => e.EnrolmentDate)
                .ToListAsync();

            return Ok(enrollments);
        }

        // GET: api/enrollments/events/5
        // Organisers can view participants enrolled in an event.
        [HttpGet("events/{eventID:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetEventEnrollments(
            int eventID)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == eventID);

            if (!eventExists)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.EventCategory.EventID == eventID)
                .Select(e => new
                {
                    ID = e.EnrolmentID,

                    CategoryName =
                        e.EventCategory.Category.CategoryName,

                    EnrolledAt = e.EnrolmentDate,

                    Participant = new
                    {
                        UserID = e.Participant.UserID,
                        FirstName = e.Participant.FirstName,
                        LastName = e.Participant.LastName,
                        Email = e.Participant.Email,
                        Phone = e.Participant.Phone
                    }
                })
                .ToListAsync();

            return Ok(enrollments);
        }
    }

    // Data received when a participant enrols.
    public class EnrollmentDto
    {
        public int EventId { get; set; }

        public string CategoryName { get; set; } = string.Empty;
    }
}
