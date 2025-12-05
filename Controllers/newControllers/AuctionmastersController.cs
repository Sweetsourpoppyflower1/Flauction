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
    [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "Admin")]
    [ApiController]
    public class AuctionmastersController : ControllerBase
    {
        private readonly DBContext _context;
        public AuctionmastersController(DBContext context)
        {
            _context = context;
        }

        //GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionMaster>>> GetAuctionMasters()
        {
            return await _context.AuctionMasters.ToListAsync();
        }

        //GET by email and password
        [HttpGet("{email}/{password}")]
        public async Task<ActionResult<AuctionMaster>> GetAuctionMaster(string email, string password)
        {
            var auctionMaster = await _context.AuctionMasters
                .FirstOrDefaultAsync(am => am.Email == email && am.PasswordHash == password);
            if (auctionMaster == null)
            {
                return NotFound();
            }
            return auctionMaster;
        }

        //POST Login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuctionMaster>> Login([FromBody] AuctionMaster login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.PasswordHash))
                return BadRequest("Email and password are required.");

            var master = await _context.AuctionMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(am => am.Email == login.Email);

            if (master == null || master.PasswordHash != login.PasswordHash)
            {
                return Unauthorized();
            }

            return master;
        }

        //POST add auctionmaster
        [HttpPost("Add Auctionmaster")]
        public async Task<ActionResult<AuctionMaster>> CreateAuctionMaster(AuctionMaster auctionMaster)
        {
            _context.AuctionMasters.Add(auctionMaster);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAuctionMaster), new { id = auctionMaster.Id }, auctionMaster);
        }
    }
}
