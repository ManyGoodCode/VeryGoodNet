using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SharpDX.Multimedia
{
    public class WaveFormatExtensible : WaveFormat
    {
        short wValidBitsPerSample;    
        public Guid GuidSubFormat;
        public Speakers ChannelMask; 

        internal WaveFormatExtensible()
        {
        }


        public WaveFormatExtensible(int rate, int bits, int channels)
            : base(rate, bits, channels)
        {
            waveFormatTag = WaveFormatEncoding.Extensible;
            extraSize = 22;
            wValidBitsPerSample = (short)bits;
            int dwChannelMask = 0;
            for (int n = 0; n < channels; n++)
                dwChannelMask |= (1 << n);
            ChannelMask = (Speakers)dwChannelMask;

            GuidSubFormat = bits == 32 ? new Guid("00000003-0000-0010-8000-00aa00389b71") : new Guid("00000001-0000-0010-8000-00aa00389b71");
        }

        protected unsafe override IntPtr MarshalToPtr()
        {
            var result = Marshal.AllocHGlobal(Utilities.SizeOf<WaveFormatExtensible.__Native>());
            __MarshalTo(ref *(WaveFormatExtensible.__Native*)result);
            return result;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal new struct __Native
        {
            public WaveFormat.__Native waveFormat;
            public short wValidBitsPerSample; 
            public Speakers dwChannelMask;
            public Guid subFormat;

            internal unsafe void __MarshalFree()
            {
                waveFormat.__MarshalFree();
            }
        }
        
        internal unsafe void __MarshalFrom(ref __Native @ref)
        {
            this.__MarshalFrom(ref @ref.waveFormat);
            this.wValidBitsPerSample = @ref.wValidBitsPerSample;
            this.ChannelMask = @ref.dwChannelMask;
            this.GuidSubFormat = @ref.subFormat;
        }

        internal unsafe void __MarshalTo(ref __Native @ref)
        {
            this.__MarshalTo(ref @ref.waveFormat);
            @ref.wValidBitsPerSample = this.wValidBitsPerSample;
            @ref.dwChannelMask = this.ChannelMask;
            @ref.subFormat = this.GuidSubFormat;
        }

        internal static __Native __NewNative()
        {
            unsafe
            {
                __Native temp = default(__Native);
                temp.waveFormat.extraSize = 22;
                return temp;
            }
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} wBitsPerSample:{1} ChannelMask:{2} SubFormat:{3} extraSize:{4}",
                base.ToString(),
                wValidBitsPerSample,
                ChannelMask,
                GuidSubFormat,
                extraSize);
        }
    }
}