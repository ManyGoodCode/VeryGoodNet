using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{

    public interface IDictionaryService
    {
        Task<IDictionary<string, string>> Fetch(string typeName);
    }
}
