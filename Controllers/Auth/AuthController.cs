using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; //dit is de library die gebruikt wordt voor user management A1
using Microsoft.IdentityModel.Tokens;
using Flauction.Models;
using Microsoft.AspNetCore.Authorization;
using Flauction.Data; //hier haalt het de rollen uit en de database context A2
using Microsoft.EntityFrameworkCore;
using Flauction.DTOs.Output.ModelDTOs;
using Microsoft.Extensions.Configuration;

namespace Flauction.Controllers.newControllers
{
    public class AuthLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthLoginResponse //A3 constructor voor de response van de login
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
        public object Data { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager; //A1 initialisatie
        private readonly SignInManager<User> _signInManager;
        private readonly DBContext _db;
        private readonly IConfiguration _config;

        public AuthController(
            UserManager<User> userManager, //A1 toepassing
            SignInManager<User> signInManager, 
            DBContext db,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _config = config;
        }

        private string GenerateJwtToken(User user, string[] roles)
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"];
            var jwtAudience = _config["Jwt:Audience"];
            var jwtExpirationMinutes = int.TryParse(_config["Jwt:ExpirationMinutes"], out var mins) ? mins : 60;

            if (string.IsNullOrEmpty(jwtKey)) //als de jwt key niet staat in de settings dan krijg je geen toegang
                throw new InvalidOperationException("JWT Key is not configured. Add 'Jwt:Key' to appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> 
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            // A2 voeg de rollen toe
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthLoginResponse), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "Email and password are required." });

            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials." });

            if (!await _userManager.CheckPasswordAsync(user, req.Password))
                return Unauthorized(new { message = "Invalid credentials." });

            var roles = (await _userManager.GetRolesAsync(user)).ToArray();

            // genereer JWT token
            string token;
            try
            {
                token = GenerateJwtToken(user, roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Token generation failed.", error = ex.Message });
            }

            object data = null;
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

            var resp = new AuthLoginResponse //A3 initialisatie van de response
            {
                Token = token,
                Email = user.Email,
                Roles = roles,
                Data = data
            };

            return Ok(resp);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpGet("me")] //checkt wie er is ingelogd
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                userName = user.UserName,
                roles = roles
            });
        }
    }
}