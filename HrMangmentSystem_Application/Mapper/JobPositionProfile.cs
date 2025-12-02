using AutoMapper;
using HrMangmentSystem_Application.DTOs.Job;
using HrMangmentSystem_Domain.Entities.Recruitment;
using System.Xml.Serialization;

namespace HrMangmentSystem_Application.Mapper
{
    public class JobPositionProfile : Profile
    {
        public JobPositionProfile()
        {
            // Add your mapping configurations here in the future
            CreateMap<JobPosition, JobPositionDto>()
                .ForMember(j => j.DepartmentName,
                   opt => opt.MapFrom(src => src.Department.DeptName))
                .ReverseMap();


            // Mapping for CreateJobPositionDtoDto
            CreateMap<CreateJobPositionDto, JobPosition>()
                .ForMember(j => j.PostedDate,
                    opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(j => j.IsActive,
                    opt => opt.MapFrom(_ => true));

            // Mapping for UpdateJobPositionDtoDto
            CreateMap<UpdateJobPositionDto, JobPosition>()   //partial update
                  .ForMember(d => d.Id, opt => opt.Ignore())               // don't edit at Job Position Id  
                  .ForMember(d => d.TenantId, opt => opt.Ignore())         //  don't edit at TenantId
                  .ForMember(d => d.CreatedAt, opt => opt.Ignore())      //don't edit at Created at 
                  .ForMember(d => d.CreatedBy, opt => opt.Ignore())        //don't edit at Created by
                  .ForAllMembers(opt =>
                      opt.Condition((src, dest, srcMember, destMember) =>  
                      {
                          // Ignore if the source member is null (no value provided in DTO)
                          if (srcMember is null)
                                  return false;

                          // Ignore empty or whitespace-only strings (treat them as "not provided")
                          if (srcMember is string s && string.IsNullOrWhiteSpace(s))
                                  return false;

                          // Otherwise, apply the value to the destination
                          return true;
                      }));
                  }
    }
}
