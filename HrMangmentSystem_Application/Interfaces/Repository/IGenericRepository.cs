using HrMangmentSystem_Domain.Common;
using System.Linq.Expressions;

namespace HrMangmentSystem_Application.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity , TId> where TEntity : TenantEntity<TId>
    {
        Task<TEntity?> GetByIdAsync(TId id);
        Task<List<TEntity>> GetAllAsync();
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);

        Task DeleteAsync(TId id , Guid? deletedByEmployeeId);
        Task<int> SaveChangesAsync();

        IQueryable<TEntity> Query(bool asNoTracking =true);
    }
}
