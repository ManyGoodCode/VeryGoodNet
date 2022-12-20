
namespace OxyPlot.Axes
{
    using System;
    public class AxisChangedEventArgs : EventArgs
    {
        public AxisChangedEventArgs(AxisChangeTypes changeType, double deltaMinimum, double deltaMaximum)
        {
            this.ChangeType = changeType;
            this.DeltaMinimum = deltaMinimum;
            this.DeltaMaximum = deltaMaximum;
        }

        public AxisChangeTypes ChangeType { get; private set; }

        public double DeltaMinimum { get; private set; }
        public double DeltaMaximum { get; private set; }
    }
}
