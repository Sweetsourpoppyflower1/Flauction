//using Flauction.Data;
//using Flauction.DTOs.Output.ModelDTOs;
//using Flauction.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Flauction.Controllers.modelControllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MediaController : ControllerBase
//    {
//        private readonly DBContext _context;

//        public MediaController(DBContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Media>>> GetMedias()
//        {
//            return await _context.Medias.ToListAsync();
//        }

//        [HttpGet("dto")]
//        public async Task<ActionResult<IEnumerable<MediaDTO>>> GetMediaDTOs()
//        {
//            var mediaDTOs = await _context.Medias
//                .Join(_context.Plants,
//                      m => m.plant_id,
//                      p => p.plant_id,
//                      (m, p) => new MediaDTO
//                      {
//                          MediaId = m.media_id,
//                          PlantName = p.productname,
//                          Url = m.url,
//                          AltText = m.alt_text,
//                          IsPrimary = m.is_primary
//                      })
//                .ToListAsync();

//            return Ok(mediaDTOs);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<Media>> GetMedia(int id)
//        {
//            var media = await _context.Medias.FindAsync(id);

//            if (media == null)
//                return NotFound();

//            return media;
//        }

//        [HttpGet("dto/{id}")]
//        public async Task<ActionResult<MediaDTO>> GetMediaDTO(int id)
//        {
//            var dto = await _context.Medias
//                .Where(m => m.media_id == id)
//                .Join(_context.Plants,
//                      m => m.plant_id,
//                      p => p.plant_id,
//                      (m, p) => new MediaDTO
//                      {
//                          MediaId = m.media_id,
//                          PlantName = p.productname,
//                          Url = m.url,
//                          AltText = m.alt_text,
//                          IsPrimary = m.is_primary
//                      })
//                .FirstOrDefaultAsync();

//            if (dto == null)
//                return NotFound();

//            return Ok(dto);
//        }

//        [HttpPost]
//        public async Task<ActionResult<Media>> CreateMedia(Media media)
//        {
//            _context.Medias.Add(media);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetMedia), new { id = media.media_id }, media);
//        }

//        [HttpPost("dto")]
//        public async Task<ActionResult<MediaDTO>> CreateMediaDTO(MediaDTO dto)
//        {
//            var plant = await _context.Plants
//                .FirstOrDefaultAsync(p => p.productname == dto.PlantName);

//            if (plant == null)
//                return BadRequest($"Plant '{dto.PlantName}' does not exist");

//            var media = new Media
//            {
//                plant_id = plant.plant_id,
//                url = dto.Url,
//                alt_text = dto.AltText,
//                is_primary = dto.IsPrimary
//            };

//            _context.Medias.Add(media);
//            await _context.SaveChangesAsync();

//            var resultDto = new MediaDTO
//            {
//                MediaId = media.media_id,
//                PlantName = plant.productname,
//                Url = media.url,
//                AltText = media.alt_text,
//                IsPrimary = media.is_primary
//            };

//            return CreatedAtAction(nameof(GetMediaDTO), new { id = media.media_id }, resultDto);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateMedia(int id, Media media)
//        {
//            if (id != media.media_id)
//                return BadRequest();

//            _context.Entry(media).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!_context.Medias.Any(e => e.media_id == id))
//                    return NotFound();

//                throw;
//            }

//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteMedia(int id)
//        {
//            var media = await _context.Medias.FindAsync(id);

//            if (media == null)
//                return NotFound();

//            _context.Medias.Remove(media);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}
