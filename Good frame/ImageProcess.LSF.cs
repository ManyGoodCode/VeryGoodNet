using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCore.MathLib
{
    public partial class imageProcess
    {
        /// <summary>
        /// 基于垂直方向的狭缝图像计算MTF  Calcualte MTF based on a vertically oriented slit image
        /// X means Column, Horizontal and Width
        /// Y means Row, Veritcal and Height
        /// P_SCANBUFFER: pointer to the current frame (typedef  unsigned char byte)
        /// 
        /// PixelPitch:成像传感器的间距，单位为纳米。the pitch of imaging sensor, in nanometer。
        /// 
        /// SpatialFreqs: pointer to the frequencies at with MTF will be caluclated, in cyc/mm and shifted。指向与MTF的频率的指针将被计算，以cyc/mm为单位并移位。 
        /// 
        /// MTF: pointer to the MTF. shifted using number of bits defined by MtfDataBitsShifted
        /// 
        /// numOfFreqs: number of frequencies of interest。感兴趣的频率数。
        /// 
        /// LsfPeakValue: the peak value of LSF (line spread function), for monitoring purpose。LSF(线扩散函数)的峰值，用于监测。 
        /// 
        /// CentralPtLoc: the x-coordinate of the slit at y=ImgYDim/2, used to estimate the pointing and magnification
        ///        note this is not the true center of the slit。在y=ImgYDim/2处的狭缝的x坐标，用于估计指向和放大注意，这不是狭缝的真正中心。
        /// 
        /// 
        /// 
        /// Subroutine Return 子程序返回
        /// 0 - normal execution。正常执行
        /// 1 - not vertically slanted image or slit not extent throughout the ROI。不垂直倾斜的图像或狭缝不遍及整个ROI。 
        /// 2 - background too noisy. SNR too low。背景太吵了。信噪比太低了。
        /// 3 - The slope of line too big or too small。直线的斜率过大或过小
        /// 4 - zero counts at LSF bin. The slope of line might be too small (aligned too close to y axis)。LSF仓库的计数为零。 直线的斜率可能太小(与y轴对齐太近)。 
        /// rev.5  replace all the float point with long, number of bits shifted is determined by BitsShifted。替换所有的浮点数为长，移位的位数由bits移位决定 
        /// rev.51 add calculation for central pt location, number of bits shifted is determined by BitsShifted。添加中心pt位置的计算，移位的位数由bits移位决定 
        /// rev.52 larger area is used to calculate the background dark level。更大的区域用于计算背景暗电平 
        /// rev.57 deal with dark images。处理黑暗图像
        /// </summary>
        /// <param name="ImgXDim">当前帧沿X方向的像素数 = 1280。number of pixels in the current frame along X direction. =1280 for DR. </param>
        /// <param name="ImgYDim">number of pixels in the current frame along Y direction. =960 for DR</param>
        /// <param name="RoiStartPtX">矩形左上角的x坐标.Roi代表感兴趣的区域，也是一个矩形。 </param>
        /// <param name="RoiStartPtY">当前帧沿Y方向的像素数为960。 </param>
        private int ImageMtfLSF_rev57(
            int imageXDim, int imageYDim,
            int roiStartPtX, int roiStartPtY, int roiWidth, int roiHeight,
            int searchRoiWidth,
            int backgroundCalcMarginWidth,
            int pixelPitch,
            ref int[] spatialFreqs,
            ref long[] mtfs, int numOfFreqs, ref int lsfPeakValue, ref int centralPtLoc, ref long lineSlope)
        {
            int result = 0;
            int i, j, CurX, CurY, CurPixelValue;
            long dt, dt1;
            long f_dt, f_dt1, f_CurPixelValue;

            int UpdatedRoiStartPtX; // X value of updated ROI
            long BackgroundLevel; //dark background, be delete when calculate LSF
            long[] Centroid = new long[MaxNumRows];//150 
            long tmpCentroid;
            long line_slope = 0, offset = 0; // the line curve of centroids
            long tmp_peak_loc, wid, wid_left, wid_right; //parameters for Hamming window
            int numRows;
            //LSF alignment
            long[] binArray1 = new long[MaxNumCols * NumBin], binArray2 = new long[MaxNumCols * NumBin];
            long[] lsf = new long[MaxNumCols * NumBin], tmp_lsf = new long[MaxNumCols * NumBin];
            long new_offset, new_centroid, a, b, MTF0, g0, g1;
            int new_start, newNumPixels, new_center, old_center, pixel_shift, ling, numZeroCount = 0;
            double tmp_a, tmp_b;

            CreateCosTable();
            CreateLutTable();
            //finetune ROI based on top rows and bottom rows
            tmpCentroid = 0;
            for (j = 0; j < ColMarginRoiSearch; j++)
            {
                //top rows
                dt = 0;
                dt1 = 0;
                CurY = roiStartPtY + j;
                for (i = -RowMarginRoiSearch; i < searchRoiWidth + RowMarginRoiSearch; i++)
                {
                    CurX = roiStartPtX + i;
                    CurPixelValue = imageBufferArray[CurY * imageXDim + CurX];
                    if ((CurPixelValue > BackgroundCountThreshold) && (CurPixelValue < BackgroundCountCeiling))
                    {
                        dt += CurPixelValue * i;
                        dt1 += CurPixelValue;
                    }
                }

                if (dt1 < MinNumOfPixels)
                {
                    //theStdInterface.printf("Top MinNumOfPixels %d\r\n",dt1);
                    return 1;

                }
                else
                {
                    tmpCentroid += (dt << BitsShifted) / dt1;
                }

                // bot rows
                dt = 0;
                dt1 = 0;
                CurY = roiStartPtY + roiHeight - 1 - j;
                for (i = -RowMarginRoiSearch; i < searchRoiWidth + RowMarginRoiSearch; i++)
                {
                    CurX = roiStartPtX + i;
                    CurPixelValue = imageBufferArray[CurY * imageXDim + CurX];
                    if ((CurPixelValue > BackgroundCountThreshold) && (CurPixelValue < BackgroundCountCeiling))
                    {
                        dt += CurPixelValue * i;
                        dt1 += CurPixelValue;
                    }
                }

                if (dt1 < MinNumOfPixels)
                {
                    return 1;
                }
                else
                {
                    tmpCentroid += (dt << BitsShifted) / dt1;
                }
            }
            tmpCentroid = (tmpCentroid / (2 * ColMarginRoiSearch)) >> BitsShifted;
            UpdatedRoiStartPtX = (int)(roiStartPtX + tmpCentroid - (roiWidth >> 1));

            //calculate background, based on the left and right columns just outside the ROI
            dt = 0;
            dt1 = 0;
            for (i = 0; i < backgroundCalcMarginWidth; i++)
            {
                //left bank
                CurX = UpdatedRoiStartPtX + i;
                for (j = 0; j < roiHeight; j++)
                {
                    CurY = roiStartPtY + j;
                    if (imageBufferArray[CurY * imageXDim + CurX] < BackgroundCalcCeiling)
                    { //added in Rev 7
                        dt += imageBufferArray[CurY * imageXDim + CurX];
                        dt1 += 1;
                    }
                }
                //right bank
                CurX = UpdatedRoiStartPtX + roiWidth - i;
                for (j = 0; j < roiHeight; j++)
                {
                    CurY = roiStartPtY + j;
                    if (imageBufferArray[CurY * imageXDim + CurX] < BackgroundCalcCeiling)
                    {    //added in Rev 7
                        dt += imageBufferArray[CurY * imageXDim + CurX];
                        dt1 += 1;
                    }
                }
            }

            if (dt1 < MinNumOfPixels)
            {
                //theStdInterface.printf("background MinNumOfPixels %d\r\n",dt1);
                return 1;
            }
            BackgroundLevel = (dt << BitsShifted) / dt1;

            if (BackgroundLevel > (MaxBackgroundLevel << BitsShifted))
                result = 2;

            //locate centroids of each row
            for (j = 0; j < roiHeight; j++)
            {
                f_dt = 0;
                f_dt1 = 0;
                CurY = roiStartPtY + j;
                for (i = 0; i < roiWidth; i++)
                {
                    CurX = UpdatedRoiStartPtX + i;
                    f_CurPixelValue = (imageBufferArray[CurY * imageXDim + CurX] << BitsShifted) - BackgroundLevel;
                    f_dt += f_CurPixelValue * i;
                    f_dt1 += f_CurPixelValue;
                }
                Centroid[j] = f_dt / (f_dt1 >> BitsShifted);
            }

            //fit
            LinearRegressionMTF(roiHeight, Centroid, ref line_slope, ref offset);

            if ((Math.Abs(line_slope) > 200 * 1024) || (Math.Abs(line_slope) < 5 * 1024))  //line_slope is shifted by BitsShifted
                return 3;
            for (j = 0; j < roiHeight; j++)
            {
                f_dt = 0;
                f_dt1 = 0;
                CurY = roiStartPtY + j;
                //parameters for hamming_window
                tmp_peak_loc = (j * line_slope + offset) >> DoubleBitsShifted; // x=a*y+b
                wid_left = tmp_peak_loc;
                wid_right = roiWidth - 1 - tmp_peak_loc;
                wid = Math.Max(wid_left, wid_right);
                for (i = 0; i < roiWidth; i++)
                {
                    CurX = UpdatedRoiStartPtX + i;
                    f_CurPixelValue = (((imageBufferArray[CurY * imageXDim + CurX] << BitsShifted) - BackgroundLevel) * ((566231 + 471 * CosT[Math.Abs((i - tmp_peak_loc) * 360 / wid)]) >> BitsShifted)) >> BitsShifted;
                    f_dt += f_CurPixelValue * i;
                    f_dt1 += f_CurPixelValue;
                }
                Centroid[j] = f_dt / (f_dt1 >> BitsShifted);
            }
            //fit
            LinearRegressionMTF(roiHeight, Centroid, ref line_slope, ref offset);

            centralPtLoc = (int)((UpdatedRoiStartPtX << BitsShifted) + (((roiHeight >> 1) * line_slope + offset) >> BitsShifted));
            numRows = (int)(((roiHeight *= Math.Abs((int)line_slope)) >> DoubleBitsShifted << DoubleBitsShifted) / Math.Abs(line_slope));

            if (numRows < 10) numRows = roiHeight;
            lineSlope = line_slope;
            //project
            new_offset = (NumBin * (1 - numRows) * line_slope) >> DoubleBitsShifted;
            newNumPixels = roiWidth * NumBin;

            for (j = 0; j < MaxNumCols * NumBin; j++)
            {
                binArray1[j] = 0;
                binArray2[j] = 0;
            }

            for (j = 0; j < numRows; j++)
            {
                for (i = 0; i < roiWidth; i++)
                {
                    ling = (int)((((i << DoubleBitsShifted) - j * line_slope) >> (DoubleBitsShifted - 2)) + 51); //shift -2 becasue NumBin=4
                    binArray1[ling] += 1;
                    binArray2[ling] += (imageBufferArray[(roiStartPtY + j) * imageXDim + UpdatedRoiStartPtX + i] << BitsShifted) - BackgroundLevel;
                }
            }

            //check zero counts
            new_start = (int)(52 + (new_offset >> 1));
            j = 0;
            f_dt = 0;
            f_dt1 = 0;

            for (i = new_start; i < new_start + newNumPixels; i++)
            {
                if (binArray1[i] == 0)
                {
                    numZeroCount++;
                    binArray1[i] = (binArray1[i - 1] + binArray1[i + 1]) >> 1;
                }
                lsf[j] = binArray2[i] / binArray1[i];
                //calculate centroid
                f_dt += lsf[j] * j;
                f_dt1 += lsf[j];
                j++;
            }

            new_centroid = f_dt / (f_dt1 >> BitsShifted);

            if (numZeroCount > 0)
                result = 4;

            //apply hamming window & shift the peak to center
            new_centroid = new_centroid >> BitsShifted;
            wid_left = new_centroid;
            wid_right = newNumPixels - 1 - new_centroid;
            wid = Math.Max(wid_left, wid_right);

            new_center = newNumPixels >> 2 - 1;
            old_center = (int)new_centroid;
            pixel_shift = old_center - new_center;

            if (pixel_shift > 0)
            {
                pixel_shift = -pixel_shift;
                for (i = 0; i < newNumPixels; i++)
                {
                    tmp_lsf[newNumPixels - 1 - i] = lsf[i] * ((566231 + 471 * CosT[Math.Abs((i - new_centroid) * 360 / wid)]) >> BitsShifted) >> BitsShifted;
                }
            }
            else
            {
                for (i = 0; i < newNumPixels; i++)
                {
                    tmp_lsf[i] = lsf[i] * ((566231 + 471 * CosT[Math.Abs((i - new_centroid) * 360 / wid)]) >> BitsShifted) >> BitsShifted;
                }
            }

            for (i = 0; i < newNumPixels + pixel_shift; i++)
            {
                lsf[i - pixel_shift] = tmp_lsf[i];
            }
            for (i = newNumPixels + pixel_shift; i < newNumPixels; i++)
            {
                lsf[newNumPixels - i - 1] = tmp_lsf[i];
            }
            //fft
            for (i = 0, MTF0 = 0, b = 0; i < newNumPixels; i++)
            {
                MTF0 += lsf[i];
                if (lsf[i] > b) b = lsf[i];
            }

            lsfPeakValue = (int)b;

            for (j = 0; j < numOfFreqs; j++)
            {
                g0 = ((long)(pixelPitch * spatialFreqs[j]) << (BitsShifted + LutSizeInBits)) / (1000 * NumBin);
                for (i = 0, a = 0, b = 0; i < newNumPixels; i++)
                {
                    g1 = g0 * i;
                    g1 = g1 - ((g1 >> (BitsShifted + LutSizeInBits)) << (BitsShifted + LutSizeInBits));
                    a += lsf[i] * ((CosLut[g1 >> BitsShifted] + (((g1 - ((g1 >> BitsShifted) << BitsShifted)) * CosDeltaLut[g1 >> BitsShifted]) >> BitsShifted)) >> BitsShifted);
                    b += lsf[i] * ((SinLut[g1 >> BitsShifted] + (((g1 - ((g1 >> BitsShifted) << BitsShifted)) * SinDeltaLut[g1 >> BitsShifted]) >> BitsShifted)) >> BitsShifted);
                }
                tmp_a = ((double)a) / MTF0;
                tmp_b = ((double)b) / MTF0;
                mtfs[j] = ((long)Math.Sqrt(tmp_a * tmp_a + tmp_b * tmp_b)) << BitsShifted;
            }
            return result;
        }

        /// <summary>
        /// 创建 Cos(T) 的值表 ;  0 ~ T ~ 400
        /// </summary>
        private void CreateCosTable()
        {
            for (int angle = 0; angle < 400; angle++)
            {
                CosT[angle] = (long)((Math.Cos(angle * PI / 360)) * 1024);
            }
        }

        /// <summary>
        /// 创建 Lut相关表格数据
        /// </summary>
        private void CreateLutTable()
        {
            CosLut = new long[LutSize];
            SinLut = new long[LutSize];
            CosDeltaLut = new long[LutSize];
            SinDeltaLut = new long[LutSize];
            for (int angle = 0; angle < 512; angle++)
            {
                CosLut[angle] = (long)(Math.Cos(angle * PI / LutHalfSize) * 1024 * 1024);
                SinLut[angle] = (long)(Math.Sin(angle * PI / LutHalfSize) * 1024 * 1024);
                CosDeltaLut[angle] = (long)((Math.Cos((angle + 1) * PI / LutHalfSize) - Math.Cos(angle * PI / LutHalfSize)) * 1024 * 1024);
                SinDeltaLut[angle] = (long)((Math.Sin((angle + 1) * PI / LutHalfSize) - Math.Sin(angle * PI / LutHalfSize)) * 1024 * 1024);
            }
        }

        /// <summary>
        /// 线性回归MTF
        /// </summary>
        private void LinearRegressionMTF(int numberPoints, long[] yPoints, ref long b, ref long a)
        {
            long averageX = 0, averageY = 0, numerator = 0, denominator = 0;
            for (int i = 0; i < numberPoints; i++)
            {
                averageX += i;
                averageY += yPoints[i];
            }

            averageX = ((long)((averageX + numberPoints / 2) / numberPoints));
            averageY = ((long)((averageY + numberPoints / 2) / numberPoints));

            for (int i = 0; i < numberPoints; i++)
            {
                // 分子
                numerator += (i - averageX) * ((long)yPoints[i] - averageY);
                // 分母
                denominator += (i - averageX) * (i - averageX);
            }

            if (denominator == 0)
                denominator++;
            float bf = (float)numerator / (float)(denominator);
            float af = (float)averageY - (bf * (float)averageX);
            b = (long)(bf * 1024);
            a = (long)(af * 1024);
        }
    }
}
