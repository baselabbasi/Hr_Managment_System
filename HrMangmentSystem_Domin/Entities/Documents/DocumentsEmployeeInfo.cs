using HrMangmentSystem_Domain.Common;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Domain.Enum.Employee;

namespace HrMangmentSystem_Domain.Entities.Documents
{
    public class DocumentEmployeeInfo : BaseEntity
    {
        public string EmployeeId { get; set; } = null!;
        public Employee Employee { get; set; } = null!;


        public DocumentType DocumentCategory { get; set; }

        public string FileName { get; set; } = null!;

        public string FilePath { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public long FileSize { get; set; }


        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; } 

    }
}
