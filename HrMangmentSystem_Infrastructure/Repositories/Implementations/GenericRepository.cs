using HrMangmentSystem_Domain.Common;
using HrMangmentSystem_Infrastructure.Models;
using HrMangmentSystem_Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HrMangmentSystem_Infrastructure.Repositories.Implementations
{
    public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : SoftDeletable<TId>
    {
        private readonly AppDbContext _appDbContext;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _dbSet = _appDbContext.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            
        }

        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
           var db = _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
            return db;
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
           var db = await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
            return db;
        }

        public async Task<TEntity?> GetByIdAsync(TId id)
        {
            var db = await _dbSet
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync(e => EqualityComparer<TId>.Default.Equals(e.Id, id));
            return db;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(TId id, Guid? deletedByEmployeeId)
        {
           var entity = await _dbSet
                .FirstOrDefaultAsync(e=> !e.IsDeleted && EqualityComparer<TId>.Default.Equals(e.Id, id));

            if (entity == null)
                return;
                
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedByEmployeeId = deletedByEmployeeId;
            
            _dbSet.Update(entity);
           
       
        }

        public void Update(TEntity entity)
        {
            var db =  _dbSet.Update(entity);
        }
    }
}
