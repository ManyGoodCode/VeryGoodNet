using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CacheCow.Server
{
    public class Sha1Hasher : IHasher
    {
        private SHA256CryptoServiceProvider sha1 = new SHA256CryptoServiceProvider();       

        public string ComputeHash(byte[] bytes)
        {
            return Convert.ToBase64String(sha1.ComputeHash(bytes));
        }

        public void Dispose()
        {
            sha1.Dispose();
        }
    }
}
