using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeilingsproductenController : ControllerBase
    {
        private readonly DBContext _context;

        public VeilingsproductenController(DBContext context)
        {
            _context = context;
        }

        // read
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Veilingsproduct>>> GetVeilingsproducten()
        {
            return await _context.Veilingsproducten.Where(x=>x.Prijs > 2).ToListAsync();
            // Dit is een LINQ functie, het voert een template query 
            // om gegevens uit Veilingsproducten tabel op te halen en returns alle 
            // Veilingsproducten als een lijst
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Veilingsproduct>> GetVeilingsproductBijId(int id)
        {
            var veilingsproduct = await _context.Veilingsproducten.FindAsync(id);
            if (veilingsproduct == null)
            {
                return NotFound();
            }
            return veilingsproduct;
        }

        [HttpGet("naam/{naam}")]
        public async Task<ActionResult<Veilingsproduct>> GetVeilingsproductBijNaam(string naam)
        {
            // Hieronder is er een LINQ functie die zoekt naar Veilingsproducten met dezelfde naam als naam, 
            // "vp" in dit geval een tijdelijke placeholder voor de huidige doorzochte Veilingsproduct in de lijst
            // naam is dan de parameter die we hebben ontvangen in de GET request
            var veilingsproduct = await _context.Veilingsproducten.FirstOrDefaultAsync(vp => vp.Naam == naam);
            if (veilingsproduct == null)
            {
                return NotFound();
            }
            return veilingsproduct;
        }

        // update


        // create
        [HttpPost]
        public async Task<ActionResult<Veilingsproduct>> CreateVeilingsproduct(Veilingsproduct veilingsproduct)
        {
            _context.Veilingsproducten.Add(veilingsproduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVeilingsproductBijId), new { id = veilingsproduct.VeilingsproductID }, veilingsproduct);
        }

        // delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVeilingsproduct(int id)
        {
            var veilingsproduct = await _context.Veilingsproducten.FindAsync(id);
            if (veilingsproduct == null)
            {
                return NotFound();
            }

            _context.Veilingsproducten.Remove(veilingsproduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
