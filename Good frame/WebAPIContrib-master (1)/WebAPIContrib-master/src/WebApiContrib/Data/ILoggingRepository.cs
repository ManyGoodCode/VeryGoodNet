using WebApiContrib.Messages;

namespace WebApiContrib.Data
{
	public interface ILoggingRepository
	{
		void Log(ApiLoggingInfo loggingInfo);
	}
}
