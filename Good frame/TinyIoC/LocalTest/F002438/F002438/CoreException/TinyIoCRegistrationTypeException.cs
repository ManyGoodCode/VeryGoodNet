﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002438.CoreException
{
    public class TinyIoCRegistrationTypeException : Exception
    {
        private const string RegisterErrorText = "Cannot register type {0} - abstract classes or interfaces are not valid implementation types for {1}.";

        public TinyIoCRegistrationTypeException(Type type, string factory)
            : base(string.Format(RegisterErrorText, type.FullName, factory))
        {
        }

        public TinyIoCRegistrationTypeException(Type type, string factory, Exception innerException)
            : base(string.Format(RegisterErrorText, type.FullName, factory), innerException)
        {
        }
    }
}
