﻿using System;
using System.Globalization;

namespace SharpDX
{
    public class SharpDXException : Exception
    {
        private ResultDescriptor descriptor;
        public SharpDXException()
            : base("A SharpDX exception occurred.")
        {
            this.descriptor = ResultDescriptor.Find(Result.Fail);
            HResult = (int)Result.Fail;
        }

        public SharpDXException(Result result)
            : this(ResultDescriptor.Find(result))
        {
            HResult = (int)result;
        }

        public SharpDXException(ResultDescriptor descriptor)
            : base(descriptor.ToString())
        {
            this.descriptor = descriptor;
            HResult = (int)descriptor.Result;
        }

        public SharpDXException(Result result, string message)
            : base(message)
        {
            this.descriptor = ResultDescriptor.Find(result);
            HResult = (int)result;
        }

        public SharpDXException(Result result, string message, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture, message, args))
        {
            this.descriptor = ResultDescriptor.Find(result);
            HResult = (int)result;
        }

        public SharpDXException(string message, params object[] args) : this(Result.Fail, message, args)
        {
        }

        public SharpDXException(string message, Exception innerException, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture, message, args), innerException)
        {
            this.descriptor = ResultDescriptor.Find(Result.Fail);
            HResult = (int)Result.Fail;
        }

        public Result ResultCode
        {
            get { return this.descriptor.Result; }
        }

        public ResultDescriptor Descriptor
        {
            get { return this.descriptor; }
        }
    }
}