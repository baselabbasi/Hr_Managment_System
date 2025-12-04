using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Job.Appilcation;

namespace HrManagmentSystem_API.Interfaces
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveCvAsync(IFormFile formFile, string candidateName, string positionTitle)
        {
            var uploadsRoot = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads", "Cvs");

            var extension = Path.GetExtension(formFile.FileName);

            var candidatePart = CleanFilePart(candidateName);
            var positionPart = CleanFilePart(positionTitle);
            var shortGuid = Guid.NewGuid().ToString("N").Substring(0,8);

            var fileNameOnDisk = $"{candidateName}_{positionPart}_{shortGuid}{extension}";
            var physicalPath = Path.Combine(uploadsRoot, fileNameOnDisk);

            using (var stream = System.IO.File.Create(physicalPath))
            {
                await formFile.CopyToAsync(stream);
            }

            var realtivePath = Path.Combine("Uploads", "Cvs" , fileNameOnDisk)
                .Replace("\\" , "/");

            return realtivePath;

        }

        private static string CleanFilePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var invalid = Path.GetInvalidFileNameChars();
            var cleanedChars = value
                .Where(c => !invalid.Contains(c)) // remove invalid chars
                .ToArray();

            var cleaned = new string(cleanedChars).Replace(" ", "");

            return cleaned;
        }
    }
}
