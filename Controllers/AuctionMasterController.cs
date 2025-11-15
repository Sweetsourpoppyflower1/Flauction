using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionMasterController : ControllerBase
    {
        private readonly DBContext _context;

        public AuctionMasterController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionMaster>>> GetAuctionMasters()
        {
            return await _context.AuctionMasters.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionMaster>> GetAuctionMaster(int id)
        {
            var master = await _context.AuctionMasters.FindAsync(id);

            if (master == null)
                return NotFound();

            return master;
        }

        [HttpPost]
        public async Task<ActionResult<AuctionMaster>> CreateAuctionMaster(AuctionMaster master)
        {
            _context.AuctionMasters.Add(master);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuctionMaster),
                new { id = master.auctionmaster_id }, master);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuctionMaster(int id, AuctionMaster master)
        {
            if (id != master.auctionmaster_id)
                return BadRequest();

            _context.Entry(master).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.AuctionMasters.Any(e => e.auctionmaster_id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuctionMaster(int id)
        {
            var master = await _context.AuctionMasters.FindAsync(id);

            if (master == null)
                return NotFound();

            _context.AuctionMasters.Remove(master);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
