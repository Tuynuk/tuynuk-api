using Microsoft.AspNetCore.Mvc;
using Tuynuk.Services.Files;

namespace Tuynuk.Contollers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile formFile, [FromQuery] string sessionIdentifier, [FromQuery] string HMAC) 
        {
            return Ok(await _fileService.UploadFileAsync(formFile, sessionIdentifier, HMAC));
        }

        [HttpGet]
        public async Task<IActionResult> GetFile([FromQuery] Guid fileId) 
        {
            var fileInfo = await _fileService.GetFileAsync(fileId);

            return File(fileInfo.Content, fileInfo.ContentType, fileDownloadName: fileInfo.Name);
        }
    }
}
