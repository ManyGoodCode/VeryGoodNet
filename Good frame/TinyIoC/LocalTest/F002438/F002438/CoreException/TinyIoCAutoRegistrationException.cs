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
    class TinyIoCAutoRegistrationException : Exception
    {
        private const string ErrorText = "Duplicate implementation of type {0} found ({1}).";

        public TinyIoCAutoRegistrationException(Type registerType, IEnumerable<Type> types)
            : base(string.Format(ErrorText, registerType, GetTypesString(types)))
        {
        }

        public TinyIoCAutoRegistrationException(Type registerType, IEnumerable<Type> types, Exception innerException)
            : base(string.Format(ErrorText, registerType, GetTypesString(types)), innerException)
        {
        }

        private static string GetTypesString(IEnumerable<Type> types)
        {
            IEnumerable<string> typeNames = from type in types
                            select type.FullName;

            return string.Join(",", typeNames.ToArray());
        }
    }
}
