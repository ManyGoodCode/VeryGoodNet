using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private OpenCvSharp.Mat CreateAlphaMat()
        {
            OpenCvSharp.Mat mat = new OpenCvSharp.Mat(rows: 480, cols: 640, type: MatType.CV_8UC4);
            for (int i = 0; i < mat.Rows; ++i)
            {
                for (int j = 0; j < mat.Cols; ++j)
                {
                    OpenCvSharp.Vec4b rgba = new OpenCvSharp.Vec4b();
                    // 蓝色
                    rgba.Item0 = 0xff;
                    // 绿色
                    rgba.Item1 = (byte)(((float)mat.Cols - j) / (float)mat.Cols * 0xff);
                    // 红色
                    rgba.Item2 = (byte)(((float)mat.Rows - i) / (float)mat.Rows * 0xff);
                    // 透明度
                    rgba.Item3 = (byte)((float)0.5 * (float)(rgba[1] + rgba[2]));
                    // 设置
                    mat.Set(i, j, rgba);
                }
            }
            return mat;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenCvSharp.Mat srcImage = CreateAlphaMat();
            Bitmap map = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(srcImage);
            pictureBox1.Image = map;
            OpenCvSharp.Cv2.ImWrite("透明Alpha值图.png", srcImage);
        }
    }
}
