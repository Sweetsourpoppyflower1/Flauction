using System.Collections.Generic;
using System.Linq;
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

        // GET (admin) - requires authentication
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CompanyDTO>>> GetCompanies()
        {
            var companies = await _context.Companies
                .Select(c => new CompanyDTO
                {
                    CompanyID = c.Id,
                    CompanyName = c.name,
                    Email = c.Email,
                    Address = c.address,
                    PostalCode = c.postalcode,
                    Country = c.country
                })
                .ToListAsync();

            return Ok(companies);
        }

        // GET by ID (admin) - requires authentication
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CompanyDTO>> GetCompany(string id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound(new { message = "Company not found" });
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

            return Ok(dto);
        }

        // POST Login - anonymous
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<CompanyDTO>> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
                return BadRequest(new { message = "Email and password are required." });

            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials." });

            if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return Unauthorized(new { message = "Invalid credentials." });

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == user.Id);

            if (company == null)
                return Unauthorized(new { message = "Company not found." });

            var dto = new CompanyDTO
            {
                CompanyID = company.Id,
                CompanyName = company.name,
                Email = company.Email,
                Address = company.address,
                PostalCode = company.postalcode,
                Country = company.country
            };

            return Ok(dto);
        }

        // POST register - anonymous
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CompanyRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.CompanyEmail) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Email and password are required." });

            if (await _userManager.FindByEmailAsync(dto.CompanyEmail) != null)
                return Conflict(new { message = "Email already in use." });

            var identityUser = new User
            {
                UserName = dto.CompanyEmail,
                Email = dto.CompanyEmail,
                EmailConfirmed = true
            };

            var createUserResult = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!createUserResult.Succeeded)
            {
                var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                return BadRequest(new { message = "Failed to create user.", errors });
            }

            const string role = "Client";

            if (!await _roleManager.RoleExistsAsync(role))
                return StatusCode(500, new { message = $"Required role '{role}' not found." });

            var addRoleResult = await _userManager.AddToRoleAsync(identityUser, role);
            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                return StatusCode(500, new { message = "Failed to assign role to user.", errors });
            }

            // Link Company row to the Identity user
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Failed to save company data.", error = ex.InnerException?.Message });
            }

            var responseDto = new CompanyDTO
            {
                CompanyID = company.Id,
                CompanyName = company.name,
                Email = company.Email,
                Address = company.address,
                PostalCode = company.postalcode,
                Country = company.country
            };

            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, responseDto);
        }
    }

    // Simple login request DTO
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}