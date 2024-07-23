using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Tuynuk.Api.Data.Repositories.Clients;
using Tuynuk.Api.Data.Repositories.Files;
using Tuynuk.Api.Data.Repositories.Sessions;
using Tuynuk.Api.Hubs.Sessions;
using Tuynuk.Infrastructure.Enums.Cliens;
using Tuynuk.Infrastructure.ViewModels.Files;
using File = Tuynuk.Infrastructure.Models.File;

namespace Tuynuk.Api.Services.Files
{
    public class FileService : IFileService
    {
        private readonly IHubContext<SessionHub, ISessionClient> _sessionHub;
        private readonly IFilesRepository _filesRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ISessionRepository _sessionRepository;

        public IConfiguration Configuration { get; }

        public ILogger<IFileService> Logger { get; }

        public HttpContext HttpContext { get; }

        public FileService(
            IHubContext<SessionHub, ISessionClient> sessionHub,
            IFilesRepository filesRepository,
            IClientRepository clientRepository,
            ISessionRepository sessionRepository,
            IConfiguration configuration,
            ILogger<IFileService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _sessionHub = sessionHub;
            _filesRepository = filesRepository;
            _clientRepository = clientRepository;
            _sessionRepository = sessionRepository;
            Configuration = configuration;
            Logger = logger;
            HttpContext = httpContextAccessor.HttpContext;
        }

        public async Task<GetFileViewModel> GetFileAsync(Guid fileId)
        {
            var file = await _filesRepository.GetAll()
                                .Include(l => l.Session)
                                .FirstOrDefaultAsync(l => l.Id == fileId);

            Stream stream = new MemoryStream(file.Content);

            var fileResult = new GetFileViewModel()
            {
                Name = file.Name,
                Content = stream
            };

            HttpContext.Response.Headers.Append("HMAC", file.HMAC);

            _sessionRepository.Remove(file.Session);

            await _sessionRepository.SaveChangesAsync();

            return fileResult;
        }

        public async Task<Guid> UploadFileAsync(IFormFile formfile, string sessionIdentifier, string HMAC)
        {
            var session = await _sessionRepository.GetAll()
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

            await _filesRepository.AddAsync(file);

            await _filesRepository.SaveChangesAsync();

            var receiverClient = session.Clients.FirstOrDefault(l => l.Type == ClientType.Receiver);

            await _sessionHub.Clients.Client(receiverClient.ConnectionId).OnFileUploaded(file.Id);

            return file.Id;
        }
    }
}
