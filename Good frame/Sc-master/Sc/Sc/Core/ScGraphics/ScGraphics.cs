using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class ScGraphics : Sc.IScGraphics,IDisposable
    {
        public Sc.ScLayer layer;
        public virtual Sc.GraphicsType GetGraphicsType()
        {
            return GraphicsType.UnKnown;
        }
         
        public virtual void BeginDraw()
        {
            
        }

        public virtual void EndDraw()
        {
            
        }

        public virtual void ResetClip()
        {
           
        }

        public virtual void ResetTransform()
        {
            
        }

        public virtual void SetClip(RectangleF clipRect)
        {
            
        }

        public virtual void TranslateTransform(float dx, float dy)
        {
           
        }

        public virtual void ReSize(int width, int height)
        {

        }

        public virtual void Dispose()
        {
            
        }

        public virtual System.Drawing.Drawing2D.Matrix Transform
        {
            get { return null; }
            set { }
        }
    }
}
