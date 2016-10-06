using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Plotter3
{
    class Plot
    {
        public PointF[] pointsToDraw = new PointF[0];        
        public Color color;

        List<List<PointF>> layers;               
        int minPointsCount = 2000;        
        float minValue, maxValue;
        float deltaMinimalLimit = 10;

        int[] lastBorders;
        float[] lastRange;
        int ActiveLayerIndex;

        const float divisor = 1.7f;

        public Plot(List<PointF> points, float min, float max, Color c)
        {
            minValue = min;
            maxValue = max;
            color = c;

            //layers creating
            layers = new List<List<PointF>>();
            layers.Add(points);
            float count = (float)points.Count / divisor;
            int step = 1;
            do
            {
                count /= divisor;
                step = (int)(points.Count / count);
                layers.Add(MakeConvolution(points, step));
            } while (count > minPointsCount);

            //let's choose the smallest layer
            ActiveLayerIndex = layers.Count - 1;
            lastBorders = new int[] { 0, layers[ActiveLayerIndex].Count - 1 };
            lastRange = new float[] { points[0].X, points[points.Count - 1].X };
        }

        public void UpdateViewRange(float left, float right)
        {
            float RangeDelta = (right - left) - (lastRange[1] - lastRange[0]);
            pointsToDraw = GetPointsInRange(left, right, (int)RangeDelta).ToArray();            
        }

        private List<PointF> GetPointsInRange(float lx, float rx, int scaleDir)
        {
            List<PointF> outPoints = new List<PointF>();

            int currLayerIndex = ActiveLayerIndex;            
            int[] borders = lastBorders;
            int[] newBorders = NearBorders(layers[currLayerIndex], lx, rx, lastBorders[0], lastBorders[1]);
            int pointsCount = lastBorders[1] - lastBorders[0];
            int newPointsCount = newBorders[1] - newBorders[0];

            int deltaPointsCount = newPointsCount;// newPointsCount - pointsCount;


            if ((deltaPointsCount < minPointsCount + deltaMinimalLimit) && (deltaPointsCount > minPointsCount - deltaMinimalLimit))
            {
                borders = newBorders;
                outPoints = layers[currLayerIndex].GetRange(borders[0], borders[1] - borders[0]);

                lastBorders = borders;
                lastRange = new float[] { lx, rx };

                return outPoints;
            }            

            if (deltaPointsCount < minPointsCount)
            {                
                if (currLayerIndex-1 <= 0) // || pointsCount <= minPointsCount)
                {
                    currLayerIndex = ActiveLayerIndex;
                    borders = newBorders;
                }
                else
                    do
                    {
                        currLayerIndex--;
                        borders = NextBorders(layers[currLayerIndex], layers[ActiveLayerIndex], lx, rx, lastBorders[0], lastBorders[1]);
                        pointsCount = borders[1] - borders[0];                        
                    } while (currLayerIndex > 0 && pointsCount < minPointsCount);
            }
            else
            {                
                if (currLayerIndex+1 >= layers.Count - 1 || pointsCount <= minPointsCount)
                {
                    currLayerIndex = ActiveLayerIndex;
                    borders = newBorders;
                }
                else
                    do
                    {
                        currLayerIndex++;
                        borders = NextBorders(layers[currLayerIndex], layers[ActiveLayerIndex], lx, rx, lastBorders[0], lastBorders[1]);
                        pointsCount = borders[1] - borders[0];                        
                    } while (currLayerIndex < layers.Count - 1 && pointsCount > minPointsCount);
            }
            
            outPoints = layers[currLayerIndex].GetRange(borders[0], borders[1] - borders[0]);

            ActiveLayerIndex = currLayerIndex;
            lastBorders = borders;
            lastRange = new float[] { lx, rx };

            return outPoints;
        }        

        private int[] NearBorders(List<PointF> layer, float lx, float rx, int lastLeftIndex, int lastRightIndex)
        {
            int leftI = FindIndex(layer, lx, lastLeftIndex);
            int rightI = FindIndex(layer, rx, lastRightIndex);

            return new int[] { leftI, rightI };
        }

        private int[] NextBorders(List<PointF> layer, List<PointF> lastLayer, float lx, float rx, int lastLeftIndex, int lastRightIndex)
        {            
            float coeff = layer.Count / lastLayer.Count;

            int apprLeftIndex = (int)(lastLeftIndex * coeff);
            int apprRightIndex = (int)(lastRightIndex * coeff);                        

            int leftI = FindIndex(layer, lx, apprLeftIndex);
            int rightI = FindIndex(layer, rx, apprRightIndex);

            return new int[] { leftI, rightI };
        }

        private int FindIndex(List<PointF> layer, float target, int startIndex)
        {
            if (startIndex > layer.Count - 1) startIndex = layer.Count - 1;

            int index = startIndex;
            float value = layer[startIndex].X;

            if (value > target)            
                for (int i = startIndex; i > 0; i--)
                {
                    if (layer[i].X <= target) break;
                    index--;
                }            
            else
                for (int i = startIndex; i < layer.Count; i++)
                {
                    if (layer[i].X >= target) break;
                    index++;
                }
            return index;
        }
        private int[] GetBorders(List<PointF> layer, float lx, float rx)
        {
            int si = 0;
            int ei = layer.Count - 1;
            foreach (PointF p in layer)
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
        private List<PointF> MakeConvolution(List<PointF> src, int step)
        {
            List<PointF> layer = new List<PointF>();
            for (int i = 0; i < src.Count - step; i += step)
            {
                PointF[] minmax = getMinAndMax(src.GetRange(i, step));
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

        private PointF[] getMinAndMax(List<PointF> ps)
        {
            PointF[] minmax = new PointF[] { new PointF(ps[0].X, ps[0].Y), new PointF(0, 0) }; //[0] - min, [1] - max            
            foreach (PointF p in ps)
            {
                if (minmax[0].Y > p.Y) minmax[0] = p;
                if (minmax[1].Y < p.Y) minmax[1] = p;
                //minmax[1].X = minmax[0].X;
            }

            return minmax;
        }
        #endregion
    }
}
