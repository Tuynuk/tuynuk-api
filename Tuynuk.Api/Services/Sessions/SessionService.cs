using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Tuynuk.Api.Data.Repositories.Clients;
using Tuynuk.Api.Data.Repositories.Sessions;
using Tuynuk.Api.Extensions;
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
        private readonly IServiceProvider _serviceProvider;

        private readonly Random _random;
        private const string ALLOWED_IDENTIFIER_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public IConfiguration Configuration { get; }
        public HttpContext HttpContext { get; }
        public ILogger<ISessionService> Logger { get; }

        public SessionService(
            IHubContext<SessionHub, ISessionClient> sessionHub,
            ISessionRepository sessionRepository,
            IClientRepository clientRepository,
            IServiceProvider serviceProvider,
            Random random,
            IConfiguration configuration,
            ILogger<ISessionService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _sessionHub = sessionHub;
            _sessionRepository = sessionRepository;
            _clientRepository = clientRepository;
            _serviceProvider = serviceProvider;
            _random = random;
            Configuration = configuration;
            HttpContext = httpContextAccessor.HttpContext;
            Logger = logger;
        }

        public async Task<Guid> CreateSessionAsync(CreateSessionViewModel view)
        {
            int identifierLength = Configuration.GetValue<int>("UniqueIdentifierLength");
            var identifier = await GenerateUniqueIdentifierAsync(identifierLength);

            string hashedIdentifier = identifier.ToSHA256Hash();

            var session = new Session()
            {
                Identifier = hashedIdentifier
            };
            await _sessionRepository.AddAsync(session);

            var receiverClient = new Client(view.ConnectionId, ClientType.Receiver, session.Id, view.PublicKey);

            await _clientRepository.AddAsync(receiverClient);
            await _sessionRepository.SaveChangesAsync();

            await _sessionHub.Clients.Client(receiverClient.ConnectionId).OnSessionCreated(identifier);

            return session.Id;
        }

        public async Task<Guid> JoinSessionAsync(JoinSessionViewModel view)
        {
            view.Identifier = view.Identifier.ToSHA256Hash();
            
            var session = await _sessionRepository.GetAll()
                            .Include(l => l.Clients)
                            .FirstOrDefaultAsync(l => l.Identifier == view.Identifier);

            bool doesSenderExist = session.Clients.Any(l => l.Type == ClientType.Sender);
            if (doesSenderExist) 
            {
                throw new Exception("Sender already exist");
            }

            var senderClient = new Client(view.ConnectionId, ClientType.Sender, session.Id, view.PublicKey);

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
            string hashedIdentifier = identifier.ToSHA256Hash();

            return await _sessionRepository.GetAll().AnyAsync(l => l.Identifier == hashedIdentifier);
        }

        public async Task RemoveAbandonedSessionsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var sessionRepository = scope.ServiceProvider.GetRequiredService<ISessionRepository>();
                var abandonedSessions = sessionRepository.GetAll().Where(l => l.Clients.All(l => l.ConnectionId == null));

                if (abandonedSessions.Any()) 
                {
                    sessionRepository.RemoveRange(abandonedSessions);
                    await sessionRepository.SaveChangesAsync();   
                }
            }
        }
    }
}
