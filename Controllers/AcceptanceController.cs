using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
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
    }
}
