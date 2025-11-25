using Microsoft.AspNetCore.Mvc;
using Flauction.Data;
using Flauction.Models;
using Microsoft.EntityFrameworkCore;
using Flauction.DTOs.Output.ModelDTOs;

namespace Flauction.Controllers.modelControllers
{
    [Route("api/[controller]")]
    [Route("api/Auctions")]
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

        [HttpGet("dto")]
        public async Task<ActionResult<IEnumerable<AuctionDTO>>> GetAuctionDTOs()
        {
            var auctionDTOs = await _context.Auctions
                .Join(_context.AuctionMasters,
                    auction => auction.auctionmaster_id,
                    master => master.auctionmaster_id,
                    (auction, master) => new { auction, master })
                .Join(_context.Plants,
                    combined => combined.auction.plant_id,
                    plant => plant.plant_id,
                    (combined, plant) => new { combined.auction, combined.master, plant })
                .Join(_context.Companies,
                    combined => combined.auction.winner_company_id,
                    company => company.company_id,
                    (combined, company) => new AuctionDTO
                    {
                        AuctionId = combined.auction.auction_id,
                        AuctionMasterName = combined.master.am_name,
                        PlantName = combined.plant.p_productname,
                        WinnerCompanyName = company.c_name,
                        Status = combined.auction.au_status,
                        StartTime = combined.auction.au_start_time,
                        EndTime = combined.auction.au_end_time,
                        StartPrice = combined.auction.au_start_price,
                        CurrentPrice = combined.auction.au_current_price,
                        MinIncrement = combined.auction.au_min_increment,
                        FinalPrice = combined.auction.au_final_price
                    })
                .ToListAsync();

            return Ok(auctionDTOs);
        }

        [HttpGet("dto/{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionDTO(int id)
        {
            var auctionDTO = await _context.Auctions
                .Where(a => a.auction_id == id)
                .Join(_context.AuctionMasters,
                    auction => auction.auctionmaster_id,
                    master => master.auctionmaster_id,
                    (auction, master) => new { auction, master })
                .Join(_context.Plants,
                    combined => combined.auction.plant_id,
                    plant => plant.plant_id,
                    (combined, plant) => new { combined.auction, combined.master, plant })
                .Join(_context.Companies,
                    combined => combined.auction.winner_company_id,
                    company => company.company_id,
                    (combined, company) => new AuctionDTO
                    {
                        AuctionId = combined.auction.auction_id,
                        AuctionMasterName = combined.master.am_name,
                        PlantName = combined.plant.p_productname,
                        WinnerCompanyName = company.c_name,
                        Status = combined.auction.au_status,
                        StartTime = combined.auction.au_start_time,
                        EndTime = combined.auction.au_end_time,
                        StartPrice = combined.auction.au_start_price,
                        CurrentPrice = combined.auction.au_current_price,
                        MinIncrement = combined.auction.au_min_increment,
                        FinalPrice = combined.auction.au_final_price
                    })
                .FirstOrDefaultAsync();

            if (auctionDTO == null)
            {
                return NotFound();
            }

            return Ok(auctionDTO);
        }

        [HttpPost("dto")]
        public async Task<ActionResult<Auction>> CreateAuctionDTO(AuctionDTO auctionDTO)
        {
            if (auctionDTO == null)
                return BadRequest("Request body is required.");

            var master = await _context.AuctionMasters
                .FirstOrDefaultAsync(m => m.am_name == auctionDTO.AuctionMasterName);
            if (master == null)
                return BadRequest($"AuctionMaster '{auctionDTO.AuctionMasterName}' not found");

            var plant = await _context.Plants
                .FirstOrDefaultAsync(p => p.p_productname == auctionDTO.PlantName);
            if (plant == null)
                return BadRequest($"Plant '{auctionDTO.PlantName}' not found");

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.c_name == auctionDTO.WinnerCompanyName);
            if (company == null)
                return BadRequest($"Company '{auctionDTO.WinnerCompanyName}' not found");

            if (auctionDTO.StartPrice != Math.Truncate(auctionDTO.StartPrice))
                return BadRequest("StartPrice must be an int");
            if (auctionDTO.MinIncrement != Math.Truncate(auctionDTO.MinIncrement))
                return BadRequest("MinIncrement must be an int");

            var auction = new Auction
            {
                auctionmaster_id = master.auctionmaster_id,
                plant_id = plant.plant_id,
                winner_company_id = company.company_id,
                au_status = auctionDTO.Status,
                au_start_time = auctionDTO.StartTime,
                au_end_time = auctionDTO.EndTime,
                au_start_price = Convert.ToInt32(auctionDTO.StartPrice),
                au_current_price = auctionDTO.CurrentPrice,
                au_min_increment = Convert.ToInt32(auctionDTO.MinIncrement),
                au_final_price = auctionDTO.FinalPrice
            };

            _context.Auctions.Add(auction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuction), new { id = auction.auction_id }, auction);
        }
    }
}
