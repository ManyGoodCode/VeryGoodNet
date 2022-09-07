using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{

    public interface IUploadService
    {
        Task<string> UploadAsync(UploadRequest request);
    }
}
