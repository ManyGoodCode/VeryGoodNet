using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class ScDrawNode
    {
        public Sc.ScLayer layer = null;
        public bool isRender = true;
        public System.Drawing.RectangleF clipRect;
        public System.Drawing.Drawing2D.Matrix m;
        public Sc.ScLayer rootLayer = null;
        public List<Sc.ScDrawNode> nodes = new List<Sc.ScDrawNode>();
        public Sc.ScDrawNode parent;
    }
}
