using Microsoft.AspNetCore.Mvc;
using MyApp.Data;
using MyApp.Models;
using MyApp.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FoodController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Food
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetAllFoods()
        {
            return await _context.Food.ToListAsync();
        }

        // GET: api/Food/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
            var food = await _context.Food.FindAsync(id);

            if (food == null)
            {
                return NotFound();
            }

            return food;
        }

        // POST: api/Food
        [HttpPost]
        public async Task<ActionResult<Food>> CreateFood(FoodCreateRequest request)
        {
            var food = new Food { Name = request.Name };
            _context.Food.Add(food);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFood", new { id = food.Id }, food);
        }

        // PUT: api/Food/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFood(int id, FoodCreateRequest request)
        {
            var food = await _context.Food.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }

            food.Name = request.Name;
            _context.Entry(food).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Food.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Food/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var food = await _context.Food.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }

            _context.Food.Remove(food);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
