using HrMangmentSystem_Domain.Enum.Recruitment;

namespace HrMangmentSystem_Application.DTOs.Job.Appilcation
{
    public class ChangeJobApplicationStatusDto
    {
        public int JobApplicationId { get; set; }

        public JobApplicationStatus Newstatus { get; set; }

        public string? Notes { get; set; }

    }
}
