using AutoMapper;
using HrMangmentSystem_Application.DTOs.Job.Appilcation;
using HrMangmentSystem_Domain.Entities.Recruitment;

namespace HrMangmentSystem_Application.Mapper
{
    public class DocumentCvProfile : Profile
    {
        public DocumentCvProfile() 
        {
            CreateMap<DocumentCv , DocumentCvDto>().ReverseMap();

            CreateMap<CreateDocumentCvDto, DocumentCv>()
                .ForMember(d => d.UploadedAt,
                opt => opt.MapFrom(_ => DateTime.Now));
        }
    }
}
