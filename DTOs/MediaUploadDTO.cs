using Microsoft.AspNetCore.Mvc;

public class MediaUploadDTO
{
    [FromForm]
    public IFormFile file { get; set; }

    [FromForm]
    public int plant_id { get; set; }

    [FromForm]
    public string? alt_text { get; set; }

    [FromForm]
    public bool is_primary { get; set; } = false;
}
