using HrMangmentSystem_Domain.Tenants;
using HrMangmentSystem_Infrastructure.Interfaces.Repositories;
using HrMangmentSystem_Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace HrMangmentSystem_Infrastructure.Implementations.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly AppDbContext _appDbContext;

        public TenantRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Tenant?> FindByCodeAsync(string code)
        {
            var normalizedCode = code.Trim();
           var tenant =  await _appDbContext.Tenants.FirstOrDefaultAsync(t => t.Code == normalizedCode);
            return tenant;
        }

    

        public async Task<Tenant?> GetByIdAsync(Guid id)
        {
            var tenant = await _appDbContext.Tenants.FindAsync(id);
            return tenant;
        }

        public async Task<Tenant?> GetByNameAsync(string name)
        {
            var normalizedName = name.Trim();
            var tenant = await _appDbContext.Tenants.FirstOrDefaultAsync(t => t.Name == normalizedName);
            return tenant;
        }
    }
}
