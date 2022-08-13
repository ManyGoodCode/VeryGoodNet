namespace OxyPlot
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    public class CodeGenerator
    {
        private readonly StringBuilder sb;

        /// <summary>
        /// 变量
        /// </summary>
        private readonly Dictionary<string, bool> variables;

        /// <summary>
        /// 缩进字符串  
        /// 例如:  this.indentString = new string(' ', value); 重复个数由 indents决定
        /// </summary>
        private string indentString;

        /// <summary>
        /// 当前缩进数
        /// </summary>
        private int indents;

        public CodeGenerator(PlotModel model)
        {
            this.variables = new Dictionary<string, bool>();
            this.sb = new StringBuilder();
            this.Indents = 8;
            string title = model.Title ?? "Untitled";
            this.AppendLine("[Example({0})]", title.ToCode());
            string methodName = this.MakeValidVariableName(title);
            this.AppendLine("public static PlotModel {0}()", methodName);
            this.AppendLine("{");
            this.Indents += 4;
            string modelName = this.Add(model);
            this.AddChildren(modelName, "Axes", model.Axes);
            this.AddChildren(modelName, "Series", model.Series);
            this.AddChildren(modelName, "Annotations", model.Annotations);
            this.AddChildren(modelName, "Legends", model.Legends);
            this.AppendLine("return {0};", modelName);
            this.Indents -= 4;
            this.AppendLine("}");
        }

        private int Indents
        {
            get { return this.indents; }
            set
            {
                this.indents = value;
                this.indentString = new string(' ', value);
            }
        }

        /// <summary>
        /// 格式化代码
        /// </summary>
        public static string FormatCode(string format, params object[] values)
        {
            object[] encodedValues = new object[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                encodedValues[i] = values[i].ToCode();
            }

            return string.Format(format, encodedValues);
        }

        /// <summary>
        /// 格式化构造器
        /// </summary>
        public static string FormatConstructor(Type type, string format, params object[] values)
        {
            return string.Format("new {0}({1})", type.Name, FormatCode(format, values));
        }

        /// <summary>
        /// 返回此模型的C#代码
        /// </summary>
        public string ToCode()
        {
            return this.sb.ToString();
        }

        private string Add(object obj)
        {
            Type type = obj.GetType();
            bool hasParameterLessCtor = type.GetTypeInfo()
                .DeclaredConstructors
                .Any(ci => ci.GetParameters().Length == 0);

            if (!hasParameterLessCtor)
            {
                return string.Format("/* Cannot generate code for {0} constructor */", type.Name);
            }

            object defaultInstance = Activator.CreateInstance(type);
            var varName = this.GetNewVariableName(type);
            this.variables.Add(varName, true);
            this.AppendLine("var {0} = new {1}();", varName, type.Name);
            this.SetProperties(obj, varName, defaultInstance);
            return varName;
        }

        private void AddChildren(string name, string collectionName, IEnumerable children)
        {
            foreach (object child in children)
            {
                string childName = this.Add(child);
                this.AppendLine("{0}.{1}.Add({2});", name, collectionName, childName);
            }
        }

        private void AddItems(string name, IList list)
        {
            foreach (object item in list)
            {
                string code = item.ToCode();
                if (code == null)
                {
                    continue;
                }

                this.AppendLine("{0}.Add({1});", name, code);
            }
        }

        private void AddArray(string name, Array array)
        {
            Type elementType = array.GetType().GetElementType();
            if (array.Rank == 1)
            {
                this.AppendLine("{0} = new {1}[{2}];", name, elementType.Name, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    string code = array.GetValue(i).ToCode();
                    if (code == null)
                    {
                        continue;
                    }

                    this.AppendLine("{0}[{1}] = {2};", name, i, code);
                }
            }

            if (array.Rank == 2)
            {
                this.AppendLine("{0} = new {1}[{2}, {3}];", name, elementType.Name, array.GetLength(0), array.GetLength(1));
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        string code = array.GetValue(i, j).ToCode();
                        if (code == null)
                        {
                            continue;
                        }

                        this.AppendLine("{0}[{1}, {2}] = {3};", name, i, j, code);
                    }
                }
            }

            if (array.Rank > 2)
            {
                throw new NotImplementedException();
            }
        }

        private void AppendLine(string format, params object[] args)
        {
            if (args.Length > 0)
            {
                this.sb.AppendLine(this.indentString + string.Format(CultureInfo.InvariantCulture, format, args));
            }
            else
            {
                this.sb.AppendLine(this.indentString + format);
            }
        }

        private bool AreListsEqual(IList list1, IList list2)
        {
            if (list1 == null || list2 == null)
            {
                return false;
            }

            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private T GetFirstAttribute<T>(PropertyInfo pi) where T : Attribute
        {
            return pi.GetCustomAttributes(typeof(CodeGenerationAttribute), true).OfType<T>().FirstOrDefault();
        }

        private string GetNewVariableName(Type type)
        {
            string prefix = type.Name;
            prefix = char.ToLower(prefix[0]) + prefix.Substring(1);
            int i = 1;
            while (this.variables.ContainsKey(prefix + i))
            {
                i++;
            }

            return prefix + i;
        }

        private string MakeValidVariableName(string title)
        {
            if (title == null)
            {
                return null;
            }

            Regex regex = new Regex("[a-zA-Z_][a-zA-Z0-9_]*");
            StringBuilder result = new StringBuilder();
            foreach (char c in title)
            {
                string s = c.ToString();
                if (regex.Match(s).Success)
                {
                    result.Append(s);
                }
            }

            return result.ToString();
        }

        private void SetProperties(object instance, string varName, object defaultValues)
        {
            Type instanceType = instance.GetType();
            Dictionary<string, IList> listsToAdd = new Dictionary<string, IList>();
            Dictionary<string, Array> arraysToAdd = new Dictionary<string, Array>();

            IEnumerable<PropertyInfo> properties = instanceType.GetRuntimeProperties().Where(pi => pi.GetMethod.IsPublic && !pi.GetMethod.IsStatic);
            foreach (PropertyInfo pi in properties)
            {
                CodeGenerationAttribute cga = this.GetFirstAttribute<CodeGenerationAttribute>(pi);
                if (cga != null && !cga.GenerateCode)
                {
                    continue;
                }

                string name = varName + "." + pi.Name;
                object value = pi.GetValue(instance, null);
                object defaultValue = pi.GetValue(defaultValues, null);
                if (this.AreListsEqual(value as IList, defaultValue as IList))
                {
                    continue;
                }

                Array array = value as Array;
                if (array != null)
                {
                    arraysToAdd.Add(name, array);
                    continue;
                }

                IList list = value as IList;
                if (list != null)
                {
                    listsToAdd.Add(name, list);
                    continue;
                }

                MethodInfo setter = pi.SetMethod;
                if (setter == null || !setter.IsPublic)
                {
                    continue;
                }

                if ((value != null && value.Equals(defaultValue)) || value == defaultValue)
                {
                    continue;
                }

                this.SetProperty(name, value);
            }

            foreach (KeyValuePair<string,IList> kvp in listsToAdd)
            {
                string name = kvp.Key;
                IList list = kvp.Value;
                this.AddItems(name, list);
            }

            foreach (KeyValuePair<string, Array> kvp in arraysToAdd)
            {
                string name = kvp.Key;
                Array array = kvp.Value;
                this.AddArray(name, array);
            }
        }

        /// <summary>
        /// 设置属性
        /// </summary>
        private void SetProperty(string name, object value)
        {
            string code = value.ToCode();
            if (code != null)
            {
                this.AppendLine("{0} = {1};", name, code);
            }
        }
    }
}
