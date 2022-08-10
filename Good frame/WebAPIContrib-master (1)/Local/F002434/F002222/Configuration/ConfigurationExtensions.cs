using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace F002222.Configuration
{
    public static class ConfigurationExtensions
    {
        private static readonly IDictionary<CustomErrorsMode, IncludeErrorDetailPolicy> policyLookup 
            = new Dictionary<CustomErrorsMode, IncludeErrorDetailPolicy>
                            {
                                { CustomErrorsMode.RemoteOnly, IncludeErrorDetailPolicy.LocalOnly },
                                { CustomErrorsMode.On, IncludeErrorDetailPolicy.Never },
                                { CustomErrorsMode.Off, IncludeErrorDetailPolicy.Always },
                            };

        public static void UseWebConfigCustomErrors(this HttpConfiguration configuration)
        {
            CustomErrorsSection config = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");
            configuration.IncludeErrorDetailPolicy = policyLookup[config.Mode];
        }
    }
}
