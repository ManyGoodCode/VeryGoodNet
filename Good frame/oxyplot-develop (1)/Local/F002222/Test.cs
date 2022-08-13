using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002222
{
    public class Test
    {
        public static void Part1()
        {
            DataPoint dataPoint1 = new DataPoint(1, 2);
            DataPoint dataPoint2 = new DataPoint(double.NaN, 2);

            bool result1 = dataPoint1.IsDefined();
            bool result2 = dataPoint2.IsDefined();

            string codeString1 = dataPoint1.ToCode();
            string codeString2 = dataPoint2.ToCode();
            string indentString = new string('1', 8);
        }
    }
}
