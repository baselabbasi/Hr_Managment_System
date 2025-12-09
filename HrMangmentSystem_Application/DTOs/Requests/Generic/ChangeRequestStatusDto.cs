using HrMangmentSystem_Domain.Enum.Request;

namespace HrMangmentSystem_Application.DTOs.Requests.Generic
{
    public class ChangeRequestStatusDto
    {
        public int RequestId { get; set; }

        public RequestStatus NewStatus { get; set; }

        public string? Comment { get; set; }
    }
}
