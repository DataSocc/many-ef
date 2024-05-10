using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.DTOs;
using MyApp.Models;

namespace MyApp.Controllers;

[ApiController]
[Route("[controller]")]
public class DinnerController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DinnerController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Dinner>>> GetAllDinners()
    {
        var dinners = await _context.Dinner.Include(d => d.Foods).ToListAsync();
        if (!dinners.Any())
        {
            return NotFound("No dinners found.");
        }
        return Ok(dinners);
    }


    // Criar Dinner sem Food
    [HttpPost]
    public async Task<ActionResult<Dinner>> CreateDinner(DinnerCreateRequest request)
    {
        var dinner = new Dinner { Name = request.Name, Foods = new List<Food>() };
        _context.Dinner.Add(dinner);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDinner), new { id = dinner.Id }, dinner);
    }

    // Obter Dinner por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Dinner>> GetDinner(int id)
    {
        var dinner = await _context.Dinner.Include(d => d.Foods).FirstOrDefaultAsync(d => d.Id == id);
        if (dinner == null)
        {
            return NotFound();
        }
        return dinner;
    }

    // Atualizar Dinner (apenas o nome)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDinner(int id, DinnerCreateRequest request)
    {
        var dinner = await _context.Dinner.FirstOrDefaultAsync(d => d.Id == id);
        if (dinner == null)
        {
            return NotFound();
        }
        dinner.Name = request.Name;
        _context.Entry(dinner).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Deletar Dinner
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDinner(int id)
    {
        var dinner = await _context.Dinner.FindAsync(id);
        if (dinner == null)
        {
            return NotFound();
        }
        _context.Dinner.Remove(dinner);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Adicionar Food ao Dinner
    [HttpPost("{dinnerId}/foods/{foodId}")]
    public async Task<IActionResult> AddFoodToDinner(int dinnerId, int foodId)
    {
        var dinner = await _context.Dinner.Include(d => d.Foods).FirstOrDefaultAsync(d => d.Id == dinnerId);
        if (dinner == null)
        {
            return NotFound($"Dinner with ID {dinnerId} not found.");
        }

        var food = await _context.Food.FindAsync(foodId);
        if (food == null)
        {
            return NotFound($"Food with ID {foodId} not found.");
        }

        // Verifica se o Food já está associado a este Dinner
        if (dinner.Foods.Any(f => f.Id == food.Id))
        {
            return BadRequest("This food is already added to the dinner.");
        }

        dinner.Foods.Add(food);
        await _context.SaveChangesAsync();
        return NoContent();
    }


    // Remover Food de Dinner
    [HttpDelete("{dinnerId}/foods/{foodId}")]
    public async Task<IActionResult> RemoveFoodFromDinner(int dinnerId, int foodId)
    {
        var dinner = await _context.Dinner.Include(d => d.Foods).FirstOrDefaultAsync(d => d.Id == dinnerId);
        if (dinner == null)
        {
            return NotFound();
        }
        var food = dinner.Foods.FirstOrDefault(f => f.Id == foodId);
        if (food == null)
        {
            return NotFound();
        }
        dinner.Foods.Remove(food);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
