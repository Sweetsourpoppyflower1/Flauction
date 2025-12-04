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
    public class AuctionLotsController : ControllerBase
    {
        private readonly DBContext _context;

        public AuctionLotsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/AuctionLots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionLot>>> GetAuctionLots()
        {
            return await _context.AuctionLots.ToListAsync();
        }

        // GET: api/AuctionLots/5
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

        // PUT: api/AuctionLots/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuctionLot(int id, AuctionLot auctionLot)
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
                if (!AuctionLotExists(id))
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

        // POST: api/AuctionLots
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AuctionLot>> PostAuctionLot(AuctionLot auctionLot)
        {
            _context.AuctionLots.Add(auctionLot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuctionLot", new { id = auctionLot.auctionlot_id }, auctionLot);
        }

        // DELETE: api/AuctionLots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuctionLot(int id)
        {
            var auctionLot = await _context.AuctionLots.FindAsync(id);
            if (auctionLot == null)
            {
                return NotFound();
            }

            _context.AuctionLots.Remove(auctionLot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuctionLotExists(int id)
        {
            return _context.AuctionLots.Any(e => e.auctionlot_id == id);
        }
    }
}
