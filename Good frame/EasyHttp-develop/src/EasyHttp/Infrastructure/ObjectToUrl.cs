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

        public string ParametersToUrl(object obj)
        {
            string returnUrl = string.Empty;
            if (obj == null)
                return returnUrl;
            IEnumerable<PropertyValue> properties = GetProperties(obj);
            List<string> paramsList = properties.Select(BuildParam).ToList();
            if (paramsList.Count > 0)
            {
                returnUrl = string.Format("{0}{1}", PathStartCharacter, string.Join(PathSeparatorCharacter, paramsList));
            }

            return returnUrl;
        }

        private static IEnumerable<PropertyValue> GetProperties(object obj)
        {
            if (obj == null)
                yield break;
            if (obj is ExpandoObject)
            {
                IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
                foreach (KeyValuePair<string, object> property in dictionary)
                {
                    yield return new PropertyValue
                    {
                        Name = property.Key,
                        Value = property.Value.ToString()
                    };
                }
            }
            else
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
                foreach (PropertyDescriptor propertyDescriptor in properties)
                {
                    object val = propertyDescriptor.GetValue(obj);
                    if (val != null)
                    {
                        yield return new PropertyValue
                        {
                            Name = propertyDescriptor.Name,
                            Value = val.ToString()
                        };
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