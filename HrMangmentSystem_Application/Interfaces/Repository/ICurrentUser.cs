namespace HrMangmentSystem_Application.Interfaces.Repository
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }

        Guid? EmployeeId { get; }

        string? Email { get; }

        Guid? TenantId { get; }

        IReadOnlyList<string> Roles { get; }

    }
}
