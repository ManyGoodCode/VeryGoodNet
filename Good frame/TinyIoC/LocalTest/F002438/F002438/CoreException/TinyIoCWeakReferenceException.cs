using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002438.CoreException
{
    public class TinyIoCWeakReferenceException : Exception
    {
        private const string ErrorText = "Unable to instantiate {0} - referenced object has been reclaimed";

        public TinyIoCWeakReferenceException(Type type)
            : base(string.Format(ErrorText, type.FullName))
        {
        }

        public TinyIoCWeakReferenceException(Type type, Exception innerException)
            : base(string.Format(ErrorText, type.FullName), innerException)
        {
        }
    }
}
