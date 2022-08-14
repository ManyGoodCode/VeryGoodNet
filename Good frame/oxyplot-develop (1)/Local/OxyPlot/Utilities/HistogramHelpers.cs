namespace OxyPlot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot.Series;

    public static class HistogramHelpers
    {
        public static List<double> CreateUniformBins(double start, double end, int binCount)
        {
            if (binCount < 1)
            {
                throw new ArgumentException("The bin count must be positive.", nameof(binCount));
            }

            if (double.IsNaN(start) || double.IsInfinity(start))
            {
                throw new ArgumentException("The start may not be NaN or infinite.", nameof(start));
            }

            if (double.IsNaN(end) || double.IsInfinity(end))
            {
                throw new ArgumentException("The start may not be NaN or infinite.", nameof(end));
            }

            if (end <= start)
            {
                throw new ArgumentException("The end must be strictly greater than the start.", nameof(end));
            }

            List<double> binBreaks = new List<double>(binCount + 1);

            binBreaks.Add(start);

            for (int i = 1; i < binCount; i++)
            {
                binBreaks.Add(start + ((end - start) * i / binCount));
            }

            binBreaks.Add(end);

            return binBreaks;
        }

        public static IList<HistogramItem> Collect(IEnumerable<double> samples, IEnumerable<double> binBreaks, BinningOptions binningOptions)
        {
            if (samples is null)
            {
                throw new ArgumentNullException(nameof(samples));
            }

            if (binBreaks is null)
            {
                throw new ArgumentNullException(nameof(binBreaks));
            }

            if (binningOptions is null)
            {
                throw new ArgumentNullException(nameof(binningOptions));
            }
            List<double> orderedBreaks = binBreaks.Distinct().OrderBy(b => b).ToList();

            if (orderedBreaks.Count < 2)
            {
                throw new ArgumentException("Atleast 2 distinct bin breaks must be provided.", nameof(binBreaks));
            }

            if (orderedBreaks.Any(d => double.IsNaN(d) || double.IsInfinity(d)))
            {
                throw new ArgumentException($"Bin Breaks may not be NaN or infinite.", nameof(binBreaks));
            }

            int[] counts = new int[orderedBreaks.Count - 1];
            long total = 0;

            foreach (double sample in samples)
            {
                if (double.IsNaN(sample) || double.IsInfinity(sample))
                {
                    throw new ArgumentException($"Samples may not be NaN or infinite.", nameof(samples));
                }

                int idx = orderedBreaks.BinarySearch(sample);

                bool placed = false;

                if (idx >= 0)
                {
                    if (binningOptions.IntervalType == BinningIntervalType.InclusiveUpperBound)
                    {
                        if (idx > 0)
                        {
                            counts[idx - 1] += 1;
                            placed = true;
                        }
                        else if (binningOptions.ExtremeValuesMode == BinningExtremeValueMode.IncludeExtremeValues)
                        {
                            counts[idx] += 1;
                            placed = true;
                        }
                    }
                    else
                    {
                        if (idx < counts.Length)
                        {
                            counts[idx] += 1;
                            placed = true;
                        }
                        else if (binningOptions.ExtremeValuesMode == BinningExtremeValueMode.IncludeExtremeValues)
                        {
                            counts[idx - 1] += 1;
                            placed = true;
                        }
                    }
                }
                else
                {
                    // inexact match: place in lower bin
                    idx = ~idx - 1;

                    if (idx >= 0 && idx < counts.Length)
                    {
                        counts[idx] += 1;
                        placed = true;
                    }
                }

                if (placed)
                {
                    total++;
                }
                else
                {
                    switch (binningOptions.OutlierMode)
                    {
                        case BinningOutlierMode.RejectOutliers:
                            throw new ArgumentOutOfRangeException(nameof(samples), $"Sample with value {sample} could not be assigned to any bin.");
                        case BinningOutlierMode.IgnoreOutliers:
                            break;
                        case BinningOutlierMode.CountOutliers:
                            total++;
                            break;
                    }
                }
            }

            if (total == 0)
            {
                // in this case, we set total to 1 so that all the areas end up as 0 rather than NaN
                total = 1;
            }

            List<HistogramItem> items = new List<HistogramItem>(counts.Length);

            for (int i = 0; i < orderedBreaks.Count - 1; i++)
            {
                int count = counts[i];
                items.Add(new HistogramItem(orderedBreaks[i], orderedBreaks[i + 1], (double)count / total, count));
            }

            return items;
        }
    }
}
