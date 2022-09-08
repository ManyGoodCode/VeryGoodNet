using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{
    /// <summary>
    /// 上传服务请求
    /// </summary>
    public interface IUploadService
    {
        Task<string> UploadAsync(UploadRequest request);
    }
}
