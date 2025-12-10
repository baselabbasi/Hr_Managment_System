using Microsoft.AspNetCore.Http;

namespace HrMangmentSystem_Application.File
{
    public interface IFileStorageService

    {
        Task<string> SaveCvAsync(IFormFile formFile , string candidateName , string positionTitle);
    }
}
