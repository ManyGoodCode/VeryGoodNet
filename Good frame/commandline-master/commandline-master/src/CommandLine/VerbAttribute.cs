using System;
using System.Collections.Generic;

namespace CommandLine
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class VerbAttribute : Attribute
    {
        private readonly Infrastructure.LocalizableAttributeProperty helpText;
        private Type resourceType;
        public string Name { get; private set; }

        public bool Hidden
        {
            get;
            set;
        }

        public string HelpText
        {
            get => helpText.Value ?? string.Empty;
            set => helpText.Value = value ?? throw new ArgumentNullException("value");
        }

        public Type ResourceType
        {
            get => resourceType;
            set => resourceType = helpText.ResourceType = value;
        }

        public bool IsDefault { get; private set; }
        public string[] Aliases { get; private set; }

        public VerbAttribute(string name, bool isDefault = false, string[] aliases = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");

            Name = name;
            IsDefault = isDefault;
            helpText = new Infrastructure.LocalizableAttributeProperty(nameof(HelpText));
            resourceType = null;
            Aliases = aliases ?? new string[0];
        }
    }
}
