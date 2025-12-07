using Flauction.Data;
using Flauction.DTOs;
using Flauction.DTOs.Output.ModelDTOs;
using Flauction.Models;
using Flauction.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Flauction.Controllers.newControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CompaniesController(DBContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        // POST register 
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CompanyRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByEmailAsync(dto.CompanyEmail) != null)
                return Conflict("Email already in use.");

            var identityUser = new User
            {
                UserName = dto.CompanyEmail,
                Email = dto.CompanyEmail
            };

            var createUserResult = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!createUserResult.Succeeded)
                return BadRequest(createUserResult.Errors);

            const string role = "Client";

            if (!await _roleManager.RoleExistsAsync(role))
                return StatusCode(500, $"Required role '{role}' not found.");

            var addRoleResult = await _userManager.AddToRoleAsync(identityUser, role);
            if (!addRoleResult.Succeeded)
                return StatusCode(500, "Failed to assign role to user.");

            var company = new Company
            {
                name = dto.CompanyName,
                address = dto.Adress,
                postalcode = dto.PostalCode,
                country = dto.Country,
                vat = dto.Vat ?? "",
                iban = dto.Iban ?? "",
                bicswift = dto.BicSwift ?? ""
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompanies), new { }, company);
        }
    }
}