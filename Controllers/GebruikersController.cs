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
        

        [HttpGet("api/gebruikers")]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetGebruikers()
        {
            return await _context.Gebruikers.ToListAsync();
        }

    }
}
