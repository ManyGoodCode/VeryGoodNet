using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace EasyHttp.Infrastructure
{
    public abstract class ObjectToUrl
    {
        protected abstract string PathStartCharacter { get; }
        protected abstract string PathSeparatorCharacter { get; }
        protected abstract string BuildParam(PropertyValue propertyValue);

        public string ParametersToUrl(object parameters)
        {
            string returnuri = "";
            IEnumerable<PropertyValue> properties = GetProperties(parameters);
            if (parameters != null)
            {
                List<string> paramsList = properties.Select(BuildParam).ToList();
                if (paramsList.Count > 0)
                {
                    returnuri = string.Format("{0}{1}", PathStartCharacter, String.Join(PathSeparatorCharacter, paramsList));
                }
            }

            return returnuri;
        }

        private static IEnumerable<PropertyValue> GetProperties(object parameters)
        {
            if (parameters == null) 
                yield break;
            if (parameters is ExpandoObject)
            {
                IDictionary<string, object> dictionary = parameters as IDictionary<string, object>;
                foreach (KeyValuePair<string, object> property in dictionary)
                {
                    yield return new PropertyValue { Name = property.Key, Value = property.Value.ToString() };
                }
            }
            else
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(parameters);
                foreach (PropertyDescriptor propertyDescriptor in properties)
                {
                    object val = propertyDescriptor.GetValue(parameters);
                    if (val != null)
                    {
                        yield return new PropertyValue { Name = propertyDescriptor.Name, Value = val.ToString() };
                    }
                }
            }
        }

        protected class PropertyValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}