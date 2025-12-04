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
                Email = dto.CompanyEmail,
                EmailConfirmed = true
            };

            var createUserResult = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!createUserResult.Succeeded)
                return BadRequest(createUserResult.Errors);

            const string role = "Client";

            // Do NOT create roles here. IdentitySeeder should seed them.
            if (!await _roleManager.RoleExistsAsync(role))
                return StatusCode(500, $"Required role '{role}' not found.");

            var addRoleResult = await _userManager.AddToRoleAsync(identityUser, role);
            if (!addRoleResult.Succeeded)
                return StatusCode(500, "Failed to assign role to user.");

            var company = new Company
            {
                // If Company has a FK to the AspNetUsers table, set it (common pattern):
                // userId = identityUser.Id,
                c_address = dto.Adress,
                c_postalcode = dto.PostalCode,
                c_country = dto.Country,
                c_vat = dto.Vat ?? "",
                c_iban = dto.Iban ?? "",
                c_bicswift = dto.BicSwift ?? ""
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompanies), new { }, company);
        }
    }
}