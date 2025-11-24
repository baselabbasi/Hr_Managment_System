using AutoMapper;
using HrMangmentSystem_Application.DTOs;
using HrMangmentSystem_Domain.Entities.Employees;

namespace HrMangmentSystem_Application.Mapper
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            // Entity to DTO
            CreateMap<Employee, EmployeeDto>();

            // DTO to Entity
            CreateMap<CreateEmployeeDto, Employee>();

            // DTO to Entity for Update
            CreateMap<UpdateEmployeeDto, Employee>();
             
        }
    }
}
