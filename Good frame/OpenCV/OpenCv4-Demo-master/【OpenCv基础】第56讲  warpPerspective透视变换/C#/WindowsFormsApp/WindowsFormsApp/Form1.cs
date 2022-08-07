﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
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
            Mat srcImage = Cv2.ImRead("1.jpg");

            // 起始坐标
            List<Point2f> org = new List<Point2f>();
            org.Add(new Point2f((float)(srcImage.Width * 0.2), (float)(srcImage.Height * 0.2)));
            org.Add(new Point2f((float)(srcImage.Width * 0.8), (float)(srcImage.Height * 0.2)));
            org.Add(new Point2f((float)(srcImage.Width * 0.8), (float)(srcImage.Height * 0.8)));
            org.Add(new Point2f((float)(srcImage.Width * 0.2), (float)(srcImage.Height * 0.8)));

            // 目标坐标
            List<Point2f> dst = new List<Point2f>();
            dst.Add(new Point2f(0, 0));
            dst.Add(new Point2f(srcImage.Width, 0));
            dst.Add(new Point2f(srcImage.Width, srcImage.Height));
            dst.Add(new Point2f(0, srcImage.Height));

            Cv2.Line(srcImage, new Point(org[0].X, org[0].Y), new Point(org[1].X, org[1].Y), new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
            Cv2.Line(srcImage, new Point(org[1].X, org[1].Y), new Point(org[2].X, org[2].Y), new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
            Cv2.Line(srcImage, new Point(org[2].X, org[2].Y), new Point(org[3].X, org[3].Y), new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
            Cv2.Line(srcImage, new Point(org[3].X, org[3].Y), new Point(org[0].X, org[0].Y), new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
            Cv2.ImShow("透视线", srcImage);

            Mat warpR = Cv2.GetPerspectiveTransform(org, dst);
            Cv2.WarpPerspective(srcImage, srcImage, warpR, srcImage.Size());
            Cv2.ImShow("识别结果", srcImage);
        }
    }
}