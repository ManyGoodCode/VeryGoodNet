using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    enum RenderTargetMode
    {
        UnKonown,

        /// <summary>
        /// 渲染的为 SharpDX.WIC.Bitmap
        /// </summary>
        Wic,

        /// <summary>
        /// 渲染的为System.Windows.Forms.Control
        /// </summary>
        Hwnd
    }

    public struct D2DFont
    {
        public string FamilyName { get; set; }
        public float Size { get; set; }
        public SharpDX.DirectWrite.FontStyle Style { get; set; }

        public SharpDX.DirectWrite.FontWeight Weight { get; set; }

        public D2DFont(
            string familyName, 
            float size, 
            SharpDX.DirectWrite.FontWeight weight = FontWeight.Normal,
            SharpDX.DirectWrite.FontStyle style = FontStyle.Normal)
        {
            FamilyName = familyName;
            Size = size;
            Style = style;
            Weight = weight;
        }
    }
}
