using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Plotter4
{
    class Plot
    {
        public Point[] pointsToDraw = new Point[0];
        public Color color;
        public double xRange, yRange;
        public Point[] averagePoints;
        public bool averagePointOn = true;
        public int ActiveLayerIndex;
        public List<Point[]> layers;
        public double minValue, maxValue;

        int minPointsCount = 2500;
        int maxPointsCount = 4000;

        int[] lastBorders;
        double[] lastRange;

        const double divisor = 2f;
        const int buffer = 10; // 10 points

        public Plot(List<Point> points, double min, double max, Color c)
        {
            minValue = min;
            maxValue = max;
            color = c;

            //layers creating
            layers = new List<Point[]>();
            layers.Add(points.ToArray());
            double count = (double)points.Count / divisor;
            int step = 1;
            do
            {
                count /= divisor;
                step = (int)(points.Count / count);
                layers.Add(MakeConvolution(points, step).ToArray());
            } while (count > minPointsCount);

            //let's choose the smallest layer
            ActiveLayerIndex = layers.Count - 1;
            lastBorders = new int[] { 0, layers[ActiveLayerIndex].Length - 1 };
            lastRange = new double[] { points[0].X, points[points.Count - 1].X };

            xRange = lastRange[1] - lastRange[0];
            if (xRange == 0) xRange = 1;
            yRange = max - min;
            if (yRange == 0) yRange = 1;

            //average line
            List<Point> list = new List<Point>();
            step = points.Count / 3400;
            double buffer = 0;
            int a = 0;
            foreach (Point p in points)
            {
                buffer += p.Y;
                if (a >= step)
                {
                    list.Add(new Point(p.X, buffer / a));
                    buffer = 0;
                    a = 0;
                }
                a++;
            }
            list = list.GetRange(0, (list.Count - 1 > 3301) ? 3301 : list.Count);
            if ((list.Count - 1) % 3 != 0)
                do
                {
                    list.Remove(list.Last());
                } while ((list.Count - 1) % 3 != 0);
            averagePoints = list.ToArray();
        }

        public void DisableAverageLine()
        {
            averagePointOn = false;
        }
        public void EnableAverageLine()
        {
            averagePointOn = true;
        }

        public void UpdateViewRange(double left, double right)
        {
            double RangeDelta = (right - left) - (lastRange[1] - lastRange[0]);
            pointsToDraw = GetPointsInRange(left, right, (int)RangeDelta).ToArray();
        }

        private Point[] GetPointsInRange(double lx, double rx, int scaleDir)
        {
            Point[] outPoints;

            int currLayerIndex = ActiveLayerIndex;
            int[] borders = lastBorders;
            int[] newBorders = NearBorders(layers[currLayerIndex], lx, rx, lastBorders[0], lastBorders[1]);
            int[] nextBorders = newBorders;
            int[] tempBorders = newBorders;
            int pointsCount = lastBorders[1] - lastBorders[0];
            int newPointsCount = newBorders[1] - newBorders[0];

            int deltaPointsCount = newPointsCount;// newPointsCount - pointsCount;


            /*if ((deltaPointsCount < minPointsCount + deltaMinimalLimit) && (deltaPointsCount > minPointsCount - deltaMinimalLimit))
            {
                borders = newBorders;
                outPoints = layers[currLayerIndex].Skip(borders[0]).Take(borders[1] - borders[0]).ToArray();

                lastBorders = borders;
                lastRange = new double[] { lx, rx };

                return outPoints;
            }  */

            if (deltaPointsCount < minPointsCount)
            {
                if (currLayerIndex - 1 < 0 || newPointsCount >= minPointsCount)
                {
                    currLayerIndex = ActiveLayerIndex;
                    borders = newBorders;
                }
                else
                {
                    tempBorders = NextBorders(layers[currLayerIndex], layers[ActiveLayerIndex], lx, rx, lastBorders[0], lastBorders[1]);
                    do
                    {
                        nextBorders = tempBorders;
                        newBorders = nextBorders;
                        currLayerIndex--;
                        if (currLayerIndex >= 0) tempBorders = NextBorders(layers[currLayerIndex], layers[ActiveLayerIndex], lx, rx, lastBorders[0], lastBorders[1]);
                        else break;
                        newPointsCount = tempBorders[1] - tempBorders[0];
                    } while (newPointsCount < minPointsCount);
                    currLayerIndex++;
                }
            }
            else if (deltaPointsCount > maxPointsCount)
            {
                if (currLayerIndex + 1 >= layers.Count || newPointsCount <= maxPointsCount)
                {
                    currLayerIndex = ActiveLayerIndex;
                    borders = newBorders;
                }
                else
                    do
                    {
                        currLayerIndex++;
                        newBorders = NextBorders(layers[currLayerIndex], layers[ActiveLayerIndex], lx, rx, lastBorders[0], lastBorders[1]);
                        newPointsCount = newBorders[1] - newBorders[0];
                    } while (currLayerIndex < layers.Count - 1 && newPointsCount > maxPointsCount);
            }
            borders = newBorders;

            //danger!
            int leftIndex = borders[0] - buffer;
            if (leftIndex < 0) leftIndex = 0;
            int rightIndex = borders[1] + buffer;
            if (rightIndex > layers[currLayerIndex].Length - 1) rightIndex = layers[currLayerIndex].Length - 1;

            outPoints = layers[currLayerIndex].Skip(leftIndex).Take(rightIndex - leftIndex).ToArray();

            ActiveLayerIndex = currLayerIndex;
            lastBorders = borders;
            lastRange = new double[] { lx, rx };

            return outPoints;
        }

        private int[] NearBorders(Point[] layer, double lx, double rx, int lastLeftIndex, int lastRightIndex)
        {
            int leftI = FindIndex(layer, lx, lastLeftIndex);
            int rightI = FindIndex(layer, rx, lastRightIndex);

            return new int[] { leftI, rightI };
        }

        private int[] NextBorders(Point[] layer, Point[] lastLayer, double lx, double rx, int lastLeftIndex, int lastRightIndex)
        {
            double coeff = layer.Length / lastLayer.Length;

            int apprLeftIndex = (int)(lastLeftIndex * coeff);
            int apprRightIndex = (int)(lastRightIndex * coeff);

            int leftI = FindIndex(layer, lx, apprLeftIndex);
            int rightI = FindIndex(layer, rx, apprRightIndex);

            return new int[] { leftI, rightI };
        }

        private int FindIndex(Point[] layer, double target, int startIndex)
        {
            if (startIndex > layer.Length - 1) startIndex = layer.Length - 1;

            int index = startIndex;
            double value = layer[startIndex].X;

            if (value > target)
                for (int i = startIndex; i > 0; i--)
                {
                    if (layer[i].X <= target) break;
                    index--;
                }
            else
                for (int i = startIndex; i < layer.Length; i++)
                {
                    if (layer[i].X >= target) break;
                    index++;
                }
            return index;
        }
        private int[] GetBorders(List<Point> layer, double lx, double rx)
        {
            int si = 0;
            int ei = layer.Count - 1;
            foreach (Point p in layer)
            {
                if (p.X >= lx) break;
                si++;
            }
            for (int i = layer.Count - 1; i > 0; i--)
            {
                if (layer[i].X <= rx) break;
                ei--;
            }
            return new int[] { si, ei };
        }


        #region Additional Functions
        private List<Point> MakeConvolution(List<Point> src, int step)
        {
            List<Point> layer = new List<Point>();
            for (int i = 0; i < src.Count - step; i += step)
            {
                Point[] minmax = getMinAndMax(src.GetRange(i, step));
                if (minmax[0].X > minmax[1].X)
                {
                    layer.Add(minmax[1]);
                    layer.Add(minmax[0]);
                }
                else
                {
                    layer.Add(minmax[0]);
                    layer.Add(minmax[1]);
                }
            }
            return layer;
        }

        private Point[] getMinAndMax(List<Point> ps)
        {
            Point[] minmax = new Point[] { new Point(ps[0].X, ps[0].Y), new Point(0, 0) }; //[0] - min, [1] - max            
            foreach (Point p in ps)
            {
                if (minmax[0].Y > p.Y) minmax[0] = p;
                if (minmax[1].Y < p.Y) minmax[1] = p;
                //minmax[1].X = minmax[0].X;
            }

            return minmax;
        }
        #endregion

        public static Dictionary<byte, Plot> CreatePlotsFromFile(List<PlotParams> ps, string path)
        {
            Dictionary<byte, Plot> output = new Dictionary<byte, Plot>();

            byte[] signals = ps.Select(p => p.signalCode).ToArray();
            Dictionary<byte, long[]> parsed = Parser.parseLM2(path, signals);

            foreach (KeyValuePair<byte, long[]> pair in parsed)
            {
                List<Point> points = new List<Point>();
                long[] data = pair.Value;
                double max = 0;
                double min = (60.0 / (data[1] - data[0] * 1024 * 16e-9));
                double lastrpm = 0;

                for (int i = 1; i < data.Length; i += 1)
                {
                    double tt = data[i] - data[i - 1];
                    if (tt == 0) continue;
                    double rpm = 60.0 / (tt * 1024 * 16e-9);                    
                    points.Add(new Point((data[i] * 16e-9), rpm));
                    if (rpm > max) max = rpm;
                    else if (rpm < min) min = rpm;
                    lastrpm = rpm;
                }

                Plot plot = new Plot(points, min, max, ps.Find(p => p.signalCode == pair.Key).color);
                output[pair.Key] = plot;
            }
            return output;
        }
    }
}
