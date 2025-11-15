//using Flauction.Data;
//using Flauction.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;


//namespace Flauction.Controllers
//{
//    [ApiController]
//    public class GebruikersController : ControllerBase
//    {
//        private readonly DBContext _context;

//        public GebruikersController(DBContext context)
//        {
//            _context = context;
//        }
        

//        [HttpGet("api/gebruikers")]
//        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetGebruikers()
//        {
//            return await _context.Gebruikers.OrderByDescending(x => x.GebruikersID).ToListAsync(); // Dit is een LINQ functie, het voert een template query 
//                                                            // om gegevens uit Gebruiker tabel op te halen en returns alle 
//                                                            // gebruikers als een lijst Aflopend gesorteerd op GebruikersID
//        }

//        [HttpPost("api/gebruikers/register")]
//        public async Task<ActionResult<string>> Register([FromBody] Gebruiker gebruiker)
//        {
//            if (gebruiker == null)
//            {
//                return BadRequest("Gebruikersgegevens zijn vereist.");
//            }

//            // Hieronder is er een LINQ functie die zoekt naar gebruikers met dezelfde Gebruikersnaam OF Email als gebruiker, 
//            // "g" in dit geval een tijdelijke placeholder voor de huidige doorzochte gebruiker in de lijst
//            // gebruiker is dan de parameter die we hebben ontvangen in de POST request
//            var existingUser = await _context.Gebruikers
//                .FirstOrDefaultAsync(g => g.Gebruikersnaam == gebruiker.Gebruikersnaam || g.Email == gebruiker.Email);

//            if (existingUser != null)
//            {
//                return BadRequest("Gebruikersnaam of email bestaat al.");
//            }

//            _context.Gebruikers.Add(gebruiker);
//            await _context.SaveChangesAsync();

//            return Ok("Registratie gelukt!");
//        }

//    }
//}
