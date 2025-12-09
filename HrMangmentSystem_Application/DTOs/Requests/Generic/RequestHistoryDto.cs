using HrMangmentSystem_Domain.Enum.Request;

namespace HrMangmentSystem_Application.DTOs.Requests.Generic
{
    public class RequestHistoryDto
    {
        public int Id { get; set; }
        public int GenericRequestId { get; set; }

        public RequestAction Action { get; set; } 


        public RequestStatus? OldStatus { get; set; }
        public RequestStatus? NewStatus { get; set; }


        public string? Comment { get; set; }


        public Guid PerformedByEmployeeId { get; set; }
        public string PerformedByEmployeeName { get; set; } = null!;

        public DateTime? PerformedAt { get; set; }


    }
}
