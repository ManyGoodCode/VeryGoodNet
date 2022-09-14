using OpenCvSharp;
using System.Windows.Forms;
using System;
using System.Numerics;

namespace connectedComponentAnalysis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string imagefileString = "";
        Bitmap imageShow;

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
            imagefileString = openFileDialog1.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mat src = new Mat(imagefileString, ImreadModes.Color);
            Mat[] srcs;
            Cv2.Split(src, out srcs);

            Mat dst = srcs[1];

            pictureBox1.Image = new Bitmap(dst.ToMemoryStream()) as Image;
            pictureBox1.Image.Save(Application.StartupPath + "\\simle.bmp");

            Cv2.Threshold(dst, dst, 170, 255, ThresholdTypes.Binary);

            pictureBox1.Image = new Bitmap(dst.ToMemoryStream()) as Image;
            pictureBox1.Image.Save(Application.StartupPath + "\\simleThreshold.bmp");
            //return;




            Mat imageLables = new Mat();
            Mat imageConnect = new Mat(src.Size(), MatType.CV_8UC3);



            //统计图像中连通域的个数
            //int number = Cv2.ConnectedComponentsWithStats (dst, outPic, outPic2, centroids, PixelConnectivity.Connectivity8);
            int number = Cv2.ConnectedComponents(dst, imageLables, PixelConnectivity.Connectivity8);

            Vec3b[] colors = new Vec3b[number];
            Random random = new Random();
            for (int i = 0; i < number; i++)
            {
                int RRR = random.Next(0, 255);
                int GGG = random.Next(0, 255);
                int BBB = random.Next(0, 255);

                colors[i] = new Vec3b((Byte)RRR, (Byte)GGG, (Byte)BBB);
            }

            int height = imageLables.Rows;
            int width = imageLables.Cols;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int label_index = imageLables.At<int>(row, col);
                    //imageConnect.At<Vec3b>(row, col) = colors[label_index];
                    if (label_index != 0)
                    {
                        imageConnect.At<Vec3b>(row, col) = colors[label_index];
                    }
                }
            }
            pictureBox1.Image = new Bitmap(imageConnect.ToMemoryStream()) as Image;
            pictureBox1.Image.Save(Application.StartupPath + "\\imageConnect.bmp");
        }
    }
}