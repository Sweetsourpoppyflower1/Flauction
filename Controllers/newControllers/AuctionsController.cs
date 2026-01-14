using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flauction.Data;
using Flauction.Models;

namespace Flauction.Controllers.newControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly DBContext _context;

        public AuctionsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Auctions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Auction>>> GetAuctions()
        {
            return await _context.Auctions.ToListAsync();
        }

        // GET: api/Auctions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAuction(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null) return NotFound();

            var latestAcceptanceTime = await _context.Acceptances
                .Where(a => a.auction_id == id)
                .OrderByDescending(a => a.time)
                .Select(a => (DateTime?)a.time)
                .FirstOrDefaultAsync();

            var effectiveStartTime = latestAcceptanceTime ?? auction.start_time;
            
            // Ensure all DateTimes are explicitly UTC formatted
            var effectiveStartTimeUtc = effectiveStartTime.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(effectiveStartTime, DateTimeKind.Utc).ToString("o")
                : effectiveStartTime.ToUniversalTime().ToString("o");
                
            var startTimeUtc = auction.start_time.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(auction.start_time, DateTimeKind.Utc).ToString("o")
                : auction.start_time.ToUniversalTime().ToString("o");

            return new
            {
                auction.auction_id,
                auction.auctionmaster_id,
                auction.plant_id,
                auction.status,
                start_time = startTimeUtc,
                auction.duration_minutes,
                effective_start_time = effectiveStartTimeUtc,
                serverTime = DateTime.UtcNow.ToString("o")
            };
        }


        // PUT: api/Auctions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuction(int id, Auction auction)
        {
            if (id != auction.auction_id)
            {
                return BadRequest();
            }

            _context.Entry(auction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuctionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Auctions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Auction>> PostAuction(Auction auction)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(auction.auctionmaster_id) || 
                string.IsNullOrEmpty(auction.status) ||
                auction.duration_minutes <= 0)
            {
                return BadRequest("auctionmaster_id, status, and duration_minutes (> 0) are required.");
            }

            // Set default start_time to now if not provided
            if (auction.start_time == default)
            {
                auction.start_time = DateTime.UtcNow;
            }

            _context.Auctions.Add(auction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuction", new { id = auction.auction_id }, auction);
        }

        // DELETE: api/Auctions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuction(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }

            // Remove all related acceptances first
            var acceptances = await _context.Acceptances
                .Where(a => a.auction_id == id)
                .ToListAsync();
            
            if (acceptances.Any())
            {
                _context.Acceptances.RemoveRange(acceptances);
            }

            // Then remove the auction
            _context.Auctions.Remove(auction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuctionExists(int id)
        {
            return _context.Auctions.Any(e => e.auction_id == id);
        }
    }
}
