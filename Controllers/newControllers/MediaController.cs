using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flauction.Data;
using Flauction.Models;
using Flauction.DTOs.Upload;

namespace Flauction.Controllers.newControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IWebHostEnvironment _env;

        public MediaController(DBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Media
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Media>>> GetMedias()
        {
            return await _context.Medias.ToListAsync();
        }

        // GET: api/Media/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Media>> GetMedia(int id)
        {
            var media = await _context.Medias.FindAsync(id);

            if (media == null)
            {
                return NotFound();
            }

            return media;
        }

        // PUT: api/Media/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedia(int id, Media media)
        {
            if (id != media.media_id)
            {
                return BadRequest();
            }

            _context.Entry(media).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MediaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Media
        [HttpPost]
        public async Task<ActionResult<Media>> PostMedia(Media media)
        {
            _context.Medias.Add(media);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedia), new { id = media.media_id }, media);
        }

        // POST: api/Media/upload
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Media>> Upload([FromForm] MediaUpload dto)
        {
            if (dto.file == null || dto.file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadDir = Path.Combine(webRoot, "uploads");
            Directory.CreateDirectory(uploadDir);

            var extension = Path.GetExtension(dto.file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadDir, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.file.CopyToAsync(stream);
            }

            var url = $"/uploads/{fileName}";

            var finalAlt = string.IsNullOrWhiteSpace(dto.alt_text)
                ? Path.GetFileNameWithoutExtension(dto.file.FileName)
                : dto.alt_text;

            var media = new Media
            {
                url = url,
                alt_text = finalAlt,
            };

            _context.Medias.Add(media);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedia), new { id = media.media_id }, media);
        }

        // DELETE: api/Media/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            var media = await _context.Medias.FindAsync(id);
            if (media == null)
            {
                return NotFound();
            }

            try
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var filePath = Path.Combine(webRoot, media.url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch
            {
                
            }

            _context.Medias.Remove(media);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MediaExists(int id)
        {
            return _context.Medias.Any(e => e.media_id == id);
        }
    }
}
