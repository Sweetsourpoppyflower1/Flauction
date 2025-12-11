using Microsoft.AspNetCore.Mvc;

namespace Flauction.DTOs.Upload
{
    public class MediaUpload
    {
        [FromForm]
        public IFormFile file { get; set; }

        [FromForm]
        public string? alt_text { get; set; }
    }
}
