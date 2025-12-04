using Flauction.Data;
using Flauction.DTOs;
using Flauction.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flauction.DTOs.Output.RegisterDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flauction.Controllers.newControllers
{
    [Route("api/[controller]")]
    [Authorize()]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SuppliersController(DBContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            return await _context.Suppliers.ToListAsync();
        }

        //GET by email and password
        [HttpGet("{email}/{password}")]
        public async Task<ActionResult<Supplier>> GetSupplier(string email, string password)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Email == email && s.PasswordHash == password);
            if (supplier == null)
            {
                return NotFound();
            }
            return supplier;
        }

        //POST register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] SupplierRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByEmailAsync(dto.SupplierEmail) != null)
                return Conflict("Email already in use.");

            var identityUser = new User
            {
                UserName = dto.SupplierEmail,
                Email = dto.SupplierEmail
            };

            var createUserResult = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!createUserResult.Succeeded)
                return BadRequest(createUserResult.Errors);

            const string role = "Supplier";

            if (!await _roleManager.RoleExistsAsync(role))
                return StatusCode(500, $"Required role '{role}' not found.");

            var addRoleResult = await _userManager.AddToRoleAsync(identityUser, role);
            if (!addRoleResult.Succeeded)
                return StatusCode(500, "Failed to assign role to user.");

            var supplier = new Supplier
            {
                name = dto.SupplierName,
                address = dto.Address,
                postalcode = dto.PostalCode,
                country = dto.Country,
                iban = dto.Iban,
                desc = dto.Desc,
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSuppliers), new { }, supplier);
        }
    }
}