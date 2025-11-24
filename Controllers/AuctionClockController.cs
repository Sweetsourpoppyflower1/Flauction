using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Flauction.DTOs.Output;
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

        [HttpGet("dto")]
        public async Task<ActionResult<IEnumerable<AuctionClockDTO>>> GetAuctionClocksDTO ()
        {
            var auctionClockDTOs = await _context.AuctionClocks
                .Join(_context.Auctions,
                      ac => ac.auction_id,
                      a => a.auction_id,
                      (ac, a) => new AuctionClockDTO
                      {
                          AuctionClockId = ac.auctionclock_id,
                          TickIntervalSeconds = ac.ac_tick_interval_seconds,
                          DecrementAmount = ac.ac_decrement_amount,
                          FinalCallSeconds = ac.ac_final_call_seconds,
                      }).ToListAsync();

            return Ok(auctionClockDTOs);
        }

        [HttpGet("dto/{id}")]
        public async Task<ActionResult<AuctionClockDTO>> GetAuctionClockDTO(int id)
        {
            var auctionClockDTO = await _context.AuctionClocks
                .Where(ac => ac.auctionclock_id == id)
                .Join(_context.Auctions,
                      ac => ac.auction_id,
                      a => a.auction_id,
                      (ac, a) => new AuctionClockDTO
                      {
                          AuctionClockId = ac.auctionclock_id,
                          TickIntervalSeconds = ac.ac_tick_interval_seconds,
                          DecrementAmount = ac.ac_decrement_amount,
                          FinalCallSeconds = ac.ac_final_call_seconds,
                      })
                .FirstOrDefaultAsync();

            if (auctionClockDTO == null)
            {
                return NotFound();
            }
                

            return Ok(auctionClockDTO);
        }

        [HttpPost("dto")]
        public async Task<ActionResult<AuctionClockDTO>> CreateAuctionClockDTO(AuctionClockDTO acDTO)
        {
            var auctions = await _context.Auctions
                .FirstOrDefaultAsync(a => a.auction_id == acDTO.AuctionId);

            if (auctions == null)
            {
                return BadRequest($"Auction with ID {acDTO.AuctionId} does not exist.");
            }

            var auctionClock = new AuctionClock
            {
                auction_id = acDTO.AuctionId,
                ac_tick_interval_seconds = acDTO.TickIntervalSeconds,
                ac_decrement_amount = acDTO.DecrementAmount,
                ac_final_call_seconds = acDTO.FinalCallSeconds
            };

            _context.AuctionClocks.Add(auctionClock);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuctionClockDTO), new { id = auctionClock.auctionclock_id }, auctionClock);
        }
    }
}
