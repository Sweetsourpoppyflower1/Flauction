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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // Simple POST: api/Media (keeps existing behavior)
        [HttpPost]
        public async Task<ActionResult<Media>> PostMedia(Media media)
        {
            _context.Medias.Add(media);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMedia", new { id = media.media_id }, media);
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Media>> Upload([FromForm] MediaUploadDTO dto)
        {
            if (dto.file == null || dto.file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Ensure uploads directory exists in wwwroot
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadDir = Path.Combine(webRoot, "uploads");
            Directory.CreateDirectory(uploadDir);

            // Generate unique filename
            var extension = Path.GetExtension(dto.file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadDir, fileName);

            // Save file
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.file.CopyToAsync(stream);
            }

            // Build URL
            var url = $"/uploads/{fileName}";

            // Provide sensible alt text
            var finalAlt = string.IsNullOrWhiteSpace(dto.alt_text)
                ? Path.GetFileNameWithoutExtension(dto.file.FileName)
                : dto.alt_text;

            // If this is primary, unset existing primary images
            if (dto.is_primary)
            {
                var existingPrimaries = _context.Medias
                    .Where(m => m.plant_id == dto.plant_id && m.is_primary)
                    .ToList();

                foreach (var p in existingPrimaries)
                {
                    p.is_primary = false;
                    _context.Entry(p).State = EntityState.Modified;
                }
            }

            var media = new Media
            {
                plant_id = dto.plant_id,
                url = url,
                alt_text = finalAlt,
                is_primary = dto.is_primary
            };

            _context.Medias.Add(media);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMedia", new { id = media.media_id }, media);
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

            // Optionally delete the physical file
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
                // swallow file delete errors — DB deletion should still proceed
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
