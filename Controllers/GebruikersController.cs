using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Flauction.Controllers
{
    [ApiController]
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
            return await _context.Gebruikers.OrderByDescending(x => x.GebruikersID).ToListAsync(); // Dit is een LINQ functie, het voert een template query 
                                                            // om gegevens uit Gebruiker tabel op te halen en returns alle 
                                                            // gebruikers als een lijst Aflopend gesorteerd op GebruikersID
        }

        // update
        

        // create
        [HttpPost("api/gebruikers")]
        public async Task<ActionResult<Gebruiker>> CreateGebruiker(Gebruiker gebruiker)
        {
            if (gebruiker == null)
            {
                return BadRequest("Gebruikersgegevens zijn vereist.");
            }

            // Hieronder is er een LINQ functie die zoekt naar gebruikers met dezelfde Gebruikersnaam OF Email als gebruiker, 
            // "g" in dit geval een tijdelijke placeholder voor de huidige doorzochte gebruiker in de lijst
            // gebruiker is dan de parameter die we hebben ontvangen in de POST request
            var existingUser = await _context.Gebruikers
                .FirstOrDefaultAsync(g => g.Gebruikersnaam == gebruiker.Gebruikersnaam || g.Email == gebruiker.Email);

            if (existingUser != null)
            {
                return BadRequest("Gebruikersnaam of email bestaat al.");
            }

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
