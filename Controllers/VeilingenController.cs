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

        // read
        [HttpGet("api/veilingen")]
        public async Task<ActionResult<IEnumerable<Veiling>>> GetVeilingen()
        {
            return await _context.Veilingen.ToListAsync();
        }

        // update
        [HttpPost("api/veilingen/update")]
        public async Task<ActionResult<Veiling>> UpdateVeiling(Veiling veiling)
        {
            _context.Veilingen.Update(veiling);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVeilingen), new { id = veiling.VeilingsID }, veiling);
        }

        // create
        [HttpPost("api/veilingen")]
        public async Task<ActionResult<Veiling>> CreateVeiling(Veiling veiling)
        {
            _context.Veilingen.Add(veiling);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVeilingen), new { id = veiling.VeilingsID }, veiling);
        }

        // delete
        [HttpDelete("api/veilingen")]
        public async Task<IActionResult> DeleteVeiling(int id)
        {
            var veiling = await _context.Veilingen.FindAsync(id);
            if (veiling == null)
            {
                return NotFound();
            }

            _context.Veilingen.Remove(veiling);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
