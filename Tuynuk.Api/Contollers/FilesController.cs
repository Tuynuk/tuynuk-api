using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tuynuk.Api.Extensions;
using Tuynuk.Api.Services.Files;
using Tuynuk.Infrastructure.Exceptions.Clients;
using Tuynuk.Infrastructure.Exceptions.Commons;
using Tuynuk.Infrastructure.Exceptions.Files;
using Tuynuk.Infrastructure.Exceptions.Sessions;
using Tuynuk.Infrastructure.Response;
using FileNotFoundEx = Tuynuk.Infrastructure.Exceptions.Files.FileNotFoundEx;

namespace Tuynuk.Api.Contollers
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
        public async Task<ActionResult<BaseResponse>> UploadFile([FromForm] IFormFile formFile, [FromQuery] string sessionIdentifier, [FromQuery] string HMAC)
        {
            try
            {
                Guid fileId = await _fileService.UploadFileAsync(formFile, sessionIdentifier, HMAC);

                return Ok(new BaseResponse
                {
                    Data = fileId
                });
            }
            catch (SessionNotFoundEx ex)
            {
                return NotFound(ex.ToBaseResponse());
            }
            catch (ReceiverClientNotFoundEx ex)
            {
                return BadRequest(ex.ToBaseResponse());
            }
            catch (NoDatabaseChangesMadeEx ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.ToBaseResponse());
            }
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse>> GetFile([FromQuery] Guid fileId)
        {
            try
            {
                var fileInfo = await _fileService.GetFileAsync(fileId);

                return File(fileInfo.Content, fileInfo.ContentType, fileDownloadName: fileInfo.Name);
            }
            catch (FileNotFoundEx ex)
            {
                return NotFound(ex.ToBaseResponse());
            }
            catch (FileAlreadyRequestedEx ex)
            {
                return BadRequest(ex.ToBaseResponse());
            }
            catch (NoDatabaseChangesMadeEx ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.ToBaseResponse());
            }
        }
    }
}
