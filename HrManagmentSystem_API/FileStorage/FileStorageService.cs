using HrManagmentSystem_Shared.Resources;
using Microsoft.Extensions.Localization;

namespace HrManagmentSystem_API.Interfaces
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileStorageService> _logger;
       

        public FileStorageService(IWebHostEnvironment webHostEnvironment,  ILogger<FileStorageService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<string> SaveCvAsync(IFormFile formFile, string candidateName, string positionTitle)
        {
            if (formFile == null || formFile.Length <= 0)
            {
                _logger.LogWarning("Document CV: Empty file uploaded");
                throw new InvalidOperationException("DocumentCv_EmptyFile");
            }

            var uploadsRoot = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads", "Cvs");
            Directory.CreateDirectory(uploadsRoot);

            var extension = Path.GetExtension(formFile.FileName);

            var isPdfExtension = string.Equals(extension , ".pdf", StringComparison.OrdinalIgnoreCase);

          //  var isPdfContentType = string.Equals(formFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase);

            if (!isPdfExtension)
            {
                _logger.LogWarning("Document CV : A document was uploaded that is not PDF ");
                throw new InvalidOperationException("DocumentCv_OnlyPdfAllowed");
            }

            var candidatePart = CleanFilePart(candidateName);
            var positionPart = CleanFilePart(positionTitle);
            var shortGuid = Guid.NewGuid().ToString("N").Substring(0,8);

            var fileNameOnDisk = $"{candidatePart}_{positionPart}_{shortGuid}{extension}";
            var physicalPath = Path.Combine(uploadsRoot, fileNameOnDisk);

            using (var stream = System.IO.File.Create(physicalPath))
            {
                await formFile.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("Uploads", "Cvs" , fileNameOnDisk)
                .Replace("\\" , "/");

            return relativePath;

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
