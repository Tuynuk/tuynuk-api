using Tuynuk.Api.Services.Base;
using Tuynuk.Infrastructure.ViewModels.Files;

namespace Tuynuk.Api.Services.Files
{
    public interface IFileService : IBaseService<IFileService>
    {
        public Task<Guid> UploadFileAsync(IFormFile formFile, string sessionIdentifier, string HMAC);
        public Task<GetFileViewModel> GetFileAsync(Guid fileId);
    }
}
