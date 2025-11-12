using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    public class VeilingenController : ControllerBase
    {
        private readonly DBContext _context;

        public VeilingenController(DBContext context)
        {
            _context = context;
        }


        [HttpGet("api/veilingen")]
        public async Task<ActionResult<IEnumerable<Veiling>>> GetVeilingen()
        {
            return await _context.Veilingen.ToListAsync();
        }

        [HttpGet("api/veilingen/nieuw")]
        public async Task<ActionResult<IEnumerable<Veiling>>> GetNieuweVeilingen()
        {
            // Hieronder vind je een LINQ functie die Veilingen returnt waarvan de Status gelijk is aan "Actief",
            // De VeilingmeesterID gelijk is aan 5 en de reslutaten worden oplopend gesorteerd op VeilingsID.
            return await _context.Veilingen.Where(x => x.Status == "Actief" & x.VeilingmeesterID == 5).OrderBy(x => x.VeilingsID).ToListAsync();
        }

        [HttpPost("api/veilingen")]
        public async Task<ActionResult<Veiling>> UpdateVeiling(Veiling veiling)
        {
            _context.Veilingen.Update(veiling);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVeilingen), new { id = veiling.VeilingsID }, veiling);
        }
    }
}

