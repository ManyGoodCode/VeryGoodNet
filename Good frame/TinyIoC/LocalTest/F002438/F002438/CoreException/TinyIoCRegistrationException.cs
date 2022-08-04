using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002438.CoreException
{
#if SERIALIZABLE
    [Serializable]
#endif
#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
    class TinyIoCRegistrationException : Exception
    {
        private const string ConvertErrorText = "Cannot convert current registration of {0} to {1}";
        private const string GenericConstrantError = "Type {1} is not valid for a registration of type {0}";

        public TinyIoCRegistrationException(Type type, string method)
            : base(string.Format(ConvertErrorText, type.FullName, method))
        {
        }

        public TinyIoCRegistrationException(Type type, string method, Exception innerException)
            : base(string.Format(ConvertErrorText, type.FullName, method), innerException)
        {
        }

        public TinyIoCRegistrationException(Type registerType, Type implementationType)
            : base(string.Format(GenericConstrantError, registerType.FullName, implementationType.FullName))
        {
        }

        public TinyIoCRegistrationException(Type registerType, Type implementationType, Exception innerException)
            : base(string.Format(GenericConstrantError, registerType.FullName, implementationType.FullName), innerException)
        {
        }
    }
}
