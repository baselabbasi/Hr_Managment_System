using AutoMapper;
using HrMangmentSystem_Application.DTOs.Department;
using HrMangmentSystem_Domain.Entities.Employees;

namespace HrMangmentSystem_Application.Mapper
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            // Add your mapping configurations here in the future
            CreateMap<Department, DepartmentDto>().ReverseMap();

            // Mapping for CreateDepartmentDto
            CreateMap<CreateDepartmentDto, Department>();

            // Mapping for UpdateDepartmentDto
            CreateMap<UpdateDepartmentDto, Department>();

        }
    }
}
