using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Domain.Enum.Request;
using HrMangmentSystem_Domain.Common;

namespace HrMangmentSystem_Domain.Entities.Requests
{
    public class GenericRequest : BaseEntity
    {
        public RequestType RequestType { get; set; }
        public RequestStatus RequestStatus { get; set; }


        public string RequestedByEmployeeId { get; set; } = null!; // Employee who made the request
        public Employee RequestedByEmployee { get; set; } = null!;


        public string? TargetEmployeeId { get; set; } // For requests that target a specific employee
        public Employee? TargetEmployee { get; set; } // For requests that target a specific employee


        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }


        public FinancialType? FinancialType { get; set; }
    }
}
