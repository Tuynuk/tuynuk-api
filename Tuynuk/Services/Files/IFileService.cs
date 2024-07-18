using Tuynuk.ViewModels.Files;

namespace Tuynuk.Services.Files
{
    public interface IFileService
    {
        public Task<Guid> UploadFileAsync(IFormFile formFile, string sessionIdentifier, string HMAC);
        public Task<GetFileViewModel> GetFileAsync(Guid fileId);
    }
}
