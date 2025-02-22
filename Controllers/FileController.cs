using blob_storage.Services;
using Microsoft.AspNetCore.Mvc;

namespace blob_storage.Controllers
{
    [ApiController]
    [Route("api")]
    public class FileController(BlobStorageService blobStorageService) : ControllerBase
    {
        private readonly BlobStorageService _blobStorageService = blobStorageService;

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido!");

            var fileUrl = await _blobStorageService.UploadFileAsync(file);
            return Ok(new { FileUrl = fileUrl });
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListFiles()
        {
            var files = await _blobStorageService.ListFilesAsync();
            return Ok(files);
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            var deleted = await _blobStorageService.DeleteFileAsync(fileName);

            if (!deleted)
                return NotFound(new { Message = "Arquivo não encontrado!" });

            return Ok(new { Message = "Arquivo excluído com sucesso!" });
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var (fileStream, contentType) = await _blobStorageService.DownloadFileAsync(fileName);

            if (fileStream == null)
                return NotFound(new { Message = "Arquivo não encontrado!" });

            return File(fileStream, contentType ?? "application/octet-stream", fileName);
        }
    }
}
