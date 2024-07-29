using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Tuynuk.Api.Data.Repositories.Clients;
using Tuynuk.Api.Services.Sessions;
using Tuynuk.Infrastructure.ViewModels.Sessions;

namespace Tuynuk.Api.Hubs.Sessions
{
    public class SessionHub : Hub<ISessionClient>
    {
        private readonly ISessionService _sessionService;
        private readonly IClientRepository _clientRepository;

        public SessionHub(ISessionService sessionService, IClientRepository clientRepository)
        {
            _sessionService = sessionService;
            _clientRepository = clientRepository;
        }

        public Task CreateSession(CreateSessionViewModel view)
        {
            view.ConnectionId = Context.ConnectionId;
            return _sessionService.CreateSessionAsync(view);
        }

        public Task JoinSession(JoinSessionViewModel view)
        {
            view.ConnectionId = Context.ConnectionId;
            return _sessionService.JoinSessionAsync(view);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var disconnectedClient = await _clientRepository.GetAll().FirstOrDefaultAsync(l => l.ConnectionId == Context.ConnectionId);
            if (disconnectedClient != null) 
            {
                disconnectedClient.ConnectionId = null;
                _clientRepository.Update(disconnectedClient);
                await _clientRepository.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
