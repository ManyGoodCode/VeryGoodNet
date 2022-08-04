using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace F002438.Extensions
{
    public static class AssemblyExtensions
    {
        public static Type[] SafeGetTypes(this Assembly assembly, bool isPublic = false)
        {
            Type[] types;
            try
            {
                types = isPublic ? assembly.ExportedTypes.ToArray() : assembly.GetTypes();
            }
            catch (System.IO.FileNotFoundException)
            {
                types = new Type[] { };
            }
            catch (NotSupportedException)
            {
                types = new Type[] { };
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null).ToArray();
            }

            return types;
        }
    }
}
