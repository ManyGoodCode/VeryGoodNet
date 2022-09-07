using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Settings;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{

    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}
