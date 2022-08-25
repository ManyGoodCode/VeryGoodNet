using System;
using System.Runtime.InteropServices;

namespace SharpDX.Multimedia
{
    public class WaveFormatAdpcm : WaveFormat
    {
        internal WaveFormatAdpcm()
        {
        }

        public WaveFormatAdpcm(int rate, int channels, int blockAlign = 0) : base(rate, 4, channels)
        {
            if (blockAlign == 0)
            {
                if (rate <= 11025)
                    blockAlign = 256;
                else if (rate <= 22050)
                    blockAlign = 512;
                else
                    blockAlign = 1024;
            }

            if (rate <= 0) throw new ArgumentOutOfRangeException("rate", "Must be > 0");
            if (channels <= 0) throw new ArgumentOutOfRangeException("channels", "Must be > 0");
            if (blockAlign <= 0) throw new ArgumentOutOfRangeException("blockAlign", "Must be > 0");
            if (blockAlign > Int16.MaxValue) throw new ArgumentOutOfRangeException("blockAlign", "Must be < 32767");

            waveFormatTag = WaveFormatEncoding.Adpcm;
            this.blockAlign = (short)blockAlign;

            SamplesPerBlock = (ushort)(blockAlign * 2 / channels - 12);
            averageBytesPerSecond = (SampleRate * blockAlign) / SamplesPerBlock;

            Coefficients1 = new short[] { 256, 512, 0, 192, 240, 460, 392 };
            Coefficients2 = new short[] { 0  ,-256, 0,  64,   0,-208,-232 };
            extraSize = 32;
        }

        public ushort SamplesPerBlock { get; private set; }
        public short[] Coefficients1 { get; set; }
        public short[] Coefficients2 { get; set; }

        protected unsafe override IntPtr MarshalToPtr()
        {
            var result = Marshal.AllocHGlobal(Utilities.SizeOf<WaveFormat.__Native>() + sizeof(int) + sizeof(int) * Coefficients1.Length);
            __MarshalTo(ref *(WaveFormatAdpcm.__Native*)result);
            return result;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal unsafe new struct __Native
        {
            public WaveFormat.__Native waveFormat;
            public ushort samplesPerBlock;
            public ushort numberOfCoefficients;
            public short coefficients;

            internal unsafe void __MarshalFree()
            {
                waveFormat.__MarshalFree();
            }
        }

        internal unsafe void __MarshalFrom(ref __Native @ref)
        {
            this.__MarshalFrom(ref @ref.waveFormat);
            this.SamplesPerBlock = @ref.samplesPerBlock;
            this.Coefficients1 = new short[@ref.numberOfCoefficients];
            this.Coefficients2 = new short[@ref.numberOfCoefficients];
            if (@ref.numberOfCoefficients > 7)
                throw new InvalidOperationException("Unable to read Adpcm format. Too may coefficients (max 7)");
            fixed(short* pCoefs = &@ref.coefficients)
                for (int i = 0; i < @ref.numberOfCoefficients; i++)
                {
                    this.Coefficients1[i] = pCoefs[i*2];
                    this.Coefficients2[i] = pCoefs[i*2+1];
                }
            this.extraSize = (short)(sizeof(int) + sizeof(int) * @ref.numberOfCoefficients);
        }

        private unsafe void __MarshalTo(ref __Native @ref)
        {
            if (Coefficients1.Length > 7)
                throw new InvalidOperationException("Unable to encode Adpcm format. Too may coefficients (max 7)");

            this.extraSize = (short)(sizeof(int) + sizeof(int) * Coefficients1.Length);
            this.__MarshalTo(ref @ref.waveFormat);
            @ref.samplesPerBlock = this.SamplesPerBlock;
            @ref.numberOfCoefficients = (ushort)this.Coefficients1.Length;
            fixed (short* pCoefs = &@ref.coefficients)
                for (int i = 0; i < @ref.numberOfCoefficients; i++)
                {
                    pCoefs[i*2] = this.Coefficients1[i];
                    pCoefs[i*2+1] = this.Coefficients2[i];
                }
        }
    }
}