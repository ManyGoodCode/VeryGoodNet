using Sc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo2
{
    public partial class Form1 : Form
    {
        Sc.ScMgr scMgr;
        Demo2.App app;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadModel1();
        }

        private void LoadModel1()
        {
            //透明无边框窗口测试代码，测试时打开下面的注释
            scMgr = new Sc.ScMgr(stdControl: null, isUsedUpdateLayerFrm: true);
            scMgr.BackgroundColor = Color.FromArgb(alpha: 100, red: 0, green: 0, blue: 251);
            app = new Demo2.App(scMgr: scMgr);
            scMgr.ReBulid();
            scMgr.Show();
        }

        private void LoadModel2()
        {
            //常规窗口测试代码，测试时打开下面的注释
            scMgr = new Sc.ScMgr(stdControl: panel);
            scMgr.BackgroundColor = Color.FromArgb(alpha: 255,red: 246,green: 245,blue: 251);
            app = new Demo2.App(scMgr: scMgr);
            scMgr.ReBulid();
        }


        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (scMgr != null)
                scMgr.Refresh();
        }
    }
}
