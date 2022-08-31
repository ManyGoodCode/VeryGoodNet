using System;

namespace CleanArchitecture.Blazor.Infrastructure.Configurations
{
    public class AppConfigurationSettings
    {
        public const string SectionName = nameof(AppConfigurationSettings);
        public string Secret { get; set; } = string.Empty;
        public bool BehindSSLProxy { get; set; }
        public string ProxyIP { get; set; } = string.Empty;
        public string ApplicationUrl { get; set; } = string.Empty;
    }
}
