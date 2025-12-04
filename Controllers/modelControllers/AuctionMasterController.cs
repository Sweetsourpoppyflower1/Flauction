//using Flauction.Data;
//using Flauction.DTOs.Input;
//using Flauction.DTOs.Output.ModelDTOs;
//using Flauction.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Flauction.Controllers.modelControllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize (AuthenticationSchemes ="Identity.Bearer", Roles = "Admin")]
//    public class AuctionMasterController : ControllerBase
//    {
//        private readonly DBContext _context;
//        private readonly IConfiguration _config;

//        public AuctionMasterController(DBContext context, IConfiguration config)
//        {
//            _context = context;
//            _config = config;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<AuctionMaster>>> GetAuctionMasters()
//        {
//            return await _context.AuctionMasters.ToListAsync();
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<AuctionMaster>> GetAuctionMaster(int id)
//        {
//            var master = await _context.AuctionMasters.FindAsync(id);

//            if (master == null)
//                return NotFound();

//            return master;
//        }

//        [HttpPost("login")]
//        public async Task<ActionResult<AuctionMasterDTO>> Login([FromBody] LoginDTO login)
//        {
//            if (login == null || string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
//                return BadRequest("Email and password are required.");

//            var master = await _context.AuctionMasters
//                .AsNoTracking() // zorgt ervoor dat de gegevens niet worden getrackt, en verbetert efficientie
//                .FirstOrDefaultAsync(am => am.am_email == login.Username);

//            if (master == null)
//                return Unauthorized();

//            if (master.am_password != login.Password)
//                return Unauthorized();

//            var dto = new AuctionMasterDTO
//            {
//                AuctionMasterId = master.auctionmaster_id,
//                Name = master.am_name,
//                Phone = master.am_phone,
//                Email = master.am_email
//            };

//            return Ok(dto);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateAuctionMaster(string id, AuctionMaster master)
//        {
//            if (id != master.Id)
//                return BadRequest();

//            _context.Entry(master).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!_context.AuctionMasters.Any(e => e.auctionmaster_id == id))
//                    return NotFound();

//                throw;
//            }

//            return NoContent();
//        }

//        [HttpPost]
//        public async Task<ActionResult<AuctionMaster>> CreateAuctionMaster(AuctionMaster master)
//        {
//            _context.AuctionMasters.Add(master);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetAuctionMaster),
//                new { id = master.auctionmaster_id }, master);
//        }

//        [HttpGet("dto")]
//        public async Task<ActionResult<IEnumerable<AuctionMasterDTO>>> GetAuctionMastersDto()
//        {
//            var dtos = await _context.AuctionMasters
//                .Select(am => new AuctionMasterDTO
//                {
//                    AuctionMasterId = am.auctionmaster_id,
//                    Name = am.am_name,
//                    Phone = am.am_phone,
//                    Email = am.am_email
//                })
//                .ToListAsync();

//            return Ok(dtos);
//        }

//        [HttpGet("dto/{id}")]
//        public async Task<ActionResult<AuctionMasterDTO>> GetAuctionMasterDto(int id)
//        {
//            var dto = await _context.AuctionMasters
//                .Where(am => am.auctionmaster_id == id)
//                .Select(am => new AuctionMasterDTO
//                {
//                    AuctionMasterId = am.auctionmaster_id,
//                    Name = am.am_name,
//                    Phone = am.am_phone,
//                    Email = am.am_email
//                })
//                .FirstOrDefaultAsync();

//            if (dto == null)
//                return NotFound();

//            return Ok(dto);
//        }

//        [HttpPost("dto")]
//        public async Task<ActionResult<AuctionMasterDTO>> CreateAuctionMasterDto(AuctionMasterDTO dto)
//        {
//            if (dto == null)
//                return BadRequest();

//            var master = new AuctionMaster
//            {
//                am_name = dto.Name,
//                am_phone = dto.Phone,
//                am_email = dto.Email,
//                am_password = "changeMe",
//                address = ""
//            };

//            _context.AuctionMasters.Add(master);
//            await _context.SaveChangesAsync();

//            var createdDto = new AuctionMasterDTO
//            {
//                AuctionMasterId = master.auctionmaster_id,
//                Name = master.am_name,
//                Phone = master.am_phone,
//                Email = master.am_email
//            };

//            return CreatedAtAction(nameof(GetAuctionMasterDto),
//                new { id = master.auctionmaster_id }, createdDto);
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteAuctionMaster(int id)
//        {
//            var master = await _context.AuctionMasters.FindAsync(id);

//            if (master == null)
//                return NotFound();

//            _context.AuctionMasters.Remove(master);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}
