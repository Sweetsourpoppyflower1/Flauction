using Flauction.Data;
using Flauction.DTOs.Output.ModelDTOs;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers.modelControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcceptanceController : ControllerBase
    {
        private readonly DBContext _context;

        public AcceptanceController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Acceptance>>> GetAcceptances()
        {
            return await _context.Acceptances.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Acceptance>> GetAcceptance(int id)
        {
            var acceptance = await _context.Acceptances.FindAsync(id);

            if (acceptance == null)
                return NotFound();

            return acceptance;
        }

        [HttpPost]
        public async Task<ActionResult<Acceptance>> CreateAcceptance(Acceptance acceptance)
        {
            _context.Acceptances.Add(acceptance);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAcceptance), new { id = acceptance.acceptance_id }, acceptance);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAcceptance(int id, Acceptance acceptance)
        {
            if (id != acceptance.acceptance_id)
                return BadRequest();

            _context.Entry(acceptance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Acceptances.Any(e => e.acceptance_id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAcceptance(int id)
        {
            var acceptance = await _context.Acceptances.FindAsync(id);
            if (acceptance == null)
                return NotFound();

            _context.Acceptances.Remove(acceptance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("dto")]
        public async Task<ActionResult<IEnumerable<AcceptanceDTO>>> GetAcceptanceDTO()
        {
            var acceptanceDTOs = await _context.Acceptances
                .Join(_context.Auctions,
                      acc => acc.auction_id,
                      au => au.auction_id,
                      (acc, au) => new { acc, au })
                .Join(_context.Companies,
                      x => x.acc.company_id,
                      c => c.company_id,
                      (x, c) => new { x.acc, x.au, c })
                .Join(_context.AuctionLots,
                      x => x.acc.auction_lot_id,
                      al => al.auctionlot_id,
                      (x, al) => new AcceptanceDTO
                      {
                          AcceptanceId = x.acc.acceptance_id,
                          AuctionId = x.au.auction_id,
                          CompanyName = x.c.c_name,
                          AuctionLotId = al.auctionlot_id,
                          TickNumber = x.acc.acc_tick_number,
                          AcceptedPrice = x.acc.acc_accepted_price,
                          AcceptedQuantity = x.acc.acc_accepted_quantity,
                          AcceptanceTime = x.acc.acc_time
                      })
                .ToListAsync();

            return Ok(acceptanceDTOs);
        }

        [HttpGet("dto/{id}")]
        public async Task<ActionResult<AcceptanceDTO>> GetAcceptanceDTO(int id)
        {
            var acceptanceDTO = await _context.Acceptances
                .Where(acc => acc.acceptance_id == id)
                .Join(_context.Auctions,
                      acc => acc.auction_id,
                      au => au.auction_id,
                      (acc, au) => new { acc, au })
                .Join(_context.Companies,
                      x => x.acc.company_id,
                      c => c.company_id,
                      (x, c) => new { x.acc, x.au, c })
                .Join(_context.AuctionLots,
                      x => x.acc.auction_lot_id,
                      al => al.auctionlot_id,
                      (x, al) => new AcceptanceDTO
                      {
                          AcceptanceId = x.acc.acceptance_id,
                          AuctionId = x.au.auction_id,
                          CompanyName = x.c.c_name,
                          AuctionLotId = al.auctionlot_id,
                          TickNumber = x.acc.acc_tick_number,
                          AcceptedPrice = x.acc.acc_accepted_price,
                          AcceptedQuantity = x.acc.acc_accepted_quantity,
                          AcceptanceTime = x.acc.acc_time
                      })
                .FirstOrDefaultAsync();

            if (acceptanceDTO == null)
                return NotFound();

            return Ok(acceptanceDTO);
        }
    }
}
