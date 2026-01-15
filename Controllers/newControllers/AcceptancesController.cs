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

        // GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Acceptance>>> GetAcceptances()
        {
            return await _context.Acceptances.ToListAsync();
        }

        // GET
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

        // PUT
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

        // POST
        [HttpPost]
        public async Task<ActionResult<Acceptance>> PostAcceptance(Acceptance acceptance)
        {
            _context.Acceptances.Add(acceptance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAcceptance", new { id = acceptance.acceptance_id }, acceptance);
        }

        // DELETE
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
