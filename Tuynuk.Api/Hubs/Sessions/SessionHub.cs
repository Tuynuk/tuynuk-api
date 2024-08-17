using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Tuynuk.Api.Data.Repositories.Clients;
using Tuynuk.Api.Extensions;
using Tuynuk.Api.Services.Sessions;
using Tuynuk.Infrastructure.Exceptions.Clients;
using Tuynuk.Infrastructure.Exceptions.Commons;
using Tuynuk.Infrastructure.Exceptions.Sessions;
using Tuynuk.Infrastructure.ViewModels.Sessions;

namespace Tuynuk.Api.Hubs.Sessions
{
    public class SessionHub : Hub<ISessionClient>
    {
        private readonly ISessionService _sessionService;
        private readonly IClientRepository _clientRepository;
        private readonly ILogger<SessionHub> _logger;

        public SessionHub(ISessionService sessionService, IClientRepository clientRepository, ILogger<SessionHub> logger)
        {
            _sessionService = sessionService;
            _clientRepository = clientRepository;
            _logger = logger;
        }

        public async Task CreateSession(CreateSessionViewModel view)
        {
            try
            {
                view.ConnectionId = Context.ConnectionId;
                await _sessionService.CreateSessionAsync(view);
            }
            catch (NoDatabaseChangesMadeEx ex)
            {
                await Clients.Caller.OnErrorOccured(ex.ToBaseError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await Clients.Caller.OnErrorOccured(ex.ToBaseError());
            }

            return;
        }

        public async Task JoinSession(JoinSessionViewModel view)
        {
            try
            {
                view.ConnectionId = Context.ConnectionId;
                await _sessionService.JoinSessionAsync(view);
            }
            catch (SessionNotFoundEx ex)
            {
                await Clients.Caller.OnErrorOccured(ex.ToBaseError());
            }
            catch (NoDatabaseChangesMadeEx ex)
            {
                await Clients.Caller.OnErrorOccured(ex.ToBaseError());
            }
            catch (ReceiverClientNotFoundEx ex)
            {
                await Clients.Caller.OnErrorOccured(ex.ToBaseError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await Clients.Caller.OnErrorOccured(ex.ToBaseError());
            }

            return;
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            if (ex != null)
            {
                _logger.LogError(ex, "An unexpected error occurred during disconnection.");
            }

            var disconnectedClient = await _clientRepository.GetAll().FirstOrDefaultAsync(l => l.ConnectionId == Context.ConnectionId);
            if (disconnectedClient != null)
            {
                disconnectedClient.ConnectionId = null;
                _clientRepository.Update(disconnectedClient);
                await _clientRepository.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(ex);
        }
    }
}
