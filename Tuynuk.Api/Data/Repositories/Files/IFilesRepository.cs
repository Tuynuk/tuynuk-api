using Tuynuk.Api.Data.Repositories.Base;
using File = Tuynuk.Infrastructure.Models.File;

namespace Tuynuk.Api.Data.Repositories.Files
{
    public interface IFilesRepository : IBaseRepository<File, TuynukDbContext>
    {
    }
}
