﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 视频操作
        /// </summary>
        public VideoCapture Cap = new VideoCapture();

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// SURF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // 载入源图片并显示
            Mat img_1 = Cv2.ImRead("1.jpg");
            Mat img_2 = Cv2.ImRead("2.jpg");

            // 定义一个特征检测类对象
            var MySurf = OpenCvSharp.XFeatures2D.SURF.Create(300);

            // 模板类是能够存放任意类型的动态数组，能够增加和压缩数据
            KeyPoint[] keypoints_1 = MySurf.Detect(img_1);
            KeyPoint[] keypoints_2 = MySurf.Detect(img_2);

            Mat descriptors_1 = new Mat();
            Mat descriptors_2 = new Mat();

            // 方法1：计算描述符（特征向量）
            //MySurf.Compute(img_1, ref keypoints_1, descriptors_1);
            //MySurf.Compute(img_2, ref keypoints_2, descriptors_2);

            // 方法2：计算描述符（特征向量）
            MySurf.DetectAndCompute(img_1, null, out keypoints_1, descriptors_1);
            MySurf.DetectAndCompute(img_2, null, out keypoints_2, descriptors_2);

            // 采用FLANN算法匹配描述符向量
            FlannBasedMatcher matcher = new FlannBasedMatcher();
            DMatch[] matches = matcher.Match(descriptors_1, descriptors_2);
            double max_dist = 0; double min_dist = 100;

            // 快速计算关键点之间的最大和最小距离
            for (int i = 0; i < descriptors_1.Rows; i++)
            {
                double dist = matches[i].Distance;
                if (dist < min_dist) min_dist = dist;
                if (dist > max_dist) max_dist = dist;
            }

            //输出距离信息
            System.Diagnostics.Debug.WriteLine($"最大距离（Max dist） : {max_dist}");
            System.Diagnostics.Debug.WriteLine($"最小距离（Min dist） : {min_dist}");

            //存下符合条件的匹配结果（即其距离小于2* min_dist的），使用radiusMatch同样可行
            List<DMatch> good_matches = new List<DMatch>();
            for (int i = 0; i < descriptors_1.Rows; i++)
            {
                if (matches[i].Distance < 2 * min_dist)
                    good_matches.Add(matches[i]);
            }

            //绘制匹配点并显示窗口
            Mat img_matches = new Mat();
            Cv2.DrawMatches(img_1, keypoints_1, img_2, keypoints_2, good_matches, img_matches);

            // 显示效果图
            Cv2.ImShow("匹配效果图", img_matches);
        }

        /// <summary>
        /// SIFT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            // 载入源图片并显示
            Mat img_1 = Cv2.ImRead("1.jpg");
            Mat img_2 = Cv2.ImRead("2.jpg");

            // 定义一个特征检测类对象
            var MySift = OpenCvSharp.Features2D.SIFT.Create(300);

            // 模板类是能够存放任意类型的动态数组，能够增加和压缩数据
            KeyPoint[] keypoints_1 = MySift.Detect(img_1);
            KeyPoint[] keypoints_2 = MySift.Detect(img_2);

            Mat descriptors_1 = new Mat();
            Mat descriptors_2 = new Mat();

            // 方法1：计算描述符（特征向量）
            //MySift.Compute(img_1, ref keypoints_1, descriptors_1);
            //MySift.Compute(img_2, ref keypoints_2, descriptors_2);

            // 方法2：计算描述符（特征向量）
            MySift.DetectAndCompute(img_1, null, out keypoints_1, descriptors_1);
            MySift.DetectAndCompute(img_2, null, out keypoints_2, descriptors_2);

            // 采用FLANN算法匹配描述符向量
            FlannBasedMatcher matcher = new FlannBasedMatcher();
            DMatch[] matches = matcher.Match(descriptors_1, descriptors_2);
            double max_dist = 0; double min_dist = 100;

            // 快速计算关键点之间的最大和最小距离
            for (int i = 0; i < descriptors_1.Rows; i++)
            {
                double dist = matches[i].Distance;
                if (dist < min_dist) min_dist = dist;
                if (dist > max_dist) max_dist = dist;
            }

            //输出距离信息
            System.Diagnostics.Debug.WriteLine($"最大距离（Max dist） : {max_dist}");
            System.Diagnostics.Debug.WriteLine($"最小距离（Min dist） : {min_dist}");

            //存下符合条件的匹配结果（即其距离小于2* min_dist的），使用radiusMatch同样可行
            List<DMatch> good_matches = new List<DMatch>();
            for (int i = 0; i < descriptors_1.Rows; i++)
            {
                if (matches[i].Distance < 2 * min_dist)
                    good_matches.Add(matches[i]);
            }

            //绘制匹配点并显示窗口
            Mat img_matches = new Mat();
            Cv2.DrawMatches(img_1, keypoints_1, img_2, keypoints_2, good_matches, img_matches);

            // 显示效果图
            Cv2.ImShow("匹配效果图", img_matches);
        }
    }
}