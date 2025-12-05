using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Flauction.Models;
using Microsoft.AspNetCore.Authorization;

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
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

            var resp = new AuthLoginResponse
            {
                Email = user.Email,
                Roles = roles
            };

            return Ok(resp);
        }
    }
}