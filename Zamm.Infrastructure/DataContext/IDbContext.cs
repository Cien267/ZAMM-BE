using Microsoft.EntityFrameworkCore;

namespace Zamm.Infrastructure.DataContext
{
    public interface IDbContext : IDisposable
    {
        DbSet<TEntity> SetEntity<TEntity>() where TEntity : class;
        Task<int> CommitChangeAsync();
    }
}
