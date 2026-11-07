
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers.Models;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            return Ok(category);
        }

        // POST: api/Categories
        // Organisers only
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory(
            [FromBody] Category model)
        {
            if (string.IsNullOrWhiteSpace(model.CategoryName))
            {
                return BadRequest(new
                {
                    message = "Category name is required."
                });
            }

            model.CategoryName = model.CategoryName.Trim();

            if (model.CategoryName.Length > 100)
            {
                return BadRequest(new
                {
                    message = "Category name cannot exceed 100 characters."
                });
            }

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = model.CategoryID },
                model);
        }

        // PUT: api/Categories/5
        // Organisers only
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] Category model)
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (existingCategory == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            if (string.IsNullOrWhiteSpace(model.CategoryName))
            {
                return BadRequest(new
                {
                    message = "Category name is required."
                });
            }

            model.CategoryName = model.CategoryName.Trim();

            if (model.CategoryName.Length > 100)
            {
                return BadRequest(new
                {
                    message = "Category name cannot exceed 100 characters."
                });
            }

            existingCategory.CategoryName = model.CategoryName;
            existingCategory.Description = model.Description;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Category updated successfully.",
                category = existingCategory
            });
        }

        // DELETE: api/Categories/5
        // Organisers only
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (existingCategory == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            bool isAssigned = await _context.EventCategories
                .AnyAsync(ec => ec.CategoryID == id);

            if (isAssigned)
            {
                return Conflict(new
                {
                    message = "This category is assigned to an event and cannot be deleted."
                });
            }

            _context.Categories.Remove(existingCategory);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Category deleted successfully."
            });
        }
    }
}
