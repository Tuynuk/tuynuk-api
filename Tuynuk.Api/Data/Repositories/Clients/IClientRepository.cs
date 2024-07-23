using Tuynuk.Api.Data.Repositories.Base;
using Tuynuk.Infrastructure.Models;

namespace Tuynuk.Api.Data.Repositories.Clients
{
    public interface IClientRepository : IBaseRepository<Client, TuynukDbContext>
    { 
    }
}
