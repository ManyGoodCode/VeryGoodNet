namespace OxyPlot
{
    using System;
    using System.Collections.Generic;

    public class Decimator
    {
        public static void Decimate(List<ScreenPoint> input, List<ScreenPoint> output)
        {
            if (input == null || input.Count == 0)
            {
                return;
            }

            var point = input[0];
            double currentX = Math.Round(point.X);
            double currentMinY = Math.Round(point.Y);
            double currentMaxY = currentMinY;
            double currentFirstY = currentMinY;
            double currentLastY = currentMinY;
            for (var i = 1; i < input.Count; ++i)
            {
                point = input[i];
                double newX = Math.Round(point.X);
                double newY = Math.Round(point.Y);
                if (newX != currentX)
                {
                    AddVerticalPoints(output, currentX, currentFirstY, currentLastY, currentMinY, currentMaxY);
                    currentFirstY = currentLastY = currentMinY = currentMaxY = newY;
                    currentX = newX;
                    continue;
                }

                if (newY < currentMinY)
                {
                    currentMinY = newY;
                }

                if (newY > currentMaxY)
                {
                    currentMaxY = newY;
                }

                currentLastY = newY;
            }

            currentLastY = currentFirstY == currentMinY ? currentMaxY : currentMinY;
            AddVerticalPoints(output, currentX, currentFirstY, currentLastY, currentMinY, currentMaxY);
        }


        private static void AddVerticalPoints(
            List<ScreenPoint> result,
            double x,
            double firstY,
            double lastY,
            double minY,
            double maxY)
        {
            result.Add(new ScreenPoint(x, firstY));
            if (firstY == minY)
            {
                if (minY != maxY)
                {
                    result.Add(new ScreenPoint(x, maxY));
                }

                if (maxY != lastY)
                {
                    result.Add(new ScreenPoint(x, lastY));
                }

                return;
            }

            if (firstY == maxY)
            {
                if (maxY != minY)
                {
                    result.Add(new ScreenPoint(x, minY));
                }

                if (minY != lastY)
                {
                    result.Add(new ScreenPoint(x, lastY));
                }

                return;
            }

            if (lastY == minY)
            {
                if (minY != maxY)
                {
                    result.Add(new ScreenPoint(x, maxY));
                }
            }
            else if (lastY == maxY)
            {
                if (maxY != minY)
                {
                    result.Add(new ScreenPoint(x, minY));
                }
            }
            else
            {
                result.Add(new ScreenPoint(x, minY));
                result.Add(new ScreenPoint(x, maxY));
            }

            result.Add(new ScreenPoint(x, lastY));
        }
    }
}