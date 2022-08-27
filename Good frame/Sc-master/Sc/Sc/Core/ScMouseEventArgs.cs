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
        public ScMouseEventArgs(System.Windows.Forms.MouseButtons Button, System.Drawing.PointF Location, int Delta = 0)
        {
            this.Button = Button;
            this.Location = Location;
            this.Delta = Delta;
        }

        public System.Windows.Forms.MouseButtons Button { get; }

        public System.Drawing.PointF Location { get; }

        public int Delta { get; }
    }
}
