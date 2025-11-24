using Flauction.Data;
using Flauction.Models;
using Flauction.DTOs.Output;
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

        // DTO endpoints ---------------------------------------------------

        [HttpGet("dto")]
        public async Task<ActionResult<IEnumerable<AuctionMasterDTO>>> GetAuctionMastersDto()
        {
            var dtos = await _context.AuctionMasters
                .Select(am => new AuctionMasterDTO
                {
                    AuctionMasterId = am.auctionmaster_id,
                    Name = am.am_name,
                    Phone = am.am_phone,
                    Email = am.am_email
                })
                .ToListAsync();

            return Ok(dtos);
        }

        [HttpGet("dto/{id}")]
        public async Task<ActionResult<AuctionMasterDTO>> GetAuctionMasterDto(int id)
        {
            var dto = await _context.AuctionMasters
                .Where(am => am.auctionmaster_id == id)
                .Select(am => new AuctionMasterDTO
                {
                    AuctionMasterId = am.auctionmaster_id,
                    Name = am.am_name,
                    Phone = am.am_phone,
                    Email = am.am_email
                })
                .FirstOrDefaultAsync();

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [HttpPost("dto")]
        public async Task<ActionResult<AuctionMasterDTO>> CreateAuctionMasterDto(AuctionMasterDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var master = new AuctionMaster
            {
                am_name = dto.Name,
                am_phone = dto.Phone,
                am_email = dto.Email,
                // password and address are required in model - set minimal defaults or reject.
                // For simplicity set a placeholder password; in a real app you'd require it.
                am_password = "changeMe",
                am_address = ""
            };

            _context.AuctionMasters.Add(master);
            await _context.SaveChangesAsync();

            // map back to DTO with generated id
            var createdDto = new AuctionMasterDTO
            {
                AuctionMasterId = master.auctionmaster_id,
                Name = master.am_name,
                Phone = master.am_phone,
                Email = master.am_email
            };

            return CreatedAtAction(nameof(GetAuctionMasterDto),
                new { id = master.auctionmaster_id }, createdDto);
        }

        // ----------------------------------------------------------------

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
