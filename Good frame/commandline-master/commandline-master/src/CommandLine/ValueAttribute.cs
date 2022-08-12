// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValueAttribute : BaseAttribute
    {
        private readonly int index;
        private string metaName;
        public int Index
        {
            get { return index; }
        }

        public string MetaName
        {
            get { return metaName; }
            set
            {
                metaName = value ?? throw new ArgumentNullException("value");
            }
        }

        public ValueAttribute(int index) 
            : base()
        {
            this.index = index;
            this.metaName = string.Empty;
        }
    }
}
