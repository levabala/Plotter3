using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Plotter4
{
    class DataManager
    {
        public PointD[] outputArr = new PointD[0];
        public double minValue, maxValue;

        List<PointD[]> layers = new List<PointD[]>();                
        int ActiveIndex = 0;

        const double divisor = 2f;
        const int buffer = 10; // 10 points
        public DataManager(IList<PointD> ps, Size screenSize)
        {
            PointD[] points = ps.ToArray();
            layers.Add(points);
            double count = points.Length / divisor;
            int step = 1;
            do
            {
                count /= divisor;
                step = (int)(points.Length / count);    
                layers.Add(MakeConvolution(points, step).ToArray());
            } while (count > screenSize.Width);
        }


        #region Additional Functions
        private PointD[] MakeConvolution(PointD[] src, int step)
        {
            PointD[] layer = new PointD[((int)Math.Floor((src.Length - step) / (double)step))*2];
            int c = 0;
            for (int i = 0; i < src.Length - step; i += step)
            {
                PointD[] minmax = getMinAndMax(src.Skip(i).Take(step).ToArray());
                if (minmax[0].X > minmax[1].X)
                {
                    layer[c] = minmax[1];
                    layer[c+1] = minmax[0];
                }
                else
                {
                    layer[c] = minmax[2];
                    layer[c + 1] = minmax[0];
                }
                c += 2;
            }
            return layer;
        }

        private PointD[] getMinAndMax(PointD[] ps)
        {
            PointD[] minmax = new PointD[] { new PointD(ps[0].X, ps[0].Y), new PointD(0, 0) }; //[0] - min, [1] - max            
            foreach (PointD p in ps)
            {
                if (minmax[0].Y > p.Y) minmax[0] = p;
                if (minmax[1].Y < p.Y) minmax[1] = p;
                //minmax[1].X = minmax[0].X;
            }
            minmax[0].X = ps[0].X;
            minmax[1].X = ps[ps.Length - 1].X;

            return minmax;
        }
        #endregion
    }
}
