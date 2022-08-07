using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    /// <summary>
    /// blur 图像模糊(均值滤波)
    /// </summary>
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mat srcImage = Cv2.ImRead("3.jpeg");

            // 在窗口显示原图
            Cv2.ImShow("均值滤波原图", srcImage);

            // 均值滤波
            Mat dstImage = new Mat();
            // 滤波和大小 7*7 
            Cv2.Blur(srcImage, dstImage, new OpenCvSharp.Size() { Width = 7, Height = 7 });

            // 显示图片到Picture
            Bitmap map = BitmapConverter.ToBitmap(dstImage);
            pictureBox1.Image = map;
            using (new Window("均值滤波效果", dstImage))
            {
                Cv2.WaitKey();
            }
        }
    }
}
