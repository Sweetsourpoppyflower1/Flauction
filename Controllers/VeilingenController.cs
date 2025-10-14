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

        [HttpPost("api/veilingen")]
        public async Task<ActionResult<Veiling>> UpdateVeiling(Veiling veiling)
        {
            _context.Veilingen.Update(veiling);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVeilingen), new { id = veiling.VeilingsID }, veiling);
        }
    }
}
