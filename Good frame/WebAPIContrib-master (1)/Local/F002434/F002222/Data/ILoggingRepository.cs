using F002222.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002222.Data
{
	public interface ILoggingRepository
	{
		void Log(ApiLoggingInfo loggingInfo);
	}
}
