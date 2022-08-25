using System;
using System.Collections.Generic;

namespace SharpDX.DXGI
{
    public partial class Adapter
    {
        public Output[] Outputs
        {
            get
            {
                List<Output> outputs = new List<Output>();
                do
                {
                    Output output;
                    Output result = GetOutput(outputs.Count, out output);
                    if (result == ResultCode.NotFound || output == null)
                        break;
                    outputs.Add(output);
                } while (true);
                return outputs.ToArray();
            }
        }

        public bool IsInterfaceSupported(Type type)
        {
            long userModeVersion;
            return IsInterfaceSupported(type, out userModeVersion);
        }

        public bool IsInterfaceSupported<T>() where T : ComObject
        {
            long userModeVersion;
            return IsInterfaceSupported(typeof(T), out userModeVersion);
        }

        public bool IsInterfaceSupported<T>(out long userModeVersion) where T : ComObject
        {
            return IsInterfaceSupported(typeof (T), out userModeVersion);
        }

        public bool IsInterfaceSupported(Type type, out long userModeVersion)
        {
            return CheckInterfaceSupport(Utilities.GetGuidFromType(type), out userModeVersion).Success;
        }
	
        public SharpDX.DXGI.Output GetOutput(int outputIndex)
        {
            Output output;
            GetOutput(outputIndex, out output).CheckError();
            return output;
        }

        public int GetOutputCount()
        {
            var nbOutputs = 0;
            do
            {
                Output output;
                Output result = GetOutput(nbOutputs, out output);
                if (result == ResultCode.NotFound || output == null)
                    break;
                output.Dispose();
                nbOutputs++;
            } while (true);
            return nbOutputs;
        }
    }
}