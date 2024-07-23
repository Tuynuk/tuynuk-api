using Tuynuk.Api.Data.Repositories.Base;
using Tuynuk.Infrastructure.Models;

namespace Tuynuk.Api.Data.Repositories.Sessions
{
    public interface ISessionRepository : IBaseRepository<Session, TuynukDbContext>
    {
    }
}
