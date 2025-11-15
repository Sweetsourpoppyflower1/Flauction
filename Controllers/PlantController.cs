using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly DBContext _context;

        public PlantController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plant>>> GetPlants()
        {
            return await _context.Plants.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Plant>> GetPlant(int id)
        {
            var plant = await _context.Plants.FindAsync(id);

            if (plant == null)
                return NotFound();

            return plant;
        }

        [HttpPost]
        public async Task<ActionResult<Plant>> CreatePlant(Plant plant)
        {
            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlant), new { id = plant.plant_id }, plant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlant(int id, Plant plant)
        {
            if (id != plant.plant_id)
                return BadRequest();

            _context.Entry(plant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Plants.Any(e => e.plant_id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlant(int id)
        {
            var plant = await _context.Plants.FindAsync(id);

            if (plant == null)
                return NotFound();

            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
