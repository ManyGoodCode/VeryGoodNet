namespace OxyPlot
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class ListBuilder<T>
    {
        private readonly List<string> properties;
        private readonly List<object> defaultValues;
        public ListBuilder()
        {
            this.properties = new List<string>();
            this.defaultValues = new List<object>();
        }

        public void Add<TProperty>(string propertyName, TProperty defaultValue)
        {
            this.properties.Add(propertyName);
            this.defaultValues.Add(defaultValue);
        }

        public void FillT(IList<T> target, IEnumerable source, Func<IList<object>, T> instanceCreator)
        {
            this.Fill((IList)target, source, args => instanceCreator(args));
        }

        public void Fill(IList target, IEnumerable source, Func<IList<object>, object> instanceCreator)
        {
            ReflectionPath[] pi = null;
            Type t = null;
            foreach (var sourceItem in source)
            {
                if (pi == null || sourceItem.GetType() != t)
                {
                    t = sourceItem.GetType();
                    pi = this.properties.Select(p => p != null ? new ReflectionPath(p) : null).ToArray();
                }

                var args = new List<object>(pi.Length);
                for (int j = 0; j < pi.Length; j++)
                {
                    object value;
                    args.Add(pi[j] != null && pi[j].TryGetValue(sourceItem, out value) ? value : this.defaultValues[j]);
                }

                var item = instanceCreator(args);
                target.Add(item);
            }
        }
    }
}