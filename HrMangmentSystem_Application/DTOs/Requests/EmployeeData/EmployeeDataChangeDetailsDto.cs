using HrMangmentSystem_Application.DTOs.Requests.Generic;

namespace HrMangmentSystem_Application.DTOs.Requests.EmployeeData
{
    public class EmployeeDataChangeDetailsDto
    {
        public GenericRequestListItemDto Generic { get; set; } = null!;

        public EmployeeDataChangeDto DataChange { get; set; } = null!;

        public List<RequestHistoryDto> History { get; set; } = new();
    }
}
