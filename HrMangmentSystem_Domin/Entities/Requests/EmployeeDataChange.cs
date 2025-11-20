using HrMangmentSystem_Domain.Enum.Request;
using HrMangmentSystem_Domain.Common;


namespace HrMangmentSystem_Domain.Entities.Requests
{
    public class EmployeeDataChange : SoftDeletable<int>
    {
        public string RequsetedDataJson { get; set; } = null!;

        public string? ApprovedDataJson { get; set; }

        public bool IsApproved { get; set; } 

        public DateTime? AppliedAt { get; set; }
        
        public GenericRequest GenericRequest { get; set; } = null!;
    }
}
