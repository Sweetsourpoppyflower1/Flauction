using Flauction.Data;
using Flauction.DTOs;
using Flauction.DTOs.Output.ModelDTOs;
using Flauction.DTOs.Output.RegisterDTOs;
using Flauction.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flauction.Controllers.newControllers
{
    [Route("api/[controller]")]
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

        //GET all suppliers
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SupplierDTO>>> GetSuppliers()
        {
            var suppliers = await _context.Suppliers
                .Select(s => new SupplierDTO
                {
                    SupplierId = s.Id,
                    Name = s.name,
                    Email = s.Email,
                    Address = s.address,
                    PostalCode = s.postalcode,
                    Country = s.country,
                    Description = s.desc
                })
                .ToListAsync();

            return Ok(suppliers);
        }

        //GET supplier by ID
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<SupplierDTO>> GetSupplier(string id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound(new { message = "Supplier not found" });
            }

            var supplierDto = new SupplierDTO
            {
                SupplierId = supplier.Id,
                Name = supplier.name,
                Email = supplier.Email,
                Address = supplier.address,
                PostalCode = supplier.postalcode,
                Country = supplier.country,
                Description = supplier.desc
            };

            return Ok(supplierDto);
        }

        //GET plants by supplier ID with images
        [HttpGet("{supplierId}/plants")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetSupplierPlants(string supplierId)
        {
            // valideerr of supplier bestaat
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null)
            {
                return NotFound(new { message = "Supplier not found" });
            }

            // get de planten van de leverancier met hun primaire afbeeldingen
            var plants = await _context.Plants
                .Where(p => p.supplier_id == supplierId)
                .Select(p => new
                {
                    PlantId = p.plant_id,
                    ProductName = p.productname,
                    SupplierName = supplier.name,
                    Category = p.category,
                    Form = p.form,
                    Quality = p.quality,
                    MinStem = p.min_stem,
                    StemsBunch = p.stems_bunch,
                    Maturity = p.maturity,
                    Description = p.desc,
                    StartPrice = p.start_price,
                    MinPrice = p.min_price,
                    // get de primaire afbeelding
                    ImageUrl = _context.MediaPlants
                        .Where(m => m.plant_id == p.plant_id && m.is_primary)
                        .Select(m => m.url)
                        .FirstOrDefault(),
                    ImageAlt = _context.MediaPlants
                        .Where(m => m.plant_id == p.plant_id && m.is_primary)
                        .Select(m => m.alt_text)
                        .FirstOrDefault() ?? p.productname
                })
                .ToListAsync();

            return Ok(plants);
        }

        //POST login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<SupplierDTO>> Login([FromBody] Supplier login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.PasswordHash))
                return BadRequest("Email and password are required.");

            var supplier = await _context.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == login.Email);

            if (supplier == null || supplier.PasswordHash != login.PasswordHash)
            {
                return Unauthorized();
            }

            var supplierDTO = new SupplierDTO
            {
                SupplierId = supplier.Id,
                Name = supplier.name,
                Email = supplier.Email,
                Address = supplier.address,
                PostalCode = supplier.postalcode,
                Country = supplier.country,
                Description = supplier.desc
            };
            await _context.SaveChangesAsync();

            return Ok(supplierDTO);
        }

            //POST register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] SupplierRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByEmailAsync(dto.SupplierEmail) != null)
                return Conflict(new { message = "Email already in use." });

            var identityUser = new User
            {
                UserName = dto.SupplierEmail,
                Email = dto.SupplierEmail,
                EmailConfirmed = true
            };

            var createUserResult = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!createUserResult.Succeeded)
                return BadRequest(createUserResult.Errors);

            const string role = "Supplier";

            if (!await _roleManager.RoleExistsAsync(role))
                return StatusCode(500, new { message = $"Required role '{role}' not found." });

            var addRoleResult = await _userManager.AddToRoleAsync(identityUser, role);
            if (!addRoleResult.Succeeded)
                return StatusCode(500, new { message = "Failed to assign role to user." });

            // maak de Supplier aan en link deze aan de IdentityUser
            var supplier = new Supplier
            {
                Id = identityUser.Id,  
                name = dto.SupplierName,
                address = dto.Address,
                postalcode = dto.PostalCode,
                country = dto.Country,
                iban = dto.Iban,
                desc = dto.Desc,
                Email = dto.SupplierEmail,
                UserName = dto.SupplierEmail
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, new SupplierDTO
            {
                SupplierId = supplier.Id,
                Name = supplier.name,
                Email = supplier.Email,
                Address = supplier.address,
                PostalCode = supplier.postalcode,
                Country = supplier.country,
                Description = supplier.desc
            });
        }
    }
}
