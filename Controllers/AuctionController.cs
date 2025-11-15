using Microsoft.AspNetCore.Mvc;
using Flauction.Data;
using Flauction.Models;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly DBContext _context;

        public AuctionController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Auction>>> GetAuctions()
        {
            return await _context.Auctions.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Auction>> GetAuction(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);

            if (auction == null)
            {
                return NotFound();
            }

            return auction;
        }


        [HttpPost]
        public async Task<ActionResult<Auction>> CreateAuction(Auction auction)
        {
            _context.Auctions.Add(auction);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAuction), new { id = auction.auction_id }, auction);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuction(int id, Auction auction)
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
                if (!_context.Auctions.Any(e => e.auction_id == id))
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
        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuction(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            _context.Auctions.Remove(auction);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
