using Tuynuk.ViewModels.Sessions;

namespace Tuynuk.Services.Sessions
{
    public interface ISessionService
    {
        Task<Guid> CreateSessionAsync(CreateSessionViewModel view);
        Task<Guid> JoinSessionAsync(JoinSessionViewModel view);
    }
}
