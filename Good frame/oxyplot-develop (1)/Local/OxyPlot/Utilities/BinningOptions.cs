namespace OxyPlot
{
    using System;

    public enum BinningOutlierMode
    {
        RejectOutliers,
        IgnoreOutliers,
        CountOutliers,
    }


    public enum BinningIntervalType
    {
        InclusiveLowerBound,
        InclusiveUpperBound,
    }

    public enum BinningExtremeValueMode
    {
        ExcludeExtremeValues,
        IncludeExtremeValues,
    }

    public class BinningOptions
    {
        public BinningOptions(BinningOutlierMode outlierMode, BinningIntervalType intervalType, BinningExtremeValueMode extremeValuesMode)
        {
            if (outlierMode != BinningOutlierMode.RejectOutliers &&
                outlierMode != BinningOutlierMode.CountOutliers &&
                outlierMode != BinningOutlierMode.IgnoreOutliers)
            {
                throw new ArgumentException(nameof(outlierMode), "Unsupported binning outlier mode");
            }

            if (intervalType != BinningIntervalType.InclusiveLowerBound &&
                intervalType != BinningIntervalType.InclusiveUpperBound)
            {
                throw new ArgumentException(nameof(outlierMode), "Unsupported bin interval type");
            }

            if (intervalType != BinningIntervalType.InclusiveLowerBound &&
                intervalType != BinningIntervalType.InclusiveUpperBound)
            {
                throw new ArgumentException(nameof(outlierMode), "Unsupported bin interval type");
            }

            this.OutlierMode = outlierMode;
            this.IntervalType = intervalType;
            this.ExtremeValuesMode = extremeValuesMode;
        }

        public BinningOutlierMode OutlierMode { get; }
        public BinningIntervalType IntervalType { get; }
        public BinningExtremeValueMode ExtremeValuesMode { get; }
    }
}
