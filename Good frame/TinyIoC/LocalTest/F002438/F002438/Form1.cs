using F002438.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F002438
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 构造瞬时对象
            TinyIoCContainer.InstanceContainer.Register<St>();
            ISt aa1 = TinyIoCContainer.InstanceContainer.Resolve<St>();
            aa1.Age = 4;
            ISt aa2 = TinyIoCContainer.InstanceContainer.Resolve<St>();
            aa2.Age = 5;

            // 构造单列对象
            TinyIoCContainer.InstanceContainer.Register(typeof(St), new St() { Age = 99 });
            ISt bb1 = TinyIoCContainer.InstanceContainer.Resolve<St>();
            bb1.Age = 88;
            ISt bb2 = TinyIoCContainer.InstanceContainer.Resolve<St>();
            bb2.Age = 77;
        }
    }

    public class St : ISt
    {
        public override int Age { get; set; }
    }
    public abstract class ISt
    {
        public virtual int Age { get; set; }
    }
}
