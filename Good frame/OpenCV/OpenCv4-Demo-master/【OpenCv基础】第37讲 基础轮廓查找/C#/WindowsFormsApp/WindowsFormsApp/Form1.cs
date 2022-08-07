﻿using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            // 【1】载入原始图，且必须以二值图模式载入
            Mat srcImage = Cv2.ImRead("1.jpg", 0);
            Cv2.ImShow("原始图", srcImage);

            //【2】初始化结果图
            Mat dstImage = new Mat(srcImage.Size(), MatType.CV_8UC3);

            //【3】srcImage取大于阈值119的那部分
            // Cv2.CvtColor(srcImage, srcImage, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(srcImage, srcImage, 119, 255, ThresholdTypes.Binary);
            Cv2.ImShow("取阈值后的原始图", srcImage);

            Point[][] contours = new Point[][] { };
            HierarchyIndex[] hierarcy;
            Cv2.FindContours(srcImage, out contours, out hierarcy, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple, null);

            Random g_rng = new Random();

            // 【6】遍历所有顶层的轮廓， 以随机颜色绘制出每个连接组件颜色
            for (int index = 0; index < contours.Length; index++)
            {
                //随机生成bgr值
                int b = g_rng.Next(255);//随机返回一个0~255之间的值
                int g = g_rng.Next(255);//随机返回一个0~255之间的值
                int r = g_rng.Next(255);//随机返回一个0~255之间的值
                Cv2.DrawContours(dstImage, contours, index, new Scalar(b, g, r), -1, LineTypes.Link8, hierarcy);
            }

            //【7】显示最后的轮廓图
            Cv2.ImShow("【C# 轮廓图】", dstImage);
			Cv2.WaitKey(0);
		}
    }
}

