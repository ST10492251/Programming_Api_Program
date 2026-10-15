
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
        // Logged-in participants can enter events
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

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == dto.EventId);

            if (!eventExists)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            var enrollment = new Enrollment
            {
                UserID = userId,
                EventID = dto.EventId,
                CategoryName = dto.CategoryName.Trim(),
                EnrolledAt = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Enrollment successful.",
                enrollment.ID,
                enrollment.UserID,
                enrollment.EventID,
                enrollment.CategoryName,
                enrollment.EnrolledAt
            });
        }

        // GET: api/enrollments/events/5
        // Organisers can view participants enrolled in an event
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
                .Where(e => e.EventID == eventID)
                .Include(e => e.User)
                .Select(e => new
                {
                    e.ID,
                    e.CategoryName,
                    e.EnrolledAt,
                    Participant = new
                    {
                        e.User.UserID,
                        e.User.FirstName,
                        e.User.LastName,
                        e.User.Email,
                        e.User.Phone
                    }
                })
                .ToListAsync();

            return Ok(enrollments);
        }
    }

    public class EnrollmentDto
    {
        public int EventId { get; set; }

        public string CategoryName { get; set; } = string.Empty;
    }
}