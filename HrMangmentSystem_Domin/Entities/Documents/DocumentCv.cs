using HrMangmentSystem_Domain.Common;
using HrMangmentSystem_Domain.Entities.Recruitment;

namespace HrMangmentSystem_Domain.Entities.Documents
{

    public class DocumentCv : BaseEntity
    {
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public string CandidateName { get; set; } = null!;
        public string CandidateEmail { get; set; } = null!;
        public string? CandidatePhone { get; set; }

       //  public ICollection<JobApplication> JobApplications { get; set; } = new();
    }



}
