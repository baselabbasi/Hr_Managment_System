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
    public class NormalRepository<TEntity, TId> : INormalRepository<TEntity, TId> where TEntity : BaseEntity<TId>
    {
        private readonly DbSet<TEntity> _dbSet;
        private readonly AppDbContext _appDbContext;

        public NormalRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _dbSet = _appDbContext.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);

        }

        public async Task DeleteAsync(TId id)
        {
            var db = await _dbSet
                .FirstOrDefaultAsync(e => EqualityComparer<TId>.Default.Equals(e.Id, id));

            if (db == null)
                return;


                _dbSet.Remove(db);
            

        }

        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var db = _dbSet.Where(predicate).ToListAsync();
            return db;
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            var db = await _dbSet.ToListAsync();
            return db;
        }

        public async Task<TEntity?> GetByIdAsync(TId id)
        {
            var db = await _dbSet
                .FirstOrDefaultAsync(e => EqualityComparer<TId>.Default.Equals(e.Id, id));
            return db;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }

     

        public void Update(TEntity entity)
        {
            var db = _dbSet.Update(entity);
        }

    
    }
}

   

