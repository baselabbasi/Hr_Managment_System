using HrMangmentSystem_Domain.Tenants;

namespace HrMangmentSystem_Domain.Common
{
    public class TenantEntity<T> : BaseEntity<T>
    {
        public Guid TenantId { get; set; } 
        public Tenant Tenant { get; set; } = null!;

    }
}
