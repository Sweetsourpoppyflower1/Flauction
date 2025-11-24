using Flauction.Data;
using Flauction.DTOs.Output;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Linq;

namespace Flauction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly DBContext _context;

        public CompanyController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
                return NotFound();

            return company;
        }

        [HttpPost]
        public async Task<ActionResult<List<Company>>> PostCompanies([FromBody] List<Company> companies)
        {
            if (companies == null || companies.Count == 0)
                return BadRequest("Request body must be a non-empty array of companies.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var c in companies)
            {
                c.company_id = 0;
            }

            try
            {
                _context.Companies.AddRange(companies);
                await _context.SaveChangesAsync();
                return Ok(companies);
            }
            catch (DbUpdateException ex)
            {
                return Problem(detail: ex.InnerException?.Message ?? ex.Message, statusCode: 500);
            }
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCompany(int id, Company company)
        {
            if (id != company.company_id) return BadRequest();

            _context.Entry(company).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("dto")]
        public async Task<ActionResult<IEnumerable<CompanyDTO>>> GetCompanyDTO()
        {
            var CompanyDTOs = await _context.Companies
                .Select(c => new CompanyDTO
                {
                    CompanyID = c.company_id,
                    CompanyName = c.c_name,
                    Adress = c.c_address,
                    PostalCode = c.c_postalcode,
                    Country = c.c_country
                })
                .ToListAsync();

            return Ok(CompanyDTOs);
        }

        [HttpGet("dto/{id}")]
        public async Task<ActionResult<CompanyDTO>> GetCompanyDTO(int id)
        {
            var companyDTO = await _context.Companies
                .Where(c => c.company_id == id)
                .Select(c => new CompanyDTO
                {
                    CompanyID = c.company_id,
                    CompanyName = c.c_name,
                    Adress = c.c_address,
                    PostalCode = c.c_postalcode,
                    Country = c.c_country,
                })
                .FirstOrDefaultAsync();
            if (companyDTO == null)
                return NotFound();

            return Ok(companyDTO);
        }

    }
}
