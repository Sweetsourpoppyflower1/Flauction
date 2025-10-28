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
        

        [HttpGet("api/gebruikers")]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetGebruikers()
        {
            return await _context.Gebruikers.ToListAsync();
        }

        [HttpPost("api/gebruikers/register")]
        public async Task<ActionResult<string>> Register([FromBody] Gebruiker gebruiker)
        {
            if (gebruiker == null)
            {
                return BadRequest("Gebruikersgegevens zijn vereist.");
            }

            // Check if username or email already exists
            var existingUser = await _context.Gebruikers
                .FirstOrDefaultAsync(g => g.Gebruikersnaam == gebruiker.Gebruikersnaam || g.Email == gebruiker.Email);

            if (existingUser != null)
            {
                return BadRequest("Gebruikersnaam of email bestaat al.");
            }

            // Add the new user
            _context.Gebruikers.Add(gebruiker);
            await _context.SaveChangesAsync();

            return Ok("Registratie gelukt!");
        }

    }
}
