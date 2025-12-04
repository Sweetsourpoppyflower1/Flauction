using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Authorization;

namespace Flauction.Controllers.newControllers
{
    [Route("api/[controller]")]
    [Authorize()]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly DBContext _context;
        public SuppliersController(DBContext context)
        {
            _context = context;
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
        //[HttpPost("register")]
    }
}