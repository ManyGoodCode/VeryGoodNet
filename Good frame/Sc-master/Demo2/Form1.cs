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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadModel2();
        }

        private void LoadModel1()
        {
            scMgr = new Sc.ScMgr(stdControl: null, isUsedUpdateLayerFrm: true)
            {
                // 网页搜索   在线颜色选择器|RGB颜色查询对照表
                // 此处设置的颜色为 panel的背景色值接近白色
                BackgroundColor = Color.FromArgb(alpha: 100, red: 0, green: 0, blue: 251)
            };

            Demo2.App app = new Demo2.App(scMgr: scMgr);
            scMgr.ReBulid();
            scMgr.Show();
        }

        private void LoadModel2()
        {
            scMgr = new Sc.ScMgr(stdControl: panel)
            {
                // 网页搜索   在线颜色选择器|RGB颜色查询对照表
                // 此处设置的颜色为 panel的背景色值接近白色
                BackgroundColor = Color.FromArgb(alpha: 255, red: 246, green: 245, blue: 251),
            };

            Demo2.App app = new Demo2.App(scMgr: scMgr);
            scMgr.ReBulid();
        }


        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            scMgr?.Refresh();
        }
    }
}
