using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flauction.DTOs.Output.ModelDTOs;

namespace Flauction.Controllers.modelControllers
{
    [Route("api/[controller]")]
    [Route("api/Plants")]
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

        [HttpGet("dto")]
        public async Task<ActionResult<IEnumerable<PlantDTO>>> GetPlantDTOs()
        {
            var plantDTOs = await _context.Plants
                .Join(_context.Suppliers,
                    p => p.supplier_id,
                    s => s.supplier_id,
                    (p, s) => new PlantDTO
                    {
                        PlantId = p.plant_id,
                        SupplierName = s.s_name,
                        ProductName = p.productname,
                        Category = p.category,
                        Form = p.form,
                        Quality = p.quality,
                        MinStem = p.min_stem,
                        StemsBunch = p.stems_bunch,
                        Maturity = p.maturity,
                        Description = p.desc
                    })
                .ToListAsync();

            return Ok(plantDTOs);
        }

        [HttpGet("dto/{id}")]
        public async Task<ActionResult<Plant>> GetPlantDTO(int id)
        {
            var plantDTO = await _context.Plants
                .Where(p => p.plant_id == id)
                .Join(_context.Suppliers,
                    p => p.supplier_id,
                    s => s.supplier_id,
                    (p, s) => new PlantDTO
                    {
                        PlantId = p.plant_id,
                        SupplierName = s.s_name,
                        ProductName = p.productname,
                        Category = p.category,
                        Form = p.form,
                        Quality = p.quality,
                        MinStem = p.min_stem,
                        StemsBunch = p.stems_bunch,
                        Maturity = p.maturity,
                        Description = p.desc
                    })
                .FirstOrDefaultAsync();

            if (plantDTO == null)
            {
                return NotFound();
            }

            return Ok(plantDTO);
        }

        [HttpPost("dto")]
        public async Task<ActionResult<Plant>> CreatePlantDTO(PlantDTO plantDTO)
        {
            if (plantDTO == null)
            {
                return BadRequest("Request body is required.");
            }
                
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.s_name == plantDTO.SupplierName);

            if (supplier == null)
            {
                return BadRequest($"Supplier '{plantDTO.SupplierName}' not found");
            }

            var plant = new Plant
            {
                supplier_id = supplier.supplier_id,
                productname = plantDTO.ProductName,
                category = plantDTO.Category,
                form = plantDTO.Form,
                quality = plantDTO.Quality,
                min_stem = plantDTO.MinStem,
                stems_bunch = plantDTO.StemsBunch,
                maturity = plantDTO.Maturity,
                desc = plantDTO.Description
            };

            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlant), new { id = plant.plant_id }, plant);
        }
    }
}
