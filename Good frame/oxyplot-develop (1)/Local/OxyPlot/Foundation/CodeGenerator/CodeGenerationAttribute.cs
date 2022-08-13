namespace OxyPlot
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class CodeGenerationAttribute : Attribute
    {
        public CodeGenerationAttribute(bool generateCode)
        {
            this.GenerateCode = generateCode;
        }

        public bool GenerateCode { get; set; }
    }
}