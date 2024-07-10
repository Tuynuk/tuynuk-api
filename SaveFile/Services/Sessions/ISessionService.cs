using SaveFile.ViewModels.Sessions;

namespace SaveFile.Services.Sessions
{
    public interface ISessionService
    {
        Task<Guid> CreateSessionAsync(CreateSessionViewModel view);
        Task<Guid> JoinSessionAsync(JoinSessionViewModel view);
    }
}
