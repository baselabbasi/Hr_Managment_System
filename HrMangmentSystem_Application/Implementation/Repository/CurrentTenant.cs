using HrMangmentSystem_Application.Interfaces.Repository;

namespace HrMangmentSystem_Application.Implementation.Repository
{
    public class CurrentTenant : ICurrentTenant
    {
        public Guid TenantId { get; private set; }

        public bool IsSet => TenantId  != Guid.Empty;

        public void SetTenant(Guid tenantId)
        {
            TenantId = tenantId;
        }
    }
}
