
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
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .AsNoTracking()
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/5
        [HttpGet("{ id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            return Ok(eventItem);
        }

        // POST: api/events
        // Organisers only
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateEvent(
            [FromBody] Event model)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            model.OrganiserId = userId;

            _context.Events.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = model.EventID },
                model);
        }
    }
}