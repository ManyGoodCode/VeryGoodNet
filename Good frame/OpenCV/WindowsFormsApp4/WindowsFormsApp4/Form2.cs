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
    /// OpenCV 图像合并
    /// </summary>
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mat image1 = Cv2.ImRead("2a.jpg");



            Mat image2 = new Mat("2b.jpg");

            // 设置图片2需要显示的区域
            Mat imageROI = image1[new Rect()
            {
                X = 400,
                Y = 400,
                Height = image2.Rows,
                Width = image2.Cols
            }];

            // 重合两张图片
            Cv2.AddWeighted(imageROI, 0.7, image2, 0.3, 0.0, imageROI);
            // 显示图片到Picture
            Bitmap map = BitmapConverter.ToBitmap(image1);
            pictureBox1.Image = map;
            using (new Window("合并", image1))
            {
                Cv2.WaitKey();
            }

        }
    }
}
