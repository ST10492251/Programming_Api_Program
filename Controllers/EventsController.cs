
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
<<<<<<< HEAD
        [HttpGet("{id:int}")]
=======
        [HttpGet("{ id:int}")]
>>>>>>> 1ca75e54721b6f6e0cb19dc8518f6ba6ae7ee484
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

<<<<<<< HEAD
            if (!int.TryParse(userId, out int organiserId))
=======
            if (string.IsNullOrEmpty(userId))
>>>>>>> 1ca75e54721b6f6e0cb19dc8518f6ba6ae7ee484
            {
                return Unauthorized();
            }

<<<<<<< HEAD
            model.OrganiserId = organiserId;
=======
            model.OrganiserId = userId;
>>>>>>> 1ca75e54721b6f6e0cb19dc8518f6ba6ae7ee484

            _context.Events.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = model.EventID },
                model);
<<<<<<< HEAD
       
       }

        // PUT: api/events/5
        // Organisers only
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] Event model)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
            {
                return Unauthorized();
            }

            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (existingEvent == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            // Ensure organisers can only update their own events.
            if (existingEvent.OrganiserId != organiserId)
            {
                return Forbid();
            }

            existingEvent.EventName = model.EventName;
            existingEvent.EventDate = model.EventDate;
            existingEvent.Location = model.Location;
            existingEvent.Description = model.Description;
            existingEvent.Status = model.Status;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event updated successfully.",
                eventDetails = existingEvent
            });
        }

    }
}
=======
        }
    }
}
>>>>>>> 1ca75e54721b6f6e0cb19dc8518f6ba6ae7ee484
