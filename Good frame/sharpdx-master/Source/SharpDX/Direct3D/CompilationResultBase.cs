using System;

namespace SharpDX
{
    public abstract class CompilationResultBase<T> : DisposeBase where T : class, IDisposable
    {
        protected CompilationResultBase(T bytecode, Result resultCode, string message = null)
        {
            Bytecode = bytecode;
            ResultCode = resultCode;
            Message = message;
        }

        public T Bytecode { get; private set; }

        public Result ResultCode { get; private set; }

        public bool HasErrors
        {
            get { return ResultCode.Failure; }
        }

        public string Message { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Bytecode != null)
                {
                    Bytecode.Dispose();
                    Bytecode = null;
                }
            }
        }
    }
}