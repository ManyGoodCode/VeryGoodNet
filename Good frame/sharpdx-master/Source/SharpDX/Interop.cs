using System;
using System.Reflection;

namespace SharpDX
{
    internal class Interop
    {
        public static unsafe void* Fixed<T>(ref T data) 
        {
            throw new NotImplementedException();
        }

        public static unsafe void* Fixed<T>(T[] data)
        {
            throw new NotImplementedException();
        }

        public static unsafe void* Cast<T>(ref T data) where T : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe void* CastOut<T>(out T data) where T : struct
        {
            throw new NotImplementedException();
        }

        public static TCAST[] CastArray<TCAST, T>(T[] arrayData)
            where T : struct
            where TCAST : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe void memcpy(void* pDest, void* pSrc, int count)
        {
            throw new NotImplementedException();    
        }

        public static unsafe void memset(void* pDest, byte value, int count)
        {
            throw new NotImplementedException();
        }

        public static unsafe void* Read<T>(void* pSrc, ref T data) where T : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe T ReadInline<T>(void* pSrc) where T : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe void WriteInline<T>(void* pDest, ref T data) where T : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe void CopyInline<T>(ref T data, void* pSrc) where T : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe void CopyInline<T>(void* pDest, ref T srcData) where T : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe void CopyInlineOut<T>(out T data, void* pSrc) where T : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe void* ReadOut<T>(void* pSrc, out T data) where T : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe void* Read<T>(void* pSrc, T[] data, int offset, int count) where T : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe void* Read2D<T>(void* pSrc, T[,] data, int offset, int count) where T : struct
        {
            throw new NotImplementedException();
        }

        public static int SizeOf<T>()
        {
            throw new NotImplementedException();
        }

        public static unsafe void* Write<T>(void* pDest, ref T data) where T : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe void* Write<T>(void* pDest, T[] data, int offset, int count) where T : struct
        {
            throw new NotImplementedException();
        }

        public static unsafe void* Write2D<T>(void* pDest, T[,] data, int offset, int count) where T : struct
        {
            throw new NotImplementedException();
        }

        [Tag("SharpDX.ModuleInit")]
        public static void ModuleInit()
        {
            // Console.WriteLine("SharpDX Initialized");
        }
    }
}