using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Tuynuk.Api.Data.Repositories.Clients;
using Tuynuk.Api.Data.Repositories.Sessions;
using Tuynuk.Api.Hubs.Sessions;
using Tuynuk.Infrastructure.Enums.Cliens;
using Tuynuk.Infrastructure.Models;
using Tuynuk.Infrastructure.ViewModels.Sessions;

namespace Tuynuk.Api.Services.Sessions
{
    public class SessionService : ISessionService
    {
        private readonly IHubContext<SessionHub, ISessionClient> _sessionHub;
        private readonly ISessionRepository _sessionRepository;
        private readonly IClientRepository _clientRepository;

        private readonly Random _random;
        private const string ALLOWED_IDENTIFIER_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public IConfiguration Configuration { get; }
        public HttpContext HttpContext { get; }
        public ILogger<ISessionService> Logger { get; }

        public SessionService(
            IHubContext<SessionHub, ISessionClient> sessionHub,
            ISessionRepository sessionRepository,
            IClientRepository clientRepository,
            Random random,
            IConfiguration configuration,
            ILogger<ISessionService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _sessionHub = sessionHub;
            _sessionRepository = sessionRepository;
            _clientRepository = clientRepository;
            _random = random;
            Configuration = configuration;
            HttpContext = httpContextAccessor.HttpContext;
            Logger = logger;
        }

        public async Task<Guid> CreateSessionAsync(CreateSessionViewModel view)
        {
            int identifierLength = Configuration.GetValue<int>("UniqueIdentifierLength");
            var identifier = await GenerateUniqueIdentifierAsync(identifierLength);
            var session = new Session()
            {
                Identifier = identifier
            };
            await _sessionRepository.AddAsync(session);

            var receiverClient = new Client
            {
                PublicKey = view.PublicKey,
                ConnectionId = view.ConnectionId,
                Type = ClientType.Receiver,
                SessionId = session.Id
            };

            await _clientRepository.AddAsync(receiverClient);
            await _sessionRepository.SaveChangesAsync();

            await _sessionHub.Clients.Client(receiverClient.ConnectionId).OnSessionCreated(session.Identifier);

            return session.Id;
        }

        public async Task<Guid> JoinSessionAsync(JoinSessionViewModel view)
        {
            var session = await _sessionRepository.GetAll()
                            .Include(l => l.Clients)
                            .FirstOrDefaultAsync(l => l.Identifier == view.Identifier);
            var senderClient = new Client
            {
                SessionId = session.Id,
                Type = ClientType.Sender,
                PublicKey = view.PublicKey,
                ConnectionId = view.ConnectionId
            };

            await _clientRepository.AddAsync(senderClient);

            await _sessionRepository.SaveChangesAsync();

            var receiverClient = session.Clients.FirstOrDefault(l => l.Type == ClientType.Receiver);

            await _sessionHub.Clients.Client(receiverClient.ConnectionId).OnSessionReady(senderClient.PublicKey);
            await _sessionHub.Clients.Client(senderClient.ConnectionId).OnSessionReady(receiverClient.PublicKey);

            return session.Id;
        }

        private async Task<string> GenerateUniqueIdentifierAsync(int length)
        {
            string identifier = GenerateRandomIdentifier(length);

            while (await DoesIdentifierExistAsync(identifier))
            {
                identifier = GenerateRandomIdentifier(length);
            }

            return identifier;
        }

        private string GenerateRandomIdentifier(int length = 6)
        {
            var identifierBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int randomIndex = _random.Next(0, ALLOWED_IDENTIFIER_CHARS.Length);
                identifierBuilder.Append(ALLOWED_IDENTIFIER_CHARS[randomIndex]);
            }

            string identifier = identifierBuilder.ToString();

            return identifier;
        }

        private async Task<bool> DoesIdentifierExistAsync(string identifier)
        {
            return await _sessionRepository.GetAll().AnyAsync(l => l.Identifier == identifier);
        }
    }
}
