using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Flauction.Controllers
{
    public class GebruikersController : ControllerBase
    {
        private readonly DBContext _context;

        public GebruikersController(DBContext context)
        {
            _context = context;
        }
        
        // read
        [HttpGet("api/gebruikers")]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetGebruikers()
        {
            return await _context.Gebruikers.ToListAsync();
        }

        // update
        

        // create
        [HttpPost("api/gebruikers")]
        public async Task<ActionResult<Gebruiker>> CreateGebruiker(Gebruiker gebruiker)
        {
            _context.Gebruikers.Add(gebruiker);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGebruikers), new { id = gebruiker.GebruikersID }, gebruiker);
        }

        // delete
        [HttpDelete("api/gebruikers/{id}")]
        public async Task<IActionResult> DeleteGebruiker(int id)
        {
            var gebruiker = await _context.Gebruikers.FindAsync(id);
            if (gebruiker == null)
            {
                return NotFound();
            }

            _context.Gebruikers.Remove(gebruiker);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
