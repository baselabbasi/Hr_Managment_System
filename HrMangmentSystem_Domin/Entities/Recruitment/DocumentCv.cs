using HrMangmentSystem_Domain.Common;

namespace HrMangmentSystem_Domain.Entities.Recruitment
{

    public class DocumentCv : BaseEntity<int>
    {
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public string CandidateName { get; set; } = null!;
        public string CandidateEmail { get; set; } = null!;
        public string? CandidatePhone { get; set; }
        public bool IsDeleted { get ; set ; }
        public DateTime DeletedAt { get ; set; }
        public string? DeletedByEmployeeId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //  public ICollection<JobApplication> JobApplications { get; set; } = new();
    }



}
