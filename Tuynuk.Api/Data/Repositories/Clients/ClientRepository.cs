using Tuynuk.Api.Data.Repositories.Base;
using Tuynuk.Infrastructure.Models;

namespace Tuynuk.Api.Data.Repositories.Clients
{
    public class ClientRepository : BaseRepository<Client, TuynukDbContext>, IClientRepository
    {
        public ClientRepository(TuynukDbContext dbContext) : base(dbContext)
        {
        }
    }
}
