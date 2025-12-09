using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Flauction.Models;
using Microsoft.AspNetCore.Authorization;
using Flauction.Data;
using Microsoft.EntityFrameworkCore;
using Flauction.DTOs.Output.ModelDTOs;

namespace Flauction.Controllers.newControllers
{
    public class AuthLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthLoginResponse
    {
        public string Email { get; set; }
        public string[] Roles { get; set; }
        public object? Data { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DBContext _db;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, DBContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthLoginResponse), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Email and password are required.");

            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            if (!await _userManager.CheckPasswordAsync(user, req.Password))
                return Unauthorized("Invalid credentials.");

            var roles = (await _userManager.GetRolesAsync(user)).ToArray();

            object? data = null;
            var primaryRole = roles.FirstOrDefault()?.ToLowerInvariant();

            if (primaryRole == "supplier")
            {
                var supplier = await _db.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == user.Id);
                if (supplier != null)
                {
                    data = new SupplierDTO
                    {
                        SupplierId = supplier.Id,
                        Email = supplier.Email ?? string.Empty,
                        Name = supplier.name,
                        Address = supplier.address,
                        PostalCode = supplier.postalcode,
                        Country = supplier.country,
                        Description = supplier.desc
                    };
                }
            }
            else if (primaryRole == "client")
            {
                var company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == user.Id);
                if (company != null)
                {
                    data = new CompanyDTO
                    {
                        CompanyID = company.Id,
                        Email = company.Email ?? string.Empty,
                        CompanyName = company.name,
                        Address = company.address,
                        PostalCode = company.postalcode,
                        Country = company.country,
                    };
                }
            }
            else if (primaryRole == "admin")
            {
                var master = await _db.AuctionMasters.AsNoTracking().FirstOrDefaultAsync(m => m.Id == user.Id);
                if (master != null)
                {
                    data = new AuctionMasterDTO
                    {
                        AuctionMasterId = master.Id,
                        Email = master.Email ?? string.Empty
                    };
                }
            }

            var resp = new AuthLoginResponse
            {
                Email = user.Email,
                Roles = roles,
                Data = data
            };

            return Ok(resp);
        }
    }
}