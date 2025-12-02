using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Flauction.DTOs.Output.ModelDTOs;

namespace Flauction.Controllers.modelControllers
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
        public async Task<ActionResult<ContactPerson>> CreateContactPerson([FromBody] ContactPerson cp)
        {
            if (cp == null)
                return BadRequest("Request body is null.");

            cp.contactperson_id = 0;

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            _context.ContactPersons.Add(cp);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Problem(detail: ex.InnerException?.Message ?? ex.Message, statusCode: 500);
            }

            return CreatedAtAction(nameof(GetContactPerson), new { id = cp.contactperson_id }, cp);
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

        [HttpGet("dto")]
        public async Task<ActionResult<IEnumerable<ContactPersonDTO>>> GetContactpersonDTO()
        {
            var contactPersonDTOs = await _context.ContactPersons
                .Join(_context.Companies,
                      cp => cp.company_id,
                      c => c.company_id,
                      (cp, c) => new ContactPersonDTO
                      {
                          ContactPersonId = cp.contactperson_id,
                          CompanyName = c.c_name,
                          ContactPersonName = cp.cp_name,
                          ContactPersonPhone = cp.cp_phone,
                          ContactPersonEmail = cp.cp_email
                      }).ToListAsync();

            return Ok(contactPersonDTOs);
        }

        [HttpGet("dto/{id}")]
        public async Task<ActionResult<ContactPersonDTO>> GetContactpersonDTO(int id)
        {
            var contactpersonDTOs = await _context.ContactPersons
                .Where(cp => cp.contactperson_id == id)
                .Join(_context.Companies,
                    cp => cp.company_id,
                    c => c.company_id,
                    (cp, c) => new ContactPersonDTO
                    {
                        ContactPersonId = cp.contactperson_id,
                        CompanyName = c.c_name,
                        ContactPersonName = cp.cp_name,
                        ContactPersonPhone = cp.cp_phone,
                        ContactPersonEmail = cp.cp_email
                    })
                .FirstOrDefaultAsync();

            if (contactpersonDTOs == null)
            {
                return NotFound();
            }

            return Ok(contactpersonDTOs);
        }

        [HttpPost("dto")]
        public async Task<ActionResult<ContactPersonDTO>> CreateContactpersonDTO(ContactPersonDTO cpDTO)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.c_name == cpDTO.CompanyName);

            if (company == null)
            {
                return BadRequest($"Company '{cpDTO.CompanyName}' does not exist");
            }

            var contactPerson = new ContactPerson
            {
                company_id = company.company_id,
                cp_name = cpDTO.ContactPersonName,
                cp_phone = cpDTO.ContactPersonPhone,
                cp_email = cpDTO.ContactPersonEmail
            };

            _context.ContactPersons.Add(contactPerson);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContactPerson), new { id = contactPerson.contactperson_id }, contactPerson);
        }
    }
}
