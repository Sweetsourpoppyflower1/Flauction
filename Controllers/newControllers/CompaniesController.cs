using System.Collections.Generic;
using System.Threading.Tasks;
using Flauction.Data;
using Flauction.DTOs;
using Flauction.DTOs.Output.ModelDTOs;
using Flauction.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers.newControllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "Admin")]
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

        // GET (admin)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        //POST Login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<CompanyDTO>> Login([FromBody] Company login)
            {
            if (login == null || string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.PasswordHash))
                return BadRequest("Email and password are required.");

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == login.Email);

            if (company == null || company.PasswordHash != login.PasswordHash)
            {
                return Unauthorized();
            }

            var dto = new CompanyDTO
            {
                CompanyID = company.Id,
                CompanyName = company.name,
                Email = company.Email,
                Address = company.address,
                PostalCode = company.postalcode,
                Country = company.country
            };
            await _context.SaveChangesAsync();

            return Ok(dto);

        }

        // POST register (anonymous)
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

            if (!await _roleManager.RoleExistsAsync(role))
                return StatusCode(500, $"Required role '{role}' not found.");

            var addRoleResult = await _userManager.AddToRoleAsync(identityUser, role);
            if (!addRoleResult.Succeeded)
                return StatusCode(500, "Failed to assign role to user.");

            // IMPORTANT: link Company row to the Identity user by setting Id = identityUser.Id
            var company = new Company
            {
                Id = identityUser.Id,
                UserName = identityUser.UserName,
                Email = identityUser.Email,
                name = dto.CompanyName,
                address = dto.Adress,
                postalcode = dto.PostalCode,
                country = dto.Country,
                vat = dto.Vat ?? string.Empty,
                iban = dto.Iban ?? string.Empty,
                bicswift = dto.BicSwift ?? string.Empty
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompanies), new { }, company);
        }
    }
}