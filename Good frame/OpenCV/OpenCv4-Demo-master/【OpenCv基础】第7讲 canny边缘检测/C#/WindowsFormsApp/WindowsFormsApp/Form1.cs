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
            // 【1】读取图像
            OpenCvSharp.Mat srcImage = OpenCvSharp.Cv2.ImRead("1.jpg");
            OpenCvSharp.Mat srcImage1 = srcImage.Clone();

            // 【2】显示原图
            OpenCvSharp.Cv2.ImShow("原图", srcImage);

            // 【3】创建与src同类型和大小的矩阵(dst)
            OpenCvSharp.Mat dstImage = new OpenCvSharp.Mat(srcImage.Cols, srcImage.Rows, srcImage.Type());

            // 【4】将原图像转换为灰度图像
            OpenCvSharp.Mat grayImage = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.CvtColor(srcImage1, grayImage, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

            // 【5】先用使用 3x3内核来降噪
            OpenCvSharp.Mat edge = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Blur(grayImage, edge, new OpenCvSharp.Size() 
            {
                Width = 3, 
                Height = 3 
            });

            // 【6】运行Canny算子
            OpenCvSharp.Cv2.Canny(edge, edge, 3, 9, 3);

            // 【7】使用Canny算子输出的边缘图g_cannyDetectedEdges作为掩码，来将原图g_srcImage拷到目标图g_dstImage中
            srcImage1.CopyTo(dstImage, edge);

            // 【8】显示效果图 
            OpenCvSharp.Cv2.ImShow("【效果图】整体方向Sobel", dstImage);

            // 【9】在pictureBox1中显示效果图
            Bitmap map = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dstImage);
            pictureBox1.Image = map;
        }
    }
}
