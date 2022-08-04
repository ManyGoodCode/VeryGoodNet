using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002438.CoreException
{

    public class TinyIoCResolutionException : Exception
    {
        private const string ErrorText = "Unable to resolve type: {0}";

        public TinyIoCResolutionException(Type type)
            : base(message: string.Format(ErrorText, type.FullName))
        {
        }

        public TinyIoCResolutionException(Type type, Exception innerException)
            : base(message: string.Format(ErrorText, type.FullName), innerException: innerException)
        {
        }
    }
}
