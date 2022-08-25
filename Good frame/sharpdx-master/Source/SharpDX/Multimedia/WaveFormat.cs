using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace SharpDX.Multimedia
{
    public class WaveFormat
    {
        protected WaveFormatEncoding waveFormatTag;
        protected short channels;
        protected int sampleRate;
        protected int averageBytesPerSecond;
        protected short blockAlign;
        protected short bitsPerSample;
        protected short extraSize;

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal struct __Native
        {
            public __PcmNative pcmWaveFormat;
            public short extraSize;
            internal unsafe void __MarshalFree()
            {
            }
        }

        internal unsafe void __MarshalFree(ref __Native @ref)
        {
            @ref.__MarshalFree();
        }

        internal unsafe void __MarshalFrom(ref __Native @ref)
        {
            this.waveFormatTag = @ref.pcmWaveFormat.waveFormatTag;
            this.channels = @ref.pcmWaveFormat.channels;
            this.sampleRate = @ref.pcmWaveFormat.sampleRate;
            this.averageBytesPerSecond = @ref.pcmWaveFormat.averageBytesPerSecond;
            this.blockAlign = @ref.pcmWaveFormat.blockAlign;
            this.bitsPerSample = @ref.pcmWaveFormat.bitsPerSample;
            this.extraSize = @ref.extraSize;            
        }

        internal unsafe void __MarshalTo(ref __Native @ref)
        {
            @ref.pcmWaveFormat.waveFormatTag = this.waveFormatTag;
            @ref.pcmWaveFormat.channels = this.channels;
            @ref.pcmWaveFormat.sampleRate = this.sampleRate;
            @ref.pcmWaveFormat.averageBytesPerSecond = this.averageBytesPerSecond;
            @ref.pcmWaveFormat.blockAlign = this.blockAlign;
            @ref.pcmWaveFormat.bitsPerSample = this.bitsPerSample;
            @ref.extraSize = this.extraSize;  
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal struct __PcmNative
        {
            public WaveFormatEncoding waveFormatTag;
            public short channels;
            public int sampleRate;
            public int averageBytesPerSecond;
            public short blockAlign;
            public short bitsPerSample;
            internal unsafe void __MarshalFree()
            {
            }
        }

        internal unsafe void __MarshalFree(ref __PcmNative @ref)
        {
            @ref.__MarshalFree();
        }

        internal unsafe void __MarshalFrom(ref __PcmNative @ref)
        {
            this.waveFormatTag = @ref.waveFormatTag;
            this.channels = @ref.channels;
            this.sampleRate = @ref.sampleRate;
            this.averageBytesPerSecond = @ref.averageBytesPerSecond;
            this.blockAlign = @ref.blockAlign;
            this.bitsPerSample = @ref.bitsPerSample;
            this.extraSize = 0;
        }

        internal unsafe void __MarshalTo(ref __PcmNative @ref)
        {
            @ref.waveFormatTag = this.waveFormatTag;
            @ref.channels = this.channels;
            @ref.sampleRate = this.sampleRate;
            @ref.averageBytesPerSecond = this.averageBytesPerSecond;
            @ref.blockAlign = this.blockAlign;
            @ref.bitsPerSample = this.bitsPerSample;
        }

        public WaveFormat()
            : this(44100, 16, 2)
        {

        }

        public WaveFormat(int sampleRate, int channels)
            : this(sampleRate, 16, channels)
        {
        }

        public int ConvertLatencyToByteSize(int milliseconds)
        {
            int bytes = (int)((AverageBytesPerSecond / 1000.0) * milliseconds);
            if ((bytes % BlockAlign) != 0)
            {
                bytes = bytes + BlockAlign - (bytes % BlockAlign);
            }

            return bytes;
        }

        public static WaveFormat CreateCustomFormat(WaveFormatEncoding tag, int sampleRate, int channels, int averageBytesPerSecond, int blockAlign, int bitsPerSample)
        {
            WaveFormat waveFormat = new WaveFormat
                                 {
                                     waveFormatTag = tag,
                                     channels = (short) channels,
                                     sampleRate = sampleRate,
                                     averageBytesPerSecond = averageBytesPerSecond,
                                     blockAlign = (short) blockAlign,
                                     bitsPerSample = (short) bitsPerSample,
                                     extraSize = 0
                                 };
            return waveFormat;
        }

        public static WaveFormat CreateALawFormat(int sampleRate, int channels)
        {
            return CreateCustomFormat(WaveFormatEncoding.Alaw, sampleRate, channels, sampleRate * channels, 1, 8);
        }

        public static WaveFormat CreateMuLawFormat(int sampleRate, int channels)
        {
            return CreateCustomFormat(WaveFormatEncoding.Mulaw, sampleRate, channels, sampleRate * channels, 1, 8);
        }

        public WaveFormat(int rate, int bits, int channels)
        {
            if (channels  < 1)
            {
                throw new ArgumentOutOfRangeException("channels", "Channels must be 1 or greater");
            }

            this.waveFormatTag = bits<32?WaveFormatEncoding.Pcm:WaveFormatEncoding.IeeeFloat;
            this.channels = (short)channels;
            this.sampleRate = rate;
            this.bitsPerSample = (short)bits;
            this.extraSize = 0;

            this.blockAlign = (short)(channels * (bits / 8));
            this.averageBytesPerSecond = this.sampleRate * this.blockAlign;
        }

        public static WaveFormat CreateIeeeFloatWaveFormat(int sampleRate, int channels)
        {
            var wf = new WaveFormat
                         {
                             waveFormatTag = WaveFormatEncoding.IeeeFloat,
                             channels = (short) channels,
                             bitsPerSample = 32,
                             sampleRate = sampleRate,
                             blockAlign = (short) (4*channels)
                         };
            wf.averageBytesPerSecond = sampleRate * wf.blockAlign;
            wf.extraSize = 0;
            return wf;
        }

        public unsafe static WaveFormat MarshalFrom(byte[] rawdata)
        {
            fixed (void* pRawData = rawdata)
                return MarshalFrom((IntPtr)pRawData);
        }

        public unsafe static WaveFormat MarshalFrom(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero) return null;

            var pcmWaveFormat = *(__PcmNative*)pointer;
            var encoding = pcmWaveFormat.waveFormatTag;

            if (pcmWaveFormat.channels <= 2 && (encoding == WaveFormatEncoding.Pcm || encoding == WaveFormatEncoding.IeeeFloat || encoding == WaveFormatEncoding.Wmaudio2 || encoding == WaveFormatEncoding.Wmaudio3))
            {
                var waveFormat = new WaveFormat();
                waveFormat.__MarshalFrom(ref pcmWaveFormat);
                return waveFormat;
            }

            if (encoding == WaveFormatEncoding.Extensible)
            {
                var waveFormat = new WaveFormatExtensible();
                waveFormat.__MarshalFrom(ref *(WaveFormatExtensible.__Native*)pointer);
                return waveFormat;
            }

            if (encoding == WaveFormatEncoding.Adpcm)
            {
                var waveFormat = new WaveFormatAdpcm();
                waveFormat.__MarshalFrom(ref *(WaveFormatAdpcm.__Native*)pointer);
                return waveFormat;
            }

            throw new InvalidOperationException(string.Format("Unsupported WaveFormat [{0}]", encoding));
        }

        protected unsafe virtual IntPtr MarshalToPtr()
        {
            var result = Marshal.AllocHGlobal(Utilities.SizeOf<WaveFormat.__Native>());
            __MarshalTo(ref *(WaveFormat.__Native*)result);
            return result;
        }

        public static IntPtr MarshalToPtr(WaveFormat format)
        {
            if (format == null) return IntPtr.Zero;
            return format.MarshalToPtr();
        }

        public WaveFormat(BinaryReader br)
        {
            int formatChunkLength = br.ReadInt32();
            if (formatChunkLength < 16)
                throw new SharpDXException("Invalid WaveFormat Structure");
            this.waveFormatTag = (WaveFormatEncoding)br.ReadUInt16();
            this.channels = br.ReadInt16();
            this.sampleRate = br.ReadInt32();
            this.averageBytesPerSecond = br.ReadInt32();
            this.blockAlign = br.ReadInt16();
            this.bitsPerSample = br.ReadInt16();
            this.extraSize = 0;
            if (formatChunkLength > 16)
            {

                this.extraSize = br.ReadInt16();
                if (this.extraSize > formatChunkLength - 18)
                {
                    this.extraSize = (short)(formatChunkLength - 18);
                }
            }
        }

        public override string ToString()
        {
            switch (this.waveFormatTag)
            {
                case WaveFormatEncoding.Pcm:
                case WaveFormatEncoding.Extensible:
                    // extensible just has some extra bits after the PCM header
                    return string.Format(CultureInfo.InvariantCulture, "{0} bit PCM: {1}kHz {2} channels",
                        bitsPerSample, sampleRate / 1000, channels);
                default:
                    return this.waveFormatTag.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WaveFormat))
            return false;

            WaveFormat other = (WaveFormat)obj;
            return waveFormatTag == other.waveFormatTag &&
                channels == other.channels &&
                sampleRate == other.sampleRate &&
                averageBytesPerSecond == other.averageBytesPerSecond &&
                blockAlign == other.blockAlign &&
                bitsPerSample == other.bitsPerSample;
        }

        public override int GetHashCode()
        {
            return (int)waveFormatTag ^
                (int)channels ^
                sampleRate ^
                averageBytesPerSecond ^
                (int)blockAlign ^
                (int)bitsPerSample;
        }

        public WaveFormatEncoding Encoding
        {
            get
            {
                return waveFormatTag;
            }
        }

        public int Channels
        {
            get
            {
                return channels;
            }
        }

        public int SampleRate
        {
            get
            {
                return sampleRate;
            }
        }

        public int AverageBytesPerSecond
        {
            get
            {
                return averageBytesPerSecond;
            }
        }

        public int BlockAlign
        {
            get
            {
                return blockAlign;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return bitsPerSample;
            }
        }

        public int ExtraSize
        {
            get
            {
                return extraSize;
            }
        }
    }
}
