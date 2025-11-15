using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactPersonController : ControllerBase
    {
        private readonly DBContext _context;

        public ContactPersonController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactPerson>>> GetContactPersons()
        {
            return await _context.ContactPersons.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactPerson>> GetContactPerson(int id)
        {
            var cp = await _context.ContactPersons.FindAsync(id);

            if (cp == null)
                return NotFound();

            return cp;
        }

        [HttpPost]
        public async Task<ActionResult<ContactPerson>> CreateContactPerson(ContactPerson cp)
        {
            _context.ContactPersons.Add(cp);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContactPerson),
                new { id = cp.contactperson_id }, cp);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContactPerson(int id, ContactPerson cp)
        {
            if (id != cp.contactperson_id)
                return BadRequest();

            _context.Entry(cp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ContactPersons.Any(e => e.contactperson_id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactPerson(int id)
        {
            var cp = await _context.ContactPersons.FindAsync(id);

            if (cp == null)
                return NotFound();

            _context.ContactPersons.Remove(cp);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
