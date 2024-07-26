using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tuynuk.Infrastructure.Models.Base;

namespace Tuynuk.Api.Data.Repositories.Base
{
    public interface IBaseRepository<TEntity, TDbContext> : IDisposable
        where TEntity : BaseEntity
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; }

        DbSet<TEntity> Entities { get; }

        LocalView<TEntity> Local { get; }

        IQueryable<TEntity> GetAll();
        Task<TEntity> GetAsync(Guid id);

        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task<TEntity> AddAsync(TEntity entity);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        TEntity Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        Task<int> SaveChangesAsync();
    }
}
