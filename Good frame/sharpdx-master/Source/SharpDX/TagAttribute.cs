using System.Collections.Generic;
using System.Text;

namespace SharpDX
{
    [AttributeUsage(AttributeTargets.All)]
    public class TagAttribute : Attribute
    {
        public string Value { get; private set; }
        public TagAttribute(string value)
        {
            Value = value;
        }
    }
}
