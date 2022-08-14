namespace OxyPlot
{
    using System;
    using System.Reflection;

    public class ReflectionPath
    {
        private readonly string[] items;
        private readonly PropertyInfo[] infos;
        private readonly Type[] reflectedTypes;
        public ReflectionPath(string path)
        {
            this.items = path != null ? path.Split('.') : new string[0];
            this.infos = new PropertyInfo[this.items.Length];
            this.reflectedTypes = new Type[this.items.Length];
        }

        public object GetValue(object instance)
        {
            object result;
            if (this.TryGetValue(instance, out result))
            {
                return result;
            }

            throw new InvalidOperationException("Could not find property " + string.Join(".", this.items) + " in " + instance);
        }


        public bool TryGetValue(object instance, out object result)
        {
            var current = instance;
            for (int i = 0; i < this.items.Length; i++)
            {
                if (current == null)
                {
                    result = null;
                    return true;
                }

                var currentType = current.GetType();

                var pi = this.infos[i];
                if (pi == null || this.reflectedTypes[i] != currentType)
                {
                    pi = this.infos[i] = currentType.GetRuntimeProperty(this.items[i]);
                    this.reflectedTypes[i] = currentType;
                }

                if (pi == null)
                {
                    result = null;
                    return false;
                }

                current = pi.GetValue(current, null);
            }

            result = current;
            return true;
        }
    }
}
