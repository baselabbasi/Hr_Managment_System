using System.Linq.Expressions;

namespace HrMangmentSystem_Infrastructure.Repositories.Interfaces
{
    public interface INormalRepository <TEntity, TId> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TId id);
        Task<List<TEntity>> GetAllAsync();
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
       Task  DeleteAsync(TId id);
        Task<int> SaveChangesAsync();
    }
}
