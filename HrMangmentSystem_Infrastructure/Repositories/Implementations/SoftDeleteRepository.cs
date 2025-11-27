using HrMangmentSystem_Domain.Common;
using HrMangmentSystem_Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace HrMangmentSystem_Infrastructure.Repositories.Implementations
{
    public class SoftDeleteRepository<TEntity, TId> : GenericRepository<TEntity, TId> where TEntity : SoftDeletable<TId>
    {
        private readonly DbSet<TEntity> _dbSet;
        private readonly AppDbContext _appDbContext;

        public SoftDeleteRepository(AppDbContext appDbContext) : base(appDbContext) 
        {
            _appDbContext = appDbContext;
            _dbSet = _appDbContext.Set<TEntity>();
        }

      
        public override async Task DeleteAsync(TId id, Guid? deletedByEmployeeId)
        {
            var entity = await _dbSet
                 .FirstOrDefaultAsync(e => !e.IsDeleted && e.Id!.Equals(id));

            if (entity == null)
                return;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedByEmployeeId = deletedByEmployeeId;

            _dbSet.Update(entity);
        }

        public override async Task<List<TEntity>> GetAllAsync()
        {
            var db = await _dbSet.Where(e => !e.IsDeleted).ToListAsync();

            return db;
        }

        public override async Task<TEntity?> GetByIdAsync(TId id)
        {
            var db = await _dbSet
                .FirstOrDefaultAsync(e => !e.IsDeleted &&  e.Id!.Equals(id));
          return db;

        }
        public override async Task<List<TEntity>> FindAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            var db = await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
           
           return db;

        }
        public override IQueryable<TEntity> Query(bool asNoTracking = true)
        {
            return base.Query(asNoTracking).Where(e => !e.IsDeleted) ;
        }




    }
}

   

