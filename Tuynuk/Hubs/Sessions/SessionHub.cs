using Microsoft.AspNetCore.SignalR;
using Tuynuk.Services.Sessions;
using Tuynuk.ViewModels.Sessions;

namespace Tuynuk.Hubs.Sessions
{
    public class SessionHub : Hub<ISessionClient>
    {
        private readonly ISessionService _sessionService;

        public SessionHub(ISessionService sessionService) 
        {
            _sessionService = sessionService;
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
    }
}
