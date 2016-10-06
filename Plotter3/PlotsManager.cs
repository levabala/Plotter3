using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Plotter3
{
    class PlotsManager
    {
        public List<Plot> plots = new List<Plot>(); 
        public float absMin, absMax;
        public float xRange = 1;
        public float maxLeftX, maxRightX;
        public PlotsManager() { }

        public void CreatePlotFromFile(string path, Color c)
        {
            Int64[] data = Parser.parseLM(path, 242);
            List<PointF> points = new List<PointF>();
            float max = 0;
            float min = data[1] - data[0];
            float lastrpm = 0;

            for (int i = 1; i < data.Length; i += 1)
            {
                double tt = data[i] - data[i - 1];
                double rpm = 60.0 / (tt * 1024 * 16e-9);
                float rpmf = (float)rpm;
                points.Add(new PointF((float)(data[i] * 16e-3), rpmf));
                if (rpm > max) max = rpmf;
                else if (rpm < min) min = rpmf;
                lastrpm = rpmf;
            }

            if (min < absMin || plots.Count == 0) absMin = min;
            if (max > absMax || plots.Count == 0) absMax = max;

            float xrange = points[points.Count - 1].X - points[0].X;
            if (xrange > xRange || plots.Count == 0) xRange = xrange;
            if (points[0].X > maxLeftX || plots.Count == 0) maxLeftX = points[0].X;
            if (points[points.Count - 1].X > maxRightX || plots.Count == 0) maxRightX = points[points.Count - 1].X;

            plots.Add(new Plot(points, min, max, c));
        }
        public void AddPlot(Plot p)
        {
            plots.Add(p);
        }

        public void UpdateViewRange(float left, float right)
        {
            foreach (Plot p in plots)
                p.UpdateViewRange(left, right);
        }

        #region PrivateFunctions
        
        #endregion
    }
}
