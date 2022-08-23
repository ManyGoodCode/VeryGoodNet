using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScMouseEventArgs
    {
        public ScMouseEventArgs(MouseButtons Button, PointF Location, int Delta = 0)
        {
            this.Button = Button;
            this.Location = Location;
            this.Delta = Delta;
        }

        public MouseButtons Button { get; }

        public PointF Location { get; }

        public int Delta { get; }
    }
}
