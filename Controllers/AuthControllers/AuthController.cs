using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Flauction.DTOs.Input;
using Flauction.DTOs.Output.LoginDTO;
using Flauction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;

namespace Flauction.Controllers.AuthControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<User> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        // POST api/auth/login?requiredRole=Supplier
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto, [FromQuery] string? requiredRole = null)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // try email first, then username
            var user = await _userManager.FindByEmailAsync(dto.Email) ?? await _userManager.FindByNameAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);

            // Optionally require a role at login time
            if (!string.IsNullOrWhiteSpace(requiredRole) && !roles.Contains(requiredRole))
                return Forbid($"User does not have required role '{requiredRole}'.");

            // build claims (include role claims)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));

            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer))
                return StatusCode(500, "JWT configuration missing.");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expireMinutes = int.TryParse(_config["Jwt:ExpireMinutes"], out var m) ? m : 60;
            var expires = DateTime.UtcNow.AddMinutes(expireMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new LoginResponseDTO
            {
                Token = jwt,
                Expires = expires,
            };

            return Ok(response);
        }
    }
}