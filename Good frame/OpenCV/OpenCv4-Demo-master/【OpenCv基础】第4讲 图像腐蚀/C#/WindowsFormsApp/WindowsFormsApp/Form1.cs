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

// https://shimat.github.io/opencvsharp_docs/html/d69c29a1-7fb1-4f78-82e9-79be971c3d03.htm

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenCvSharp.Mat srcImage = OpenCvSharp.Cv2.ImRead("girl.jpg");

            // 在窗口中显示原画
            OpenCvSharp.Cv2.ImShow("原图", srcImage);

            // 进行腐蚀操作
            OpenCvSharp.Mat element = OpenCvSharp.Cv2.GetStructuringElement(
                shape: MorphShapes.Rect,
                ksize: new OpenCvSharp.Size()
                {
                    Width = 15,
                    Height = 15
                });

            OpenCvSharp.Mat dstImage = new OpenCvSharp.Mat();
            // Erode:腐蚀
            OpenCvSharp.Cv2.Erode(srcImage, dstImage, element);

            // 输出图像到pictureBox控件
            Bitmap map = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dstImage);
            pictureBox1.Image = map;

            // 弹窗显示图像
            using (new OpenCvSharp.Window("效果", dstImage))
            {
                OpenCvSharp.Cv2.WaitKey();
            }
        }
    }
}
