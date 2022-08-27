using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{

    public enum GraphicsType
    {
        UnKnown,
        D2D,
        GDIPlus
    }

    public enum DrawType
    {
        UnKnown,
        Image,
        NoImage
    }

    public enum ControlType
    {
        UnKnown,
        StdControl,
        UpdateLayerForm,
    }

    public enum ScDockStyle
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 3,
        Right = 4,
        Center = 5,
        Fill = 6,
        TopBottom = 7,
        RightTop = 8
    }
}
