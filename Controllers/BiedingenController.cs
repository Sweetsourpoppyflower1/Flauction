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
            return await _context.Biedingen.ToListAsync();
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
