using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    /// <summary>
    /// 包含 Sc.ScLayer 属性
    /// 拥有:GetGraphicsType/BeginDraw/EndDraw/ReSize(int width, int height)/Dispose函数
    /// </summary>
    public class ScGraphics : Sc.IScGraphics, IDisposable
    {
        public Sc.ScLayer layer;
        public virtual Sc.GraphicsType GetGraphicsType()
        {
            return GraphicsType.UnKnown;
        }

        public virtual void BeginDraw() { }

        public virtual void EndDraw() { }

        public virtual void ResetClip() { }

        public virtual void ResetTransform() { }

        public virtual void SetClip(System.Drawing.RectangleF clipRect) { }

        public virtual void TranslateTransform(float dx, float dy) { }

        public virtual void ReSize(int width, int height) { }

        public virtual void Dispose() { }

        public virtual System.Drawing.Drawing2D.Matrix Transform
        {
            get { return null; }
            set { }
        }
    }
}
