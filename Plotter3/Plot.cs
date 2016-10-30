using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Plotter3
{    
    class Plot
    {        
        /*{
            { 1, new int[] {1} },
            { 2, new int[] {1,2} },
            { 3, new int[] {1,3} },
            { 4, new int[] {1,2,4} },
            { 5, new int[] {1,5} },
            { 6, new int[] {1,3,6} },
            { 7, new int[] {1,7} },
            { 8, new int[] {1} },
        }; idiotizm*/

        public PointD[] pointsToDraw = new PointD[0];        
        public Color color;
        public double xRange, yRange;
        public PointD[] averagePoints;        
        public bool averagePointOn = true;
        public int ActiveLayerIndex;
        public List<PointD[]> layers = new List<PointD[]>();
        public double minValue, maxValue;        

        int minPointsCount = 2500;
        int maxPointsCount = 4000;        

        int[] lastBorders;
        double[] lastRange;        

        const double divisor = 2f;
        const int buffer = 10; // 10 points

        public Plot(List<PointD> points, double min, double max, Color c)
        {
            minValue = min;
            maxValue = max;
            color = c;

            //layers creating
            layers = new List<PointD[]>();
            layers.Add(points.ToArray());
            double count = points.Count / divisor;
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
            List<PointD> list = new List<PointD>();
            step = points.Count / 3400;
            double buffer = 0;
            int a = 0;
            foreach (PointD p in points)
            {
                buffer += p.Y;
                if (a >= step)
                {
                    list.Add(new PointD(p.X, buffer / a));
                    buffer = 0;
                    a = 0;
                }
                a++;
            }
            list = list.GetRange(0, (list.Count-1 > 3301) ? 3301 : list.Count);
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

        private PointD[] GetPointsInRange(double lx, double rx, int scaleDir)
        {
            PointD[] outPoints;

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
                if (currLayerIndex+1 >= layers.Count || newPointsCount <= maxPointsCount)
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
            if (rightIndex > layers[currLayerIndex].Length-1) rightIndex = layers[currLayerIndex].Length - 1;

            outPoints = layers[currLayerIndex].Skip(leftIndex).Take(rightIndex - leftIndex).ToArray();

            ActiveLayerIndex = currLayerIndex;
            lastBorders = borders;
            lastRange = new double[] { lx, rx };

            return outPoints;
        }        

        private int[] NearBorders(PointD[] layer, double lx, double rx, int lastLeftIndex, int lastRightIndex)
        {
            int leftI = FindIndex(layer, lx, lastLeftIndex);
            int rightI = FindIndex(layer, rx, lastRightIndex);

            return new int[] { leftI, rightI };
        }

        private int[] NextBorders(PointD[] layer, PointD[] lastLayer, double lx, double rx, int lastLeftIndex, int lastRightIndex)
        {            
            double coeff = layer.Length / lastLayer.Length;

            int apprLeftIndex = (int)(lastLeftIndex * coeff);
            int apprRightIndex = (int)(lastRightIndex * coeff);                        

            int leftI = FindIndex(layer, lx, apprLeftIndex);
            int rightI = FindIndex(layer, rx, apprRightIndex);

            return new int[] { leftI, rightI };
        }

        private int FindIndex(PointD[] layer, double target, int startIndex)
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
        private int[] GetBorders(List<PointD> layer, double lx, double rx)
        {
            int si = 0;
            int ei = layer.Count - 1;
            foreach (PointD p in layer)
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
        private List<PointD> MakeConvolution(List<PointD> src, int step)
        {
            List<PointD> layer = new List<PointD>();
            for (int i = 0; i < src.Count - step; i += step)
            {
                PointD[] minmax = getMinAndMax(src.GetRange(i, step));
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

        private PointD[] getMinAndMax(List<PointD> ps)
        {
            PointD[] minmax = new PointD[] { new PointD(ps[0].X, ps[0].Y), new PointD(0, 0) }; //[0] - min, [1] - max            
            foreach (PointD p in ps)
            {
                if (minmax[0].Y > p.Y) minmax[0] = p;
                if (minmax[1].Y < p.Y) minmax[1] = p;
                //minmax[1].X = minmax[0].X;
            }
            minmax[0].X = ps[0].X;
            minmax[1].X = ps[ps.Count - 1].X;

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
                List<PointD> points = new List<PointD>();
                long[] data = pair.Value;
                double max = 0;
                double min = (double)(60.0 / (data[1] - data[0] * 1024 * 16e-9));
                double lastrpm = 0;

                for (int i = 1; i < data.Length; i += 1)
                {
                    double tt = data[i] - data[i - 1];
                    if (tt == 0) continue;
                    double rpm = 60.0 / (tt * 1024 * 16e-9);
                    double rpmf = (double)rpm;
                    points.Add(new PointD((double)(data[i] * 16e-9), rpmf));
                    if (rpm > max) max = rpmf;
                    else if (rpm < min) min = rpmf;
                    lastrpm = rpmf;
                }

                Plot plot = new Plot(points, min, max, ps.Find(p => p.signalCode == pair.Key).color);
                output[pair.Key] = plot;
            }
            return output;
        }
    }    

    class PlotStretch
    {
        Dictionary<int, int[]> factors = new Dictionary<int, int[]>();

        public PointD[] pointsToDraw = new PointD[0];
        public Color color;
        public double xRange, yRange;
        public PointD[] averagePoints;
        public bool averagePointOn = true;
        public int ActiveLayerIndex = 0;        

        /*public delegate void AddPointDunction(PointD p);
        AddPointDunction ap;*/
        public List<List<PointD>> layers = new List<List<PointD>>()
        {
            new List<PointD>()            
        };
        public double minValue, maxValue;

        int minPointsCount = 2500;
        int maxPointsCount = 4000;
        int CurrentLayerMaxPoints;        

        int[] lastBorders;
        double[] lastRange;

        const double divisor = 2;
        const int buffer = 10; // 10 points

        public PlotStretch(Color c)
        {
            CurrentLayerMaxPoints = minPointsCount;

            color = c;
            xRange = yRange = minValue = maxValue = 0;
            //ap = AddTwoFirstPoints;
        }
        
        public void AddPoint(PointD p)
        {
            layers[0].Add(p);
            for (int i = 1; i < layers.Count; i++)
            {
                List<PointD> layer = layers[i];
                if (layers[0].Count % (i + 1) == 0)
                {

                }
                    
            }
            if (layers.Count > CurrentLayerMaxPoints)
            {
                layers.Add(new List<PointD>());
                CurrentLayerMaxPoints *= 2;
            }   
        }        

        private PointD[] getMinAndMax(List<PointD> ps)
        {
            PointD[] minmax = new PointD[] { new PointD(ps[0].X, ps[0].Y), new PointD(0, 0) }; //[0] - min, [1] - max            
            foreach (PointD p in ps)
            {
                if (minmax[0].Y > p.Y) minmax[0] = p;
                if (minmax[1].Y < p.Y) minmax[1] = p;
                //minmax[1].X = minmax[0].X;
            }

            /*PointD buff;
            if (minmax[0].X > minmax[1].X)
            {
                buff = minmax[0].X
            }
            else
            {
                layer.Add(minmax[0]);
                layer.Add(minmax[1]);
            }  */          

            return minmax;
        }
    }
    
    struct PointD
    {
        public double X,Y;

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public PointF ToPointF()
        {
            return new PointF((float)X, (float)Y);
        }
    }
}

/*private void AddTwoFirstPoints(PointD p)
        {
            layers[0].Add(p);
            if (layers[0].Count >= 2) ap = AddOtherPoints;
        }

        private void AddOtherPoints(PointD p)
        {
            int layerI = (int)Math.Floor(Math.Log(layers[0].Count, 2));
        }


        private void FillFactors(int count)
        {
            factors[1] = new int[] { 1 };
            for(int i = 2; i < count; i++)
            {
                List<int> buf = new List<int>() { i };
                for (int i2 = 1; i2 < i; i2++)
                    if (i % i2 == 0) buf.Add(i2);
                factors[i] = buf.ToArray();
            }
        }*/
