using AutoMapper;
using HrMangmentSystem_Application.DTOs.Job;
using HrMangmentSystem_Domain.Entities.Recruitment;

namespace HrMangmentSystem_Application.Mapper
{
    public class JobPositionProfile : Profile
    {
        public  JobPositionProfile() 
        {
            // Add your mapping configurations here in the future
            CreateMap<JobPosition, JobPositionsDto>().ReverseMap();

            // Mapping for CreateJobPositionDtoDto
            CreateMap<CreateJobPositionDto, JobPosition>();

            // Mapping for UpdateJobPositionDtoDto
            CreateMap<UpdateJobPositionDto, JobPosition>();
        }
    }
}
