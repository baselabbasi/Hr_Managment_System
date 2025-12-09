using HrMangmentSystem_Application.DTOs.Requests.Generic;

namespace HrMangmentSystem_Application.DTOs.Requests.Leave
{
    public class LeaveRequestDetailsDto
    {
        public GenericRequestListItemDto Request { get; set; } = null!;

        public LeaveRequestDto Leave { get; set; } = null!;

        public List<RequestHistoryDto> History { get; set; } = new();
    }
}
