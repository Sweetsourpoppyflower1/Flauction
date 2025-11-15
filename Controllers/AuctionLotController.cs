using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionLotController : ControllerBase
    {
        private readonly DBContext _context;

        public AuctionLotController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionLot>>> GetAuctionLots()
        {
            return await _context.AuctionLots.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionLot>> GetAuctionLot(int id)
        {
            var auctionLot = await _context.AuctionLots.FindAsync(id);

            if (auctionLot == null)
            {
                return NotFound();
            }

            return auctionLot;
        }


        [HttpPost]
        public async Task<ActionResult<AuctionLot>> CreateAuctionLot(AuctionLot auctionLot)
        {
            _context.AuctionLots.Add(auctionLot);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAuctionLot), new { id = auctionLot.auctionlot_id }, auctionLot);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuctionLot(int id, AuctionLot auctionLot)
        {
            if (id != auctionLot.auctionlot_id)
            {
                return BadRequest();
            }
            _context.Entry(auctionLot).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.AuctionLots.Any(e => e.auctionlot_id == id))
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
