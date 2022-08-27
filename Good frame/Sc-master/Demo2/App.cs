
using System.Collections.Generic;
using System.Drawing;
using Sc;

namespace Demo2
{
    public class App
    {
        private Sc.ScMgr scMgr;
        private Sc.ScLayer root;

        public App(Sc.ScMgr scMgr)
        {
            this.scMgr = scMgr;
            root = scMgr.GetRootLayer();

            Demo2.TestLayer testLayer = new Demo2.TestLayer(scMgr)
            {
                Name = "layer1",
                Location = new System.Drawing.PointF(100, 100),
                Width = 300,
                Height = 300,
                BackgroundColor = System.Drawing.Color.FromArgb(255, 255, 0, 255),
            };
            root.Add(testLayer);

            Sc.ScTextBox text = new Sc.ScTextBox(scMgr)
            {
                Location = new System.Drawing.PointF(150, 0),
                Width = 200,
                Height = 30
            };

            root.Add(text);
        }
    }
}
