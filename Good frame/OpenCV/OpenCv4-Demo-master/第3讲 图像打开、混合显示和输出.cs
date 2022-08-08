using System;
using System.Drawing;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

// 官方文档
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
            // 读取图片
            OpenCvSharp.Mat image1 = OpenCvSharp.Cv2.ImRead("a.jpg");
            OpenCvSharp.Mat image2 = new OpenCvSharp.Mat("b.jpg");
            // 设置图片2需要显示的区域
            OpenCvSharp.Mat imageROI = image1[new OpenCvSharp.Rect() 
            { 
                X = 800, Y = 350, 
                Height = image2.Cols, 
                Width = image2.Rows
            }];

            // 重叠两张图片
            OpenCvSharp.Cv2.AddWeighted(imageROI, 0.7, image2, 0.3, 0.0, imageROI);
            // 显示图片到pictureBox
            Bitmap map = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image1);
            pictureBox1.Image = map;

            // 弹窗显示
            using (new OpenCvSharp.Window("合并", image1))
            {
                Cv2.WaitKey();
            }
        }
    }
}
