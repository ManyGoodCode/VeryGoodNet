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
    /// sobel 边缘检测
    /// </summary>
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // X方向上边缘检测
            Mat gradX = new Mat();

            // Y方向上边缘检测
            Mat gradY = new Mat();

            // X方向上边缘检测
            Mat absGradX = new Mat();

            // Y方向上边缘检测
            Mat absGradY = new Mat();


            Mat dest = new Mat();


            // 【1】读取图像
            Mat srcImage = Cv2.ImRead("6.jpeg");

            // 【2】在窗口显示原图
            Cv2.ImShow("边缘检测原图", srcImage);

            // 【3】求X方向梯度
            Cv2.Sobel(srcImage, gradX, MatType.CV_16S, 1, 0, 3, 1, 1, BorderTypes.Default);
            Cv2.ConvertScaleAbs(gradX, absGradX);
            Cv2.ImShow("X方向Sobel", absGradX);

            // 【4】求Y方向梯度
            Cv2.Sobel(srcImage, gradY, MatType.CV_16S, 0, 1, 3, 1, 1, BorderTypes.Default);
            Cv2.ConvertScaleAbs(gradY, absGradY);
            Cv2.ImShow("Y方向Sobel", absGradY);

            // 【5】合并梯度(近似)
            Cv2.AddWeighted(absGradX, 0.5, absGradY, 0.5, 0, dest);
            Cv2.ImShow("整体方向Sobel", dest);

            // 显示图片到Picture
            Bitmap map = BitmapConverter.ToBitmap(dest);
            pictureBox1.Image = map;
            using (new Window("整体方向Sobel", dest))
            {
                Cv2.WaitKey();
            }
        }
    }
}
