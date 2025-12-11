using Microsoft.AspNetCore.Mvc;
using Flauction.Data;
using Flauction.DTOs.Output.ModelDTOs;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    [ApiController]
    [Route("api/plant")]
    public class PlantOverviewController : ControllerBase
    {
        private readonly DBContext _db;
        public PlantOverviewController(DBContext db) => _db = db;

        [HttpGet("overview")]
        public async Task<ActionResult<IEnumerable<PlantOverviewDTO>>> GetOverview()
        {
            var list = await _db.Plants
                .Select(p => new PlantOverviewDTO
                {
                    PlantId = p.plant_id,
                    PlantName = p.productname,
                    Supplier = _db.Suppliers.Where(s => s.Id == p.supplier_id).Select(s => s.name).FirstOrDefault(),
                    ProductName = p.productname,
                    Category = p.category,
                    Form = p.form,
                    Quality = p.quality,
                    MinStem = p.min_stem,
                    StemsBunch = p.stems_bunch,
                    Maturity = p.maturity,
                    Desc = p.desc,
                    ImageUrl = _db.MediaPlants
                        .Where(mp => mp.plant_id == p.plant_id && mp.is_primary)
                        .Select(mp => mp.url)
                        .FirstOrDefault(),
                    MinPrice = p.min_price,
                    MaxPrice = p.start_price,
                    RemainingQuantity = _db.AuctionLots
                        .Where(al => al.plant_id == p.plant_id)
                        .Select(al => (int?)al.remaining_quantity)
                        .FirstOrDefault()
                }).ToListAsync();

            return Ok(list);
        }
    }
}
