//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Flauction.Data;
//using Flauction.Models;
//using Microsoft.AspNetCore.Authorization;

//namespace Flauction.Controllers.newControllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CompaniesController : ControllerBase
//    {
//        private readonly DBContext _context;
//        public CompaniesController(DBContext context)
//        {
//            _context = context;
//        }

//        //GET
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
//        {
//            return await _context.Companies.ToListAsync();
//        }

//        //GET by email and password
//        [HttpGet("{email}/{password}")]
//        public async Task<ActionResult<Company>> GetAuctionMaster(string email, string password)
//        {
//            var company = await _context.AuctionMasters
//                .FirstOrDefaultAsync(c => c.Email == email && c.PasswordHash == password);

//            if (company == null)
//            {
//                return NotFound();
//            }

//            return company;
//        }
//    }
//}