namespace HrManagmentSystem_API.Interfaces
{
    public interface IFileStorageService

    {
        Task<string> SaveCvAsync(IFormFile formFile , string candidateName , string positionTitle);
    }
}
