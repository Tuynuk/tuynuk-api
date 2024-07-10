using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SaveFile.Data;
using SaveFile.Enums;
using SaveFile.Hubs.Sessions;
using SaveFile.Models;
using SaveFile.ViewModels.Sessions;
using System.Text;

namespace SaveFile.Services.Sessions
{
    public class SessionService : ISessionService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHubContext<SessionHub, ISessionClient> _sessionHub;
        private readonly Random _random;
        private const string ALLOWED_IDENTIFIER_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public SessionService(
            AppDbContext dbContext, 
            IHubContext<SessionHub, 
            ISessionClient> sessionHub,
            Random random) 
        {
            _dbContext = dbContext; 
            _sessionHub = sessionHub;
            _random = random;
        }

        public async Task<Guid> CreateSessionAsync(CreateSessionViewModel view)
        {
            var identifier = GetRandomIdentifier();
            var session = new Session()
            {
                Identifier = identifier
            };
            await _dbContext.Sessions.AddAsync(session);

            var receiverClient = new Client
            {
                 PublicKey = view.PublicKey,
                 ConnectionId = view.ConnectionId,
                 Type = ClientType.Receiver,
                 SessionId = session.Id
            };

            await _dbContext.Clients.AddAsync(receiverClient);
            await _dbContext.SaveChangesAsync();

            await _sessionHub.Clients.Client(receiverClient.ConnectionId).OnSessionCreated(session.Identifier);

            return session.Id;
        }

        public async Task<Guid> JoinSessionAsync(JoinSessionViewModel view) 
        {
            var session = _dbContext.Sessions
                            .Include(l => l.Clients)
                            .FirstOrDefault(l => l.Identifier == view.Identifier);
            var senderClient = new Client
            {
                SessionId = session.Id,
                Type = ClientType.Sender,
                PublicKey = view.PublicKey,
                ConnectionId = view.ConnectionId
            };

            await _dbContext.Clients.AddAsync(senderClient);

            await _dbContext.SaveChangesAsync();

            var receiverClient = session.Clients.FirstOrDefault(l => l.Type == ClientType.Receiver);
            
            await _sessionHub.Clients.Client(receiverClient.ConnectionId).OnSessionReady(senderClient.PublicKey);
            await _sessionHub.Clients.Client(senderClient.ConnectionId).OnSessionReady(receiverClient.PublicKey);

            return session.Id;
        }

        private string GetRandomIdentifier(int length = 6) 
        {
            var identifierBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int index = _random.Next(0, ALLOWED_IDENTIFIER_CHARS.Length - 1);
                identifierBuilder.Append(ALLOWED_IDENTIFIER_CHARS[index]);
            }

            string identifier = identifierBuilder.ToString();

            bool isDuplicate = _dbContext.Sessions.Any(l => l.Identifier == identifier);

            if (isDuplicate) 
            {
                return GetRandomIdentifier(length);
            }

            return identifier;
        }
    }
}
