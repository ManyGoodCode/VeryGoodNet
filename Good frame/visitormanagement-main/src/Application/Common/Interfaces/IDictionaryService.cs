using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{
    /// <summary>
    /// 根据 typeName 获取字典
    /// </summary>
    public interface IDictionaryService
    {
        Task<IDictionary<string, string>> Fetch(string typeName);
    }
}
