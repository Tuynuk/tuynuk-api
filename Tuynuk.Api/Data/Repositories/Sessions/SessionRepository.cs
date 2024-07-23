using Tuynuk.Api.Data.Repositories.Base;
using Tuynuk.Infrastructure.Models;

namespace Tuynuk.Api.Data.Repositories.Sessions
{
    public class SessionRepository : BaseRepository<Session, TuynukDbContext>, ISessionRepository
    {
        public SessionRepository(TuynukDbContext dbContext) : base(dbContext)
        {
        }
    }
}
