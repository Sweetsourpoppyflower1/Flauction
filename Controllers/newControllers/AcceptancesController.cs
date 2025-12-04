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
    public class AcceptancesController : ControllerBase
    {
        private readonly DBContext _context;

        public AcceptancesController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Acceptances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Acceptance>>> GetAcceptances()
        {
            return await _context.Acceptances.ToListAsync();
        }

        // GET: api/Acceptances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Acceptance>> GetAcceptance(int id)
        {
            var acceptance = await _context.Acceptances.FindAsync(id);

            if (acceptance == null)
            {
                return NotFound();
            }

            return acceptance;
        }

        // PUT: api/Acceptances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAcceptance(int id, Acceptance acceptance)
        {
            if (id != acceptance.acceptance_id)
            {
                return BadRequest();
            }

            _context.Entry(acceptance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AcceptanceExists(id))
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

        // POST: api/Acceptances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Acceptance>> PostAcceptance(Acceptance acceptance)
        {
            _context.Acceptances.Add(acceptance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAcceptance", new { id = acceptance.acceptance_id }, acceptance);
        }

        // DELETE: api/Acceptances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAcceptance(int id)
        {
            var acceptance = await _context.Acceptances.FindAsync(id);
            if (acceptance == null)
            {
                return NotFound();
            }

            _context.Acceptances.Remove(acceptance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AcceptanceExists(int id)
        {
            return _context.Acceptances.Any(e => e.acceptance_id == id);
        }
    }
}
