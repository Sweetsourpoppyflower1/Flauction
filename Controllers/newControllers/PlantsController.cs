using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flauction.Data;
using Flauction.Models;

namespace Flauction.Controllers.newControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantsController : ControllerBase
    {
        private readonly DBContext _context;

        public PlantsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Plants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plant>>> GetPlants()
        {
            return await _context.Plants.ToListAsync();
        }

        // GET: api/Plants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Plant>> GetPlant(int id)
        {
            var plant = await _context.Plants.FindAsync(id);

            if (plant == null)
            {
                return NotFound();
            }

            return plant;
        }

        // PUT: api/Plants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlant(int id, Plant plant)
        {
            if (id != plant.plant_id)
            {
                return BadRequest();
            }

            var existingPlant = await _context.Plants.FindAsync(id);
            if (existingPlant == null)
            {
                return NotFound();
            }

            // Save price history before updating
            var priceHistory = new PlantPriceHistory
            {
                plant_id = id,
                old_min_price = existingPlant.min_price,
                old_start_price = existingPlant.start_price,
                new_min_price = plant.min_price,
                new_start_price = plant.start_price,
                changed_at = DateTime.UtcNow,
                changed_by = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown",
            };

            _context.PlantPriceHistories.Add(priceHistory);

            // Update plant
            existingPlant.min_price = plant.min_price;
            existingPlant.start_price = plant.start_price;
            _context.Entry(existingPlant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantExists(id))
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

        // POST: api/Plants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Plant>> PostPlant(Plant plant)
        {
            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlant", new { id = plant.plant_id }, plant);
        }

        // DELETE: api/Plants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlant(int id)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant == null)
            {
                return NotFound();
            }

            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Plants/5/price-history
        [HttpGet("{plantId}/price-history")]
        public async Task<ActionResult<IEnumerable<PlantPriceHistory>>> GetPlantPriceHistory(int plantId)
        {
            var priceHistory = await _context.PlantPriceHistories
                .Where(p => p.plant_id == plantId)
                .OrderByDescending(p => p.changed_at)
                .ToListAsync();

            if (!priceHistory.Any())
            {
                return NotFound("No price history found for this plant");
            }

            return Ok(priceHistory);
        }

        private bool PlantExists(int id)
        {
            return _context.Plants.Any(e => e.plant_id == id);
        }
    }
}
