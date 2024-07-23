using Tuynuk.Api.Data.Repositories.Base;
using File = Tuynuk.Infrastructure.Models.File;

namespace Tuynuk.Api.Data.Repositories.Files
{
    public class FilesRepository : BaseRepository<File, TuynukDbContext>, IFilesRepository
    {
        public FilesRepository(TuynukDbContext dbContext) : base(dbContext)
        {
        }
    }
}
