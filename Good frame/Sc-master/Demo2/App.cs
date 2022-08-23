
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

            Demo2.TestLayer testLayer = new Demo2.TestLayer(scMgr);
            testLayer.Name = "layer1";
            testLayer.Location = new System.Drawing.PointF(100, 100);
            testLayer.Width = 300;
            testLayer.Height = 300;
            testLayer.BackgroundColor = System.Drawing.Color.FromArgb(255, 255, 0, 255);
            root.Add(testLayer);


            Sc.ScTextBox text = new Sc.ScTextBox(scMgr);
            text.Location = new System.Drawing.PointF(150, 0);
            text.Width = 200;
            text.Height = 30;
            root.Add(text);
        }
    }
}
