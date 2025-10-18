using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Images.Services;

namespace MultiShop.Images.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleDriveImageUploadsController : ControllerBase
    {
        private readonly ICloudStorageService _cloudStorageService;

        public GoogleDriveImageUploadsController(ICloudStorageService cloudStorageService)
        {
            _cloudStorageService = cloudStorageService;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(50_000_000)] // ~50MB
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string? fileName)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }

            var safeFileName = string.IsNullOrWhiteSpace(fileName)
                ? $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}"
                : Path.GetFileName(fileName);

            var savedUrl = await _cloudStorageService.UploadFileAsync(file, safeFileName);

            return Ok(new { fileName = safeFileName, url = savedUrl });
        }

        [HttpGet("signed-url/{fileName}")]
        public async Task<IActionResult> GetSignedUrl(string fileName, [FromQuery] int minutes = 30)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest("fileName is required");
            }

            var signedUrl = await _cloudStorageService.GetSignedUrlAsync(fileName, minutes <= 0 ? 30 : minutes);
            return Ok(new { fileName, signedUrl, expiresInMinutes = minutes <= 0 ? 30 : minutes });
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest("fileName is required");
            }

            await _cloudStorageService.DeleteFileAsync(fileName);
            return NoContent();
        }
    }
}
