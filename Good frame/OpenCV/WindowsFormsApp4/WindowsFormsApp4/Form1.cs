using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace WindowsFormsApp4
{
    /// <summary>
    /// OpenCV 打开图像
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mat srcImage = new Mat("1.jpg");
            //此处通过窗体显示
            Cv2.ImShow("Image", srcImage);
            Cv2.WaitKey(0);

            Bitmap bitmap = BitmapConverter.ToBitmap(srcImage);
            pictureBox1.Image = bitmap;
        }
    }
}
