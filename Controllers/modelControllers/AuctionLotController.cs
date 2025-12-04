//using Flauction.Data;
//using Flauction.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Flauction.DTOs.Output.ModelDTOs;

//namespace Flauction.Controllers.modelControllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuctionLotController : ControllerBase
//    {
//        private readonly DBContext _context;

//        public AuctionLotController(DBContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<AuctionLot>>> GetAuctionLots()
//        {
//            return await _context.AuctionLots.ToListAsync();
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<AuctionLot>> GetAuctionLot(int id)
//        {
//            var auctionLot = await _context.AuctionLots.FindAsync(id);

//            if (auctionLot == null)
//            {
//                return NotFound();
//            }

//            return auctionLot;
//        }

//        // DTO endpoints ---------------------------------------------------

//        [HttpGet("dto")]
//        public async Task<ActionResult<IEnumerable<AuctionLotDTO>>> GetAuctionLotsDto()
//        {
//            var dtos = await (from al in _context.AuctionLots
//                              join m in _context.Medias on al.image_id equals m.media_id into ms
//                              from m in ms.DefaultIfEmpty()
//                              select new AuctionLotDTO
//                              {
//                                  AuctionLotId = al.auctionlot_id,
//                                  ImageUrl = m != null ? m.url : null,
//                                  ImageAltText = m != null ? m.alt_text : null,
//                                  UnitPerContainer = al.unit_per_container,
//                                  ContainersInLot = al.containers_in_lot,
//                                  MinPickup = al.min_pickup,
//                                  Fustcode = al.fustcode,
//                                  TotalQuantity = al.total_quantity,
//                                  RemainingQuantity = al.remaining_quantity
//                              }).ToListAsync();

//            return Ok(dtos);
//        }

//        [HttpGet("dto/{id}")]
//        public async Task<ActionResult<AuctionLotDTO>> GetAuctionLotDto(int id)
//        {
//            var dto = await (from al in _context.AuctionLots
//                             join m in _context.Medias on al.image_id equals m.media_id into ms
//                             from m in ms.DefaultIfEmpty()
//                             where al.auctionlot_id == id
//                             select new AuctionLotDTO
//                             {
//                                 AuctionLotId = al.auctionlot_id,
//                                 ImageUrl = m != null ? m.url : null,
//                                 ImageAltText = m != null ? m.alt_text : null,
//                                 UnitPerContainer = al.unit_per_container,
//                                 ContainersInLot = al.containers_in_lot,
//                                 MinPickup = al.min_pickup,
//                                 Fustcode = al.fustcode,
//                                 TotalQuantity = al.total_quantity,
//                                 RemainingQuantity = al.remaining_quantity
//                             }).FirstOrDefaultAsync();

//            if (dto == null)
//                return NotFound();

//            return Ok(dto);
//        }

//        [HttpPost("dto")]
//        public async Task<ActionResult<AuctionLotDTO>> CreateAuctionLotDto(AuctionLotDTO dto)
//        {
//            if (dto == null)
//                return BadRequest();

//            // Create Media record if an URL is provided
//            int imageId = 0;
//            if (!string.IsNullOrEmpty(dto.ImageUrl))
//            {
//                var media = new Media
//                {
//                    plant_id = 0, // unknown plant here; set to 0 or adjust according to your domain rules
//                    url = dto.ImageUrl,
//                    alt_text = dto.ImageAltText ?? "",
//                    is_primary = false
//                };

//                _context.Medias.Add(media);
//                await _context.SaveChangesAsync();
//                imageId = media.media_id;
//            }

//            var auctionLot = new AuctionLot
//            {
//                image_id = imageId,
//                unit_per_container = dto.UnitPerContainer,
//                containers_in_lot = dto.ContainersInLot,
//                min_pickup = dto.MinPickup,
//                fustcode = dto.Fustcode,
//                total_quantity = dto.TotalQuantity,
//                remaining_quantity = dto.RemainingQuantity,
//                auction_id = 0 // unknown auction - set to 0 or adjust to your domain rules
//            };

//            _context.AuctionLots.Add(auctionLot);
//            await _context.SaveChangesAsync();

//            var createdDto = new AuctionLotDTO
//            {
//                AuctionLotId = auctionLot.auctionlot_id,
//                ImageUrl = dto.ImageUrl,
//                ImageAltText = dto.ImageAltText,
//                UnitPerContainer = auctionLot.unit_per_container,
//                ContainersInLot = auctionLot.containers_in_lot,
//                MinPickup = auctionLot.min_pickup,
//                Fustcode = auctionLot.fustcode,
//                TotalQuantity = auctionLot.total_quantity,
//                RemainingQuantity = auctionLot.remaining_quantity
//            };

//            return CreatedAtAction(nameof(GetAuctionLotDto),
//                new { id = auctionLot.auctionlot_id }, createdDto);
//        }

//        // ----------------------------------------------------------------

//        [HttpPost]
//        public async Task<ActionResult<AuctionLot>> CreateAuctionLot(AuctionLot auctionLot)
//        {
//            _context.AuctionLots.Add(auctionLot);
//            await _context.SaveChangesAsync();
//            return CreatedAtAction(nameof(GetAuctionLot), new { id = auctionLot.auctionlot_id }, auctionLot);
//        }


//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateAuctionLot(int id, AuctionLot auctionLot)
//        {
//            if (id != auctionLot.auctionlot_id)
//            {
//                return BadRequest();
//            }
//            _context.Entry(auctionLot).State = EntityState.Modified;
//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!_context.AuctionLots.Any(e => e.auctionlot_id == id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }
//            return NoContent();
//        }


//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteAuction(int id)
//        {
//            var auction = await _context.Auctions.FindAsync(id);
//            if (auction == null)
//            {
//                return NotFound();
//            }
//            _context.Auctions.Remove(auction);
//            await _context.SaveChangesAsync();
//            return NoContent();
//        }
//    }
//}
