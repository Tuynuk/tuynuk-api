using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Tuynuk.Data;
using Tuynuk.Enums;
using Tuynuk.Hubs.Sessions;
using Tuynuk.ViewModels.Files;
using File = Tuynuk.Models.File;

namespace Tuynuk.Services.Files
{
    public class FileService : IFileService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHubContext<SessionHub, ISessionClient> _sessionHub;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileService(AppDbContext dbContext, 
            IHubContext<SessionHub, ISessionClient> sessionHub, 
            IHttpContextAccessor httpContextAccessor) 
        {
            _dbContext = dbContext;
            _sessionHub = sessionHub;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetFileViewModel> GetFileAsync(Guid fileId)
        {
            var file = await _dbContext.Files.FirstOrDefaultAsync(l => l.Id == fileId);
            Stream stream = new MemoryStream(file.Content);

            var fileResult = new GetFileViewModel()
            {
                Name = file.Name,
                Content = stream
            };

            _httpContextAccessor.HttpContext.Response.Headers.Append("HMAC", file.HMAC);

            return fileResult;
        }

        public async Task<Guid> UploadFileAsync(IFormFile formfile, string sessionIdentifier, string HMAC)
        {
            var session = await _dbContext.Sessions
                                .Include(l => l.Clients)
                                .FirstOrDefaultAsync(l => l.Identifier == sessionIdentifier);

            byte[] fileContent; 

            using (var ms = new MemoryStream())
            {
                formfile.CopyTo(ms);
                fileContent = ms.ToArray();
            }

            var file = new File
            {
                Content = fileContent,
                Name = formfile.FileName,
                SessionId = session.Id,
                HMAC = HMAC
            };

            await _dbContext.AddAsync(file);

            await _dbContext.SaveChangesAsync();

            var receiverClient = session.Clients.FirstOrDefault(l => l.Type == ClientType.Receiver);

            await _sessionHub.Clients.Client(receiverClient.ConnectionId).OnFileUploaded(file.Id);

            return file.Id;
        }
    }
}
