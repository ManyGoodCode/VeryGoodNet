using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F002222
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DataPoint dataPoint1 = new DataPoint(1,2);
            DataPoint dataPoint2 = new DataPoint(double.NaN, 2);

            bool result1 = dataPoint1.IsDefined();
            bool result2 = dataPoint2.IsDefined();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
