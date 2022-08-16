namespace CacheCow.Common
{
#if NET452

    using System.Configuration;

    /// <summary>
    ///
    /// </summary>
    public class ConfigurationValueProvider : IConfigurationValueProvider
    {
        /// <inheritdoc />
        public string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }

#endif


    public interface IConfigurationValueProvider
    {
        string GetValue(string key);
    }
}
