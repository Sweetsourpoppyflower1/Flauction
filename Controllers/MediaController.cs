using Flauction.Data;
using Flauction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flauction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly DBContext _context;

        public MediaController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Media>>> GetMedias()
        {
            return await _context.Medias.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Media>> GetMedia(int id)
        {
            var media = await _context.Medias.FindAsync(id);

            if (media == null)
                return NotFound();

            return media;
        }

        [HttpPost]
        public async Task<ActionResult<Media>> CreateMedia(Media media)
        {
            _context.Medias.Add(media);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedia), new { id = media.media_id }, media);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedia(int id, Media media)
        {
            if (id != media.media_id)
                return BadRequest();

            _context.Entry(media).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Medias.Any(e => e.media_id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            var media = await _context.Medias.FindAsync(id);

            if (media == null)
                return NotFound();

            _context.Medias.Remove(media);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
