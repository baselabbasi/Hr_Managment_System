using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Job.Appilcation;

namespace HrManagmentSystem_API.Interfaces
{
    public interface IFileStorageService

    {
        Task<string> SaveCvAsync(IFormFile formFile , string candidateName , string positionTitle);
    }
}
