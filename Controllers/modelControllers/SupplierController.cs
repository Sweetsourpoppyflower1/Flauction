using Flauction.Data;
using Flauction.DTOs.Input;
using Flauction.DTOs.Output.ModelDTOs;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers.modelControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly DBContext _context;

        public SupplierController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            return await _context.Suppliers.ToListAsync();
        }

        [HttpGet("dto")]
        public async Task<ActionResult<IEnumerable<SupplierDTO>>> GetSupplierDTOs()
        {
            var supplierDTOs = await _context.Suppliers
                .Select(s => new SupplierDTO
                {
                    SupplierId = s.supplier_id,
                    Name = s.s_name,
                    Email = s.s_email,
                    Address = s.address,
                    PostalCode = s.postalcode,
                    Country = s.country,
                    Description = s.desc
                })
                .ToListAsync();

            return Ok(supplierDTOs);
        }

        [HttpGet("dto/{id}")]
        public async Task<ActionResult<SupplierDTO>> GetSupplierDTO(int id)
        {
            var dto = await _context.Suppliers
                .Where(s => s.supplier_id == id)
                .Select(s => new SupplierDTO
                {
                    SupplierId = s.supplier_id,
                    Name = s.s_name,
                    Email = s.s_email,
                    Address = s.s_address,
                    PostalCode = s.s_postalcode,
                    Country = s.s_country,
                    Description = s.s_desc
                })
                .FirstOrDefaultAsync();

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<SupplierDTO>> Login([FromBody] LoginDTO login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
                return BadRequest("Email and password are required");

            var master = await _context.Suppliers
                .AsNoTracking() 
                .FirstOrDefaultAsync(s => s.s_email == login.Username);

            if (master == null)
                return Unauthorized();

            if (master.s_password != login.Password)
                return Unauthorized();

            var dto = new SupplierDTO
            {
                SupplierId = master.supplier_id,
                Name = master.s_name,
                Email = master.s_email
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<Supplier>> CreateSupplier(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSupplier),
                new { id = supplier.supplier_id }, supplier);
        }

        [HttpPost("dto")]
        public async Task<ActionResult<SupplierDTO>> CreateSupplierDTO(SupplierDTO dto)
        {
            var supplier = new Supplier
            {
                s_name = dto.Name,
                s_email = dto.Email,
                address = dto.Address,
                postalcode = dto.PostalCode,
                country = dto.Country,
                desc = dto.Description
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            var resultDto = new SupplierDTO
            {
                SupplierId = supplier.supplier_id,
                Name = supplier.s_name,
                Email = supplier.s_email,
                Address = supplier.address,
                PostalCode = supplier.postalcode,
                Country = supplier.country,
                Description = supplier.desc
            };

            return CreatedAtAction(nameof(GetSupplierDTO), new { id = supplier.supplier_id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, Supplier supplier)
        {
            if (id != supplier.supplier_id)
                return BadRequest();

            _context.Entry(supplier).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Suppliers.Any(e => e.supplier_id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
                return NotFound();

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
                return NotFound();

            return Ok(supplier);
        }
    }
}
