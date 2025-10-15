using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeilingsproductenController : ControllerBase
    {
        private readonly DBContext _context;

        public VeilingsproductenController(DBContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Veilingsproduct>>> GetVeilingsproducten()
        {
            return await _context.Veilingsproducten.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Veilingsproduct>> GetVeilingsproductBijId(int id)
        {
            var veilingsproduct = await _context.Veilingsproducten.FindAsync(id);
            if (veilingsproduct == null)
            {
                return NotFound();
            }
            return veilingsproduct;
        }

        [HttpGet("naam/{naam}")]
        public async Task<ActionResult<Veilingsproduct>> GetVeilingsproductBijNaam(string naam)
        {
            var veilingsproduct = await _context.Veilingsproducten.FirstOrDefaultAsync(vp => vp.Naam == naam);
            if (veilingsproduct == null)
            {
                return NotFound();
            }
            return veilingsproduct;
        }
    }
}
