
namespace SharpDX.Win32
{
    public class PropertyBagKey<T1,T2> 
    {
        public PropertyBagKey(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}