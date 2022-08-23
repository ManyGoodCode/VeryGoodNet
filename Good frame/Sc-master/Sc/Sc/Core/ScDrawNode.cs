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
        public ScLayer layer = null;
        public bool isRender = true;
        public RectangleF clipRect;
        public Matrix m;
        public ScLayer rootLayer = null;
        public List<ScDrawNode> nodes = new List<ScDrawNode>();
        public ScDrawNode parent;
    }
}
