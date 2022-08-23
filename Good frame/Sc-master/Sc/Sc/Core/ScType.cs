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
        //
        // 摘要:
        //     该控件未停靠。
        None = 0,
        //
        // 摘要:
        //     该控件的上边缘停靠在其包含控件的顶端。
        Top = 1,
        //
        // 摘要:
        //     该控件的下边缘停靠在其包含控件的底部。
        Bottom = 2,
        //
        // 摘要:
        //     该控件的左边缘停靠在其包含控件的左边缘。
        Left = 3,
        //
        // 摘要:
        //     该控件的右边缘停靠在其包含控件的右边缘。
        Right = 4,

        Center = 5,

        //
        // 摘要:
        //     控件的各个边缘分别停靠在其包含控件的各个边缘，并且适当调整大小。
        Fill = 6,

        TopBottom = 7,

        RightTop = 8
    }
}
