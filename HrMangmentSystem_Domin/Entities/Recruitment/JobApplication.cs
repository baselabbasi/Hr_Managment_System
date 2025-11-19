using HrMangmentSystem_Domain.Common;
using HrMangmentSystem_Domain.Entities.Documents;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Domain.Enum.Recruitment;

namespace HrMangmentSystem_Domain.Entities.Recruitment
{
    public class JobApplication : BaseEntity
    {
        public string JobPositionId { get; set; } = null!;
        public JobPosition JobPosition { get; set; } = null!;

        public string DocumentCvId { get; set; } = null!;
        public DocumentCv DocumentCv { get; set; } = null!;

        public JobApplicationStatus Status { get; set; } = JobApplicationStatus.New;


        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public Employee? ReviewedByEmployeeId { get; set; }

        public string? Notes { get; set; }

        }
}
