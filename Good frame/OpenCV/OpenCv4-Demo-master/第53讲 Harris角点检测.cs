﻿using OpenCvSharp;
using System;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private string WINDOW_NAME1 = "【程序窗口1】";
        private string WINDOW_NAME2 = "【程序窗口2】";

        Mat g_srcImage = new Mat();
        Mat g_srcImage1 = new Mat();
        Mat g_grayImage = new Mat();
        int thresh = 30; //当前阈值
        int max_thresh = 175; //最大阈值

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //【1】载入原图像和模板块
            g_srcImage = Cv2.ImRead("1.jpg");

            Cv2.ImShow(WINDOW_NAME1, g_srcImage);
            g_srcImage.CopyTo(g_srcImage1);

            //【2】存留一张灰度图
            Cv2.CvtColor(g_srcImage1, g_grayImage, ColorConversionCodes.BGR2GRAY);

            //【3】创建窗口和滚动条
            Cv2.NamedWindow(WINDOW_NAME1, WindowFlags.AutoSize);
            int v = Cv2.CreateTrackbar("阈值:", WINDOW_NAME1, ref thresh, max_thresh, on_CornerHarris);
            on_CornerHarris(0, IntPtr.Zero);

            Cv2.WaitKey(0);
        }

        public void on_CornerHarris(int pos, IntPtr userData)
        {
            //【1】定义一些局部变量
            Mat dstImage = new Mat();//目标图
            Mat normImage = new Mat();//归一化后的图
            Mat scaledImage = new Mat();//线性变换后的八位无符号整型的图

            //【2】初始化
            //置零当前需要显示的两幅图，即清除上一次调用此函数时他们的值
            dstImage = new Mat(g_srcImage.Size(), MatType.CV_32FC1);
            g_srcImage.CopyTo(g_srcImage1);

            //【3】正式检测
            //进行角点检测
            Cv2.CornerHarris(g_grayImage, dstImage, 2, 3, 0.04, BorderTypes.Default);

            // 归一化与转换
            Cv2.Normalize(dstImage, normImage, 0, 255, NormTypes.MinMax, MatType.CV_32FC1);
            //将归一化后的图线性变换成8位无符号整型 
            Cv2.ConvertScaleAbs(normImage, scaledImage);

            //【4】进行绘制
            // 将检测到的，且符合阈值条件的角点绘制出来
            for (int j = 0; j < normImage.Rows; j++)
            {
                for (int i = 0; i < normImage.Cols; i++)
                {
                    if ((int)normImage.At<float>(j, i) > thresh + 80)
                    {
                        Cv2.Circle(g_srcImage1, new Point(i, j), 5, new Scalar(10, 10, 255), 2, LineTypes.Link8, 0);
                        Cv2.Circle(scaledImage, new Point(i, j), 5, new Scalar(0, 10, 255), 2, LineTypes.Link8, 0);
                    }
                }
            }
            //【4】显示最终效果
            Cv2.ImShow(WINDOW_NAME1, g_srcImage1);
            Cv2.ImShow(WINDOW_NAME2, scaledImage);
        }
    }
}