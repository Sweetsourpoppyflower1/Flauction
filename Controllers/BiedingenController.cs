using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    public class BiedingenController : ControllerBase
    {
        private readonly DBContext _context;

        public BiedingenController(DBContext context)
        {
            _context = context;
        }


        [HttpGet("api/biedingen")]
        public async Task<ActionResult<IEnumerable<Bod>>> GetBiedingen()
        {
            // Hieronder vind je een LINQ functie die alle biedingen ophaalt met een bedrag groter dan 2
            // en geplaatst door de bieder met de naam "peter_bakker". De resultaten worden gesorteerd op BodID in oplopende volgorde.
            return await _context.Biedingen.Where(x => x.Bedrag > 3 & x.Bieder == "peter_bakker").OrderBy(x => x.BodID).ToListAsync();
        }

        [HttpGet("api/biedingen/nieuw")]
        public async Task<ActionResult<IEnumerable<Bod>>> GetNieuweBiedingen()
        {
            // Hieronder vind je een LINQ functie die alle biedingen ophaalt die zijn geplaatst op of na 20 januari 2025 om 9:45 uur,
            // met een BodID groter dan 30 en een bedrag groter dan 4. De resultaten worden gesorteerd op BodID in aflopende volgorde.
            // Het is een Asynchrone functie, dat wil zeggen dat de functie wacht op de database om de gegevens op te halen
            // zonder dat de programma moet wachten met runnen
            return await _context.Biedingen.Where(x => x.Tijdstip >= new DateTime(2025, 1, 20, 9, 45, 0) & x.BodID > 30 & x.Bedrag > 4).OrderByDescending(x => x.BodID).ToListAsync();
        }



        [HttpPost("api/biedingen")]
        public async Task<ActionResult<Bod>> UpdateBod(Bod bod)
        {
            _context.Biedingen.Update(bod);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBiedingen), new { id = bod.BodID }, bod);
        }
    }
}
