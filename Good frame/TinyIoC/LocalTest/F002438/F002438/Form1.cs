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
            //TinyIoCContainer.MultiRegisterOptions
            TinyIoCContainer.Current.Register<ISt, St>();
            ISt st = TinyIoCContainer.Current.Resolve<St>();
            st.Age = 4;

            ISt st1 = TinyIoCContainer.Current.Resolve<ISt>();
        }
    }

    public class St : ISt
    {
        public override int Age { get; set; }
    }
    public abstract class ISt
    {
        public  virtual int Age { get; set; }
    }
}
