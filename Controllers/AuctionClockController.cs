using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionClockController : ControllerBase
    {
        private readonly DBContext _context;

        public AuctionClockController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionClock>>> GetAuctionClocks()
        {
            return await _context.AuctionClocks.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionClock>> GetAuctionClock(int id)
        {
            var auctionClock = await _context.AuctionClocks.FindAsync(id);

            if (auctionClock == null)
                return NotFound();

            return auctionClock;
        }

        [HttpPost]
        public async Task<ActionResult<AuctionClock>> CreateAuctionClock(AuctionClock auctionClock)
        {
            _context.AuctionClocks.Add(auctionClock);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuctionClock), new { id = auctionClock.auctionclock_id }, auctionClock);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuctionClock(int id, AuctionClock auctionClock)
        {
            if (id != auctionClock.auctionclock_id)
                return BadRequest();

            _context.Entry(auctionClock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.AuctionClocks.Any(e => e.auctionclock_id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuctionClock(int id)
        {
            var auctionClock = await _context.AuctionClocks.FindAsync(id);
            if (auctionClock == null)
                return NotFound();

            _context.AuctionClocks.Remove(auctionClock);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
