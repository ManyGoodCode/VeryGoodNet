using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HCore.MathLib
{
    public partial class imageProcess
    {
            #region Member variables
            //*************************MTF**********************//
            #region Blur
            private double _DistanceToCenter = 0.36;
            private double _LaserHeight_L = 99999;
            private double _LaserHeight_R = 99999;
            private double _CentreRaitio = 99999;
            public double CentreRaitio
            {
                set { _CentreRaitio = value; }
                get { return _CentreRaitio; }
            }
            public double DistanceToCenter
            {
                set { _DistanceToCenter = value; }
                get { return _DistanceToCenter; }
            }
            public double LaserHeight_L
            {
                set { _LaserHeight_L = value; }
                get { return _LaserHeight_L; }
            }
            public double LaserHeight_R
            {
                set { _LaserHeight_R = value; }
                get { return _LaserHeight_R; }
            }
            #endregion
            private int _ABSPosition_X = 500;
            public int ABSPosition_X
            {
                set { _ABSPosition_X = value; }
                get { return _ABSPosition_X; }
            }
            private int _ABSPosition_Y = 290;
            public int ABSPosition_Y
            {
                set { _ABSPosition_Y = value; }
                get { return _ABSPosition_Y; }
            }


            //**************************************************//

            private bool _FilterNoise = false;
            public bool FilterNoise
            {
                set { _FilterNoise = value; }
                get { return _FilterNoise; }
            }

            public imageProcess()
            {
                mtfValues = new double[mtfCount];
                peakLSFValues = new double[mtfCount];
                centralLocValues = new double[mtfCount];
                lineSlopeValues = new double[mtfCount];
                resultValues = new int[mtfCount];
            }
            #region aimer doe
            private int ROI_x = 0, ROI_y = 0;
            private byte[] roi;
            private byte[,] array2d;
            private byte[] array1d;
            private double[] array1d_X;
            private double[] array1d_Y;
            private int _ROI_StartX = 0;
            public int ROI_StartX
            {
                set { _ROI_StartX = value; }
                get { return _ROI_StartX; }
            }
            private int _ROI_StartY = 0;
            public int ROI_StartY
            {
                set { _ROI_StartY = value; }
                get { return _ROI_StartY; }
            }
            private int _ROI_EndX = 0;
            public int ROI_EndX
            {
                set { _ROI_EndX = value; }
                get { return _ROI_EndX; }
            }
            private int _ROI_EndY = 0;
            public int ROI_EndY
            {
                set { _ROI_EndY = value; }
                get { return _ROI_EndY; }
            }

            private double CurrentRatio = 0;
            private double MaxRatio = 0;
            private int _AimerCenter_X = 0, _AimerCenter_Y = 0;
            private int _Imbalance_Line = 0;
            private double _Rotation_Deg = 0;

            public struct AimerParameter_t
            {
                public int XStart;
                public int YStart;
                public int RoiWidth;
                public int RoiHeight;
                public int Size;
                public int Threshold;
            }
            public struct AimerResult_t
            {
                public int XTop;
                public int YTop;
                public int XBottom;
                public int YBottom;
                public int Height;
            }
            private enum enFindEdgeDir
            {
                enFromUpToBottom = 0,
                enFromBottomToUp = 1,
            }
            #endregion
            private int maxX = 0, maxY = 0, minX = 600, minY = 600, middleX = 0, middleY = 0;
            private int standardCount = 0;

            [System.Runtime.InteropServices.DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);
            private int _xPoint = 0;
            private int _yPoint = 0;
            private int _BlackThreshold = 80;
            private int _MatrixSize = 1;
            private string _ErrorMessage = "";
            private int _Aimer_LEDLine_Width = 0;
            //private int _Aimer_LEDLine_Width_V = 0;
            private int _MaxPixel = 0;
            public int MaxPixel
            {
                get { return MaxPixel; }
            }
            private int _MinPixel = 255;
            public int MinPixel
            {
                get { return MinPixel; }
            }
            private int _Edge_UpX = 0;
            public int Edge_UpX
            {
                get { return _Edge_UpX; }
            }
            private int _Edge_UpY = 0;
            public int Edge_UpY
            {
                get { return _Edge_UpY; }
            }

            private int _Edge_BotX = 0;
            public int Edge_BotX
            {
                get { return _Edge_BotX; }
            }
            private int _Edge_BotY = 0;
            public int Edge_BotY
            {
                get { return _Edge_BotY; }
            }

            private int _Edge_LeftX = 0;
            public int Edge_LeftX
            {
                get { return _Edge_LeftX; }
            }
            private int _Edge_LeftY = 0;
            public int Edge_LeftY
            {
                get { return _Edge_LeftY; }
            }

            private int _Edge_RightX = 0;
            public int Edge_RightX
            {
                get { return _Edge_RightX; }
            }
            private int _Edge_RightY = 0;
            public int Edge_RightY
            {
                get { return _Edge_RightY; }
            }


            private int _WhiteThreshold = 120;
            private int _RedThreshold = 150;
            private int _GreenThreshold = 100;
            private int _BlueThreshold = 100;
            private int _WhiteCount = 100;
            private Bitmap _Bitmap = null;
            private int _line = 0;
            private int _diameter_H = 0;
            private int _diameter_V = 0;

            public int Diameter_H
            {
                get { return _diameter_H; }
            }
            public int Diameter_V
            {
                get { return _diameter_V; }
            }
            public int Imbalance_Line
            {
                get { return _Imbalance_Line; }
            }
            //Left middle right //LED Line
            private int _SharpnessLEDLine_bot_L = 9999;
            public int SharpnessLEDLine_bot_L
            {
                get { return _SharpnessLEDLine_bot_L; }
            }
            private int _SharpnessLEDLine_bot_M = 9999;
            public int SharpnessLEDLine_bot_M
            {
                get { return _SharpnessLEDLine_bot_M; }
            }
            private int _SharpnessLEDLine_bot_R = 9999;
            public int SharpnessLEDLine_bot_R
            {
                get { return _SharpnessLEDLine_bot_R; }
            }

            private int _SharpnessLEDLine_up_L = 9999;
            public int SharpnessLEDLine_up_L
            {
                get { return _SharpnessLEDLine_up_L; }
            }
            private int _SharpnessLEDLine_up_M = 9999;
            public int SharpnessLEDLine_up_M
            {
                get { return _SharpnessLEDLine_up_M; }
            }
            private int _SharpnessLEDLine_up_R = 9999;
            public int SharpnessLEDLine_up_R
            {
                get { return _SharpnessLEDLine_up_R; }
            }


            private int _SharpnessLEDLine_Left_M = 9999;
            public int SharpnessLEDLine_Left_M
            {
                get { return _SharpnessLEDLine_Left_M; }
            }
            private int _SharpnessLEDLine_Right_M = 9999;
            public int SharpnessLEDLine_Right_M
            {
                get { return _SharpnessLEDLine_Right_M; }
            }



            private int _SharpnessLEDCross_bot_L = 9999;
            public int SharpnessLEDCross_bot_L
            {
                get { return _SharpnessLEDCross_bot_L; }
            }
            private int _SharpnessLEDCross_bot_R = 9999;
            public int SharpnessLEDCross_bot_R
            {
                get { return _SharpnessLEDCross_bot_R; }
            }
            private int _SharpnessLEDCross_up_L = 9999;
            public int SharpnessLEDCross_up_L
            {
                get { return _SharpnessLEDCross_up_L; }
            }
            private int _SharpnessLEDCross_up_R = 9999;
            public int SharpnessLEDCross_up_R
            {
                get { return _SharpnessLEDCross_up_R; }
            }

            private double _imbalanceL = 0;
            public double imbalanceL
            {
                get { return _imbalanceL; }
            }

            private double _imbalanceU = 0;
            public double imbalanceU
            {
                get { return _imbalanceU; }
            }
            private double _imbalanceR = 0;
            public double imbalanceR
            {
                get { return _imbalanceR; }
            }

            private double _imbalanceB = 0;
            public double imbalanceB
            {
                get { return _imbalanceB; }
            }
            private int _SharpnessLEDLine_up_R_V = 9999;
            public int SharpnessLEDLine_up_R_V
            {
                get { return _SharpnessLEDLine_up_R_V; }
            }
            private int _SharpnessLEDCross_bot_L_V = 9999;
            public int SharpnessLEDCross_bot_L_V
            {
                get { return _SharpnessLEDCross_bot_L_V; }
            }
            private int _SharpnessLEDCross_bot_R_V = 9999;
            public int SharpnessLEDCross_bot_R_V
            {
                get { return _SharpnessLEDCross_bot_R_V; }
            }
            private int _SharpnessLEDCross_up_L_V = 9999;
            public int SharpnessLEDCross_up_L_V
            {
                get { return _SharpnessLEDCross_up_L_V; }
            }
            private int _SharpnessLEDCross_up_R_V = 9999;
            public int SharpnessLEDCross_up_R_V
            {
                get { return _SharpnessLEDCross_up_R_V; }
            }

            private int _catchPoint = 0;
            private int _AverageThreshold = 150;
            private int _whiteArea = 60;
            private int _blackArea = 120;
            private int _radius = 100;
            private int _radiusTemp = 55;
            private int _LaserCross_x_L = 0;
            private int _LaserCross_y_L = 0;
            private int _LaserCross_x_R = 0;
            private int _LaserCross_y_R = 0;
            #region Properties
            public int Aimer_LEDLine_Width
            {
                get { return _Aimer_LEDLine_Width; }
            }

            public string ErrorMessage
            {
                get { return _ErrorMessage; }
            }

            public int LaserCross_X_Left
            {
                get { return _LaserCross_x_L; }
            }
            public int LaserCross_Y_Left
            {
                get { return _LaserCross_y_L; }
            }
            public int LaserCross_X_Right
            {
                get { return _LaserCross_x_R; }
            }
            public int LaserCross_Y_Right
            {
                get { return _LaserCross_y_R; }
            }
            public double Rotation_Deg
            {
                get
                {
                    return _Rotation_Deg;
                }

            }
            private int _AimerLeft_Height = 0;
            public int AimerLeft_Height
            {
                get
                {
                    return _AimerLeft_Height;
                }

            }
            private int _AimerLeft_X = 0;
            public int AimerLeft_X
            {
                get
                {
                    return _AimerLeft_X;
                }

            }
            private int _AimerLeft_Y = 0;
            public int AimerLeft_Y
            {
                get
                {
                    return _AimerLeft_Y;
                }

            }
            private int _AimerRight_Height = 0;
            public int AimerRight_Height
            {
                get
                {
                    return _AimerRight_Height;
                }

            }
            private int _AimerRight_X = 0;
            public int AimerRight_X
            {
                get
                {
                    return _AimerRight_X;
                }

            }
            private int _AimerRight_Y = 0;
            public int AimerRight_Y
            {
                get
                {
                    return _AimerRight_Y;
                }

            }
            public int AimerCenter_X
            {
                get
                {
                    return _AimerCenter_X;
                }

            }
            public int AimerCenter_Y
            {
                get
                {
                    return _AimerCenter_Y;
                }
                set
                {
                    _AimerCenter_Y = value;
                }
            }
            /// <summary>
            /// Get or Set the white Area
            /// </summary>
            public int radiusTemp
            {
                get
                {
                    return _radiusTemp;
                }

            }
            /// <summary>
            /// Get or Set the white Area
            /// </summary>
            public int radius
            {
                get
                {
                    return _radius;
                }
                set
                {
                    _radius = value;
                }
            }
            /// <summary>
            /// Get or Set the white Area
            /// </summary>
            public int whiteArea
            {
                get
                {
                    return _whiteArea;
                }
                set
                {
                    _whiteArea = value;
                }
            }
            /// <summary>
            /// Get or Set the  blackArea.
            /// </summary>
            public int blackArea
            {
                get
                {
                    return _blackArea;
                }
                set
                {
                    _blackArea = value;
                }
            }
            /// <summary>
            /// Get or Set the y position.
            /// </summary>
            public int yPoint
            {
                get
                {
                    return _yPoint;
                }
                set
                {
                    _yPoint = value;
                }
            }
            /// <summary>
            /// Get or Set the y position.
            /// </summary>
            public int xPoint
            {
                get
                {
                    return _xPoint;
                }
                set
                {
                    _xPoint = value;
                }
            }
            /// <summary>
            /// Get or Set the BlackThreshold.
            /// </summary>
            public int BlackThreshold
            {
                get
                {
                    return _BlackThreshold;
                }
                set
                {
                    _BlackThreshold = value;
                }
            }
            /// <summary>
            /// Get or Set the WhiteThreshold.
            /// </summary>
            public int WhiteThreshold
            {
                get
                {
                    return _WhiteThreshold;
                }
                set
                {
                    _WhiteThreshold = value;
                }
            }
            /// <summary>
            /// Get or Set the GreenThreshold.
            /// </summary>
            public int GreenThreshold
            {
                get
                {
                    return _GreenThreshold;
                }
                set
                {
                    _GreenThreshold = value;
                }
            }

            /// <summary>
            /// Get or Set the BlueThreshold.
            /// </summary>
            public int BlueThreshold
            {
                get
                {
                    return _BlueThreshold;
                }
                set
                {
                    _BlueThreshold = value;
                }
            }
            /// <summary>
            /// Get or Set the MatrixSize.
            /// </summary>
            public int MatrixSize
            {
                get
                {
                    return _MatrixSize;
                }
                set
                {
                    _MatrixSize = value;
                }
            }


            /// <summary>
            /// Get or Set the WhiteCount.
            /// </summary>
            public int WhiteCount
            {
                get
                {
                    return _WhiteCount;
                }
                set
                {
                    _WhiteCount = value;
                }
            }

            /// <summary>
            /// Get or Set the bitmap.
            /// </summary>
            public Bitmap Bitmap
            {
                get
                {
                    return _Bitmap;
                }
                set
                {
                    _Bitmap = value;
                }
            }
            /// <summary>
            /// Get or Set the line.
            /// </summary>
            public int line
            {
                get
                {
                    return _line;
                }
                set
                {
                    _line = value;
                }
            }

            /// <summary>
            /// Get or Set the AverageThreshold.
            /// </summary>
            public int AverageThreshold
            {
                get
                {
                    return _AverageThreshold;
                }
                set
                {
                    _AverageThreshold = value;
                }
            }
            /// <summary>
            /// Get or Set the AverageThreshold.
            /// </summary>
            public int RedThreshold
            {
                get
                {
                    return _RedThreshold;
                }
                set
                {
                    _RedThreshold = value;
                }
            }
            #endregion

            //Product Serial Port

            #endregion Member variables
            #region IMAGE PROCESS
            #region 固定阈值法二值化模块
            private Bitmap Threshoding(Bitmap b, byte threshold, ref string sErrorMessage)
            {

                int width = b.Width;
                int height = b.Height;
                BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                unsafe
                {
                    byte* p = (byte*)data.Scan0;
                    int offset = data.Stride - width * 4;
                    byte R, G, B, gray;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            R = p[2];
                            G = p[1];
                            B = p[0];
                            //gray =(byte)((R * 19595 + G * 38469 + B * 7472) >> 16);
                            gray = (byte)(.299 * R + .587 * G + .114 * B);
                            if (gray >= threshold)
                            {
                                p[0] = p[1] = p[2] = 255;
                            }
                            else
                            {
                                p[0] = p[1] = p[2] = 0;
                            }
                            p += 4;
                        }
                        p += offset;
                    }
                    b.UnlockBits(data);
                    return b;
                }
            }
            #endregion
            #region 灰阶
            public Bitmap Gray(Bitmap b)
            {

                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - b.Width * 3;
                    byte red, green, blue;
                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < b.Width; ++x)
                        {
                            blue = p[0];
                            green = p[1];
                            red = p[2];
                            p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);
                            p += 3;
                        }
                        p += nOffset;
                    }
                }
                b.UnlockBits(bmData);
                return b;

            }
            #endregion

            #region CollectImage
            private Bitmap CollectImage(Bitmap InputValue, Bitmap OutputValue)
            {

                BitmapData bmDataInput = InputValue.LockBits(new Rectangle(0, 0, InputValue.Width, InputValue.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData bmDataOutput = OutputValue.LockBits(new Rectangle(0, 0, OutputValue.Width, OutputValue.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int strideInput = bmDataInput.Stride;
                int strideOutput = bmDataOutput.Stride;
                System.IntPtr Scan0 = bmDataInput.Scan0;
                System.IntPtr Scan1 = bmDataOutput.Scan0;
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    byte* p1 = (byte*)(void*)Scan1;
                    int nOffset = strideInput - InputValue.Width * 3;
                    int nOffsetout = strideOutput - OutputValue.Width * 3;

                    for (int y = 0; y < InputValue.Height; ++y)
                    {
                        for (int x = 0; x < InputValue.Width; ++x)
                        {
                            if (p[0] > p1[0])
                            {
                                p1[0] = p[0];
                            }
                            else
                            {
                                p1[0] = p1[0];
                            }
                            if (p[1] > p1[1])
                            {
                                p1[1] = p[1];
                            }
                            else
                            {
                                p1[1] = p1[1];
                            }
                            if (p[2] > p1[2])
                            {
                                p1[2] = p[2];
                            }
                            else
                            {
                                p1[2] = p1[2];
                            }
                            p1 += 3;
                            p += 3;
                        }
                        p1 += nOffset;
                        p += nOffset;

                    }
                }

                OutputValue.UnlockBits(bmDataOutput);
                return OutputValue;

            }
            #endregion
            #region LinePositionCheck
            private Bitmap LinePositionCheck(Bitmap b)
            {

                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = bmData.Stride;
                //int distance = 0;
                int[,] PointArray = new int[2, 921600];
                System.IntPtr Scan0 = bmData.Scan0;
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    standardCount = 0;
                    int nOffset = stride - b.Width * 3;
                    int red = 0, green = 0, blue = 0, tempcolor = 0;
                    maxX = 0; maxY = 0; minX = 600; minY = 600;
                    for (int y = 10; y < b.Height - 10; ++y)
                    {
                        for (int x = 10; x < b.Width - 10; ++x)
                        {
                            //blue = p[0 + x * 3 + y * stride];
                            //green = p[1 + x * 3 + y * stride];
                            //red = p[2 + x * 3 + y * stride];
                            //tempcolor = (byte)((red + green + blue) / 3);
                            tempcolor = MatrixArea(p, x, y, stride, 5);
                            //p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);
                            if (tempcolor < _BlackThreshold)
                            {
                                PointArray[0, standardCount] = x;
                                PointArray[1, standardCount] = y;
                                standardCount++;
                                // p += 3;
                                //red = p[2];
                                _catchPoint++;
                                if (x < minX)
                                {
                                    minX = x;
                                }
                                if (y < minY)
                                {
                                    minY = y;
                                }
                                if (x > maxX)
                                {
                                    maxX = x;
                                }
                                if (y > maxY)
                                {
                                    maxY = y;
                                }

                            }
                            /*
                        else
                        {
                            p[0] = 255;
                            p[1] = 255;
                            p[2] = 255;
                        }*/
                            // p += 3;
                        }
                        p += nOffset;
                    }
                    for (int n = 0; n < standardCount; n++)
                    {

                        p[0 + PointArray[0, n] * 3 + PointArray[1, n] * stride] = 0;
                        p[1 + PointArray[0, n] * 3 + PointArray[1, n] * stride] = 255;
                        p[2 + PointArray[0, n] * 3 + PointArray[1, n] * stride] = 255;
                    }
                }
                middleX = (minX + maxX) / 2;
                //_line = maxX - minX;
                middleY = (minY + maxY) / 2;
                _line = (int)Math.Sqrt((maxX - minX) * (maxX - minX) + (maxY - minY) * (maxY - minY));
                //_line = _linedistance;
                // MessageBox.Show("线长：  "+ distance.ToString(), "Capture camera", MessageBoxButtons.OK, MessageBoxIcon.Error);

                b.UnlockBits(bmData);
                return b;

            }
            #endregion
            #region PointPositionCheck
            #region matrixArea
            unsafe private int MatrixArea(byte* ImageBuffer, int x, int y, int stride, int cricletime)
            {
                int red = 0, green = 0, blue = 0;
                int lightness = 0;
                for (int m = 0; m < cricletime; m++)
                {
                    for (int n = 0; n < cricletime; n++)
                    {
                        blue = ImageBuffer[0 + (x + m) * 3 + (y + m) * stride];
                        green = ImageBuffer[1 + (x + m) * 3 + (y + m) * stride];
                        red = ImageBuffer[2 + (x + m) * 3 + (y + m) * stride];
                        lightness += (3 * blue + 6 * green + red) / 10;
                    }
                }
                lightness = lightness / (cricletime * cricletime);
                return lightness;
            }
            #endregion
            #region valueOfBackground
            unsafe private int valueOfBackground(Bitmap b)
            {
                byte* ImageBuffer;

                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                ImageBuffer = (byte*)(bmData.Scan0.ToPointer());
                int red = 0, green = 0, blue = 0;
                int lightness = 0;
                int stride = bmData.Stride;
                for (int m = 0; m < b.Height; m++)
                {
                    for (int n = 0; n < b.Width; n++)
                    {
                        blue = ImageBuffer[0 + n * 3 + m * stride];
                        green = ImageBuffer[1 + n * 3 + m * stride];
                        red = ImageBuffer[2 + n * 3 + m * stride];
                        lightness += (3 * blue + 6 * green + red) / 10;
                    }
                }
                lightness = lightness / (b.Height * b.Width);
                b.UnlockBits(bmData);
                return lightness;
            }
            #endregion
            #region CentrePixelCheck
            //*************check the centre of a circle******************//
            unsafe private bool CentrePixelCheck(byte* ImageBuffer, int x, int y, int Width, int Height, int stride, int CricleTime, int BlackThreshold, int whiteThreshold)
            {


                int color1 = 0, color2 = 0, color3 = 0, color4 = 0;   //the value of lightness for five  pixels 
                int check1_length = 0, check2_length = 0, check3_length = 0, check4_length = 0;//the radius about four reference pixels.
                bool check1 = false, check2 = false, check3 = false, check4 = false;//flags of four reference pixels.


                int matrixSize = 1;


                System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
                string IniPath = System.IO.Directory.GetCurrentDirectory() + @"\" + "option.ini";

                if (_MatrixSize > 0 && _MatrixSize < 10)
                {
                    matrixSize = _MatrixSize;
                }
                if (MatrixArea(ImageBuffer, x, y, stride, matrixSize) < BlackThreshold)
                {
                    for (int cricle = 0; cricle < CricleTime; cricle++)
                    {
                        if (x + _blackArea + cricle > Width - 5 || x - _blackArea - cricle < 5 || y - _blackArea - cricle < 5 || y + _blackArea + cricle > Height - 5)
                        {
                            break;
                        }
                        //*****four points of white area******//


                        color1 = MatrixArea(ImageBuffer, (x + _whiteArea), y, stride, matrixSize);
                        color2 = MatrixArea(ImageBuffer, (x - _whiteArea), y, stride, matrixSize);
                        color3 = MatrixArea(ImageBuffer, x, (y + _whiteArea), stride, matrixSize);
                        color4 = MatrixArea(ImageBuffer, x, (y - _whiteArea), stride, matrixSize);

                        //**********//

                        if (MatrixArea(ImageBuffer, (x + _blackArea + cricle), y, stride, matrixSize) < BlackThreshold && color1 > whiteThreshold)
                        {
                            check1 = true;
                            check1_length = _blackArea + cricle;
                        }

                        if (MatrixArea(ImageBuffer, (x - _blackArea - cricle), y, stride, matrixSize) < BlackThreshold && color2 > whiteThreshold)
                        {
                            check2 = true;
                            check2_length = _blackArea + cricle;
                        }

                        if (MatrixArea(ImageBuffer, x, (y + _blackArea + cricle), stride, matrixSize) < BlackThreshold && color3 > whiteThreshold)
                        {
                            check3 = true;
                            check3_length = _blackArea + cricle;
                        }

                        if (MatrixArea(ImageBuffer, x, (y - _blackArea - cricle), stride, matrixSize) < BlackThreshold && color4 > whiteThreshold)
                        {
                            check4 = true;
                            check4_length = _blackArea + cricle;
                        }

                        if (check1 == true && check2 == true && check3 == true && check4 == true)
                        {
                            if (Math.Abs(check1_length - check2_length) > _radiusTemp)
                            {
                                check1 = false; check2 = false;
                                //break;
                            };

                            if (Math.Abs(check3_length - check4_length) > _radiusTemp)
                            {
                                check3 = false; check4 = false;
                                //break;
                            }
                            if (check1_length < _radius || check2_length < _radius || check3_length < _radius || check4_length < _radius)
                            {
                                check1 = false; check2 = false; check3 = false; check4 = false;
                                //break;
                            }

                        }
                        if (check1 == true && check2 == true && check3 == true && check4 == true)
                        {
                            break;
                        }
                    }

                }

                if (check1 == true && check2 == true && check3 == true && check4 == true)
                {
                    return true;
                }
                else
                    return false;
            }
            //**************************************//
            #endregion
            #region RedPixelsCheck

            //*************check red and white points******************//
            unsafe private bool RedPixelsCheck(byte* ImageBuffer, int x, int y, int stride, int redThreshold, int whiteThreshold)
            {

                int red, green, blue;
                int red1, green1, blue1;//the second pixel.
                int tempcolor = 0;

                blue = ImageBuffer[0 + x * 3 + y * stride];
                green = ImageBuffer[1 + x * 3 + y * stride];
                red = ImageBuffer[2 + x * 3 + y * stride];

                blue1 = ImageBuffer[0 + (x + 2) * 3 + y * stride];
                green1 = ImageBuffer[1 + (x + 2) * 3 + y * stride];
                red1 = ImageBuffer[2 + (x + 2) * 3 + y * stride];

                tempcolor = (red + red1 + green + green1 + blue + blue1) / 6;
                //check the red and white pixels.
                if (((red + red1) - (green + green1) > redThreshold * 2 && (red + red1) - (blue + blue1) > redThreshold * 2) || tempcolor > whiteThreshold)
                {
                    return true;
                }
                else
                    return false;

            }
            //**************************************//
            #endregion
            public bool PointPositionCheck(Bitmap b, int type)
            {
                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = bmData.Stride;
                int xPoint, yPoint;
                int nOffset = stride - b.Width * 3;
                int[,] PointArray = new int[2, 921600];
                string IniPath;

                System.Text.StringBuilder temp = new System.Text.StringBuilder(255);

                // section=配置节，key=键名，temp=上面，path=路径
                IniPath = System.IO.Directory.GetCurrentDirectory() + @"\" + "option.ini";

                // System.IntPtr Scan0 = bmData.Scan0;
                unsafe
                {
                    byte* p = (byte*)(bmData.Scan0.ToPointer());
                    xPoint = 0;
                    yPoint = 0;
                    standardCount = 0;
                    DateTime dtStart = DateTime.Now;
                    for (int y = 30; y < b.Height - 30; ++y)
                    {
                        for (int x = 30; x < b.Width - 30; ++x)
                        {
                            if (type == 0)
                            {
                                if (CentrePixelCheck(p, x, y, b.Width, b.Height, stride, 200, _BlackThreshold, _WhiteThreshold) == true)
                                {
                                    xPoint = x + xPoint;
                                    yPoint = y + yPoint;
                                    PointArray[0, standardCount] = x;
                                    PointArray[1, standardCount] = y;
                                    standardCount++;
                                }
                            }

                            if (type == 1)
                            {
                                if (RedPixelsCheck(p, x, y, stride, _RedThreshold, _AverageThreshold) == true)
                                {
                                    xPoint = x + xPoint;
                                    yPoint = y + yPoint;
                                    PointArray[0, standardCount] = x;
                                    PointArray[1, standardCount] = y;
                                    standardCount++;
                                }
                            }

                        }
                        p += nOffset;
                    }
                    DateTime dtEnd = DateTime.Now;
                    System.TimeSpan ts = dtEnd.Subtract(dtStart);

                    for (int n = 0; n < standardCount; n++)
                    {

                        p[0 + PointArray[0, n] * 3 + PointArray[1, n] * stride] = 0;
                        p[1 + PointArray[0, n] * 3 + PointArray[1, n] * stride] = 255;
                        p[2 + PointArray[0, n] * 3 + PointArray[1, n] * stride] = 255;
                    }
                    _catchPoint = standardCount;
                }


                if (standardCount == 0 && type == 0)
                {
                    //MessageBox.Show("没有找到参考点", "Capture camera", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (standardCount == 0 && type == 1)
                {
                    //MessageBox.Show("没有找到VLD ", "Capture camera", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                xPoint = xPoint / standardCount;
                yPoint = b.Height - yPoint / standardCount;
                _Bitmap = b;
                _xPoint = xPoint;
                _yPoint = yPoint;
                _catchPoint = standardCount;
                if (xPoint < 0 && yPoint < 0 && type == 0)
                {
                    //MessageBox.Show("参考点X:" + xPoint.ToString() + ";" + "Y:" + yPoint.ToString() + "超出范围", "Capture camera", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }


                if (xPoint < 0 && yPoint < 0 && type == 1)
                {
                    //MessageBox.Show("VLD X:" + xPoint.ToString() + ";" + "Y:" + yPoint.ToString() + "超出范围", "Capture camera", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // LineText.Text = distance.ToString();
                //LineText.ForeColor = System.Drawing.Color.Red;

                b.UnlockBits(bmData);
                return true;

            }
            #endregion
            #region buffer VS Array
            private byte[,] BmpToArray2d(Bitmap b, bool color)
            {
                BitmapData bmData;
                int nOffset = 0, stride = 0;
                System.IntPtr Scan0;
                int j = 0;
                unsafe
                {
                    byte* p;
                    if (color)
                    {
                        bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        Scan0 = bmData.Scan0;
                        stride = bmData.Stride;
                        p = (byte*)(void*)Scan0;

                        ROI_x = (b.Width - _ROI_EndX - _ROI_StartX) * 3;
                        ROI_y = b.Height - _ROI_EndY - _ROI_StartY;
                        roi = new byte[b.Width * b.Height * 3];
                        array2d = new byte[ROI_x * 3, ROI_y];
                        nOffset = stride - b.Width * 3;
                        for (int y = _ROI_StartY; y < b.Height - _ROI_EndY; ++y)
                        {
                            if (y == _ROI_StartY)
                            {
                                p = p + ((_ROI_StartX + _ROI_StartY * b.Width) * 3);
                            }
                            else
                            {
                                p = p + (_ROI_StartX * 3);
                            }
                            for (int x = _ROI_StartX; x < b.Width - _ROI_EndX; ++x)
                            {
                                roi[j] = p[0];
                                roi[j + 1] = p[1];
                                roi[j + 2] = p[2];
                                j += 3;
                                p += 3;
                                if (x == b.Width - _ROI_EndX - 1)
                                {
                                    p = p + (_ROI_EndX * 3);
                                }
                            }
                            p += nOffset;
                        }
                        for (int i = 0; i < ROI_y; ++i)
                        {
                            for (int k = 0; k < ROI_x; ++k)
                            {
                                array2d[k, i] = roi[i * ROI_x + k];
                            }
                        }
                    }
                    else
                    {
                        bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                        Scan0 = bmData.Scan0;
                        stride = bmData.Stride;
                        p = (byte*)(void*)Scan0;

                        ROI_x = b.Width - _ROI_EndX - _ROI_StartX;
                        ROI_y = b.Height - _ROI_EndY - _ROI_StartY;
                        roi = new Byte[b.Width * b.Height];
                        array2d = new byte[ROI_x, ROI_y];
                        nOffset = stride - b.Width;
                        for (int y = _ROI_StartY; y < b.Height - _ROI_EndY; ++y)
                        {
                            if (y == _ROI_StartY)
                            {
                                p = p + (_ROI_StartX + _ROI_StartY * b.Width);
                            }
                            else
                            {
                                p = p + _ROI_StartX;
                            }

                            for (int x = _ROI_StartX; x < b.Width - _ROI_EndX; ++x)
                            {
                                roi[j] = p[0];
                                if (y == _ROI_StartY || y == b.Height - _ROI_EndY - 1) roi[j] = p[0] = 190;
                                if (x == _ROI_StartX || x == b.Width - _ROI_EndX - 1) roi[j] = p[0] = 190;
                                j += 1;
                                p += 1;
                                if (x == b.Width - _ROI_EndX - 1)
                                {
                                    p = p + _ROI_EndX;
                                }
                            }
                            p += nOffset;
                        }
                        int m = 0;
                        for (int i = 0; i < ROI_y; ++i)
                        {
                            for (int k = 0; k < ROI_x; ++k)
                            {
                                array2d[k, i] = roi[m];
                                m++;
                            }
                        }
                    }
                }
                b.UnlockBits(bmData);
                return array2d;
            }
            private Rectangle SearchAreaFromTop(Byte[,] Array2d, int width, int height, bool direction)
            {
                Rectangle rec = new Rectangle();
                Rectangle roi = new Rectangle(0, 0, width, height);
                Int32 i32_SearchPixelValue = 0, i32_AdjacentSearchPixelValue = 0;
                int x = 0, y = 0;

                //'Find the min xy and max xy of the Pixels >150 within the ROI x,y,width,hight
                //'Set the intial values to the opposite max and min
                //'Use a rectangle to hold the values (x,y is the top Left xy),( Width,Hight is the Bottom Right xy)

                try
                {
                    rec.X = roi.X + roi.Width;//min x
                    rec.Y = roi.Y + roi.Height;//min y
                    rec.Width = roi.X;//max x
                    rec.Height = roi.Y;//max y
                    i32_SearchPixelValue = 200;
                    i32_AdjacentSearchPixelValue = 100;
                    if (direction == true)
                    {
                        for (y = roi.Y; y < roi.Y + roi.Height; y++)
                        {
                            for (x = roi.X; x < roi.X + roi.Width; x++)
                            {
                                if (Array2d[x, y] > i32_SearchPixelValue)
                                {
                                    //Verify there is at least 3 pixels > 150 around it
                                    if (PixelFilter(Array2d, x, y, i32_AdjacentSearchPixelValue) > 2)
                                    {
                                        if (x < rec.X) rec.X = x;
                                        //If x > rec.Width Then rec.Width = x
                                        rec.Width = 3;
                                        if (y < rec.Y) rec.Y = y;
                                        //If y > rec.Height Then rec.Height = y
                                        rec.Height = 3;
                                        return rec;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (y = roi.Y + roi.Height - 1; y > roi.Y - 1; y--)
                        {
                            for (x = roi.X + roi.Width - 1; x > roi.X - 1; x--)
                            {
                                if (Array2d[x, y] > i32_SearchPixelValue)
                                {
                                    //Verify there is at least 3 pixels > 150 around it

                                    if (PixelFilter(Array2d, x, y, i32_AdjacentSearchPixelValue) > 2)
                                    {
                                        if (x < rec.X) rec.X = x;
                                        //If x > rec.Width Then rec.Width = x
                                        rec.Width = 3;
                                        if (y < rec.Y) rec.Y = y;
                                        //If y > rec.Height Then rec.Height = y
                                        rec.Height = 3;
                                        return rec;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return rec;
                }

                return rec;
            }
            private int PixelFilter(Byte[,] Array2d, int x, int y, int SearchPixelValue)
            {
                int i = 0;
                try
                {
                    for (int x1 = -2; x1 < 2; x1++)
                    {
                        for (int y1 = -2; y1 < 2; y1++)
                        {
                            if (Array2d[x + x1, y + y1] > SearchPixelValue)
                                i++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }

                return i;
            }
            private Rectangle FindBarWidth(Byte[,] Array2d, int width, int height, int x, int y)
            {
                Rectangle rec = new Rectangle(0, 0, 0, 0);
                Int32[] sx = new int[width * height];
                Int32[] ex = new int[width * height];
                Int32[] sy = new int[width * height];
                Int32[] ey = new int[width * height];
                Int32[] Delta = new int[width * height];
                Int32 i = 0, x2 = 0, y2 = 0, max = 0, DeltaNum = 0;
                Byte PixelValue;
                Boolean LookForStart = true, LookForEnd = false;

                if (x < 10 || x + 10 > width) return rec;
                if (y < 20 || y + 20 > height) return rec;
                for (x2 = x - 10; x2 < x + 10; x2++)
                {
                    for (y2 = y - 20; y2 < y + 20; y2++)
                    {
                        PixelValue = Array2d[x2, y2];
                        if (PixelValue > 200 && LookForStart)
                        {
                            sx[DeltaNum] = x2;
                            sy[DeltaNum] = y2;
                            LookForStart = false;
                            LookForEnd = true;
                        }
                        else if (PixelValue < 100 && LookForEnd)
                        {
                            ex[DeltaNum] = x2;
                            ey[DeltaNum] = y2;
                            Delta[DeltaNum] = ey[DeltaNum] - sy[DeltaNum];
                            LookForStart = true;
                            LookForEnd = false;
                            DeltaNum += 1;
                        }
                    }
                }

                //Find the max delta
                max = 0;
                for (i = 0; i < DeltaNum; i++)
                {
                    if (Delta[i] > Delta[max]) max = i;
                }
                rec.X = sx[max];
                rec.Y = (sy[max] + ey[max]) / 2;

                return rec;
            }
            private Rectangle FindBarWidth_plus(Byte[,] Array2d, int width, int height, int x, int y)
            {
                Rectangle rec = new Rectangle(0, 0, 0, 0);
                Int32[] sx = new int[width * height];
                Int32[] ex = new int[width * height];
                Int32[] sy = new int[width * height];
                Int32[] ey = new int[width * height];
                Int32[] Delta = new int[width * height];
                Int32 Delta_Temp = 0;
                Int32 y_temp = 0;
                Int32 i = 0, x2 = 0, y2 = 0, max = 0, DeltaNum = 0;
                Int32 DeltaNum_Max = 0, DeltaNum_Min = 0, LineWidth_Max = 0, LineWidth_Min = 0;
                Byte PixelValue;
                Boolean LookForStart = true, LookForEnd = false, changeValue = false;

                if (y < 0 || y + 20 > height) return rec;
                if (x < 50 || x + 50 > width) return rec;
                for (y2 = y; y2 < y + 20; y2++)
                {
                    LookForStart = true;
                    LookForEnd = false;
                    y_temp = y2;
                    for (x2 = x - 50; x2 < x + 50; x2++)
                    {
                        PixelValue = (Byte)((Array2d[x2, y2] + Array2d[x2 + 1, y2]) / 2);

                        if (PixelValue > 200 && LookForStart)
                        {
                            sx[DeltaNum] = x2;
                            sy[DeltaNum] = y2;
                            LookForStart = false;
                            LookForEnd = true;
                            changeValue = true;
                            Array2d[x2, y2] = 0;

                        }
                        else if (PixelValue < 100 && LookForEnd)
                        {
                            ex[DeltaNum] = x2;
                            ey[DeltaNum] = y2;
                            // if ((ey[DeltaNum] - sy[DeltaNum] > 5)&&( ey[DeltaNum] - sy[DeltaNum]<12))
                            if (ex[DeltaNum] - sx[DeltaNum] > 0)
                            {
                                Delta[DeltaNum] = ex[DeltaNum] - sx[DeltaNum];
                                //LookForStart = true;

                                changeValue = false;
                                if (ey[DeltaNum] - sy[DeltaNum] > 13)
                                    DeltaNum_Max += 1;
                                DeltaNum += 1;
                                Array2d[x2, y2] = 0;
                            }
                            LookForEnd = false;
                            x2 = x + 50;

                        }
                        if (changeValue)
                            Array2d[x2, y2] = 0;

                    }

                }


                max = 0;
                for (i = 0; i < DeltaNum; i++)
                {
                    if (Delta[i] > Delta[max]) max = i;
                    Delta_Temp = Delta[i] + Delta_Temp;
                }
                //Find the max delta
                if (DeltaNum < 5)
                    return rec;
                if (DeltaNum_Max > 4)
                    return rec;
                Delta_Temp = Delta_Temp / DeltaNum;
                if (Delta_Temp > 12)
                    return rec;

                return rec;
            }
            private Bitmap Array2dToBMP(Bitmap b, Byte[,] Array2d, bool color)
            {
                BitmapData bmData;
                int nOffset = 0, stride = 0;
                System.IntPtr Scan0;
                int j = 0;
                unsafe
                {
                    byte* p;
                    if (color)
                    {
                        bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        Scan0 = bmData.Scan0;
                        stride = bmData.Stride;
                        p = (byte*)(void*)Scan0;

                        ROI_x = (b.Width - _ROI_EndX - _ROI_StartX) * 3;
                        ROI_y = b.Width - _ROI_EndY - _ROI_StartY;
                        nOffset = stride - b.Width * 3;
                        for (int y = _ROI_StartY; y < b.Height - _ROI_EndY; ++y)
                        {
                            if (y == _ROI_StartY)
                            {
                                p = p + ((_ROI_StartX + _ROI_StartY * b.Width) * 3);
                            }
                            else
                            {
                                p = p + (_ROI_StartX * 3);
                            }
                            for (int x = _ROI_StartX; x < b.Width - _ROI_EndX; ++x)
                            {
                                p[0] = array2d[(x - _ROI_StartX) * 3, y - _ROI_StartY];
                                p[1] = array2d[(x - _ROI_StartX) * 3 + 1, y - _ROI_StartY];
                                p[2] = array2d[(x - _ROI_StartX) * 3 + 2, y - _ROI_StartY];
                                p += 3;
                                if (x == b.Width - _ROI_EndX - 1)
                                {
                                    p = p + (_ROI_EndX * 3);
                                }
                            }
                            p += nOffset;
                        }

                    }
                    else
                    {
                        bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                        Scan0 = bmData.Scan0;
                        stride = bmData.Stride;
                        p = (byte*)(void*)Scan0;

                        ROI_x = b.Width - _ROI_EndX - _ROI_StartX;
                        ROI_y = b.Height - _ROI_EndY - _ROI_StartY;
                        nOffset = stride - b.Width;
                        for (int y = _ROI_StartY; y < b.Height - _ROI_EndY; ++y)
                        {
                            if (y == _ROI_StartY)
                            {
                                p = p + (_ROI_StartX + _ROI_StartY * b.Width);
                            }
                            else
                            {
                                p = p + _ROI_StartX;
                            }

                            for (int x = _ROI_StartX; x < b.Width - _ROI_EndX; ++x)
                            {
                                p[0] = array2d[x - _ROI_StartX, y - _ROI_StartY];
                                p += 1;
                                if (x == b.Width - _ROI_EndX - 1)
                                {
                                    p = p + _ROI_EndX;
                                }
                            }
                            p += nOffset;
                        }

                    }
                }
                b.UnlockBits(bmData);
                return b;
            }
            private Bitmap Array2dToBMP(Bitmap b, Byte[,] Array2d)
            {
                BitmapData bmData;
                int nOffset = 0, stride = 0;
                System.IntPtr Scan0;
                int j = 0;
                unsafe
                {
                    byte* p;
                    bmData = b.LockBits(new Rectangle(0, 0, Array2d.GetLength(0), Array2d.GetLength(1)), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    Scan0 = bmData.Scan0;
                    stride = bmData.Stride;
                    p = (byte*)(void*)Scan0;
                    nOffset = stride - Array2d.GetLength(0) * 3;
                    for (int y = 0; y < Array2d.GetLength(1); ++y)
                    {
                        for (int x = 0; x < Array2d.GetLength(0); ++x)
                        {
                            p[2] = p[1] = p[0] = array2d[x, y];
                            p += 3;
                        }
                        p += nOffset;
                    }

                }
                b.UnlockBits(bmData);
                return b;
            }
            #endregion
            #region Gen7.1 AimerCenter
            //***************Gen7.1 project***************
            //***************EOL**************************
            //***************F001799**********************
            //***************TestDOEPattern()*************
            //********************************************
            //                    .
            //                    .
            //                    .
            //                    .
            //            . . . . o . . . . 
            //                    .
            //                    .
            //                    .
            //                    .
            //********************************************
            //***************imageProcess.AmierCenter(TargetBMP, 50, 50, 1000, 600)*******

            private bool FindCenterTemp(Byte[,] Array2d_roi, ref int CenterTemp_x, ref int CenterTemp_y)
            {
                try
                {
                    Int32 i = 0, j = 0, x = 0, y = 0;
                    int PixelValue = 0, pixelvalue_temp = 0;
                    double Ratio_temp = 0, Ratio_Max = 0;
                    for (x = 20; x < Array2d_roi.GetLength(0) - 20; x++)
                    {
                        for (y = 20; y < Array2d_roi.GetLength(1) - 20; y++)
                        {
                            PixelValue = Array2d_roi[x, y];
                            if (PixelValue > _WhiteThreshold)
                            {
                                pixelvalue_temp = 0;
                                for (i = 0; i < 20; i++)
                                {
                                    for (j = 0; j < 20; j++)
                                    {
                                        pixelvalue_temp = pixelvalue_temp + Array2d_roi[x + i, y + i];
                                    }
                                }
                                Ratio_temp = pixelvalue_temp / 400;
                                if (Ratio_temp > Ratio_Max)
                                {
                                    Ratio_Max = Ratio_temp;
                                    CenterTemp_x = x;
                                    CenterTemp_y = y;
                                }

                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return false;
                }
                return true;
            }
            private bool FindCenterTemp_LEDLine(Byte[,] Array2d_roi, ref int CenterTemp_x, ref int CenterTemp_y)
            {
                try
                {
                    _Edge_BotY = 0;
                    _Edge_UpY = Array2d_roi.GetLength(1);
                    _Edge_RightX = 0;
                    _Edge_LeftX = Array2d_roi.GetLength(0);

                    Int32 i = 0, j = 0, x = 0, y = 0;
                    CenterTemp_y = 0;
                    CenterTemp_x = 0;
                    int PixelValue = 0, pixelvalue_temp = 0;
                    double Ratio_temp = 0;
                    int Ratio_count = 0;
                    for (x = 5; x < Array2d_roi.GetLength(0) - 5; x++)
                    {
                        for (y = 5; y < Array2d_roi.GetLength(1) - 5; y++)
                        {
                            PixelValue = Array2d_roi[x, y];
                            if (PixelValue > AutoThreshold)
                            {
                                pixelvalue_temp = 0;
                                for (i = 0; i < 3; i++)
                                {
                                    for (j = 0; j < 3; j++)
                                    {
                                        pixelvalue_temp = pixelvalue_temp + Array2d_roi[x + i, y + j];
                                    }
                                }
                                Ratio_temp = pixelvalue_temp / 9;
                                if (Ratio_temp > AutoThreshold)
                                {
                                    Ratio_count = Ratio_count + 1;
                                    //CenterTemp_x = CenterTemp_x + x;  
                                    if (y > _Edge_BotY)
                                    {
                                        _Edge_BotY = y + _ROI_StartY;
                                        _Edge_BotX = x + _ROI_StartX;
                                    }
                                    if (y < _Edge_UpY)
                                    {
                                        _Edge_UpY = y + _ROI_StartY;
                                        _Edge_UpX = x + _ROI_StartX;
                                    }
                                    CenterTemp_y = CenterTemp_y + y;

                                }

                            }
                        }
                    }
                    if (Ratio_count == 0)
                    {
                        _ErrorMessage = "Find center fail.";
                        return false;
                    }
                    //CenterTemp_x = CenterTemp_x / Ratio_count;
                    CenterTemp_y = CenterTemp_y / Ratio_count;
                    Ratio_count = 0;
                    CenterTemp_x = 0;
                    for (x = 5; x < Array2d_roi.GetLength(0) - 5; x++)
                    {
                        //for (y = 5; y < Array2d_roi.GetLength(1) - 5; y++)
                        //{
                        PixelValue = Array2d_roi[x, CenterTemp_y];
                        if (PixelValue > AutoThreshold)
                        {
                            pixelvalue_temp = 0;
                            for (i = 0; i < 3; i++)
                            {
                                for (j = 0; j < 3; j++)
                                {
                                    pixelvalue_temp = pixelvalue_temp + Array2d_roi[x + i, CenterTemp_y + j];
                                }
                            }
                            Ratio_temp = pixelvalue_temp / 9;
                            if (Ratio_temp > AutoThreshold)
                            {
                                Ratio_count = Ratio_count + 1;
                                if (x > _Edge_RightX)
                                {
                                    _Edge_RightY = CenterTemp_y + _ROI_StartY;
                                    _Edge_RightX = x + _ROI_StartX;
                                }
                                if (x < _Edge_LeftX)
                                {
                                    _Edge_LeftY = CenterTemp_y + _ROI_StartY;
                                    _Edge_LeftX = x + _ROI_StartX;
                                }
                                CenterTemp_x = CenterTemp_x + x;
                                //CenterTemp_y = CenterTemp_y + y;
                            }

                            //}
                        }
                    }
                    CenterTemp_x = CenterTemp_x / Ratio_count;
                }
                catch (Exception err)
                {
                    _ErrorMessage = "Find center fail." + err.Message;
                    return false;
                }
                return true;
            }

            private bool FindCenterTemp_LEDCross(Byte[,] Array2d_roi, ref int CenterTemp_x, ref int CenterTemp_y)
            {
                try
                {
                    _Edge_BotY = 0;
                    _Edge_UpY = Array2d_roi.GetLength(1);
                    _Edge_RightX = 0;
                    _Edge_LeftY = Array2d_roi.GetLength(0);

                    Int32 i = 0, j = 0, x = 0, y = 0;
                    int PixelValue = 0, pixelvalue_temp = 0, array_size = 80;
                    bool CenterDot_Cross_X = false, CenterDot_Cross_Y = false;
                    int CenterDot_Cross_Num = 0;
                    for (x = array_size; x < Array2d_roi.GetLength(0) - array_size; x++)
                    {
                        for (y = array_size; y < Array2d_roi.GetLength(1) - array_size; y++)
                        {
                            CenterDot_Cross_X = false;
                            CenterDot_Cross_Y = false;
                            PixelValue = Array2d_roi[x, y];
                            if (PixelValue > AutoThreshold)
                            {
                                pixelvalue_temp = 0;
                                for (i = 0; i < array_size * 2; i++)
                                {
                                    pixelvalue_temp = pixelvalue_temp + Array2d_roi[x + i - array_size, y];
                                }
                                pixelvalue_temp = pixelvalue_temp / (array_size * 2);
                                if (pixelvalue_temp > AutoThreshold)
                                    CenterDot_Cross_X = true;
                                else
                                    CenterDot_Cross_X = false;

                                if (CenterDot_Cross_X == true)
                                {
                                    if (x > 605)
                                        pixelvalue_temp = 0;
                                    pixelvalue_temp = 0;
                                    for (j = 0; j < array_size * 2; j++)
                                    {
                                        pixelvalue_temp = pixelvalue_temp + Array2d_roi[x, y + j - array_size];
                                    }
                                    pixelvalue_temp = pixelvalue_temp / (array_size * 2);
                                    if (pixelvalue_temp > AutoThreshold)
                                        CenterDot_Cross_Y = true;
                                    else
                                        CenterDot_Cross_Y = false;
                                }
                                if (CenterDot_Cross_X == true && CenterDot_Cross_Y == true)
                                {
                                    CenterTemp_x = CenterTemp_x + x;
                                    CenterTemp_y = CenterTemp_y + y;
                                    CenterDot_Cross_Num++;
                                }
                            }
                        }
                    }
                    if (CenterDot_Cross_Num == 0)
                    {
                        _ErrorMessage = "Can't find the center of cross image";
                        return false;
                    }
                    CenterTemp_x = CenterTemp_x / CenterDot_Cross_Num;
                    CenterTemp_y = CenterTemp_y / CenterDot_Cross_Num;
                    #region find  x edge point
                    int temp_startX = Array2d_roi.GetLength(0) / 10;
                    int temp_XW = Array2d_roi.GetLength(0) - temp_startX;
                    for (x = temp_startX; x < temp_XW; x++)
                    {
                        PixelValue = 0;
                        PixelValue = Array2d_roi[x, CenterTemp_y];
                        if (PixelValue > AutoThreshold)
                        {
                            pixelvalue_temp = 0;
                            for (i = 0; i < 3; i++)
                            {
                                for (j = 0; j < 3; j++)
                                {
                                    pixelvalue_temp = pixelvalue_temp + Array2d_roi[x + i, CenterTemp_y + j];
                                }
                            }
                            if (pixelvalue_temp / 9 > AutoThreshold)
                            {
                                if (x > _Edge_RightX)
                                {
                                    _Edge_RightY = CenterTemp_y;
                                    _Edge_RightX = x;
                                }
                                if (x < _Edge_LeftX)
                                {
                                    _Edge_LeftY = CenterTemp_y;
                                    _Edge_LeftX = x;
                                }
                            }
                        }
                    }
                    #endregion
                    //find  y edge point
                    #region find  y edge point
                    int temp_startY = Array2d_roi.GetLength(1) / 10;
                    int temp_Yh = Array2d_roi.GetLength(1) - temp_startY;
                    for (y = temp_startY; y < temp_Yh; y++)
                    {
                        PixelValue = 0;
                        PixelValue = Array2d_roi[CenterTemp_x, y];
                        if (PixelValue > AutoThreshold)
                        {
                            pixelvalue_temp = 0;
                            for (i = 0; i < 3; i++)
                            {
                                for (j = 0; j < 3; j++)
                                {
                                    pixelvalue_temp = pixelvalue_temp + Array2d_roi[CenterTemp_x + i, y + j];
                                }
                            }
                            if (pixelvalue_temp / 9 > AutoThreshold)
                            {
                                if (y > _Edge_BotY)
                                {
                                    _Edge_BotY = y;
                                    _Edge_BotX = CenterTemp_x;
                                }
                                if (y < _Edge_UpY)
                                {
                                    _Edge_UpY = y;
                                    _Edge_UpX = CenterTemp_x;
                                }
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return false;
                }
                return true;
            }
            private bool FindCenterTemp_LEDCross_Dot(Byte[,] Array2d_roi, ref int CenterTemp_x, ref int CenterTemp_y)
            {
                try
                {
                    Int32 i = 0, j = 0, x = 0, y = 0;
                    _Edge_BotY = 0;
                    _Edge_UpY = Array2d_roi.GetLength(1);
                    _Edge_RightX = 0;
                    _Edge_LeftX = Array2d_roi.GetLength(0);
                    int PixelValue = 0, pixelvalue_temp = 0, array_size = 80;
                    bool CenterDot_Cross_X = false, CenterDot_Cross_Y = false;
                    int CenterDot_Cross_Num = 0;
                    for (x = array_size; x < Array2d_roi.GetLength(0) - array_size; x++)
                    {
                        for (y = array_size; y < Array2d_roi.GetLength(1) - array_size; y++)
                        {
                            CenterDot_Cross_X = false;
                            CenterDot_Cross_Y = false;
                            PixelValue = Array2d_roi[x, y];
                            if (PixelValue > AutoThreshold)
                            {
                                pixelvalue_temp = 0;
                                //for (i = 0; i < array_size * 2; i++)
                                //{
                                //    pixelvalue_temp = pixelvalue_temp + Array2d_roi[x + i - array_size, y];
                                //}
                                //pixelvalue_temp = pixelvalue_temp / (array_size * 2);

                                pixelvalue_temp = Array2d_roi[x - array_size, y] + Array2d_roi[x + array_size, y];
                                pixelvalue_temp = pixelvalue_temp / 2;
                                if (pixelvalue_temp > AutoThreshold)
                                    CenterDot_Cross_X = true;
                                else
                                    CenterDot_Cross_X = false;

                                if (CenterDot_Cross_X == true)
                                {
                                    if (x > 605)
                                        pixelvalue_temp = 0;
                                    pixelvalue_temp = 0;
                                    //for (j = 0; j < array_size * 2; j++)
                                    //{
                                    //    pixelvalue_temp = pixelvalue_temp + Array2d_roi[x, y + j - array_size];
                                    //}
                                    //pixelvalue_temp = pixelvalue_temp / (array_size * 2);
                                    pixelvalue_temp = Array2d_roi[x, y - array_size] + Array2d_roi[x, y + array_size];
                                    pixelvalue_temp = pixelvalue_temp / 2;
                                    if (pixelvalue_temp > AutoThreshold)
                                        CenterDot_Cross_Y = true;
                                    else
                                        CenterDot_Cross_Y = false;
                                }
                                if (CenterDot_Cross_X == true && CenterDot_Cross_Y == true)
                                {
                                    CenterTemp_x = CenterTemp_x + x;
                                    CenterTemp_y = CenterTemp_y + y;
                                    CenterDot_Cross_Num++;
                                }
                            }
                        }
                    }
                    if (CenterDot_Cross_Num == 0)
                    {
                        _ErrorMessage = "Can't find the center of cross image";
                        return false;
                    }
                    CenterTemp_x = CenterTemp_x / CenterDot_Cross_Num;
                    CenterTemp_y = CenterTemp_y / CenterDot_Cross_Num;

                    #region find  x edge point
                    int temp_startX = Array2d_roi.GetLength(0) / 10;
                    int temp_XW = Array2d_roi.GetLength(0) - temp_startX;
                    for (x = temp_startX; x < temp_XW; x++)
                    {
                        PixelValue = 0;
                        PixelValue = Array2d_roi[x, CenterTemp_y];
                        if (PixelValue > AutoThreshold)
                        {
                            pixelvalue_temp = 0;
                            for (i = 0; i < 3; i++)
                            {
                                for (j = 0; j < 3; j++)
                                {
                                    pixelvalue_temp = pixelvalue_temp + Array2d_roi[x + i, CenterTemp_y + j];
                                }
                            }
                            if (pixelvalue_temp / 9 > AutoThreshold)
                            {
                                if (x > _Edge_RightX)
                                {
                                    _Edge_RightY = CenterTemp_y;
                                    _Edge_RightX = x;
                                }
                                if (x < _Edge_LeftX)
                                {
                                    _Edge_LeftY = CenterTemp_y;
                                    _Edge_LeftX = x;
                                }
                            }
                        }
                    }
                    #endregion
                    //find  y edge point
                    #region find  y edge point
                    int temp_startY = Array2d_roi.GetLength(1) / 10;
                    int temp_Yh = Array2d_roi.GetLength(1) - temp_startY;
                    for (y = temp_startY; y < temp_Yh; y++)
                    {
                        PixelValue = 0;
                        PixelValue = Array2d_roi[CenterTemp_x, y];
                        if (PixelValue > AutoThreshold)
                        {
                            pixelvalue_temp = 0;
                            for (i = 0; i < 3; i++)
                            {
                                for (j = 0; j < 3; j++)
                                {
                                    pixelvalue_temp = pixelvalue_temp + Array2d_roi[CenterTemp_x + i, y + j];
                                }
                            }
                            if (pixelvalue_temp / 9 > AutoThreshold)
                            {
                                if (y > _Edge_BotY)
                                {
                                    _Edge_BotY = y;
                                    _Edge_BotX = CenterTemp_x;
                                }
                                if (y < _Edge_UpY)
                                {
                                    _Edge_UpY = y;
                                    _Edge_UpX = CenterTemp_x;
                                }
                            }
                        }
                    }
                    #endregion

                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return false;
                }
                return true;
            }
            private bool DoubleFindCenter_LEDCross(Byte[,] Array2d_roi, ref int CenterTemp_x, ref int CenterTemp_y)
            {
                try
                {
                    Int32 i = 0, j = 0;
                    int PixelValue = 0, array_size = 80;
                    int count_x = 0, count_y = 0;
                    int centerCount_X = 0, centerCount_Y = 0;
                    for (i = 0; i < array_size; i++)
                    {
                        for (j = 0; j < array_size; j++)
                        {
                            PixelValue = Array2d_roi[CenterTemp_x + i, CenterTemp_y + j];
                            if (PixelValue > AutoThreshold)
                            {
                                count_x++;
                                count_y++;
                                centerCount_X = centerCount_X + CenterTemp_x + i;
                                centerCount_Y = CenterTemp_y + j + centerCount_Y;
                            }
                        }
                    }
                    if (count_x == 0)
                        count_x = 1;
                    if (count_y == 0)
                        count_y = 1;
                    CenterTemp_x = centerCount_X / count_x;
                    CenterTemp_y = centerCount_Y / count_y;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return false;
                }
                return true;
            }
            private bool FindFirstEndDot(Byte[,] Array2d_roi, ref int X_L, ref int Y_L, ref int X_R, ref int Y_R)
            {
                int firstDot_value = 0;
                int min_x = AimerCenter_X - _ROI_StartX, max_x = AimerCenter_X - _ROI_StartX;

                try
                {
                    Int32 i = 0, j = 0, x = 0, y = 0;
                    int PixelValue = 0, pixelvalue_temp = 0;
                    double first_temp = 0;
                    for (x = 20; x < Array2d_roi.GetLength(0) - 20; x++)
                    {
                        for (y = 20; y < Array2d_roi.GetLength(1) - 20; y++)
                        {
                            if (x == 20 && y == 20)
                            {
                                for (i = 0; i < 20; i++)
                                {
                                    for (j = 0; j < 20; j++)
                                    {
                                        firstDot_value = pixelvalue_temp + Array2d_roi[x + i, y + i];
                                    }
                                }
                                firstDot_value = firstDot_value / 400;
                            }
                            PixelValue = Array2d_roi[x, y];
                            if (PixelValue > firstDot_value + 20)
                            {
                                pixelvalue_temp = 0;
                                for (i = 0; i < 20; i++)
                                {
                                    for (j = 0; j < 20; j++)
                                    {
                                        pixelvalue_temp = pixelvalue_temp + Array2d_roi[x + i, y + i];
                                    }
                                }
                                pixelvalue_temp = pixelvalue_temp / 400;
                                if (pixelvalue_temp > firstDot_value + 20)
                                {
                                    if (x < min_x)
                                    {
                                        min_x = x;
                                        X_L = x;
                                        Y_L = y;
                                    }
                                    else if (x > max_x)
                                    {
                                        max_x = x;
                                        X_R = x;
                                        Y_R = y;
                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return false;
                }
                return true;
            }
            private bool DoubleFindCenter(Byte[,] Array2d_roi, ref int CenterTemp_x, ref int CenterTemp_y)
            {
                try
                {
                    Int32 i = 0, j = 0;
                    int PixelValue = 0;
                    int count_x = 0, count_y = 0;
                    int centerCount_X = 0, centerCount_Y = 0;
                    for (i = 0; i < 20; i++)
                    {
                        for (j = 0; j < 20; j++)
                        {
                            PixelValue = Array2d_roi[CenterTemp_x + i, CenterTemp_y + j];
                            if (PixelValue > _WhiteThreshold)
                            {
                                count_x++;
                                count_y++;
                                centerCount_X = centerCount_X + CenterTemp_x + i;
                                centerCount_Y = CenterTemp_y + j + centerCount_Y;
                            }
                        }
                    }
                    if (count_x == 0)
                        count_x = 1;
                    if (count_y == 0)
                        count_y = 1;
                    CenterTemp_x = centerCount_X / count_x;
                    CenterTemp_y = centerCount_Y / count_y;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return false;
                }
                return true;
            }
            public Bitmap AimerCenter_LaserCross(Bitmap b)
            {
                int center_X = 0, center_Y = 0;

                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                array1d = new byte[b.Width * b.Height];
                BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                FindCenterTemp(array2d, ref center_X, ref center_Y);
                DoubleFindCenter(array2d, ref center_X, ref center_Y);
                _AimerCenter_X = center_X + _ROI_StartX;
                _AimerCenter_Y = center_Y + _ROI_StartY;
                //Array2dToBMP(b, array2d);
                return b;
            }
            public Bitmap AimerDOE(Bitmap b)
            {
                //_ROI_StartX = ROI_PointX;
                //_ROI_StartY = ROI_PointY;

                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                int center_X = 0, center_Y = 0;
                int x_L = 0, y_L = 0, x_R = 0, y_R = 0;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                array1d = new byte[b.Width * b.Height];
                BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                FindCenterTemp(array2d, ref center_X, ref center_Y);
                DoubleFindCenter(array2d, ref center_X, ref center_Y);
                if (center_X == 0 || center_Y == 0)
                {
                    _Rotation_Deg = 99999;
                    return b;
                }
                _AimerCenter_X = center_X + _ROI_StartX;
                _AimerCenter_Y = center_Y + _ROI_StartY;
                //resize ROI by center position
                int temp_ROI_PointX = _ROI_StartX;
                int temp_ROI_PointY = _AimerCenter_Y - 100;
                int temp_ROI_Width = ROI_Width;
                int temp_ROI_Height = 200;

                BmpToArray2d(b, temp_ROI_PointX, temp_ROI_PointY, temp_ROI_Width, temp_ROI_Height);
                // FindCenterTemp(array2d, ref center_X, ref center_Y);
                FindFirstEndDot(array2d, ref x_L, ref y_L, ref x_R, ref y_R);
                _LaserCross_x_L = x_L + _ROI_StartX;
                _LaserCross_y_L = y_L + temp_ROI_PointY;
                _LaserCross_x_R = x_R + _ROI_StartX;
                _LaserCross_y_R = y_R + temp_ROI_PointY;

                if (x_R != x_L)
                {
                    _Rotation_Deg = System.Math.Atan2((y_R - y_L), (x_R - x_L)) * 180 / System.Math.PI;
                }
                else
                    _Rotation_Deg = 99999;
                //Array2dToBMP(b, array2d);
                return b;
            }

            private Bitmap Load2DBmp(Bitmap b, int roiX, int roiY, int roiW, int roiH)
            {
                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                if (roiW > b.Width) roiW = b.Width;
                if (roiH > b.Height) roiH = b.Height;
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - b.Width * 3;
                    byte red, green, blue, color = 0;
                    array2d = new byte[roiW, roiH];
                    for (int y = roiY; y < roiY + roiH; ++y)//
                    {
                        if (y == roiY)
                        {
                            p = p + ((roiY * b.Width + roiX) * 3);
                        }
                        else
                        {
                            p = p + (roiX * 3);
                        }
                        for (int x = roiX; x < roiX + roiW; ++x)
                        {
                            blue = p[0];
                            green = p[1];
                            red = p[2];
                            color = (byte)((blue + 6 * green + 3 * red) / 10);
                            if (x == roiX || x == roiX + roiW - 1 || y == roiY || y == roiY + roiH - 1)
                            {
                                p[1] = p[2] = 255;//Draw ROI
                            }
                            array2d[x - roiX, y - roiY] = color;

                            if (x == roiX + roiW - 1)
                            {
                                p += (b.Width - (roiX + roiW - 1)) * 3;
                            }
                            else
                            {
                                p += 3;
                            }
                        }
                        p += nOffset;
                    }
                }
                b.UnlockBits(bmData);
                return b;
            }

            private bool GetNeighbors(Byte[,] Array2d_roi, int x, int y, int size, ref int value)
            {
                int sum = 0;
                try
                {
                    for (int j = 0; j < size; j++)
                    {
                        for (int i = 0; i < size; i++)
                        {
                            sum += Array2d_roi[x + i, y + j];
                        }
                    }
                    value = sum / (size * size);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            private bool FindEdgePoint(Byte[,] Array2d_roi, AimerParameter_t para, enFindEdgeDir direction, ref AimerResult_t result)
            {
                int i = 0, j = 0;
                int value = 0;
                bool res = false;
                try
                {
                    switch (direction)
                    {
                        case enFindEdgeDir.enFromUpToBottom:
                            for (j = 0; j < para.RoiHeight; j++)
                            {
                                for (i = 0; i < para.RoiWidth; i++)
                                {
                                    res = GetNeighbors(Array2d_roi, i, j, para.Size, ref value);
                                    if (value > para.Threshold)
                                    {
                                        result.XTop = i + para.XStart;
                                        result.YTop = j + para.YStart;
                                        j = 1000;
                                        break;
                                    }
                                }
                            }
                            break;
                        case enFindEdgeDir.enFromBottomToUp:
                            for (j = para.RoiHeight - para.Size; j > 0; j--)
                            {
                                for (i = 0; i < para.RoiWidth; i++)
                                {
                                    res = GetNeighbors(Array2d_roi, i, j, para.Size, ref value);
                                    if (value > para.Threshold)
                                    {
                                        result.XBottom = i + para.XStart;
                                        result.YBottom = j + para.YStart;
                                        j = 0;
                                        break;
                                    }
                                }
                            }
                            break;
                        default:
                            return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public bool AimerDOE(Bitmap b, AimerParameter_t parameter, ref AimerResult_t result)
            {
                try
                {
                    int ROIX = parameter.XStart;
                    int ROIY = parameter.YStart;
                    int ROIW = parameter.RoiWidth;
                    int ROIH = parameter.RoiHeight;

                    byte[] array1d = new byte[b.Width * b.Height];
                    Bitmap b2 = Load2DBmp(b, ROIX, ROIY, ROIW, ROIH);
                    //b2.Save("roi2d.bmp");
                    FindEdgePoint(array2d, parameter, enFindEdgeDir.enFromUpToBottom, ref result);
                    FindEdgePoint(array2d, parameter, enFindEdgeDir.enFromBottomToUp, ref result);
                    result.Height = Math.Abs(result.YBottom - result.YTop);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            public Bitmap AimerDOE(Bitmap b, bool BlurEnable)
            {
                //_ROI_StartX = ROI_PointX;
                //_ROI_StartY = ROI_PointY;

                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                int center_X = 0, center_Y = 0;
                int x_L = 0, y_L = 0, x_R = 0, y_R = 0;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                array1d = new byte[b.Width * b.Height];
                BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                FindCenterTemp(array2d, ref center_X, ref center_Y);
                DoubleFindCenter(array2d, ref center_X, ref center_Y);
                if (center_X == 0 || center_Y == 0)
                {
                    _Rotation_Deg = 99999;
                    return b;
                }
                _AimerCenter_X = center_X + _ROI_StartX;
                _AimerCenter_Y = center_Y + _ROI_StartY;
                //resize ROI by center position
                int temp_ROI_PointX = _ROI_StartX;
                int temp_ROI_PointY = _ROI_StartY;
                int temp_ROI_Width = ROI_Width;
                int temp_ROI_Height = ROI_Height;

                //BmpToArray2d(b, temp_ROI_PointX, temp_ROI_PointY, temp_ROI_Width, temp_ROI_Height);
                // FindCenterTemp(array2d, ref center_X, ref center_Y);
                FindFirstEndDot(array2d, ref x_L, ref y_L, ref x_R, ref y_R);
                _LaserCross_x_L = x_L + _ROI_StartX;
                _LaserCross_y_L = y_L + temp_ROI_PointY;
                _LaserCross_x_R = x_R + _ROI_StartX;
                _LaserCross_y_R = y_R + temp_ROI_PointY;

                if (x_R != x_L)
                {
                    _Rotation_Deg = System.Math.Atan2((y_R - y_L), (x_R - x_L)) * 180 / System.Math.PI;
                }
                else
                {
                    _Rotation_Deg = 99999;
                    return b;
                }
                if (BlurEnable == true)
                {
                    CalculateLaserHeight();
                }
                //Array2dToBMP(b, array2d);
                return b;
            }
            private bool CalculateLaserHeight()
            {
                _LaserHeight_R = 0;
                _LaserHeight_L = 0;
                try
                {
                    int temp_X_R = (int)(_AimerCenter_X - _DistanceToCenter);
                    int temp_Y_R = 0;
                    int temp_X_L = (int)(_DistanceToCenter + _AimerCenter_X);
                    int temp_Y_L = 0;
                    int rang = (int)_DistanceToCenter * 2;
                    for (int i = 0; i < rang; i++)
                    {
                        temp_Y_R = _AimerCenter_Y - ROI_StartY - (rang / 2) + i;
                        temp_Y_L = _AimerCenter_Y - ROI_StartY - (rang / 2) + i;
                        if (array2d[temp_X_R, temp_Y_R] > AutoThreshold * 5)
                            _LaserHeight_R++;
                        if (array2d[temp_X_L, temp_Y_L] > AutoThreshold * 5)
                            _LaserHeight_L++;
                        //sumPixel_R = sumPixel_R + array2d[temp_X_R, temp_Y_R];
                        //sumPixel_L = sumPixel_L + array2d[temp_X_L, temp_Y_L];
                    }
                    //_LaserHeight_L = sumPixel_L / rang;
                    //_LaserHeight_R = sumPixel_R / rang;
                    int width = 50;
                    int half_width = width / 2;
                    double centreArea = 0;
                    for (int m = 0; m < width; m++)
                    {
                        for (int n = 0; n < width; n++)
                        {
                            if (array2d[_AimerCenter_X - half_width + m, _AimerCenter_Y - half_width - _ROI_StartY + n] > 220)
                                centreArea++;
                        }
                    }
                    _CentreRaitio = centreArea / (width * width);
                }
                catch (Exception ERR)
                {
                    _LaserHeight_L = 99999;
                    _LaserHeight_R = 99999;
                    return false;
                }



                return true;
            }
            public Bitmap AimerCenter_LEDLine(Bitmap b)
            {
                _ErrorMessage = "";
                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                int center_X = 0, center_Y = 0;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                array1d = new byte[b.Width * b.Height];
                ThresholdFactor = 0.5;
                BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                if (FindCenterTemp_LEDLine(array2d, ref center_X, ref center_Y) == false) return b;
                _AimerCenter_X = center_X + _ROI_StartX;
                _AimerCenter_Y = center_Y + _ROI_StartY;
                return b;
            }
            public Bitmap Sharpness_LEDLine(Bitmap b)
            {
                //_ROI_StartX = ROI_PointX;
                //_ROI_StartY = ROI_PointY;
                //AimerCenter_LEDLine(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                AimerCenter_LEDLine(b);
                int SizeBot = 0, SizeUp = 0;
                if (CalLedProfile_LEDLine(b.Height - _ROI_StartY - _ROI_EndY, b.Width - _ROI_StartX - _ROI_EndX, array1d, ref SizeBot, ref SizeUp) == false) return b;
                return b;
            }
            public Bitmap Sharpness_LEDCross(Bitmap b)
            {
                //_ROI_StartX = ROI_PointX;
                //_ROI_StartY = ROI_PointY;

                AimerCenter_LEDCross(b);
                int SizeBot = 0, SizeUp = 0;
                if (CalLedProfile_LEDCross(b.Height - _ROI_StartY - _ROI_EndY, b.Width - _ROI_StartX - _ROI_EndX, array1d, ref SizeBot, ref SizeUp) == false) return b;
                //_Sharpness_Bot = SizeBot;
                //_Sharpness_Up = SizeUp;


                //Array2dToBMP(b, array2d);
                return b;
            }
            public Bitmap AimerCenter_LEDCross(Bitmap b)
            {
                //_ROI_StartX = ROI_PointX;
                //_ROI_StartY = ROI_PointY;
                _ErrorMessage = "";
                int center_X = 0, center_Y = 0;
                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;

                array1d = new byte[b.Width * b.Height];
                BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);

                if (FindCenterTemp_LEDCross(array2d, ref center_X, ref center_Y) == false) return b;

                _AimerCenter_X = center_X + _ROI_StartX;
                _AimerCenter_Y = center_Y + _ROI_StartY;
                return b;
            }

            public Bitmap AimerCenter_LEDCross_Dot(ref Bitmap b)
            {
                //_ROI_StartX = ROI_PointX;
                //_ROI_StartY = ROI_PointY;
                _ErrorMessage = "";
                int center_X = 0, center_Y = 0;
                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                array1d = new byte[b.Width * b.Height];
                b = BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);

                if (FindCenterTemp_LEDCross_Dot(array2d, ref center_X, ref center_Y) == false) return b;

                _AimerCenter_X = center_X + _ROI_StartX;
                _AimerCenter_Y = center_Y + _ROI_StartY;
                return b;
            }
            public Bitmap ImbalanceTest_LEDCross_Dot(ref Bitmap b)
            {
                //_ROI_StartX = ROI_PointX;
                //_ROI_StartY = ROI_PointY;
                AimerCenter_LEDCross_Dot(ref b);
                if (ImbalanceTest(b.Height - _ROI_StartY - _ROI_EndY, b.Width - _ROI_StartX - _ROI_EndX, true) == false) return b;
                return b;
            }
            public Bitmap ImbalanceTest_LEDLine(ref Bitmap b)
            {
                //_ROI_StartX = ROI_PointX;
                //_ROI_StartY = ROI_PointY;
                //AimerCenter_LEDLine(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                AimerCenter_LEDLine(b);
                if (ImbalanceTest(b.Height - _ROI_StartY - _ROI_EndY, b.Width - _ROI_StartX - _ROI_EndX, false) == false) return b;
                return b;
            }
            public Bitmap Sharpness_LEDCross_Dot(ref Bitmap b)
            {
                //_ROI_StartX = ROI_PointX;
                //_ROI_StartY = ROI_PointY;

                AimerCenter_LEDCross_Dot(ref b);

                int SizeBot = 0, SizeUp = 0;
                if (CalLedProfile_LEDCross(b.Height - _ROI_StartY - _ROI_EndY, b.Width - _ROI_StartX - _ROI_EndX, array1d, ref SizeBot, ref SizeUp) == false) return b;
                return b;
            }

            //'""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""
            //'Aimer sharpness test
            //'""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

            private bool CalLedProfile_LEDLine(int Height, int Width, byte[] Img, ref int SizeBot, ref int SizeUp)
            {
                //' take central vertical line (band of 21 pixels)
                //int xc  = Convert.ToInt32(Width / 2);
                try
                {
                    int xc = 0, yc = 0, line_interval = 80;
                    line_interval = Math.Abs(_Edge_RightX - _Edge_LeftX) / 4;
                    //int SizeBot_sum = 0, SizeUp_sum = 0;
                    int up_Y = 0, bot_Y = 0;
                    for (int Interval = 0; Interval < 3; Interval++)
                    {
                        if (Interval == 0)
                            xc = _AimerCenter_X - line_interval;
                        else if (Interval == 2)
                            xc = _AimerCenter_X + line_interval;
                        else
                            xc = _AimerCenter_X;
                        //yc=Height /10;
                        yc = array2d.GetLength(1) - Height / 10;
                        double[] line = new double[yc];
                        for (int y = Height / 10; y < yc; y++)
                        {
                            double sum = 0;
                            for (int x = xc - 10; x < xc + 10; x++)
                            {
                                //sum = sum + Img[Width * y + x];
                                sum = sum + array2d[x, y];
                            }
                            line[y] = sum;
                        }

                        //' filter line with a low pass running average over 5 pixels
                        double[] line_filtered = new double[yc];
                        for (int i = 2; i < line.Length - 3; i++)
                        {
                            double sum = 0;
                            for (int k = i - 2; k < i + 2; k++)
                            {
                                sum = sum + line[k];
                            }
                            line_filtered[i] = sum / 5;
                        }
                        line_filtered[0] = line_filtered[2];
                        line_filtered[1] = line_filtered[2];
                        line_filtered[line.Length - 2] = line_filtered[line.Length - 3];
                        line_filtered[line.Length - 1] = line_filtered[line.Length - 3];

                        //' take first derivative of filtered line
                        double[] line_filtered_der = new double[Height];
                        for (int i = 1; i < line.Length - 1; i++)
                        {
                            line_filtered_der[i] = line_filtered[i] - line_filtered[i - 1];
                        }
                        line_filtered_der[0] = line_filtered_der[1];

                        //' search min, max of derivative
                        double max = -255, min = 255, value = 0;
                        //for (int i = 10; i < line_filtered_der.Length - 10; i++)
                        for (int i = 10 + Height / 10; i < line_filtered_der.Length - 10 - Height / 10; i++)
                        {

                            value = line_filtered_der[i];
                            if (value > max)
                            {
                                max = value;
                                up_Y = i;
                            }
                            if (value < min)
                            {
                                bot_Y = i;
                                min = value;
                            }
                        }

                        //' count number of derivative values over max/2 and under min/2
                        SizeBot = 0;
                        SizeUp = 0;
                        for (int i = 10 + Height / 10; i < line_filtered_der.Length - 10 - Height / 10; i++)
                        {
                            value = line_filtered_der[i];
                            //'If value > max / 2 Then
                            if (value > max / 4) SizeUp = SizeUp + 1;
                            //'If value < min / 2 Then
                            if (value < min / 4) SizeBot = SizeBot + 1;
                        }
                        if (Interval == 0)
                        {
                            //_LEDLine_bot_L = SizeBot;
                            //_LEDLine_up_L = SizeUp;
                            _SharpnessLEDLine_bot_L = SizeBot;
                            _SharpnessLEDLine_up_L = SizeUp;
                        }
                        else if (Interval == 1)
                        {
                            //_LEDLine_bot_M = SizeBot;
                            //_LEDLine_up_M = SizeUp;
                            _SharpnessLEDLine_bot_M = SizeBot;
                            _SharpnessLEDLine_up_M = SizeUp;
                            _Aimer_LEDLine_Width = bot_Y - up_Y;
                        }
                        else
                        {
                            //_LEDLine_bot_R = SizeBot;
                            //_LEDLine_up_R = SizeUp;
                            _SharpnessLEDLine_bot_R = SizeBot;
                            _SharpnessLEDLine_up_R = SizeUp;
                        }
                        //SizeBot_sum = SizeBot_sum + SizeBot;
                        //SizeUp_sum = SizeUp_sum + SizeUp;
                    }
                    //SizeBot = SizeBot_sum / 3;
                    //SizeUp=SizeUp_sum/3;
                }
                catch (Exception err)
                {
                    _ErrorMessage = "CallLedProfile fail" + err.Message;
                    return false;
                }

                return true;
            }
            private bool CalLedProfile_LEDCircle(int Height, int Width, byte[] Img, ref int SizeBot, ref int SizeUp)
            {
                try
                {
                    #region vertical
                    int xc = 0, yc = 0;
                    int up_Y = 0, bot_Y = 0;

                    xc = _AimerCenter_X - _ROI_StartX;
                    yc = array2d.GetLength(1) - Height / 10;
                    double[] line = new double[yc];
                    for (int y = Height / 10; y < yc; y++)
                    {
                        double sum = 0;
                        for (int x = xc - 10; x < xc + 10; x++)
                        {
                            //sum = sum + Img[Width * y + x];
                            sum = sum + array2d[x, y];
                        }
                        line[y] = sum;
                    }

                    //' filter line with a low pass running average over 5 pixels
                    double[] line_filtered = new double[yc];
                    for (int i = 2; i < line.Length - 3; i++)
                    {
                        double sum = 0;
                        for (int k = i - 2; k < i + 2; k++)
                        {
                            sum = sum + line[k];
                        }
                        line_filtered[i] = sum / 5;
                    }
                    line_filtered[0] = line_filtered[2];
                    line_filtered[1] = line_filtered[2];
                    line_filtered[line.Length - 2] = line_filtered[line.Length - 3];
                    line_filtered[line.Length - 1] = line_filtered[line.Length - 3];

                    //' take first derivative of filtered line
                    double[] line_filtered_der = new double[Height];
                    for (int i = 1; i < line.Length - 1; i++)
                    {
                        line_filtered_der[i] = line_filtered[i] - line_filtered[i - 1];
                    }
                    line_filtered_der[0] = line_filtered_der[1];

                    //' search min, max of derivative
                    double max = -255, min = 255, value = 0;
                    //for (int i = 10; i < line_filtered_der.Length - 10; i++)
                    for (int i = 10 + Height / 10; i < line_filtered_der.Length - 10 - Height / 10; i++)
                    {

                        value = line_filtered_der[i];
                        if (value > max)
                        {
                            max = value;
                            up_Y = i;
                        }
                        if (value < min)
                        {
                            bot_Y = i;
                            min = value;
                        }
                    }

                    //' count number of derivative values over max/2 and under min/2
                    SizeBot = 0;
                    SizeUp = 0;
                    for (int i = 10 + Height / 10; i < line_filtered_der.Length - 10 - Height / 10; i++)
                    {
                        value = line_filtered_der[i];
                        //'If value > max / 2 Then
                        if (value > max / 4) SizeUp = SizeUp + 1;
                        //'If value < min / 2 Then
                        if (value < min / 4) SizeBot = SizeBot + 1;
                    }
                    _SharpnessLEDLine_bot_M = SizeBot;
                    _SharpnessLEDLine_up_M = SizeUp;
                    _diameter_V = bot_Y - up_Y;

                    #endregion
                    #region H
                    int Yc = 0, Xc = 0;
                    int L_X = 0, R_X = 0;

                    Yc = _AimerCenter_Y - _ROI_StartY;
                    Xc = array2d.GetLength(0) - Width / 10;
                    line = new double[Xc];
                    for (int X = Width / 10; X < Xc; X++)
                    {
                        double sum = 0;
                        for (int Y = Yc - 10; Y < Yc + 10; Y++)
                        {
                            //sum = sum + Img[Width * y + x];
                            sum = sum + array2d[X, Y];
                        }
                        line[X] = sum;
                    }

                    //' filter line with a low pass running average over 5 pixels
                    line_filtered = new double[Xc];
                    for (int i = 2; i < line.Length - 3; i++)
                    {
                        double sum = 0;
                        for (int k = i - 2; k < i + 2; k++)
                        {
                            sum = sum + line[k];
                        }
                        line_filtered[i] = sum / 5;
                    }
                    line_filtered[0] = line_filtered[2];
                    line_filtered[1] = line_filtered[2];
                    line_filtered[line.Length - 2] = line_filtered[line.Length - 3];
                    line_filtered[line.Length - 1] = line_filtered[line.Length - 3];

                    //' take first derivative of filtered line
                    line_filtered_der = new double[Width];
                    for (int i = 1; i < line.Length - 1; i++)
                    {
                        line_filtered_der[i] = line_filtered[i] - line_filtered[i - 1];
                    }
                    line_filtered_der[0] = line_filtered_der[1];

                    //' search min, max of derivative
                    max = -255; min = 255; value = 0;
                    //for (int i = 10; i < line_filtered_der.Length - 10; i++)
                    for (int i = 10 + Width / 10; i < line_filtered_der.Length - 10 - Width / 10; i++)
                    {

                        value = line_filtered_der[i];
                        if (value > max)
                        {
                            max = value;
                            L_X = i;
                        }
                        if (value < min)
                        {
                            R_X = i;
                            min = value;
                        }
                    }

                    //' count number of derivative values over max/2 and under min/2
                    SizeBot = 0;
                    SizeUp = 0;
                    for (int i = 10 + Width / 10; i < line_filtered_der.Length - 10 - Width / 10; i++)
                    {
                        value = line_filtered_der[i];
                        //'If value > max / 2 Then
                        if (value > max / 4) SizeUp = SizeUp + 1;
                        //'If value < min / 2 Then
                        if (value < min / 4) SizeBot = SizeBot + 1;
                    }
                    _SharpnessLEDLine_Right_M = SizeBot;
                    _SharpnessLEDLine_Left_M = SizeUp;
                    //_Aimer_LEDLine_Width_V = R_X - L_X;
                    _diameter_H = R_X - L_X;
                    #endregion
                }
                catch (Exception err)
                {
                    _ErrorMessage = "CallLedProfile fail" + err.Message;
                    return false;
                }

                return true;
            }

            private bool CalLedProfile_LEDCross(int Height, int Width, byte[] Img, ref int SizeBot, ref int SizeUp)
            {
                //' take central vertical line (band of 21 pixels)
                //int xc  = Convert.ToInt32(Width / 2);

                try
                {
                    #region H sharpness
                    int xc = 0, line_interval = 80;
                    line_interval = (_Edge_RightX - _Edge_LeftX) / 4;
                    //int SizeBot_sum = 0, SizeUp_sum = 0;
                    int up_Y = 0, yc = 0, bot_Y = 0;
                    for (int Interval = 0; Interval < 2; Interval++)
                    {
                        if (Interval == 0)
                            xc = _AimerCenter_X - line_interval - _ROI_StartX;
                        else if (Interval == 1)
                            xc = _AimerCenter_X + line_interval - _ROI_StartX;
                        else
                            xc = _AimerCenter_X - _ROI_StartX;
                        yc = array2d.GetLength(1) - Height / 10;
                        //yc = Height / 3 * 2;
                        double[] line = new double[yc];
                        for (int y = Height / 10; y < yc; y++)
                        {
                            double sum = 0;
                            for (int x = xc - 10; x < xc + 10; x++)
                            {
                                //sum = sum + Img[Width * y + x] ;
                                sum = sum + array2d[x, y];
                            }
                            line[y] = sum;
                        }

                        //' filter line with a low pass running average over 5 pixels
                        double[] line_filtered = new double[yc];
                        for (int i = 2; i < line.Length - 3; i++)
                        {
                            double sum = 0;
                            for (int k = i - 2; k < i + 2; k++)
                            {
                                sum = sum + line[k];
                            }
                            line_filtered[i] = sum / 5;
                        }
                        line_filtered[0] = line_filtered[2];
                        line_filtered[1] = line_filtered[2];
                        line_filtered[line.Length - 2] = line_filtered[line.Length - 3];
                        line_filtered[line.Length - 1] = line_filtered[line.Length - 3];

                        //' take first derivative of filtered line
                        double[] line_filtered_der = new double[line.Length];
                        for (int i = 1; i < line.Length - 1; i++)
                        {
                            line_filtered_der[i] = line_filtered[i] - line_filtered[i - 1];
                        }
                        line_filtered_der[0] = line_filtered_der[1];

                        //' search min, max of derivative
                        double max = -255, min = 255, value = 0;
                        for (int i = 10 + Height / 10; i < line_filtered_der.Length - 10 - Height / 10; i++)
                        {

                            value = line_filtered_der[i];
                            if (value > max)
                            {
                                max = value;
                                up_Y = i;
                            }
                            if (value < min)
                            {
                                bot_Y = i;
                                min = value;
                            }

                        }
                        //' count number of derivative values over max/2 and under min/2
                        SizeBot = 0;
                        SizeUp = 0;
                        for (int i = 10 + Height / 10; i < line_filtered_der.Length - 10 - +Height / 10; i++)
                        {
                            value = line_filtered_der[i];
                            if (value > max / 4) SizeUp = SizeUp + 1;
                            if (value < min / 4) SizeBot = SizeBot + 1;
                        }
                        if (Interval == 0)
                        {
                            _Aimer_LEDLine_Width = bot_Y - up_Y;
                            _SharpnessLEDCross_bot_L = SizeBot;
                            _SharpnessLEDCross_up_L = SizeUp;
                        }
                        else
                        {
                            _SharpnessLEDCross_bot_R = SizeBot;
                            _SharpnessLEDCross_up_R = SizeUp;
                        }
                    }
                    #endregion
                    #region V sharpness
                    int Yc = 0;
                    line_interval = 80;
                    line_interval = (_Edge_BotY - _Edge_UpY) / 4;
                    //int SizeBot_sum = 0, SizeUp_sum = 0;
                    int up_X = 0, Xc = 0, bot_X = 0;
                    for (int Interval = 0; Interval < 2; Interval++)
                    {
                        if (Interval == 0)
                            Yc = _AimerCenter_Y - line_interval - _ROI_StartY;
                        else if (Interval == 1)
                            Yc = _AimerCenter_Y + line_interval - _ROI_StartY;
                        else
                            Yc = _AimerCenter_Y - _ROI_StartY;

                        Xc = array2d.GetLength(0) - Width / 10;
                        double[] line = new double[Xc];
                        for (int X = Width / 10; X < Xc; X++)
                        {
                            double sum = 0;
                            for (int Y = Yc - 10; Y < Yc + 10; Y++)
                            {
                                //sum = sum + Img[Width * y + x] ;
                                sum = sum + array2d[X, Y];
                            }
                            line[X] = sum;
                        }

                        //' filter line with a low pass running average over 5 pixels
                        double[] line_filtered = new double[Xc];
                        for (int i = 2; i < line.Length - 3; i++)
                        {
                            double sum = 0;
                            for (int k = i - 2; k < i + 2; k++)
                            {
                                sum = sum + line[k];
                            }
                            line_filtered[i] = sum / 5;
                        }
                        line_filtered[0] = line_filtered[2];
                        line_filtered[1] = line_filtered[2];
                        line_filtered[line.Length - 2] = line_filtered[line.Length - 3];
                        line_filtered[line.Length - 1] = line_filtered[line.Length - 3];

                        //' take first derivative of filtered line
                        double[] line_filtered_der = new double[line.Length];
                        for (int i = 1; i < line.Length - 1; i++)
                        {
                            line_filtered_der[i] = line_filtered[i] - line_filtered[i - 1];
                        }
                        line_filtered_der[0] = line_filtered_der[1];

                        //' search min, max of derivative
                        double max = -255, min = 255, value = 0;
                        for (int i = Width / 10 + 10; i < line_filtered_der.Length - 10 - Width / 10; i++)
                        {

                            value = line_filtered_der[i];
                            if (value > max)
                            {
                                max = value;
                                up_X = i;
                            }
                            if (value < min)
                            {
                                bot_X = i;
                                min = value;
                            }

                        }
                        //' count number of derivative values over max/2 and under min/2
                        SizeBot = 0;
                        SizeUp = 0;
                        for (int i = 10 + Width / 10; i < line_filtered_der.Length - 10 - Width / 10; i++)
                        {
                            value = line_filtered_der[i];
                            if (value > max / 4) SizeUp = SizeUp + 1;
                            if (value < min / 4) SizeBot = SizeBot + 1;
                        }
                        if (Interval == 0)
                        {
                            _Aimer_LEDLine_Width = bot_X - up_X;
                            _SharpnessLEDCross_bot_L_V = SizeBot;
                            _SharpnessLEDCross_up_L_V = SizeUp;
                        }
                        else
                        {
                            _SharpnessLEDCross_bot_R_V = SizeBot;
                            _SharpnessLEDCross_up_R_V = SizeUp;
                        }
                    }
                    #endregion
                }
                catch (Exception err)
                {
                    _ErrorMessage = "CallLedProfile fail" + err.Message;
                    return false;
                }
                return true;
            }

            private bool ImbalanceTest(int Height, int Width, bool UpBottom/*, ref int SizeBot, ref int SizeUp*/)
            {
                try
                {
                    int Yc = 0, Xc = 0;
                    //int SizeBot_sum = 0, SizeUp_sum = 0;
                    int L_X = 0, R_X = 0;
                    Yc = _AimerCenter_Y - _ROI_StartY;
                    //yc=Height /10;
                    Xc = array2d.GetLength(0) - Width / 10;
                    double[] line = new double[Xc];
                    for (int X = Width / 10; X < Xc; X++)
                    {
                        double sum = 0;
                        for (int Y = Yc - 10; Y < Yc + 10; Y++)
                        {
                            //sum = sum + Img[Width * y + x];
                            sum = sum + array2d[X, Y];
                        }
                        line[X] = sum;
                    }

                    //' filter line with a low pass running average over 5 pixels
                    double[] line_filtered = new double[Xc];
                    for (int i = 2; i < line.Length - 3; i++)
                    {
                        double sum = 0;
                        for (int k = i - 2; k < i + 2; k++)
                        {
                            sum = sum + line[k];
                        }
                        line_filtered[i] = sum / 5;
                    }
                    line_filtered[0] = line_filtered[2];
                    line_filtered[1] = line_filtered[2];
                    line_filtered[line.Length - 2] = line_filtered[line.Length - 3];
                    line_filtered[line.Length - 1] = line_filtered[line.Length - 3];

                    //' take first derivative of filtered line 
                    double max = -255, min = 255, value = 0;
                    double[] line_filtered_der = new double[Width];
                    for (int i = 10 + Width / 10; i < line_filtered_der.Length - 10 - Width / 10; i++)
                    {
                        line_filtered_der[i] = line_filtered[i] - line_filtered[i - 1];
                        if (line_filtered_der[i] > max)
                        {
                            max = line_filtered_der[i];
                            L_X = i;
                            _imbalanceL = max;
                        }
                        if (line_filtered_der[i] < min)
                        {
                            R_X = i;
                            min = line_filtered_der[i];
                            _imbalanceR = min;
                        }
                    }
                    //_imbalanceLR = Math.Abs(max - min);

                    if (UpBottom == true)
                    {
                        int xc = 0, yc = 0;
                        //int SizeBot_sum = 0, SizeUp_sum = 0;
                        int up_Y = 0, bot_Y = 0;
                        xc = _AimerCenter_X - _ROI_StartX;
                        //yc=Height /10;
                        yc = array2d.GetLength(1) - Height / 10;
                        line = new double[yc];
                        for (int y = Height / 10; y < yc; y++)
                        {
                            double sum = 0;
                            for (int x = xc - 10; x < xc + 10; x++)
                            {
                                //sum = sum + Img[Width * y + x];
                                sum = sum + array2d[x, y];
                            }
                            line[y] = sum;
                        }

                        //' filter line with a low pass running average over 5 pixels
                        line_filtered = new double[yc];
                        for (int i = 2; i < line.Length - 3; i++)
                        {
                            double sum = 0;
                            for (int k = i - 2; k < i + 2; k++)
                            {
                                sum = sum + line[k];
                            }
                            line_filtered[i] = sum / 5;
                        }
                        line_filtered[0] = line_filtered[2];
                        line_filtered[1] = line_filtered[2];
                        line_filtered[line.Length - 2] = line_filtered[line.Length - 3];
                        line_filtered[line.Length - 1] = line_filtered[line.Length - 3];

                        //' take first derivative of filtered line 
                        max = -255; min = 255; value = 0;
                        line_filtered_der = new double[Height];
                        for (int i = 10 + Height / 10; i < line_filtered_der.Length - 10 - Height / 10; i++)
                        {
                            line_filtered_der[i] = line_filtered[i] - line_filtered[i - 1];
                            if (line_filtered_der[i] > max)
                            {
                                max = line_filtered_der[i];
                                _imbalanceU = max;
                                up_Y = i;
                            }
                            if (line_filtered_der[i] < min)
                            {
                                bot_Y = i;
                                min = line_filtered_der[i];
                                _imbalanceB = min;
                            }
                        }
                        //_imbalanceUB = Math.Abs(max + min);
                    }

                }
                catch (Exception err)
                {
                    _ErrorMessage = "CallLedProfile fail" + err.Message;
                    return false;
                }

                return true;
            }


            #endregion
            #region 1200 alignment: measure line
            private bool FindLeftAndRightOfLine(Byte[,] Array2d_roi, ref int Left_x, ref int Left_y, ref int Right_x, ref int Right_y)
            {
                try
                {
                    Int32 i = 0, j = 0, x = 0, y = 0;
                    int PixelValue_current = 0, PixelValue_before = 0, pixelvalue_temp = 0, slope_first = 0, slope_second = 0;
                    bool slope_step = false;
                    double slope_left = 0, slope_leftMax = 0, slope_right = 0, slope_rightMin = 0;
                    for (x = 20; x < Array2d_roi.GetLength(0) - 20; x++)
                    {
                        for (y = 20; y < Array2d_roi.GetLength(1) - 20; y++)
                        {
                            PixelValue_current = Array2d_roi[x, y];
                            if (PixelValue_current > _RedThreshold)
                            {
                                //20x20 pixels, average value
                                if (slope_step == false)
                                {
                                    slope_first = y;
                                    slope_step = true;
                                }
                                else
                                {
                                    slope_second = y;
                                }
                                PixelValue_before = pixelvalue_temp;//save before value.
                                pixelvalue_temp = 0;//init pixel vale
                                for (i = 0; i < 20; i++)
                                {
                                    for (j = 0; j < 20; j++)
                                    {
                                        pixelvalue_temp = pixelvalue_temp + Array2d_roi[x + i, y + i];
                                    }
                                }
                                if (slope_second - slope_first > 5)
                                {
                                    slope_step = false;

                                }
                                pixelvalue_temp = pixelvalue_temp / 400;
                                if (PixelValue_before != 0)
                                {
                                    slope_left = pixelvalue_temp / PixelValue_before;// find left 
                                    slope_right = pixelvalue_temp / PixelValue_before;
                                }
                                else
                                {
                                    slope_left = 0;
                                    slope_right = 0;
                                }
                                if (slope_left > 0 && slope_left > slope_leftMax)
                                {
                                    slope_leftMax = slope_left;
                                    Left_x = x;
                                    Left_y = y;
                                }
                                if (slope_right < 0 && slope_right < slope_rightMin)
                                {
                                    slope_rightMin = slope_right;
                                    Right_x = x;
                                    Right_x = y;
                                }
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return false;
                }
                return true;
            }

            public bool MeasureLaserLine(Bitmap b, int ROI_PointX, int ROI_PointY, int ROI_Width, int ROI_Height)
            {
                int Left_X = 0, Left_Y = 0, Right_X = 0, Right_Y = 0;
                int line = 0;
                BmpToArray2d(b, ROI_PointX, ROI_PointY, ROI_Width, ROI_Height);
                FindLeftAndRightOfLine(array2d, ref Left_X, ref Left_Y, ref Right_X, ref Right_Y);
                line = Convert.ToInt32(Math.Sqrt(Math.Pow((Right_X - Left_X), 2) + Math.Pow((Right_Y - Left_Y), 2)));
                //1166,306,0,0
                return true;
            }
            #endregion
            #region SLIM Sharpness

            #endregion
            #region Gen8

            public Bitmap AimerCenter_Gen8(Bitmap b)
            {
                _ErrorMessage = "";
                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;

                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                AutoThreshold = 0;
                _ErrorMessage = "";
                array1d = new byte[b.Width * b.Height];
                BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                int X_position = 0, Y_Position = 0, m = 0;
                for (int x = 0; x < ROI_Width; x++)
                {
                    for (int y = 0; y < ROI_Height; y++)
                    {
                        if (array2d[x, y] > AutoThreshold && x > 4 && y > 4 && x < ROI_Width - 4 && y < ROI_Height - 4)
                        {

                            if (array2d[x, y - 3] > AutoThreshold && array2d[x - 3, y - 3] > AutoThreshold && array2d[x + 3, y - 3] > AutoThreshold
                                && array2d[x - 3, y] > AutoThreshold && array2d[x + 3, y] > AutoThreshold
                                && array2d[x, y + 3] > AutoThreshold && array2d[x - 3, y + 3] > AutoThreshold && array2d[x + 3, y + 3] > AutoThreshold)
                            {
                                X_position = X_position + x;
                                Y_Position = Y_Position + y;
                                m++;
                            }
                            else
                                m = m;
                            if (x > 650)
                                m = m;
                        }
                    }
                }
                if (m > 0)
                {
                    _AimerCenter_X = X_position / m + _ROI_StartX;
                    _AimerCenter_Y = Y_Position / m + _ROI_StartY;
                }
                else
                {
                    _AimerCenter_X = 0;
                    _AimerCenter_Y = 0;
                }

                //   if (FindCenterTemp_LEDLine_Gen8(array2d) == false) return b;

                return b;
            }


            private bool FindCenterTemp_LEDLine_Gen8(Byte[,] Array2d_roi)
            {
                try
                {
                    _Edge_BotY = 0;
                    _Edge_UpY = Array2d_roi.GetLength(1);
                    _Edge_RightX = 0;
                    _Edge_LeftX = Array2d_roi.GetLength(0);
                    int temp_X_center = 0;
                    int temp_Y_center = 0;

                    int i = 0, j = 0, x = 0, y = 0;
                    int PixelValue = 0, pixelvalue_temp = 0;
                    double Ratio_temp = 0;
                    int up_Y = 0, yc = 0, bot_Y = 0;
                    #region Get CenterTemp_y
                    int Height = array2d.GetLength(1);
                    int xc = array2d.GetLength(0) / 2;
                    yc = array2d.GetLength(1) - Height / 10;
                    //yc = Height / 3 * 2;
                    double[] line = new double[yc];
                    for (y = Height / 10; y < yc; y++)
                    {
                        double sum = 0;
                        for (x = xc - 10; x < xc + 10; x++)
                        {
                            //sum = sum + Img[Width * y + x] ;
                            sum = sum + array2d[x, y];
                        }
                        line[y] = sum;
                    }

                    //' filter line with a low pass running average over 5 pixels
                    double[] line_filtered = new double[yc];
                    for (i = 2; i < line.Length - 3; i++)
                    {
                        double sum = 0;
                        for (int k = i - 2; k < i + 3; k++)
                        {
                            sum = sum + line[k];
                        }
                        line_filtered[i] = sum / 5;
                    }
                    line_filtered[0] = line_filtered[2];
                    line_filtered[1] = line_filtered[2];
                    line_filtered[line.Length - 2] = line_filtered[line.Length - 3];
                    line_filtered[line.Length - 1] = line_filtered[line.Length - 3];

                    //' take first derivative of filtered line
                    double[] line_filtered_der = new double[line.Length];
                    for (i = 1; i < line.Length - 1; i++)
                    {
                        line_filtered_der[i] = line_filtered[i] - line_filtered[i - 1];
                    }
                    line_filtered_der[0] = line_filtered_der[1];

                    //' search min, max of derivative
                    double max = -255, min = 255, value = 0;
                    for (i = 10 + Height / 10; i < line_filtered_der.Length - 10 - Height / 10; i++)
                    {

                        value = line_filtered_der[i];
                        if (value > max)
                        {
                            max = value;
                            up_Y = i;
                        }
                        if (value < min)
                        {
                            bot_Y = i;
                            min = value;
                        }

                    }
                    _AimerCenter_Y = up_Y + (bot_Y - up_Y) / 2;
                    #endregion
                    #region Get Left and Right  X_point
                    int left_x = array2d.GetLength(0), right_x = 0, left_y = 0, right_y = 0;
                    for (x = 50; x < Array2d_roi.GetLength(0) - 50; x++)
                    {
                        for (y = _AimerCenter_Y - 50; y < _AimerCenter_Y + 50; y++)
                        {
                            PixelValue = Array2d_roi[x, y];
                            if (PixelValue > AutoThreshold)
                            {
                                pixelvalue_temp = 0;
                                for (i = 0; i < 3; i++)
                                {
                                    for (j = 0; j < 3; j++)
                                    {
                                        pixelvalue_temp = pixelvalue_temp + Array2d_roi[x + i, y + j];
                                    }
                                }
                                Ratio_temp = pixelvalue_temp / 9;
                                if (Ratio_temp > AutoThreshold)
                                {
                                    if (x > right_x) { right_x = x; }
                                    if (x < left_x) { left_x = x; }
                                }

                            }
                        }
                    }
                    _AimerLeft_X = left_x;
                    _AimerRight_X = right_x;
                    #endregion
                    #region Get Left and Right  Y_point
                    int LeftCenterTemp_y = 0, RightCenterTemp_y = 0;
                    int ycount = 0, y_value = 0;
                    int max_y = 0, min_y = 0, left_height = 0, right_height = 0;
                    right_y = 0;
                    left_y = 0;
                    left_height = 0; right_height = 0;
                    for (int leftRight = 0; leftRight < 2; leftRight++)
                    {
                        ycount = 0;
                        y_value = 0;

                        max_y = 0; min_y = Array2d_roi.GetLength(1) - 50;
                        for (y = 50; y < Array2d_roi.GetLength(1) - 50; y++)
                        {
                            pixelvalue_temp = 0;
                            Ratio_temp = 0;
                            for (i = 0; i < 3; i++)
                            {
                                for (j = 0; j < 3; j++)
                                {
                                    switch (leftRight)
                                    {
                                        case 0:
                                            pixelvalue_temp = pixelvalue_temp + Array2d_roi[left_x + i, y + j];
                                            break;
                                        case 1:

                                            pixelvalue_temp = pixelvalue_temp + Array2d_roi[right_x + i, y + j];
                                            break;
                                    }
                                }
                            }
                            Ratio_temp = pixelvalue_temp / 9;
                            if (Ratio_temp > AutoThreshold / 3)
                            {
                                y_value = y_value + y;
                                if (y > max_y)
                                    max_y = y;
                                if (y < min_y)
                                    min_y = y;
                                ycount++;
                            }
                        }
                        switch (leftRight)
                        {
                            case 0:
                                left_height = max_y - min_y;
                                left_y = LeftCenterTemp_y = y_value / ycount;
                                break;
                            case 1:
                                right_y = RightCenterTemp_y = y_value / ycount;
                                right_height = max_y - min_y;
                                break;
                        }
                    }
                    _AimerLeft_Y = left_y + 1;
                    _AimerRight_Y = right_y + 1;
                    _AimerLeft_Height = left_height;
                    _AimerRight_Height = right_height;
                    #endregion
                    #region get rotation_deg
                    if (left_x != right_x)
                    {
                        _Rotation_Deg = System.Math.Atan2((RightCenterTemp_y - LeftCenterTemp_y), (right_x - left_x)) * 180 / System.Math.PI;
                    }
                    else
                    {
                        _Rotation_Deg = 99999;
                        _ErrorMessage = "Can't calculate Rotation_Deg ";
                        return false;
                    }

                    if (right_height == 0)
                    {
                        _ErrorMessage = "Can't found left edge";
                        return false;
                    }
                    if (left_height == 0)
                    {
                        _ErrorMessage = "Can't found right edge.";
                        return false;
                    }
                    #endregion
                    #region get center XY
                    int temp_y_height = (right_height + left_height) / 2;
                    int x_count = 0;
                    temp_X_center = 0; temp_Y_center = 0;
                    for (x = left_x; x < right_x; x++)
                    {
                        max_y = 0; min_y = 800;

                        for (y = right_y - 50; y < right_y + 50; y++)
                        {
                            pixelvalue_temp = 0;
                            if (x == 983 && y == 411)
                                temp_X_center = temp_X_center;
                            for (int k = 0; k < 3; k++)
                            {
                                pixelvalue_temp = pixelvalue_temp + Array2d_roi[x, y + j];
                            }
                            pixelvalue_temp = pixelvalue_temp / 3;
                            if (pixelvalue_temp > AutoThreshold)
                            {
                                if (y > max_y) max_y = y;
                                if (y < min_y) min_y = y;
                            }
                        }
                        if (max_y - min_y > 2 * temp_y_height)
                        {
                            temp_X_center = temp_X_center + x;
                            temp_Y_center = temp_Y_center + y;
                            x_count++;
                        }
                    }
                    _AimerCenter_X = temp_X_center = (temp_X_center / x_count);
                    _AimerCenter_Y = temp_Y_center = (temp_Y_center / x_count) - 50 + 1;

                    #endregion
                }
                catch (Exception err)
                {
                    _ErrorMessage = "Find center fail." + err.Message;
                    return false;
                }
                return true;
            }
            private bool FindCenterTemp_LEDLine_Gen8_VLDADJ(Byte[,] Array2d_roi)
            {
                try
                {
                    _Edge_BotY = 0;
                    _Edge_UpY = Array2d_roi.GetLength(1);
                    _Edge_RightX = 0;
                    _Edge_LeftX = Array2d_roi.GetLength(0);
                    int temp_X_center = 0;
                    int temp_Y_center = 0;

                    int i = 0, j = 0, x = 0, y = 0;
                    int PixelValue = 0, pixelvalue_temp = 0;
                    double Ratio_temp = 0;
                    int up_Y = 0, yc = 0, bot_Y = 0;
                    #region Get CenterTemp_y
                    int Height = array2d.GetLength(1);
                    int xc = array2d.GetLength(0) / 2;
                    yc = array2d.GetLength(1) - Height / 10;
                    //yc = Height / 3 * 2;
                    double[] line = new double[yc];
                    for (y = Height / 10; y < yc; y++)
                    {
                        double sum = 0;
                        for (x = xc - 10; x < xc + 10; x++)
                        {
                            //sum = sum + Img[Width * y + x] ;
                            sum = sum + array2d[x, y];
                        }
                        line[y] = sum;
                    }

                    //' filter line with a low pass running average over 5 pixels
                    double[] line_filtered = new double[yc];
                    for (i = 2; i < line.Length - 3; i++)
                    {
                        double sum = 0;
                        for (int k = i - 2; k < i + 3; k++)
                        {
                            sum = sum + line[k];
                        }
                        line_filtered[i] = sum / 5;
                    }
                    line_filtered[0] = line_filtered[2];
                    line_filtered[1] = line_filtered[2];
                    line_filtered[line.Length - 2] = line_filtered[line.Length - 3];
                    line_filtered[line.Length - 1] = line_filtered[line.Length - 3];

                    //' take first derivative of filtered line
                    double[] line_filtered_der = new double[line.Length];
                    for (i = 1; i < line.Length - 1; i++)
                    {
                        line_filtered_der[i] = line_filtered[i] - line_filtered[i - 1];
                    }
                    line_filtered_der[0] = line_filtered_der[1];

                    //' search min, max of derivative
                    double max = -255, min = 255, value = 0;
                    for (i = 10 + Height / 10; i < line_filtered_der.Length - 10 - Height / 10; i++)
                    {

                        value = line_filtered_der[i];
                        if (value > max)
                        {
                            max = value;
                            up_Y = i;
                        }
                        if (value < min)
                        {
                            bot_Y = i;
                            min = value;
                        }

                    }
                    _AimerCenter_Y = up_Y + (bot_Y - up_Y) / 2;
                    #endregion
                    #region Get Left and Right  X_point
                    int left_x = array2d.GetLength(0), right_x = 0, left_y = 0, right_y = 0;
                    for (x = 50; x < Array2d_roi.GetLength(0) - 50; x++)
                    {
                        for (y = _AimerCenter_Y - 50; y < _AimerCenter_Y + 50; y++)
                        {
                            PixelValue = Array2d_roi[x, y];
                            if (PixelValue > AutoThreshold)
                            {
                                pixelvalue_temp = 0;
                                for (i = 0; i < 3; i++)
                                {
                                    for (j = 0; j < 3; j++)
                                    {
                                        pixelvalue_temp = pixelvalue_temp + Array2d_roi[x + i, y + j];
                                    }
                                }
                                Ratio_temp = pixelvalue_temp / 9;
                                if (Ratio_temp > AutoThreshold)
                                {
                                    if (x > right_x) { right_x = x; }
                                    if (x < left_x) { left_x = x; }
                                }

                            }
                        }
                    }
                    _AimerLeft_X = left_x;
                    _AimerRight_X = right_x;
                    #endregion
                    #region Get Left and Right  Y_point
                    int LeftCenterTemp_y = 0, RightCenterTemp_y = 0;
                    int ycount = 0, y_value = 0;
                    int max_y = 0, min_y = 0, left_height = 0, right_height = 0;
                    right_y = 0;
                    left_y = 0;
                    left_height = 0; right_height = 0;
                    for (int leftRight = 0; leftRight < 2; leftRight++)
                    {
                        ycount = 0;
                        y_value = 0;

                        max_y = 0; min_y = Array2d_roi.GetLength(1) - 50;
                        for (y = 50; y < Array2d_roi.GetLength(1) - 50; y++)
                        {
                            pixelvalue_temp = 0;
                            Ratio_temp = 0;
                            for (i = 0; i < 3; i++)
                            {
                                for (j = 0; j < 3; j++)
                                {
                                    switch (leftRight)
                                    {
                                        case 0:
                                            pixelvalue_temp = pixelvalue_temp + Array2d_roi[left_x + i, y + j];
                                            break;
                                        case 1:

                                            pixelvalue_temp = pixelvalue_temp + Array2d_roi[right_x + i, y + j];
                                            break;
                                    }
                                }
                            }
                            Ratio_temp = pixelvalue_temp / 9;
                            if (Ratio_temp > AutoThreshold / 3)
                            {
                                y_value = y_value + y;
                                if (y > max_y)
                                    max_y = y;
                                if (y < min_y)
                                    min_y = y;
                                ycount++;
                            }
                        }
                        switch (leftRight)
                        {
                            case 0:
                                left_height = max_y - min_y;
                                left_y = LeftCenterTemp_y = y_value / ycount;
                                break;
                            case 1:
                                right_y = RightCenterTemp_y = y_value / ycount;
                                right_height = max_y - min_y;
                                break;
                        }
                    }
                    _AimerLeft_Y = left_y + 1;
                    _AimerRight_Y = right_y + 1;
                    _AimerLeft_Height = left_height;
                    _AimerRight_Height = right_height;
                    #endregion
                    #region get rotation_deg
                    if (left_x != right_x)
                    {
                        _Rotation_Deg = System.Math.Atan2((RightCenterTemp_y - LeftCenterTemp_y), (right_x - left_x)) * 180 / System.Math.PI;
                    }
                    else
                    {
                        _Rotation_Deg = 99999;
                        _ErrorMessage = "Can't calculate Rotation_Deg ";
                        return false;
                    }

                    if (right_height == 0)
                    {
                        _ErrorMessage = "Can't found left edge";
                        return false;
                    }
                    if (left_height == 0)
                    {
                        _ErrorMessage = "Can't found right edge.";
                        return false;
                    }
                    #endregion
                    #region get center XY
                    int temp_y_height = (right_height + left_height) / 2;
                    int x_count = 0;
                    temp_X_center = 0; temp_Y_center = 0;
                    for (x = left_x; x < right_x; x++)
                    {
                        max_y = 0; min_y = 800;

                        for (y = right_y - 50; y < right_y + 50; y++)
                        {
                            pixelvalue_temp = 0;
                            for (int k = 0; k < 3; k++)
                            {
                                pixelvalue_temp = pixelvalue_temp + Array2d_roi[x, y + j];
                            }
                            pixelvalue_temp = pixelvalue_temp / 3;
                            if (pixelvalue_temp > AutoThreshold)
                            {
                                if (y > max_y) max_y = y;
                                if (y < min_y) min_y = y;
                            }
                        }
                        if (max_y - min_y > 2 * temp_y_height)
                        {
                            temp_X_center = temp_X_center + x;
                            temp_Y_center = temp_Y_center + y;
                            x_count++;
                        }
                    }
                    _AimerCenter_X = temp_X_center = (temp_X_center / x_count);
                    _AimerCenter_Y = temp_Y_center = (temp_Y_center / x_count) - 50 + 1;

                    #endregion
                    if (_AimerCenter_X < left_x || _AimerCenter_X > right_x) return false;
                    #region Get Left and Right  Y_point
                    LeftCenterTemp_y = 0; RightCenterTemp_y = 0;
                    ycount = 0; y_value = 0;
                    max_y = 0; min_y = 0; left_height = 0; right_height = 0;
                    right_y = 0;
                    left_y = 0;
                    left_height = 0; right_height = 0;
                    _AimerLeft_X = left_x = left_x + (_AimerCenter_X - left_x) / 2;
                    _AimerRight_X = right_x = right_x - (right_x - _AimerCenter_X) / 2;
                    for (int leftRight = 0; leftRight < 2; leftRight++)
                    {
                        ycount = 0;
                        y_value = 0;

                        max_y = 0; min_y = Array2d_roi.GetLength(1) - 50;
                        for (y = 50; y < Array2d_roi.GetLength(1) - 50; y++)
                        {
                            pixelvalue_temp = 0;
                            Ratio_temp = 0;
                            for (i = 0; i < 3; i++)
                            {
                                for (j = 0; j < 3; j++)
                                {
                                    switch (leftRight)
                                    {
                                        case 0:
                                            pixelvalue_temp = pixelvalue_temp + Array2d_roi[left_x + i, y + j];
                                            break;
                                        case 1:

                                            pixelvalue_temp = pixelvalue_temp + Array2d_roi[right_x + i, y + j];
                                            break;
                                    }
                                }
                            }
                            Ratio_temp = pixelvalue_temp / 9;
                            if (Ratio_temp > AutoThreshold / 3)
                            {
                                y_value = y_value + y;
                                if (y > max_y)
                                    max_y = y;
                                if (y < min_y)
                                    min_y = y;
                                ycount++;
                            }
                        }
                        switch (leftRight)
                        {
                            case 0:
                                left_height = max_y - min_y;
                                left_y = LeftCenterTemp_y = y_value / ycount;
                                break;
                            case 1:
                                right_y = RightCenterTemp_y = y_value / ycount;
                                right_height = max_y - min_y;
                                break;
                        }
                    }
                    _AimerLeft_Y = left_y + 1;
                    _AimerRight_Y = right_y + 1;
                    _AimerLeft_Height = left_height;
                    _AimerRight_Height = right_height;
                    #endregion
                    #region get rotation_deg
                    if (left_x != right_x)
                    {
                        _Rotation_Deg = System.Math.Atan2((RightCenterTemp_y - LeftCenterTemp_y), (right_x - left_x)) * 180 / System.Math.PI;
                    }
                    else
                    {
                        _Rotation_Deg = 99999;
                        _ErrorMessage = "Can't calculate Rotation_Deg ";
                        return false;
                    }

                    if (right_height == 0)
                    {
                        _ErrorMessage = "Can't found left edge";
                        return false;
                    }
                    if (left_height == 0)
                    {
                        _ErrorMessage = "Can't found right edge.";
                        return false;
                    }
                    #endregion
                }
                catch (Exception err)
                {
                    _ErrorMessage = "Find center fail." + err.Message;
                    return false;
                }
                return true;
            }

            public Bitmap AimerCenter_LEDLine_Gen8(Bitmap b)
            {
                _ErrorMessage = "";
                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                int center_X = 0, center_Y = 0;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                _Aimer_LEDLine_Width = 0; _AimerCenter_X = 0; _AimerCenter_Y = 0; _AimerLeft_Height = 0; _AimerLeft_X = 0;
                _AimerLeft_Y = 0; _AimerRight_X = 0; _AimerRight_Y = 0; _AimerRight_Height = 0; AutoThreshold = 0;
                _ErrorMessage = "";
                array1d = new byte[b.Width * b.Height];
                BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                if (FindCenterTemp_LEDLine_Gen8(array2d) == false) return b;

                return b;
            }

            public Bitmap AimerCenter_LEDLine_Gen8VLDAdj(Bitmap b)
            {
                _ErrorMessage = "";
                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                int center_X = 0, center_Y = 0;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                _Aimer_LEDLine_Width = 0; _AimerCenter_X = 0; _AimerCenter_Y = 0; _AimerLeft_Height = 0; _AimerLeft_X = 0;
                _AimerLeft_Y = 0; _AimerRight_X = 0; _AimerRight_Y = 0; _AimerRight_Height = 0; AutoThreshold = 0;
                _ErrorMessage = "";
                array1d = new byte[b.Width * b.Height];
                BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                if (FindCenterTemp_LEDLine_Gen8_VLDADJ(array2d) == false) return b;

                return b;
            }
            private Bitmap BmpToArray2d_Gen8(Bitmap b, int ROI_PointX, int ROI_PointY, int ROI_Width, int ROI_Height)
            {
                PixelFormat bitmaptype = b.PixelFormat;// PixelFormat.Format8bppIndexed;
                _MaxPixel = 0; _MinPixel = 255;
                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, bitmaptype);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                if (ROI_Width >= b.Width) ROI_Width = b.Width - 5;
                if (ROI_Height >= b.Height) ROI_Height = b.Height - 5;
                unsafe
                {
                    int nPixelBytes = 1;
                    switch (bitmaptype)
                    {
                        case PixelFormat.Format8bppIndexed:
                            nPixelBytes = 8 >> 3;
                            break;
                        case PixelFormat.Format24bppRgb:
                            nPixelBytes = 24 >> 3;
                            break;
                        case PixelFormat.Format32bppRgb:
                            nPixelBytes = 32 >> 3;
                            break;
                        case PixelFormat.Format32bppArgb:
                            nPixelBytes = 32 >> 3;
                            break;
                        default:
                            break;
                    }
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - b.Width * nPixelBytes;
                    byte red, green, blue, color = 0;
                    array2d = new byte[b.Width - ROI_StartX - ROI_EndX, b.Height - ROI_EndY - ROI_StartY];

                    for (int y = ROI_PointY; y < ROI_PointY + ROI_Height; y++)//
                    {
                        if (y == ROI_PointY)
                        {
                            p += ((ROI_PointY * b.Width + ROI_PointX) * nPixelBytes);
                        }
                        else
                        {
                            p += (ROI_PointX * nPixelBytes);
                        }
                        for (int x = ROI_PointX; x < ROI_PointX + ROI_Width; x++)
                        {
                            color = 0;
                            for (int k = 0; k < nPixelBytes; k++)
                            {
                                //*(ptr + nDesOffset + k) = bytes[nSrcOffset + k];
                                switch (nPixelBytes)
                                {
                                    case 1:
                                        color = (byte)p[k];
                                        break;
                                    case 3:
                                        color = (byte)(color + p[k]);
                                        if (k == nPixelBytes - 1)
                                            color = (byte)(color / 3);
                                        break;
                                    case 4:
                                        color = (byte)(color + p[k]);
                                        if (k == nPixelBytes - 1)
                                            color = (byte)(color / 3);
                                        break;
                                    default:
                                        break;
                                }
                            }

                            if (color > _MaxPixel && color != 255)
                            {
                                _MaxPixel = color;
                            }
                            if (color < _MinPixel)
                                _MinPixel = color;
                            array2d[x - ROI_PointX, y - ROI_PointY] = color;
                            array1d[(x - ROI_PointX) + (y - ROI_PointY) * b.Width] = color;

                            if (x == ROI_PointX + ROI_Width - 1)
                            {
                                p = p + (b.Width - (ROI_PointX + ROI_Width - 1)) * nPixelBytes;
                            }
                            else
                            {
                                p = p + nPixelBytes;
                            }
                        }
                        p += nOffset;
                    }
                    AutoThreshold = Convert.ToInt32(Math.Abs((_MaxPixel - _MinPixel) * ThresholdFactor + _MinPixel));
                }
                b.UnlockBits(bmData);

                return b;
            }
            private void FindFWHMCenter_Gen8()
            {
                int width = array2d.GetLength(0);
                int height = array2d.GetLength(1);
                int tempColor_16X16 = 0;
                int center_X_temp = 0, center_Y_temp = 0, count = 0;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (array2d[x, y] > AutoThreshold && x < width - 20 && y < height - 20)
                        {
                            tempColor_16X16 = 0;
                            for (int m = 0; m < 16; m++)
                            {
                                for (int n = 0; n < 16; n++)
                                {
                                    tempColor_16X16 = tempColor_16X16 + array2d[x, y];
                                }
                            }
                            tempColor_16X16 = tempColor_16X16 / 16 / 16;
                            if (tempColor_16X16 > AutoThreshold)
                            {
                                count++;
                                center_X_temp = center_X_temp + x;
                                center_Y_temp = center_Y_temp + y;
                            }
                        }
                    }
                }

                _AimerCenter_X = center_X_temp / count;
                _AimerCenter_Y = center_Y_temp / count;
                return;
            }
            private int CopyDataToArra(ref double leftColor_16X16, ref double rightColor_16X16, ref  double UpColor_16X16, ref  double BotColor_16X16, ref  double CentreColor_16X16)
            {
                int width = array2d.GetLength(0);
                int height = array2d.GetLength(1);
                int L_Count = 0, R_Count = 0, U_Count = 0, B_Count = 0, M_Count = 0, step = 8;

                if (_AimerCenter_X < _ABSPosition_X || _AimerCenter_X + _ABSPosition_X > width) return -1;
                if (_AimerCenter_Y < _ABSPosition_Y || _AimerCenter_Y + _ABSPosition_Y > width) return -2;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (y == _AimerCenter_Y)
                        {
                            array1d_X[x] = array2d[x, y];
                        }
                        if (x == _AimerCenter_X)
                        {
                            array1d_Y[y] = array2d[x, y];
                        }
                        if (x > (_AimerCenter_X - _ABSPosition_X - step) && x < (_AimerCenter_X - _ABSPosition_X + step) && y > (_AimerCenter_Y - step) && y < (_AimerCenter_Y + step))
                        {
                            leftColor_16X16 = leftColor_16X16 + array2d[x, y];
                            L_Count++;
                        }
                        if (x > (_AimerCenter_X + _ABSPosition_X - step) && x < (_AimerCenter_X + _ABSPosition_X + step) && y > (_AimerCenter_Y - step) && y < (_AimerCenter_Y + step))
                        {
                            rightColor_16X16 = rightColor_16X16 + array2d[x, y];
                            R_Count++;
                        }
                        if (y > (_AimerCenter_Y - _ABSPosition_Y - step) && y < (_AimerCenter_Y - _ABSPosition_Y + step) && x > (_AimerCenter_X - step) && x < (_AimerCenter_X + step))
                        {
                            UpColor_16X16 = UpColor_16X16 + array2d[x, y];
                            U_Count++;
                        }
                        if (y > (_AimerCenter_Y + _ABSPosition_Y - step) && y < (_AimerCenter_Y + _ABSPosition_Y + step) && x > (_AimerCenter_X - step) && x < (_AimerCenter_X + step))
                        {
                            BotColor_16X16 = BotColor_16X16 + array2d[x, y];
                            B_Count++;
                        }
                        if (y > (_AimerCenter_Y - step) && y < (_AimerCenter_Y + step) && x > (_AimerCenter_X - step) && x < (_AimerCenter_X + step))
                        {
                            CentreColor_16X16 = CentreColor_16X16 + array2d[x, y];
                            M_Count++;
                        }
                    }

                }
                if (L_Count != 225)
                    leftColor_16X16 = 0;
                else
                    leftColor_16X16 = leftColor_16X16 / L_Count;
                if (R_Count != 225)
                    rightColor_16X16 = 0;
                else
                    rightColor_16X16 = rightColor_16X16 / R_Count;
                if (U_Count != 225)
                    UpColor_16X16 = 0;
                else
                    UpColor_16X16 = UpColor_16X16 / U_Count;
                if (B_Count != 225)
                    BotColor_16X16 = 0;
                else
                    BotColor_16X16 = BotColor_16X16 / B_Count;
                if (M_Count != 225)
                    CentreColor_16X16 = 0;
                else
                    CentreColor_16X16 = CentreColor_16X16 / B_Count;
                return 0;
            }
            private int CopyDataToArra_Near(ref double leftColor_16X16, ref double rightColor_16X16, ref  double UpColor_16X16, ref  double BotColor_16X16, ref  double CentreColor_16X16)
            {
                int i = 0, j = 0, step = 16;
                leftColor_16X16 = 0;
                rightColor_16X16 = 0;
                UpColor_16X16 = 0;
                BotColor_16X16 = 0;
                CentreColor_16X16 = 0;
                for (i = 0; i < step; i++)
                {
                    for (j = 0; j < step; j++)
                    {
                        leftColor_16X16 = leftColor_16X16 + array2d[i, j + 392];
                    }
                }
                leftColor_16X16 = leftColor_16X16 / 256;
                for (i = 0; i < step; i++)
                {
                    for (j = 0; j < step; j++)
                    {
                        rightColor_16X16 = rightColor_16X16 + array2d[1904 + i, j + 392];
                    }
                }
                rightColor_16X16 = rightColor_16X16 / 256;
                for (i = 0; i < step; i++)
                {
                    for (j = 0; j < step; j++)
                    {
                        UpColor_16X16 = UpColor_16X16 + array2d[952 + i, j];
                    }
                }
                UpColor_16X16 = UpColor_16X16 / 256;
                for (i = 0; i < step; i++)
                {
                    for (j = 0; j < step; j++)
                    {
                        BotColor_16X16 = BotColor_16X16 + array2d[952 + i, j + 784];
                    }
                }
                BotColor_16X16 = BotColor_16X16 / 256;
                //find the brightest point
                double MaxValue = 0;
                for (int k = 0; k < array2d.GetLength(0); k++)
                {
                    for (i = 0; i < step; i++)
                    {
                        for (j = 0; j < step; j++)
                        {
                            if (k + i > array2d.GetLength(0) - 1) continue;
                            CentreColor_16X16 = CentreColor_16X16 + array2d[k + i, j + 392];
                        }
                    }
                    if (CentreColor_16X16 > MaxValue)
                        MaxValue = CentreColor_16X16;
                    CentreColor_16X16 = 0;
                }
                CentreColor_16X16 = MaxValue / 256;
                return 0;
            }
            private int CopyDataToArra_Far(ref double leftColor_16X16, ref double rightColor_16X16, ref  double UpColor_16X16, ref  double BotColor_16X16, ref  double CentreColor_16X16)
            {
                int i = 0, j = 0, step = 16;
                leftColor_16X16 = 0;
                rightColor_16X16 = 0;
                UpColor_16X16 = 0;
                BotColor_16X16 = 0;
                CentreColor_16X16 = 0;
                for (i = 0; i < step; i++)
                {
                    for (j = 0; j < step; j++)
                    {
                        leftColor_16X16 = leftColor_16X16 + array2d[i, j + 392];
                    }
                }
                leftColor_16X16 = leftColor_16X16 / 256;
                for (i = 0; i < step; i++)
                {
                    for (j = 0; j < step; j++)
                    {
                        rightColor_16X16 = rightColor_16X16 + array2d[1264 + i, j + 392];
                    }
                }
                rightColor_16X16 = rightColor_16X16 / 256;
                for (i = 0; i < step; i++)
                {
                    for (j = 0; j < step; j++)
                    {
                        UpColor_16X16 = UpColor_16X16 + array2d[632 + i, j];
                    }
                }
                UpColor_16X16 = UpColor_16X16 / 256;
                for (i = 0; i < step; i++)
                {
                    for (j = 0; j < step; j++)
                    {
                        BotColor_16X16 = BotColor_16X16 + array2d[632 + i, j + 784];
                    }
                }
                BotColor_16X16 = BotColor_16X16 / 256;

                double MaxValue = 0;
                for (int k = 0; k < array2d.GetLength(0); k++)
                {
                    for (i = 0; i < step; i++)
                    {
                        for (j = 0; j < step; j++)
                        {
                            if (k + i > array2d.GetLength(0) - 1) continue;
                            CentreColor_16X16 = CentreColor_16X16 + array2d[k + i, j + 392];
                        }
                    }
                    if (CentreColor_16X16 > MaxValue)
                        MaxValue = CentreColor_16X16;
                    CentreColor_16X16 = 0;
                }
                CentreColor_16X16 = MaxValue / 256;
                return 0;
            }

            public int CalculateFWHM(Bitmap b, string NearFar, ref double left, ref double right, ref  double Up, ref  double Bot, ref double center)
            {
                //int c=0;
                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                _ErrorMessage = "";
                array1d = new byte[b.Width * b.Height];
                array1d_X = new double[b.Width];
                array1d_Y = new double[b.Height];
                BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                //FindFWHMCenter_Gen8();
                left = 0; right = 0; Up = 0; Bot = 0;
                if (NearFar == "Near")
                    CopyDataToArra_Near(ref left, ref right, ref  Up, ref  Bot, ref center);
                else
                    CopyDataToArra_Far(ref left, ref right, ref  Up, ref  Bot, ref center);
                return 0;
            }

            public void SearchCorner(Bitmap b, ref int[,] Corner)
            {
                //int c=0;
                int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                _ErrorMessage = "";
                array1d = new byte[b.Width * b.Height];
                array1d_X = new double[b.Width];
                array1d_Y = new double[b.Height];
                BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                FindFWHMCenter_Gen8();
                int width = array2d.GetLength(0);
                int height = array2d.GetLength(1);
                int[] width_position = new int[width];
                int[] Height_position = new int[width];
                int m = 0, n = 0;
                #region LeftUpCorner
                //slope
                for (int k = 0; k < 4; k++)
                {
                    m = 0; n = 0;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            double slope = 0;
                            switch (k)
                            {
                                case 0:
                                    Corner[k, 0] = width;
                                    Corner[k, 1] = height;
                                    x1 = x + 2;
                                    y1 = y;
                                    x2 = x;
                                    y2 = y;
                                    //slope = array2d[x1, y1] - array2d[x2, y2];
                                    break;
                                case 1:
                                    Corner[k, 0] = 0;
                                    Corner[k, 1] = height;
                                    x1 = x + 2 + _AimerCenter_X;
                                    y1 = y;
                                    x2 = x + _AimerCenter_X;
                                    y2 = y;
                                    //slope = array2d[x + 2 + _AimerCenter_X, y] - array2d[x + _AimerCenter_X , y];
                                    break;
                                case 2:
                                    Corner[k, 0] = width;
                                    Corner[k, 1] = 0;
                                    x1 = x + 2;
                                    y1 = y + _AimerCenter_Y;
                                    x2 = x;
                                    y2 = y + _AimerCenter_Y;
                                    //slope = array2d[x + 2, y + _AimerCenter_Y] - array2d[x , y + _AimerCenter_Y];
                                    break;
                                case 3:
                                    Corner[k, 0] = 0;
                                    Corner[k, 1] = 0;
                                    x1 = x + 2 + _AimerCenter_X;
                                    y1 = y + _AimerCenter_Y;
                                    x2 = x + _AimerCenter_X;
                                    y2 = y + _AimerCenter_Y;
                                    //slope = array2d[x + 2 + _AimerCenter_X, y + _AimerCenter_Y] - array2d[x + _AimerCenter_X , y + _AimerCenter_Y];
                                    break;
                            }
                            if (x1 > array2d.GetLength(0) - 2)
                                x1 = array2d.GetLength(0) - 5;
                            if (x2 > array2d.GetLength(0) - 2)
                                x2 = array2d.GetLength(0) - 5;
                            if (y1 > array2d.GetLength(1) - 2)
                                y1 = array2d.GetLength(1) - 5;
                            if (y2 > array2d.GetLength(1) - 2)
                                y2 = array2d.GetLength(1) - 5;
                            slope = array2d[x1, y1] - array2d[x2, y2];
                            //slope = array2d[x+2, y] - array2d[x , y];
                            double jlj = (double)AutoThreshold / 255;
                            if (slope > AutoThreshold * jlj)
                            {
                                width_position[m] = x;
                                Height_position[m] = y;

                                m++;
                            }
                        }
                    }
                    for (n = 0; n < m; n++)
                    {
                        switch (k)
                        {
                            case 0:
                                if (Corner[k, 0] > width_position[n])
                                {
                                    Corner[k, 0] = width_position[n];
                                }
                                if (Corner[k, 1] > Height_position[n])
                                {
                                    Corner[k, 1] = Height_position[n];
                                }
                                break;
                            case 1:
                                if (Corner[k, 0] < width_position[n])
                                {
                                    Corner[k, 0] = width_position[n];
                                }
                                if (Corner[k, 1] > Height_position[n])
                                {
                                    Corner[k, 1] = Height_position[n];
                                }
                                break;
                            case 2:
                                if (Corner[k, 0] > width_position[n])
                                {
                                    Corner[k, 0] = width_position[n];
                                }
                                if (Corner[k, 1] < Height_position[n])
                                {
                                    Corner[k, 1] = Height_position[n];
                                }
                                break;
                            case 3:
                                if (Corner[k, 0] < width_position[n])
                                {
                                    Corner[k, 0] = width_position[n];
                                }
                                if (Corner[k, 1] < Height_position[n])
                                {
                                    Corner[k, 1] = Height_position[n];
                                }
                                break;
                        }
                    }
                    switch (k)
                    {
                        case 0:
                            break;
                        case 1:
                            Corner[k, 0] = Corner[k, 0] + _AimerCenter_X;
                            break;
                        case 2:
                            Corner[k, 1] = Corner[k, 1] + _AimerCenter_Y;
                            break;
                        case 3:
                            Corner[k, 0] = Corner[k, 0] + _AimerCenter_X;
                            Corner[k, 1] = Corner[k, 1] + _AimerCenter_Y;
                            break;
                    }
                }
                #endregion

                return;
            }

            public int FOV_Near(Bitmap b, ref int[,] Corner)
            {
                //int c=0;
                int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                _ErrorMessage = "";
                array1d = new byte[b.Width * b.Height];
                array1d_X = new double[b.Width];
                array1d_Y = new double[b.Height];
                //BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                //blur 
                BmpToArray2d_blur(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                //Left-Up 
                int Limt_X = 800, Limt_Y = 400, First_Dot_X = 0, First_Dot_Y = 0;

                int width = array2d.GetLength(0);
                int height = array2d.GetLength(1);
                int[] width_position = new int[width];
                int[] Height_position = new int[width];
                int m = 0, n = 0;
                double Threshold_Max_Temp = AutoThreshold * AutoThreshold / 255;
                int Threshold_Min_Temp = _MinPixel;
                #region First dot
                for (First_Dot_X = 0; First_Dot_X < Limt_X; First_Dot_X++)
                {
                    for (First_Dot_Y = 0; First_Dot_Y < Limt_Y; First_Dot_Y++)
                    {
                        int Dis1 = Math.Abs(array2d[First_Dot_X + 20, First_Dot_Y] - array2d[First_Dot_X, First_Dot_Y]);
                        int Dis2 = Math.Abs(array2d[First_Dot_X, First_Dot_Y + 20] - array2d[First_Dot_X, First_Dot_Y]);
                        int Dis3 = array2d[First_Dot_X + 120, First_Dot_Y] - array2d[First_Dot_X, First_Dot_Y];
                        int Dis4 = array2d[First_Dot_X, First_Dot_Y + 120] - array2d[First_Dot_X, First_Dot_Y];

                        if (Dis1 <= Threshold_Min_Temp && Dis2 <= Threshold_Min_Temp &&
                            Dis3 >= Threshold_Max_Temp && Dis4 >= Threshold_Max_Temp)
                        {
                            width_position[m] = First_Dot_X;
                            Height_position[m] = First_Dot_Y;
                            m++;
                        }
                    }
                }
                if (m < 2) return -1;//can't find the first dot
                int tempfirstDot_X = 0, tempfirstDot_Y = 0;
                for (int i = 0; i < m; i++)
                {
                    tempfirstDot_X = tempfirstDot_X + width_position[i];
                    tempfirstDot_Y = tempfirstDot_Y + Height_position[i];
                }
                Corner[0, 0] = tempfirstDot_X / m;
                Corner[0, 1] = tempfirstDot_Y / m;
                #endregion

                #region second dot
                int Second_Dot_X = 0, Second_Dot_Y = 0, step = 0;
                m = 0; step = 500;
                int Limt_X_second = Limt_X + Corner[0, 0] + step;
                if (Limt_X_second > 1900)
                    Limt_X_second = 1900;
                //if (Limt_X_second > b.Width) return -2;// 
                for (Second_Dot_X = Corner[0, 0] + step; Second_Dot_X < Limt_X_second; Second_Dot_X++)
                {
                    for (Second_Dot_Y = 0; Second_Dot_Y < Limt_Y; Second_Dot_Y++)
                    {
                        int Dis1 = Math.Abs(array2d[Second_Dot_X - 20, Second_Dot_Y] - array2d[Second_Dot_X, Second_Dot_Y]);
                        int Dis2 = Math.Abs(array2d[Second_Dot_X, Second_Dot_Y + 20] - array2d[Second_Dot_X, Second_Dot_Y]);
                        int Dis3 = array2d[Second_Dot_X - 120, Second_Dot_Y] - array2d[Second_Dot_X, Second_Dot_Y];
                        int Dis4 = array2d[Second_Dot_X, Second_Dot_Y + 120] - array2d[Second_Dot_X, Second_Dot_Y];

                        if (Dis1 <= Threshold_Min_Temp && Dis2 <= Threshold_Min_Temp &&
                            Dis3 >= Threshold_Max_Temp && Dis4 >= Threshold_Max_Temp)
                        {
                            width_position[m] = Second_Dot_X;
                            Height_position[m] = Second_Dot_Y;
                            m++;
                        }
                    }
                }
                if (m < 2) return -3;//can't find the Second dot
                int tempSecondDot_X = 0, tempSecondDot_Y = 0;
                for (int i = 0; i < m; i++)
                {
                    tempSecondDot_X = tempSecondDot_X + width_position[i];
                    tempSecondDot_Y = tempSecondDot_Y + Height_position[i];
                }
                Corner[1, 0] = tempSecondDot_X / m;
                Corner[1, 1] = tempSecondDot_Y / m;
                #endregion

                #region third dot
                int third_Dot_X = 0, third_Dot_Y = 0;
                m = 0; step = 500;
                int thirdLimt_X = Limt_X;
                int thirdLimt_Y = Corner[0, 1] + 200 + Limt_Y;
                if (thirdLimt_Y > 800) thirdLimt_Y = 800;
                for (third_Dot_X = 0; third_Dot_X < thirdLimt_X; third_Dot_X++)
                {
                    for (third_Dot_Y = Corner[0, 1] + 200; third_Dot_Y < thirdLimt_Y; third_Dot_Y++)
                    {
                        if (third_Dot_Y > b.Height - 2) continue;
                        if (third_Dot_X > b.Width - 2) continue;
                        int Dis1 = Math.Abs(array2d[third_Dot_X + 20, third_Dot_Y] - array2d[third_Dot_X, third_Dot_Y]);
                        int Dis2 = Math.Abs(array2d[third_Dot_X, third_Dot_Y - 20] - array2d[third_Dot_X, third_Dot_Y]);
                        int Dis3 = array2d[third_Dot_X + 120, third_Dot_Y] - array2d[third_Dot_X, third_Dot_Y];
                        int Dis4 = array2d[third_Dot_X, third_Dot_Y - 120] - array2d[third_Dot_X, third_Dot_Y];

                        if (Dis1 <= Threshold_Min_Temp && Dis2 <= Threshold_Min_Temp &&
                            Dis3 >= Threshold_Max_Temp && Dis4 >= Threshold_Max_Temp)
                        {
                            width_position[m] = third_Dot_X;
                            Height_position[m] = third_Dot_Y;
                            m++;
                        }
                    }
                }
                if (m < 2) return -3;//can't find the Second dot
                int tempThirdDot_X = 0, tempThirdDot_Y = 0;
                for (int i = 0; i < m; i++)
                {
                    tempThirdDot_X = tempThirdDot_X + width_position[i];
                    tempThirdDot_Y = tempThirdDot_Y + Height_position[i];
                }
                Corner[2, 0] = tempThirdDot_X / m;
                Corner[2, 1] = tempThirdDot_Y / m;
                #endregion
                #region fourth dot
                int fourth_Dot_X = 0, fourth_Dot_Y = 0;
                m = 0; step = 500;
                int fourthLimt_X = Corner[2, 0] + step + Limt_X;
                if (fourthLimt_X > 1920) fourthLimt_X = 1900;
                int fourthLimt_Y = Corner[0, 1] + 200 + Limt_Y;

                if (fourthLimt_Y > 800) fourthLimt_Y = 800;

                for (fourth_Dot_X = Corner[0, 0] + step; fourth_Dot_X < fourthLimt_X; fourth_Dot_X++)
                {
                    for (fourth_Dot_Y = Corner[0, 1] + 200; fourth_Dot_Y < fourthLimt_Y; fourth_Dot_Y++)
                    {
                        if (fourth_Dot_Y > b.Height - 2) continue;
                        if (fourth_Dot_X > b.Width - 2) continue;
                        int Dis1 = Math.Abs(array2d[fourth_Dot_X - 20, fourth_Dot_Y] - array2d[fourth_Dot_X, fourth_Dot_Y]);
                        int Dis2 = Math.Abs(array2d[fourth_Dot_X, fourth_Dot_Y - 20] - array2d[fourth_Dot_X, fourth_Dot_Y]);
                        int Dis3 = array2d[fourth_Dot_X - 120, fourth_Dot_Y] - array2d[fourth_Dot_X, fourth_Dot_Y];
                        int Dis4 = array2d[fourth_Dot_X, fourth_Dot_Y - 120] - array2d[fourth_Dot_X, fourth_Dot_Y];

                        if (Dis1 <= Threshold_Min_Temp && Dis2 <= Threshold_Min_Temp &&
                            Dis3 >= Threshold_Max_Temp && Dis4 >= Threshold_Max_Temp)
                        {
                            width_position[m] = fourth_Dot_X;
                            Height_position[m] = fourth_Dot_Y;
                            m++;
                        }
                    }
                }
                if (m < 2) return -4;//can't find the Second dot
                int tempfourthDot_X = 0, tempfourthDot_Y = 0;
                for (int i = 0; i < m; i++)
                {
                    tempfourthDot_X = tempfourthDot_X + width_position[i];
                    tempfourthDot_Y = tempfourthDot_Y + Height_position[i];
                }
                Corner[3, 0] = tempfourthDot_X / m;
                Corner[3, 1] = tempfourthDot_Y / m;
                #endregion
                return 0;
            }
            public int FOV_Far(Bitmap b, ref int[,] Corner)
            {
                //int c=0;
                ThresholdFactor = 0.95;
                int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                int ROI_Width = b.Width - _ROI_StartX - _ROI_EndX;
                int ROI_Height = b.Height - _ROI_StartY - _ROI_EndY;
                if (ROI_Width > b.Width) ROI_Width = b.Width;
                if (ROI_Height > b.Height) ROI_Height = b.Height;
                _ErrorMessage = "";
                array1d = new byte[b.Width * b.Height];
                array1d_X = new double[b.Width];
                array1d_Y = new double[b.Height];
                //BmpToArray2d(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                //blur 
                BmpToArray2d_blur_Far(b, _ROI_StartX, _ROI_StartY, ROI_Width, ROI_Height);
                //Left-Up 
                int Limt_X = 600, Limt_Y = 400, First_Dot_X = 0, First_Dot_Y = 0;

                int width = array2d.GetLength(0);
                int height = array2d.GetLength(1);
                int[] width_position = new int[width];
                int[] Height_position = new int[width];
                int m = 0, n = 0;
                int step_X = 10, step_Y = 100;
                double Threshold_Max_Temp = AutoThreshold * AutoThreshold / 255;
                int Threshold_Min_Temp = _MinPixel;
                #region First dot
                for (First_Dot_X = 0; First_Dot_X < Limt_X; First_Dot_X++)
                {
                    for (First_Dot_Y = 0; First_Dot_Y < Limt_Y; First_Dot_Y++)
                    {
                        int Dis1 = Math.Abs(array2d[First_Dot_X + step_X, First_Dot_Y] - array2d[First_Dot_X, First_Dot_Y]);
                        int Dis2 = Math.Abs(array2d[First_Dot_X, First_Dot_Y + step_X] - array2d[First_Dot_X, First_Dot_Y]);
                        int Dis3 = array2d[First_Dot_X + step_Y, First_Dot_Y] - array2d[First_Dot_X, First_Dot_Y];
                        int Dis4 = array2d[First_Dot_X, First_Dot_Y + step_Y] - array2d[First_Dot_X, First_Dot_Y];

                        if (Dis1 <= Threshold_Min_Temp && Dis2 <= Threshold_Min_Temp &&
                            Dis3 >= Threshold_Max_Temp && Dis4 >= Threshold_Max_Temp)
                        {
                            width_position[m] = First_Dot_X;
                            Height_position[m] = First_Dot_Y;
                            m++;
                        }
                    }
                }
                if (m < 2) return -1;//can't find the first dot
                int tempfirstDot_X = 0, tempfirstDot_Y = 0;
                for (int i = 0; i < m; i++)
                {
                    tempfirstDot_X = tempfirstDot_X + width_position[i];
                    tempfirstDot_Y = tempfirstDot_Y + Height_position[i];
                }
                Corner[0, 0] = tempfirstDot_X / m;
                Corner[0, 1] = tempfirstDot_Y / m;
                #endregion

                #region second dot
                int Second_Dot_X = 0, Second_Dot_Y = 0, step = 0;
                m = 0; step = 300;
                int Limt_X_second = Limt_X + Corner[0, 0] + step;
                if (Limt_X_second > width)
                    Limt_X_second = width - 5;
                for (Second_Dot_X = Corner[0, 0] + step; Second_Dot_X < Limt_X_second; Second_Dot_X++)
                {
                    for (Second_Dot_Y = 0; Second_Dot_Y < Limt_Y; Second_Dot_Y++)
                    {
                        int Dis1 = Math.Abs(array2d[Second_Dot_X - step_X, Second_Dot_Y] - array2d[Second_Dot_X, Second_Dot_Y]);
                        int Dis2 = Math.Abs(array2d[Second_Dot_X, Second_Dot_Y + step_X] - array2d[Second_Dot_X, Second_Dot_Y]);
                        int Dis3 = array2d[Second_Dot_X - step_Y, Second_Dot_Y] - array2d[Second_Dot_X, Second_Dot_Y];
                        int Dis4 = array2d[Second_Dot_X, Second_Dot_Y + step_Y] - array2d[Second_Dot_X, Second_Dot_Y];

                        if (Dis1 <= Threshold_Min_Temp && Dis2 <= Threshold_Min_Temp &&
                            Dis3 >= Threshold_Max_Temp && Dis4 >= Threshold_Max_Temp)
                        {
                            width_position[m] = Second_Dot_X;
                            Height_position[m] = Second_Dot_Y;
                            m++;
                        }
                    }
                }
                if (m < 2) return -3;//can't find the Second dot
                int tempSecondDot_X = 0, tempSecondDot_Y = 0;
                for (int i = 0; i < m; i++)
                {
                    tempSecondDot_X = tempSecondDot_X + width_position[i];
                    tempSecondDot_Y = tempSecondDot_Y + Height_position[i];
                }
                Corner[1, 0] = tempSecondDot_X / m;
                Corner[1, 1] = tempSecondDot_Y / m;
                #endregion

                #region third dot
                int third_Dot_X = 0, third_Dot_Y = 0;
                m = 0; step = 300;
                int thirdLimt_X = Limt_X;
                int thirdLimt_Y = Corner[0, 1] + 200 + Limt_Y;
                if (thirdLimt_Y > height)
                    thirdLimt_Y = height - 5;
                if (Limt_X > b.Width) return -2;// 
                for (third_Dot_X = 0; third_Dot_X < thirdLimt_X; third_Dot_X++)
                {
                    for (third_Dot_Y = Corner[0, 1] + 200; third_Dot_Y < thirdLimt_Y; third_Dot_Y++)
                    {
                        int Dis1 = Math.Abs(array2d[third_Dot_X + step_X, third_Dot_Y] - array2d[third_Dot_X, third_Dot_Y]);
                        int Dis2 = Math.Abs(array2d[third_Dot_X, third_Dot_Y - step_X] - array2d[third_Dot_X, third_Dot_Y]);
                        int Dis3 = array2d[third_Dot_X + step_Y, third_Dot_Y] - array2d[third_Dot_X, third_Dot_Y];
                        int Dis4 = array2d[third_Dot_X, third_Dot_Y - step_Y] - array2d[third_Dot_X, third_Dot_Y];

                        if (Dis1 <= Threshold_Min_Temp && Dis2 <= Threshold_Min_Temp &&
                            Dis3 >= Threshold_Max_Temp && Dis4 >= Threshold_Max_Temp)
                        {
                            width_position[m] = third_Dot_X;
                            Height_position[m] = third_Dot_Y;
                            m++;
                        }
                    }
                }
                if (m < 2) return -3;//can't find the Second dot
                int tempThirdDot_X = 0, tempThirdDot_Y = 0;
                for (int i = 0; i < m; i++)
                {
                    tempThirdDot_X = tempThirdDot_X + width_position[i];
                    tempThirdDot_Y = tempThirdDot_Y + Height_position[i];
                }
                Corner[2, 0] = tempThirdDot_X / m;
                Corner[2, 1] = tempThirdDot_Y / m;
                #endregion
                #region fourth dot
                int fourth_Dot_X = 0, fourth_Dot_Y = 0;
                m = 0; step = 300;
                int fourthLimt_X = Corner[2, 0] + step + Limt_X;
                if (fourthLimt_X > width) fourthLimt_X = width - 5;
                int fourthLimt_Y = Corner[0, 1] + 200 + Limt_Y;
                if (fourthLimt_Y > height) fourthLimt_Y = height - 5;

                for (fourth_Dot_X = Corner[0, 0] + step; fourth_Dot_X < fourthLimt_X; fourth_Dot_X++)
                {
                    for (fourth_Dot_Y = Corner[0, 1] + 200; fourth_Dot_Y < fourthLimt_Y; fourth_Dot_Y++)
                    {
                        if (fourth_Dot_Y > b.Height - 2) continue;
                        if (fourth_Dot_X > b.Width - 2) continue;
                        int Dis1 = Math.Abs(array2d[fourth_Dot_X - step_X, fourth_Dot_Y] - array2d[fourth_Dot_X, fourth_Dot_Y]);
                        int Dis2 = Math.Abs(array2d[fourth_Dot_X, fourth_Dot_Y - step_X] - array2d[fourth_Dot_X, fourth_Dot_Y]);
                        int Dis3 = array2d[fourth_Dot_X - step_Y, fourth_Dot_Y] - array2d[fourth_Dot_X, fourth_Dot_Y];
                        int Dis4 = array2d[fourth_Dot_X, fourth_Dot_Y - step_Y] - array2d[fourth_Dot_X, fourth_Dot_Y];

                        if (Dis1 <= Threshold_Min_Temp && Dis2 <= Threshold_Min_Temp &&
                            Dis3 >= Threshold_Max_Temp && Dis4 >= Threshold_Max_Temp)
                        {
                            width_position[m] = fourth_Dot_X;
                            Height_position[m] = fourth_Dot_Y;
                            m++;
                        }
                    }
                }
                if (m < 2) return -4;//can't find the Second dot
                int tempfourthDot_X = 0, tempfourthDot_Y = 0;
                for (int i = 0; i < m; i++)
                {
                    tempfourthDot_X = tempfourthDot_X + width_position[i];
                    tempfourthDot_Y = tempfourthDot_Y + Height_position[i];
                }
                Corner[3, 0] = tempfourthDot_X / m;
                Corner[3, 1] = tempfourthDot_Y / m;
                #endregion
                return 0;
            }


            // 定义子函数 *************************
            int ArrAve(double[] x, double average)
            {
                int result = x.Length;
                int position = 0;
                for (int i = 1; i < x.Length; i++)
                {
                    int width = Convert.ToInt32(Math.Abs(x[i] - average));
                    if (result > width)
                    {
                        result = width;
                        position = i;
                    }
                }
                return position;
            }

            // 数组拷贝
            void ArrCopy(double[] data, double[] DataCopy)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    DataCopy[i] = data[i];
                }
            }
            // 最大值
            double ArrMax(double[] x)
            {
                double result = x[0];
                for (int i = 1; i < x.Length; i++)
                {
                    if (result < x[i])
                    {
                        result = x[i];
                    }
                }
                return result;
            }

            /// 最小值
            double ArrMin(double[] x)
            {
                double result = x[0];
                for (int i = 1; i < x.Length; i++)
                {
                    if (result > x[i])
                    {
                        result = x[i];
                    }
                }
                return result;
            }

            // 高斯拟合目标函数//Difference of Gaussians 
            double Obj_Gaussian(double[] p, double[] x, double[] y)
            {
                double result = 0;
                double[] jlj = new double[y.Length];
                double[] jlj_p = new double[y.Length];
                for (int i = 0; i < x.Length; i++)
                {
                    double ExpValue = Math.Exp(-Math.Pow(x[i] - p[1], 2) / Math.Pow(p[2], 2));
                    jlj[i] = Math.Pow(y[i] - p[0] * ExpValue, 2);
                    result = result + jlj[i];
                    jlj_p[i] = jlj[i] / y.Length;
                }

                return Math.Sqrt(result / y.Length);
            }

            // 条件判断
            bool IsTrue(double[] x, double[] x_min, double[] x_max)
            {
                bool result = false;
                bool Judge1 = (x[0] > x_min[0]) && (x[0] < x_max[0]);
                bool Judge2 = (x[1] > x_min[1]) && (x[1] < x_max[1]);
                bool Judge3 = (x[2] > x_min[2]) && (x[2] < x_max[2]);
                if (Judge1 && Judge2 && Judge3)
                {
                    result = true;
                }
                return result;
            }

            // 产生 0~1 之间的随机数
            double random()
            {
                var seed = Guid.NewGuid().GetHashCode();  // 这个种子很重要，不然随机数是重复的
                Random r = new Random(seed);
                int i = r.Next(0, 100000);
                return (double)i / 100000;
            }
            public double PeakPicker(double[] xfit, double[] yfit)
            {
                // 程序主体 ************************
                // 常量值
                double T0 = 1000;
                double Alpha = 0.99;
                double IteMax = 3000;

                // 变量初始化
                double[] MassComps = new double[3];
                double[] MassCompsNew = new double[3];
                double[] MassLB = new double[3];
                double[] MassUB = new double[3];
                double[] dMass = new double[3];
                MassLB[0] = ArrMin(yfit);
                MassLB[1] = ArrMin(xfit);
                MassLB[2] = 0;
                MassUB[0] = ArrMax(yfit);
                MassUB[1] = ArrMax(xfit);
                MassUB[2] = 5;

                // 初始模型
                for (int j = 0; j < 3; j++)
                {
                    //MassComps[j] =  (MassUB[j] - MassLB[j]) + MassLB[j];
                    MassComps[j] = random() * (MassUB[j] - MassLB[j]) + MassLB[j];
                }
                // 计算目标函数
                double Fun = Obj_Gaussian(MassComps, xfit, yfit);
                // 迭代
                int iii = 0;
                double[] ljlj = new double[3000];


                for (int i = 0; i < IteMax; i++)
                {
                    double Tk = T0 * Math.Pow(Alpha, i);
                    // 预更新
                    double[] rand = new double[3];
                    for (int j = 0; j < 3; j++)
                    {
                        rand[j] = random();
                        dMass[j] = Tk * Math.Sign(rand[j] - 0.5) * (Math.Pow(1 + 1.0 / Tk, Math.Abs(2 * rand[j] - 1)) - 1);
                        //dMass[j] = Tk * Math.Sign(0.5) * (Math.Pow(1 + 1.0 / Tk, Math.Abs(2 * 1- 1)) - 1);

                        MassCompsNew[j] = MassComps[j] + dMass[j] * (MassUB[j] - MassLB[j]);
                    }
                    // 判断
                    if (IsTrue(MassCompsNew, MassLB, MassUB))
                    {
                        iii++;
                        double NewFun = Obj_Gaussian(MassCompsNew, xfit, yfit);
                        if (NewFun <= Fun)
                        {
                            ArrCopy(MassCompsNew, MassComps);
                            Fun = NewFun;
                        }
                        else
                        {
                            if (Math.Exp(-(NewFun - Fun) / Tk) > random())
                            {
                                ArrCopy(MassCompsNew, MassComps);
                                Fun = NewFun;
                            }
                        }
                        ljlj[iii] = MassComps[0];
                    }
                }
                // 程序计算完成
                return MassComps[1];
            }
            public double PeakPicker_MID(double[] xfit, double[] yfit)
            {
                // 程序主体 ************************
                // 常量值
                double T0 = 1000;
                double Alpha = 0.99;
                double IteMax = 3000;

                // 变量初始化
                double[] MassComps = new double[3];
                double[] MassCompsNew = new double[3];
                double[] MassLB = new double[3];
                double[] MassUB = new double[3];
                double[] dMass = new double[3];
                MassLB[0] = ArrMin(yfit);
                MassLB[1] = ArrMin(xfit);
                MassLB[2] = 0;
                MassUB[0] = ArrMax(yfit);
                MassUB[1] = ArrMax(xfit);
                MassUB[2] = 5;

                // 初始模型
                for (int j = 0; j < 3; j++)
                {
                    //MassComps[j] =  (MassUB[j] - MassLB[j]) + MassLB[j];
                    MassComps[j] = random() * (MassUB[j] - MassLB[j]) + MassLB[j];
                }
                // 计算目标函数
                double Fun = Obj_Gaussian(MassComps, xfit, yfit);
                // 迭代
                for (int i = 0; i < IteMax; i++)
                {
                    double Tk = T0 * Math.Pow(Alpha, i);
                    // 预更新
                    double[] rand = new double[3];
                    for (int j = 0; j < 3; j++)
                    {
                        rand[j] = random();
                        dMass[j] = Tk * Math.Sign(rand[j] - 0.5) * (Math.Pow(1 + 1.0 / Tk, Math.Abs(2 * rand[j] - 1)) - 1);
                        //dMass[j] = Tk * Math.Sign(0.5) * (Math.Pow(1 + 1.0 / Tk, Math.Abs(2 * 1- 1)) - 1);

                        MassCompsNew[j] = MassComps[j] + dMass[j] * (MassUB[j] - MassLB[j]);
                    }
                    // 判断
                    if (IsTrue(MassCompsNew, MassLB, MassUB))
                    {
                        double NewFun = Obj_Gaussian(MassCompsNew, xfit, yfit);
                        if (NewFun <= Fun)
                        {
                            ArrCopy(MassCompsNew, MassComps);
                            Fun = NewFun;
                        }
                        else
                        {
                            if (Math.Exp(-(NewFun - Fun) / Tk) > random())
                            {
                                ArrCopy(MassCompsNew, MassComps);
                                Fun = NewFun;
                            }
                        }
                    }
                }
                // 程序计算完成
                return MassComps[1];
            }


            private double FWHM(double[] data)
            {
                double fwhm = 0;
                double[] x = new double[data.Length];
                double[] y = new double[data.Length];
                //double[] xfit = new double[data.Length/5];
                //double[] yfit = new double[data.Length / 5];
                double c = 5;
                double sigm = 3;
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = i;
                    y[i] = data[i];
                    //y[i] = Math.Exp(-(data[i] - c) * (data[i] - c) / (sigm * sigm));
                    //y[i] = y[i] + 0.1 * (random() - 0.5) * y[i];
                }
                //for (int i = 0; i < xfit.Length; i++)
                //{
                //    xfit[i] = x[xfit.Length*2 + i];
                //    yfit[i] = y[xfit.Length * 2 + i];
                //}
                double result = PeakPicker(x, y);
                int leftWidht = Convert.ToInt32(result);
                double[] x_left = new double[leftWidht];
                double[] y_left = new double[leftWidht];
                for (int i = 0; i < leftWidht; i++)
                {
                    x_left[i] = i;
                    y_left[i] = data[i];
                }

                double result1 = PeakPicker(x_left, y_left);
                int rightWidht = Convert.ToInt32(data.Length - result);
                double[] x_right = new double[rightWidht];
                double[] y_right = new double[rightWidht];
                for (int j = 0; j < rightWidht; j++)
                {
                    x_right[j] = j;
                    y_right[j] = data[j + (int)result1];
                }
                double result2 = PeakPicker(x_right, y_right);
                return result;
            }//end main
            private double FWHM_gen8(double[] data_X, double[] data_Y, ref double left, ref double right, ref  double Up, ref  double Bot)
            {
                double[] LeftM = new double[_AimerCenter_X];
                if ((_AimerCenter_X < _ABSPosition_X) || _AimerCenter_X + _ABSPosition_X > data_X.Length) return -1;
                if ((_AimerCenter_Y < _ABSPosition_Y) || _AimerCenter_Y + _ABSPosition_Y > data_Y.Length) return -2;
                for (int i = 0; i < 16; i++)
                {
                    left = data_X[_AimerCenter_X - _ABSPosition_X];
                }
                return 0;
            }//end main

            private double FWHM(double[] data, int c) // data as 2d data and c is integer
            {
                //FWHM = 2*sqrt(2*ln(2))*c
                double[] datax = new double[data.Length];
                double[] datay = new double[data.Length];
                double PP = 2.2;
                int CI, k, L, Mag = 4;
                double Interp, Tlead, Ttrail, fwhm = 0;
                L = datay.Length;

                // Create datax as index for the number of elemts in data from 1-Length(data).
                for (int i = 0; i < data.Length; i++)
                {
                    datax[i] = (i + 1);
                }

                //Find max in datay and divide all elements by maxValue.
                var m = 0;// datay.Length; // Find length of  datay

                Array.ForEach(datay, (x) => { datay[m] = (double)(data[m] / data.Max()); m++; }); // Divide all elements of datay by max(datay)
                double maxValue = data.Max();
                CI = data.ToList().IndexOf((byte)maxValue); // Push that index to CI

                // Start to search lead
                k = 2;
                while (Math.Sign(datay[k]) == Math.Sign(datay[k - 1] - 0.5))
                {
                    k = k + 1;
                }
                Interp = (0.5 - datay[k - 1]) / (datay[k] - datay[k - 1]);
                Tlead = datax[k - 1] + Interp * (datax[k] - datax[k - 1]);
                CI = CI + 1;

                // Start search for the trail
                while (Math.Sign(datay[k] - 0.5) == Math.Sign(datay[k - 1] - 0.5) && (k <= L - 1))
                {
                    k = k + 1;
                }
                if (k != L)
                {
                    Interp = (0.5 - datay[k - 1]) / (datay[k] - datay[k - 1]);
                    Ttrail = datax[k - 1] + Interp * (datax[k] - datax[k - 1]);
                    fwhm = ((Ttrail - Tlead) * 2.235) / Mag;
                }
                return fwhm;
            }//end main

            #endregion

            #region 1470 圆斑中心点,sharpness,直径。
            public bool CircleTest(Bitmap b)
            {

                AimerCenter_LEDLine(b);
                int SizeBot = 0, SizeUp = 0;
                if (CalLedProfile_LEDCircle(b.Height - _ROI_StartY - _ROI_EndY, b.Width - _ROI_StartX - _ROI_EndX, array1d, ref SizeBot, ref SizeUp) == false) return false;
                //_diameter = _Aimer_LEDLine_Width;
                return true;
            }
            #endregion
            #region public function
            private unsafe byte FilterNoiseAPI(byte* sourcePixel, int matrixSize)
            {
                byte PixelValue = 0;
                return PixelValue;
            }
            private Bitmap BmpToArray2d(Bitmap b, int ROI_PointX, int ROI_PointY, int ROI_Width, int ROI_Height)
            {
                PixelFormat bitmaptype = b.PixelFormat;// PixelFormat.Format8bppIndexed;
                _MaxPixel = 0; _MinPixel = 255;
                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, bitmaptype);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                if (ROI_Width > b.Width) ROI_Width = b.Width - 5;
                if (ROI_Height > b.Height) ROI_Height = b.Height - 5;
                unsafe
                {
                    int nPixelBytes = 1;
                    switch (bitmaptype)
                    {
                        case PixelFormat.Format8bppIndexed:
                            nPixelBytes = 8 >> 3;
                            break;
                        case PixelFormat.Format24bppRgb:
                            nPixelBytes = 24 >> 3;
                            break;
                        case PixelFormat.Format32bppRgb:
                            nPixelBytes = 32 >> 3;
                            break;
                        case PixelFormat.Format32bppArgb:
                            nPixelBytes = 32 >> 3;
                            break;
                        default:
                            break;
                    }
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - b.Width * nPixelBytes;
                    byte red, green, blue, color = 0;
                    array2d = new byte[b.Width - ROI_StartX - ROI_EndX, b.Height - ROI_EndY - ROI_StartY];

                    for (int y = ROI_PointY; y < ROI_PointY + ROI_Height; y++)//
                    {
                        if (y == ROI_PointY)
                        {
                            p += ((ROI_PointY * b.Width + ROI_PointX) * nPixelBytes);
                        }
                        else
                        {
                            p += (ROI_PointX * nPixelBytes);
                        }
                        for (int x = ROI_PointX; x < ROI_PointX + ROI_Width; x++)
                        {
                            color = 0;
                            for (int k = 0; k < nPixelBytes; k++)
                            {
                                //*(ptr + nDesOffset + k) = bytes[nSrcOffset + k];
                                switch (nPixelBytes)
                                {
                                    case 1:
                                        color = (byte)p[k];
                                        break;
                                    case 3:
                                        color = (byte)(color + p[k]);
                                        if (k == nPixelBytes - 1)
                                            color = (byte)(color / 3);
                                        break;
                                    case 4:
                                        color = (byte)(color + p[k]);
                                        if (k == nPixelBytes - 1)
                                            color = (byte)(color / 3);
                                        break;
                                    default:
                                        break;
                                }
                                //blue = p[0];
                                //green = p[1];
                                //red = p[2];

                                //if (x == ROI_PointX || x == ROI_PointX + ROI_Width - 1 || y == ROI_PointY || y == ROI_PointY + ROI_Height - 1)
                                //{
                                //    p[k] = 255;//Draw ROI
                                //}
                            }

                            if (color > _MaxPixel && color != 255)
                            {
                                _MaxPixel = color;
                            }
                            if (color < _MinPixel)
                                _MinPixel = color;
                            array2d[x - ROI_PointX, y - ROI_PointY] = color;
                            array1d[(x - ROI_PointX) + (y - ROI_PointY) * b.Width] = color;

                            if (x == ROI_PointX + ROI_Width - 1)
                            {
                                p = p + (b.Width - (ROI_PointX + ROI_Width - 1)) * nPixelBytes;
                            }
                            else
                            {
                                p = p + nPixelBytes;
                            }
                        }
                        p += nOffset;
                    }
                    AutoThreshold = Convert.ToInt32(Math.Abs((_MaxPixel - _MinPixel) * ThresholdFactor + _MinPixel));
                    // AutoThreshold = Convert.ToInt32(min * 2);
                    //AutoThreshold = Convert.ToInt32( min * 1.5);// Math.Abs((max - min) / 4 + min);
                }
                b.UnlockBits(bmData);

                return b;
            }

            private Bitmap BmpToArray2d_blur(Bitmap b, int ROI_PointX, int ROI_PointY, int ROI_Width, int ROI_Height)
            {
                PixelFormat bitmaptype = b.PixelFormat;// PixelFormat.Format8bppIndexed;
                _MaxPixel = 0; _MinPixel = 255;
                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, bitmaptype);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                if (ROI_Width > b.Width) ROI_Width = b.Width - 5;
                if (ROI_Height > b.Height) ROI_Height = b.Height - 5;
                unsafe
                {
                    int nPixelBytes = 1;
                    switch (bitmaptype)
                    {
                        case PixelFormat.Format8bppIndexed:
                            nPixelBytes = 8 >> 3;
                            break;
                        case PixelFormat.Format24bppRgb:
                            nPixelBytes = 24 >> 3;
                            break;
                        case PixelFormat.Format32bppRgb:
                            nPixelBytes = 32 >> 3;
                            break;
                        case PixelFormat.Format32bppArgb:
                            nPixelBytes = 32 >> 3;
                            break;
                        default:
                            break;
                    }
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - b.Width * nPixelBytes;
                    byte red, green, blue, color = 0;
                    array2d = new byte[b.Width - ROI_StartX - ROI_EndX, b.Height - ROI_EndY - ROI_StartY];

                    for (int y = ROI_PointY; y < ROI_PointY + ROI_Height; y++)//
                    {
                        if (y == ROI_PointY)
                        {
                            p += ((ROI_PointY * b.Width + ROI_PointX) * nPixelBytes);
                        }
                        else
                        {
                            p += (ROI_PointX * nPixelBytes);
                        }
                        for (int x = ROI_PointX; x < ROI_PointX + ROI_Width; x++)
                        {
                            color = 0;
                            byte r1, r2, r3, r4, r5, r6, r7, r8, r9;
                            for (int k = 0; k < nPixelBytes; k++)
                            {
                                //*(ptr + nDesOffset + k) = bytes[nSrcOffset + k];

                                if (x > ROI_PointX + ROI_Width - 10 || y > ROI_PointY + ROI_Height - 10)
                                {
                                    switch (nPixelBytes)
                                    {
                                        case 1:
                                            color = (byte)p[k];
                                            break;
                                        case 3:
                                            color = (byte)(color + p[k]);
                                            if (k == nPixelBytes - 1)
                                                color = (byte)(color / 3);
                                            break;
                                        case 4:
                                            color = (byte)(color + p[k]);
                                            if (k == nPixelBytes - 1)
                                                color = (byte)(color / 3);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (nPixelBytes)
                                    {
                                        case 1:
                                            //color = (byte)p[k];
                                            r1 = (byte)(p[k + 1]);
                                            r2 = (byte)(p[k + 2]);
                                            r3 = (byte)(p[k + 3]);

                                            r4 = (byte)(p[k + stride + 1]);
                                            r5 = (byte)(p[k + stride + 2]);
                                            r6 = (byte)(p[k + stride + 3]);

                                            r7 = (byte)(p[k + stride + stride + 1]);
                                            r8 = (byte)(p[k + stride + stride + 2]);
                                            r9 = (byte)(p[k + stride + stride + 3]);
                                            color = (byte)((r1 + r2 + r3 + r4 + r5 + r6 + r7 + r8 + r9) / 9);
                                            break;
                                        case 3:
                                            color = (byte)(color + p[k]);
                                            if (k == nPixelBytes - 1)
                                                color = (byte)(color / 3);
                                            break;
                                        case 4:
                                            color = (byte)(color + p[k]);
                                            if (k == nPixelBytes - 1)
                                                color = (byte)(color / 3);
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                //blue = p[0];
                                //green = p[1];
                                //red = p[2];

                                //if (x == ROI_PointX || x == ROI_PointX + ROI_Width - 1 || y == ROI_PointY || y == ROI_PointY + ROI_Height - 1)
                                //{
                                //    p[k] = 255;//Draw ROI
                                //}
                            }

                            if (color > _MaxPixel && color != 255)
                            {
                                _MaxPixel = color;
                            }
                            if (color < _MinPixel)
                                _MinPixel = color;
                            array2d[x - ROI_PointX, y - ROI_PointY] = color;
                            array1d[(x - ROI_PointX) + (y - ROI_PointY) * b.Width] = color;

                            if (x == ROI_PointX + ROI_Width - 1)
                            {
                                p = p + (b.Width - (ROI_PointX + ROI_Width - 1)) * nPixelBytes;
                            }
                            else
                            {
                                p = p + nPixelBytes;
                            }
                        }
                        p += nOffset;
                    }
                    AutoThreshold = Convert.ToInt32(Math.Abs((_MaxPixel - _MinPixel) * ThresholdFactor + _MinPixel));
                    // AutoThreshold = Convert.ToInt32(min * 2);
                    //AutoThreshold = Convert.ToInt32( min * 1.5);// Math.Abs((max - min) / 4 + min);
                }
                b.UnlockBits(bmData);

                return b;
            }
            private Bitmap BmpToArray2d_blur_Far(Bitmap b, int ROI_PointX, int ROI_PointY, int ROI_Width, int ROI_Height)
            {
                PixelFormat bitmaptype = b.PixelFormat;// PixelFormat.Format8bppIndexed;
                _MaxPixel = 0; _MinPixel = 255;
                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, bitmaptype);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                if (ROI_Width > b.Width) ROI_Width = b.Width - 5;
                if (ROI_Height > b.Height) ROI_Height = b.Height - 5;
                unsafe
                {
                    int nPixelBytes = 1;
                    switch (bitmaptype)
                    {
                        case PixelFormat.Format8bppIndexed:
                            nPixelBytes = 8 >> 3;
                            break;
                        case PixelFormat.Format24bppRgb:
                            nPixelBytes = 24 >> 3;
                            break;
                        case PixelFormat.Format32bppRgb:
                            nPixelBytes = 32 >> 3;
                            break;
                        case PixelFormat.Format32bppArgb:
                            nPixelBytes = 32 >> 3;
                            break;
                        default:
                            break;
                    }
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - b.Width * nPixelBytes;
                    byte red, green, blue, color = 0;
                    array2d = new byte[b.Width - ROI_StartX - ROI_EndX, b.Height - ROI_EndY - ROI_StartY];

                    for (int y = ROI_PointY; y < ROI_PointY + ROI_Height; y++)//
                    {
                        if (y == ROI_PointY)
                        {
                            p += ((ROI_PointY * b.Width + ROI_PointX) * nPixelBytes);
                        }
                        else
                        {
                            p += (ROI_PointX * nPixelBytes);
                        }
                        for (int x = ROI_PointX; x < ROI_PointX + ROI_Width; x++)
                        {
                            color = 0;
                            byte r1, r2, r3, r4, r5, r6, r7, r8, r9;
                            for (int k = 0; k < nPixelBytes; k++)
                            {
                                //*(ptr + nDesOffset + k) = bytes[nSrcOffset + k];

                                if (x > ROI_PointX + ROI_Width - 10 || y > ROI_PointY + ROI_Height - 10)
                                {
                                    switch (nPixelBytes)
                                    {
                                        case 1:
                                            color = (byte)p[k];
                                            break;
                                        case 3:
                                            color = (byte)(color + p[k]);
                                            if (k == nPixelBytes - 1)
                                                color = (byte)(color / 3);
                                            break;
                                        case 4:
                                            color = (byte)(color + p[k]);
                                            if (k == nPixelBytes - 1)
                                                color = (byte)(color / 3);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (nPixelBytes)
                                    {
                                        case 1:
                                            //color = (byte)p[k];
                                            r1 = (byte)(p[k + 1]);
                                            r2 = (byte)(p[k + 2]);
                                            r3 = (byte)(p[k + 3]);

                                            r4 = (byte)(p[k + stride + 1]);
                                            r5 = (byte)(p[k + stride + 2]);
                                            r6 = (byte)(p[k + stride + 3]);

                                            r7 = (byte)(p[k + stride + stride + 1]);
                                            r8 = (byte)(p[k + stride + stride + 2]);
                                            r9 = (byte)(p[k + stride + stride + 3]);
                                            color = (byte)((r1 + r2 + r3 + r4 + r5 + r6 + r7 + r8 + r9) / 9);
                                            break;
                                        case 3:
                                            color = (byte)(color + p[k]);
                                            if (k == nPixelBytes - 1)
                                                color = (byte)(color / 3);
                                            break;
                                        case 4:
                                            color = (byte)(color + p[k]);
                                            if (k == nPixelBytes - 1)
                                                color = (byte)(color / 3);
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                //blue = p[0];
                                //green = p[1];
                                //red = p[2];

                                //if (x == ROI_PointX || x == ROI_PointX + ROI_Width - 1 || y == ROI_PointY || y == ROI_PointY + ROI_Height - 1)
                                //{
                                //    p[k] = 255;//Draw ROI
                                //}
                            }

                            if (color > _MaxPixel && color != 255 && x < 1200)
                            {

                                _MaxPixel = color;
                            }
                            if (color < _MinPixel && x < 1200)
                                _MinPixel = color;
                            array2d[x - ROI_PointX, y - ROI_PointY] = color;
                            array1d[(x - ROI_PointX) + (y - ROI_PointY) * b.Width] = color;

                            if (x == ROI_PointX + ROI_Width - 1)
                            {
                                p = p + (b.Width - (ROI_PointX + ROI_Width - 1)) * nPixelBytes;
                            }
                            else
                            {
                                p = p + nPixelBytes;
                            }
                        }
                        p += nOffset;
                    }
                    AutoThreshold = Convert.ToInt32(Math.Abs((_MaxPixel - _MinPixel) * ThresholdFactor + _MinPixel));
                }
                b.UnlockBits(bmData);

                return b;
            }


            private static int MaxNumRows = 225, MaxNumCols = 225;//Region of imterest should be smaller than 150x150
            private static int NumBin = 4;//Number of Bins used for averaging, do not change unless you know what you are doing
            private static int ColMarginRoiSearch = 3, MinNumOfPixels = 3, RowMarginRoiSearch = 10, BackgroundCountThreshold = 20, BackgroundCountCeiling = 250;
            private static int BitsShifted = 10, BackgroundCalcCeiling = 21, MaxBackgroundLevel = 20, DoubleBitsShifted = 20;





            #endregion

            #endregion
        }
}
