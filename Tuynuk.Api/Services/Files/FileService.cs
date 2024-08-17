using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Tuynuk.Api.Data.Repositories.Clients;
using Tuynuk.Api.Data.Repositories.Files;
using Tuynuk.Api.Data.Repositories.Sessions;
using Tuynuk.Api.Extensions;
using Tuynuk.Api.Hubs.Sessions;
using Tuynuk.Infrastructure.Enums.Cliens;
using Tuynuk.Infrastructure.Exceptions.Clients;
using Tuynuk.Infrastructure.Exceptions.Commons;
using Tuynuk.Infrastructure.Exceptions.Files;
using Tuynuk.Infrastructure.Exceptions.Sessions;
using Tuynuk.Infrastructure.ViewModels.Files;
using File = Tuynuk.Infrastructure.Models.File;
using FileNotFoundException = Tuynuk.Infrastructure.Exceptions.Files.FileNotFoundEx;

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

            if (file == null)
            {
                throw new FileNotFoundException("File is not found");
            }

            // File can be downloaded only once
            if (file.Session.IsFileDownloadRequested)
            {
                throw new FileAlreadyRequestedEx("File is already requested for downloading");
            }

            file.Session.IsFileDownloadRequested = true;
            _sessionRepository.Update(file.Session);
            await _sessionRepository.DbContext.SaveChangesAsync();

            Stream stream = new MemoryStream(file.Content);

            var fileResult = new GetFileViewModel()
            {
                Name = file.Name,
                Content = stream
            };

            HttpContext.Response.Headers.Append("HMAC", file.HMAC);

            // Session is removed when the file is downloaded
            _sessionRepository.Remove(file.Session);

            if (await _filesRepository.SaveChangesAsync() <= 0)
            {
                throw new NoDatabaseChangesMadeEx("No changes made on database");
            }

            return fileResult;
        }

        public async Task<Guid> UploadFileAsync(IFormFile formfile, string sessionIdentifier, string HMAC)
        {
            string hashedIdentifier = sessionIdentifier.ToSHA256Hash();
            var session = await _sessionRepository.GetAll()
                                .Include(l => l.Clients)
                                .FirstOrDefaultAsync(l => l.Identifier == hashedIdentifier);

            if (session == null)
            {
                throw new SessionNotFoundEx("Session is not found");
            }

            byte[] fileContent;

            using (var ms = new MemoryStream())
            {
                formfile.CopyTo(ms);
                fileContent = ms.ToArray();
            }

            var file = new File(fileContent, formfile.FileName, session.Id, HMAC);

            await _filesRepository.AddAsync(file);

            if (await _filesRepository.SaveChangesAsync() <= 0)
            {
                throw new NoDatabaseChangesMadeEx("No changes made on database");
            }

            var receiverClient = session.Clients.FirstOrDefault(l => l.Type == ClientType.Receiver);
            if (receiverClient == null)
            {
                throw new ReceiverClientNotFoundEx("Receiver client is not found (possibly offline)");
            }

            await _sessionHub.Clients.Client(receiverClient.ConnectionId).OnFileUploaded(file.Id, file.Name, HMAC);

            return file.Id;
        }
    }
}
