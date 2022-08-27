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

namespace demo
{
    public partial class Form1 : Form
    {
        demo.GoodsListViewer goodsListViewer;
        public Form1()
        {
            InitializeComponent();

            Load += Form1_Load;
            SizeChanged += Form1_SizeChanged;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            goodsListViewer.UpdateDataSource();
            panel.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            goodsListViewer = new demo.GoodsListViewer(control: panel);
        }
    }
}
