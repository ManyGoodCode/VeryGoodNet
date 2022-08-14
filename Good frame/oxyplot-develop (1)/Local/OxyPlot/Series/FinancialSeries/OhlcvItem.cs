namespace OxyPlot.Series
{
    using System.Collections.Generic;

    public class OhlcvItem
    {
        public static readonly OhlcvItem Undefined = new OhlcvItem(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);

        public OhlcvItem()
        { 
        }

        public OhlcvItem(
            double x, 
            double open, 
            double high, 
            double low, 
            double close,
            double buyvolume = 0, 
            double sellvolume = 0)
        {
            this.X = x;
            this.Open = open;
            this.High = high;
            this.Low = low;
            this.Close = close;
            this.BuyVolume = buyvolume;
            this.SellVolume = sellvolume;
        }

        public double X { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double BuyVolume { get; set; }
        public double SellVolume { get; set; }

        public static int FindIndex(List<OhlcvItem> items, double targetX, int guessIdx)
        {
            int lastguess = 0;
            int start = 0;
            int end = items.Count - 1;

            while (start <= end)
            {
                if (guessIdx < start)
                {
                    return lastguess;
                }
                else if (guessIdx > end)
                {
                    return end;
                }

                var guessX = items[guessIdx].X;
                if (guessX.Equals(targetX))
                {
                    return guessIdx;
                }
                else if (guessX > targetX)
                {
                    end = guessIdx - 1;
                    if (end < start)
                    {
                        return lastguess;
                    }
                    else if (end == start)
                    {
                        return end;
                    }
                }
                else
                { 
                    start = guessIdx + 1; 
                    lastguess = guessIdx; 
                }

                if (start >= end)
                {
                    return lastguess;
                }

                var endX = items[end].X;
                var startX = items[start].X;

                var m = (end - start + 1) / (endX - startX);
                guessIdx = start + (int)((targetX - startX) * m);
            }

            return lastguess;
        }

        public bool IsValid()
        {
            return !double.IsNaN(this.X) && !double.IsNaN(this.Open) && !double.IsNaN(this.High) && !double.IsNaN(this.Low) && !double.IsNaN(this.Close);
        }
    }
}