using Tuynuk.Api.Services.Base;
using Tuynuk.Infrastructure.ViewModels.Sessions;

namespace Tuynuk.Api.Services.Sessions
{
    public interface ISessionService : IBaseService<ISessionService>
    {
        Task<Guid> CreateSessionAsync(CreateSessionViewModel view);
        Task<Guid> JoinSessionAsync(JoinSessionViewModel view);
    }
}
