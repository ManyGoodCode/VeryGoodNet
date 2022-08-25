using System;
using System.Collections.Generic;

namespace SharpDX.DXGI
{
    /// <summary>
    /// Helper to use with <see cref="Format"/>.
    /// </summary>
    public static class FormatHelper
    {
        private static readonly int[] sizeOfInBits = new int[256];
        private static readonly bool[] compressedFormats = new bool[256];
        private static readonly bool[] srgbFormats = new bool[256];
        private static readonly bool[] typelessFormats = new bool[256];

        public static int SizeOfInBytes(this Format format)
        {
            var sizeInBits = SizeOfInBits(format);
            return sizeInBits >> 3;
        }

        public static int SizeOfInBits(this Format format)
        {
            return sizeOfInBits[(int) format];
        }

        public static bool IsValid(this Format format)
        {
            return ( (int)(format) >= 1 && (int)(format) <= 115 );
        }

        public static bool IsCompressed(this Format format)
        {
            return compressedFormats[(int) format];
        }

        public static bool IsPacked(this Format format )
        {
            return ((format == Format.R8G8_B8G8_UNorm) || (format == Format.G8R8_G8B8_UNorm));
        }

        public static bool IsVideo(this Format format )
        {
            switch ( format )
            {
                case Format.AYUV:
                case Format.Y410:
                case Format.Y416:
                case Format.NV12:
                case Format.P010:
                case Format.P016:
                case Format.YUY2:
                case Format.Y210:
                case Format.Y216:
                case Format.NV11:
                    return true;

                case Format.Opaque420:
                case Format.AI44:
                case Format.IA44:
                case Format.P8:
                case Format.A8P8:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsSRgb(this Format format )
        {
            return srgbFormats[(int) format];
        }

        public static bool IsTypeless(this Format format )
        {
            return typelessFormats[(int) format];
        }

        public static int ComputeScanlineCount(this Format format, int height)
        {
            switch (format)
            {
                case Format.BC1_Typeless:
                case Format.BC1_UNorm:
                case Format.BC1_UNorm_SRgb:
                case Format.BC2_Typeless:
                case Format.BC2_UNorm:
                case Format.BC2_UNorm_SRgb:
                case Format.BC3_Typeless:
                case Format.BC3_UNorm:
                case Format.BC3_UNorm_SRgb:
                case Format.BC4_Typeless:
                case Format.BC4_UNorm:
                case Format.BC4_SNorm:
                case Format.BC5_Typeless:
                case Format.BC5_UNorm:
                case Format.BC5_SNorm:
                case Format.BC6H_Typeless:
                case Format.BC6H_Uf16:
                case Format.BC6H_Sf16:
                case Format.BC7_Typeless:
                case Format.BC7_UNorm:
                case Format.BC7_UNorm_SRgb:
                    return Math.Max(1, (height + 3) / 4);

                default:
                    return height;
            }
        }

        static FormatHelper()
        {
            InitFormat(new[] { Format.R1_UNorm }, 1);

            InitFormat(new[] { Format.A8_UNorm, Format.R8_SInt, Format.R8_SNorm, Format.R8_Typeless, Format.R8_UInt, Format.R8_UNorm }, 8);

            InitFormat(new[] { 
                Format.B5G5R5A1_UNorm,
                Format.B5G6R5_UNorm,
                Format.D16_UNorm,
                Format.R16_Float,
                Format.R16_SInt,
                Format.R16_SNorm,
                Format.R16_Typeless,
                Format.R16_UInt,
                Format.R16_UNorm,
                Format.R8G8_SInt,
                Format.R8G8_SNorm,
                Format.R8G8_Typeless,
                Format.R8G8_UInt,
                Format.R8G8_UNorm,
                Format.B4G4R4A4_UNorm,
            }, 16);

            InitFormat(new[] { 
                Format.B8G8R8X8_Typeless,
                Format.B8G8R8X8_UNorm,
                Format.B8G8R8X8_UNorm_SRgb,
                Format.D24_UNorm_S8_UInt,
                Format.D32_Float,
                Format.D32_Float_S8X24_UInt,
                Format.G8R8_G8B8_UNorm,
                Format.R10G10B10_Xr_Bias_A2_UNorm,
                Format.R10G10B10A2_Typeless,
                Format.R10G10B10A2_UInt,
                Format.R10G10B10A2_UNorm,
                Format.R11G11B10_Float,
                Format.R16G16_Float,
                Format.R16G16_SInt,
                Format.R16G16_SNorm,
                Format.R16G16_Typeless,
                Format.R16G16_UInt,
                Format.R16G16_UNorm,
                Format.R24_UNorm_X8_Typeless,
                Format.R24G8_Typeless,
                Format.R32_Float,
                Format.R32_Float_X8X24_Typeless,
                Format.R32_SInt,
                Format.R32_Typeless,
                Format.R32_UInt,
                Format.R8G8_B8G8_UNorm,
                Format.R8G8B8A8_SInt,
                Format.R8G8B8A8_SNorm,
                Format.R8G8B8A8_Typeless,
                Format.R8G8B8A8_UInt,
                Format.R8G8B8A8_UNorm,
                Format.R8G8B8A8_UNorm_SRgb,
                Format.B8G8R8A8_Typeless,
                Format.B8G8R8A8_UNorm,
                Format.B8G8R8A8_UNorm_SRgb,
                Format.R9G9B9E5_Sharedexp,
                Format.X24_Typeless_G8_UInt,
                Format.X32_Typeless_G8X24_UInt,
            }, 32);

            InitFormat(new[] { 
                Format.R16G16B16A16_Float,
                Format.R16G16B16A16_SInt,
                Format.R16G16B16A16_SNorm,
                Format.R16G16B16A16_Typeless,
                Format.R16G16B16A16_UInt,
                Format.R16G16B16A16_UNorm,
                Format.R32G32_Float,
                Format.R32G32_SInt,
                Format.R32G32_Typeless,
                Format.R32G32_UInt,
                Format.R32G8X24_Typeless,
            }, 64);

            InitFormat(new[] { 
                Format.R32G32B32_Float,
                Format.R32G32B32_SInt,
                Format.R32G32B32_Typeless,
                Format.R32G32B32_UInt,
            }, 96);

            InitFormat(new[] { 
                Format.R32G32B32A32_Float,
                Format.R32G32B32A32_SInt,
                Format.R32G32B32A32_Typeless,
                Format.R32G32B32A32_UInt,
            }, 128);

            InitFormat(new[] { 
                Format.BC1_Typeless,
                Format.BC1_UNorm,
                Format.BC1_UNorm_SRgb,
                Format.BC4_SNorm,
                Format.BC4_Typeless,
                Format.BC4_UNorm,
            }, 4);

            InitFormat(new[] { 
                Format.BC2_Typeless,
                Format.BC2_UNorm,
                Format.BC2_UNorm_SRgb,
                Format.BC3_Typeless,
                Format.BC3_UNorm,
                Format.BC3_UNorm_SRgb,
                Format.BC5_SNorm,
                Format.BC5_Typeless,
                Format.BC5_UNorm,
                Format.BC6H_Sf16,
                Format.BC6H_Typeless,
                Format.BC6H_Uf16,
                Format.BC7_Typeless,
                Format.BC7_UNorm,
                Format.BC7_UNorm_SRgb,
            }, 8);


            InitDefaults(new[]
                             {
                                 Format.BC1_Typeless,
                                 Format.BC1_UNorm,
                                 Format.BC1_UNorm_SRgb,
                                 Format.BC2_Typeless,
                                 Format.BC2_UNorm,
                                 Format.BC2_UNorm_SRgb,
                                 Format.BC3_Typeless,
                                 Format.BC3_UNorm,
                                 Format.BC3_UNorm_SRgb,
                                 Format.BC4_Typeless,
                                 Format.BC4_UNorm,
                                 Format.BC4_SNorm,
                                 Format.BC5_Typeless,
                                 Format.BC5_UNorm,
                                 Format.BC5_SNorm,
                                 Format.BC6H_Typeless,
                                 Format.BC6H_Uf16,
                                 Format.BC6H_Sf16,
                                 Format.BC7_Typeless,
                                 Format.BC7_UNorm,
                                 Format.BC7_UNorm_SRgb,
                             }, compressedFormats);

            InitDefaults(new[]
                             {
                                 Format.R8G8B8A8_UNorm_SRgb,
                                 Format.BC1_UNorm_SRgb,
                                 Format.BC2_UNorm_SRgb,
                                 Format.BC3_UNorm_SRgb,
                                 Format.B8G8R8A8_UNorm_SRgb,
                                 Format.B8G8R8X8_UNorm_SRgb,
                                 Format.BC7_UNorm_SRgb,
                             }, srgbFormats);

            InitDefaults(new[]
                             {
                                 Format.R32G32B32A32_Typeless,
                                 Format.R32G32B32_Typeless,
                                 Format.R16G16B16A16_Typeless,
                                 Format.R32G32_Typeless,
                                 Format.R32G8X24_Typeless,
                                 Format.R10G10B10A2_Typeless,
                                 Format.R8G8B8A8_Typeless,
                                 Format.R16G16_Typeless,
                                 Format.R32_Typeless,
                                 Format.R24G8_Typeless,
                                 Format.R8G8_Typeless,
                                 Format.R16_Typeless,
                                 Format.R8_Typeless,
                                 Format.BC1_Typeless,
                                 Format.BC2_Typeless,
                                 Format.BC3_Typeless,
                                 Format.BC4_Typeless,
                                 Format.BC5_Typeless,
                                 Format.B8G8R8A8_Typeless,
                                 Format.B8G8R8X8_Typeless,
                                 Format.BC6H_Typeless,
                                 Format.BC7_Typeless,
                             }, typelessFormats);


        }

        private static void InitFormat(IEnumerable<Format> formats, int bitCount)
        {
            foreach (var format in formats)
                sizeOfInBits[(int)format] = bitCount;
        }

        private static void InitDefaults(IEnumerable<Format> formats, bool[] outputArray)
        {
            foreach (var format in formats)
                outputArray[(int)format] = true;
        }
    }
}