using HrMangmentSystem_Domain.Tenants;

namespace HrMangmentSystem_Application.Interfaces.Repositories
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetByIdAsync(Guid id);

        Task<Tenant?> GetByNameAsync(string name);

        Task<Tenant?> FindByCodeAsync(string code);
    }
}
