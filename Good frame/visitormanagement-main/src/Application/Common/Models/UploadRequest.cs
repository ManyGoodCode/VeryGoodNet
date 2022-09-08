using CleanArchitecture.Blazor.Domain;

namespace CleanArchitecture.Blazor.Application.Common.Models
{
    /// <summary>
    /// 上传数据实体。文件夹 / 文件名称  / 扩展名 / 上传类型 / 源数据
    /// </summary>
    public class UploadRequest
    {
        public string? Folder { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public UploadType UploadType { get; set; }
        public byte[] Data { get; set; }
    }
}