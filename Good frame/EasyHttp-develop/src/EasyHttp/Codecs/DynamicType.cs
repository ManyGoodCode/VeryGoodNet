using System.Collections.Generic;
using System.Dynamic;
using EasyHttp.Infrastructure;

namespace EasyHttp.Codecs
{
	using System.Globalization;

	public class DynamicType: DynamicObject
    {
        readonly Dictionary<string, object> properties = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
	        string binderName = binder.Name.ToLower(CultureInfo.InvariantCulture);
	        object value;
	        if (!properties.TryGetValue(binderName, out value)) 
				 throw new PropertyNotFoundException(binder.Name);

	        result = value;
	        return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            properties[binder.Name.ToLower(CultureInfo.InvariantCulture)] = value;
            return true;
        }
    }
}