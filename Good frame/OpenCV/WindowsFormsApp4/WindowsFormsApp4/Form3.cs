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
    /// 图像腐蚀
    /// </summary>
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mat srcImage = Cv2.ImRead("3.jpeg");

            // 在窗口显示原图
            Cv2.ImShow("原图", srcImage);

            // 进行腐蚀操作
            // Morph Shapes :形态学
            Mat element = Cv2.GetStructuringElement(
                shape: MorphShapes.Rect,       // 类型：Rect=腐蚀/膨胀
                ksize: new OpenCvSharp.Size()  // 腐蚀和大小
                {
                    Width = 15,
                    Height = 15
                });

            Mat dstImage = new Mat();
            Cv2.Erode(
                src: srcImage,
                dst: dstImage,
               element: element);


            // 显示图片到Picture
            Bitmap map = BitmapConverter.ToBitmap(dstImage);
            pictureBox1.Image = map;
            using (new Window("效果", dstImage))
            {
                Cv2.WaitKey();
            }
        }
    }
}
