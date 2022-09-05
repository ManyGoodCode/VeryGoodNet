using System.IO;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Models;

namespace CleanArchitecture.Blazor.Infrastructure.Services
{
    public class UploadService : IUploadService
    {
        public async Task<string> UploadAsync(UploadRequest request)
        {
            if (request.Data == null)
                return string.Empty;
            MemoryStream streamData = new MemoryStream(request.Data);
            if (streamData.Length > 0)
            {
                string folder = request.UploadType.ToDescriptionString();
                string folderName = Path.Combine("Files", folder);
                string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (!string.IsNullOrEmpty(request.Folder))
                {
                    folderName = Path.Combine(folderName, request.Folder);
                    pathToSave = Path.Combine(pathToSave, request.Folder);
                }

                bool exists = Directory.Exists(pathToSave);
                if (!exists)
                    Directory.CreateDirectory(pathToSave);

                string fileName = request.FileName.Trim('"');
                string fullPath = Path.Combine(pathToSave, fileName);
                string dbPath = Path.Combine(folderName, fileName);
                if (File.Exists(dbPath))
                {
                    dbPath = NextAvailableFilename(dbPath);
                    fullPath = NextAvailableFilename(fullPath);
                }
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    await streamData.CopyToAsync(stream);
                }

                return dbPath;
            }
            else
            {
                return string.Empty;
            }
        }

        private static string numberPattern = " ({0})";

        public static string NextAvailableFilename(string path)
        {
            if (!File.Exists(path))
                return path;

            if (Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            return GetNextFilename(path + numberPattern);
        }

        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            if (!File.Exists(tmp))
                return tmp;
            int min = 1, max = 2;
            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }
    }
}
