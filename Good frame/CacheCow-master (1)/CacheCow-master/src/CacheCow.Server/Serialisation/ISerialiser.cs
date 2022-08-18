using System;
using System.Collections.Generic;
using System.Text;

namespace CacheCow.Server
{
    public interface ISerialiser
    {
        byte[] Serialise(object o);
    }
}
