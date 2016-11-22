using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.ComponentModel;

namespace Plotter4
{
    class LayersManager : DrawStruct
    {
        //public PointF[] outputArr = new PointF[0];
        public int leftI, rightI;        
        public double minX, maxX, minY, maxY;        

        List<PointD[]> layers = new List<PointD[]>();                
        public int ActiveIndex = 0;
        int lastLeftIndex, lastRightIndex;
        double lastLeftX, lastRightX;        

        const int divisor = 2;
        const int buffer = 10; // 10 points
        const double MAX_POINTS = 3000;
        const double MIN_POINTS = 1500;

        //readonly double TenPercLength = 0;
        readonly string id;
        public LayersManager(Pen p, IList<PointD> ps, double min, double max, string id, Action<double> callback = null) : base(p)
        {
            PointD[] points = ps.ToArray();
            layers.Add(points);

            minY = min;
            maxY = max;
            minX = points[0].X;
            maxX = points.Last().X;

            leftI = 0;
            rightI = 200;

            this.id = id;

            double count = points.Length / divisor;
            int step = 1;
            long totalCount = 0;

            /*TenPercLength = points[points.Length / 10 * 2].X - points[points.Length / 10].X;
            for (int i = points.Length / 10; i < points.Length; i += points.Length/10)
                TenPercLength = (TenPercLength + points[i - points.Length / 10].X - points[points.Length / 10].X) / 2;*/

            //fill layers
            do
            {
                count /= divisor;
                step = (int)(points.Length / count);    
                layers.Add(MakeConvolution(points, step, callback).ToArray());                
                totalCount += layers[layers.Count-1].Length;
                double percentage = (double)totalCount / (points.Length);                
                callback(percentage);
            } while (count > 2500);

            ActiveIndex = layers.Count - 1;
            outputArr = ToPointFArr(layers[ActiveIndex]);
            rightI = outputArr.Length - 1;

            SetCurrentLayer(layers.Count - 1, 0, layers.Last().Length - 1);

            callback(1);
            points = null;                           
        }

        private void SetCurrentLayer(int layerIndex, int leftIndex, int rightIndex)
        {
            ActiveIndex = layerIndex;
            lastLeftIndex = leftIndex;
            lastRightIndex = rightIndex;
            lastLeftX = layers[ActiveIndex][leftIndex].X;
            lastRightX = layers[ActiveIndex][rightIndex].X;
        }

        override public void UpdateOutputArray(double leftX, double rightX)
        {
            //calc count
            int leftIndex = lastLeftIndex;
            int rightIndex = lastRightIndex;

            if (leftX < layers[ActiveIndex][0].X) leftX = layers[ActiveIndex][0].X;
            //if (rightX > layers[ActiveIndex].Last().X) rightX = layers[ActiveIndex].Last().X;

            #region Calc Borders
            if (leftX > lastLeftX)
            {
                double x = lastLeftX;                
                while (x < leftX)
                {
                    leftIndex++;
                    x = layers[ActiveIndex][leftIndex].X;
                }
                    
            }
            else if (leftX < lastLeftX)
            {
                double x = lastLeftX;
                while (x > leftX && leftIndex > 0)
                {
                    leftIndex--;
                    x = layers[ActiveIndex][leftIndex].X;
                }
            }

            if (rightX > lastRightX)
            {
                double x = lastRightX;
                while (x < rightX && rightIndex < layers[ActiveIndex].Length - 1)
                {
                    rightIndex++;
                    x = layers[ActiveIndex][rightIndex].X;
                }

            }
            else if (rightX < lastRightX)// ((layers[ActiveIndex].Length-1 > lastRightIndex) ? layers[ActiveIndex][lastRightIndex + 1].X : lastRightX)) //layers[ActiveIndex][lastRightIndex+1].X
            {
                double x = lastRightX;
                while (x > rightX)
                {
                    rightIndex--;
                    x = layers[ActiveIndex][rightIndex].X;
                }
                rightIndex++;
            }
            #endregion

            int pointsCount = rightIndex - leftIndex;
            int newActiveIndex = ActiveIndex;

            if (pointsCount > MAX_POINTS)                         
                while(newActiveIndex < layers.Count-1 && pointsCount / 2 > MAX_POINTS)
                {
                    leftIndex /= divisor;
                    rightIndex /= divisor;
                    pointsCount /= divisor;
                    newActiveIndex++;
                }
            else if (pointsCount < MIN_POINTS)
                while (newActiveIndex > 0 && pointsCount * 2 < MIN_POINTS)
                {
                    leftIndex *= divisor;
                    rightIndex *= divisor;
                    pointsCount *= divisor;
                    newActiveIndex--;
                }

            //check for borders
            rightIndex += 5; //buffer right
            leftIndex -= 5; //buffer left
            if (rightIndex > layers[newActiveIndex].Length - 1) rightIndex = layers[newActiveIndex].Length - 1;
            if (leftIndex < 0) leftIndex = 0;

            //applying 
            lastLeftIndex = leftIndex;
            lastRightIndex = rightIndex;
            lastLeftX = layers[newActiveIndex][leftIndex].X;
            lastRightX = layers[newActiveIndex][rightIndex].X;
            ActiveIndex = newActiveIndex;

            outputArr = ToPointFArr(layers[ActiveIndex].Skip(leftIndex).Take(rightIndex - leftIndex + 1).ToArray());

            //debug
            log = string.Format("ActiveIndex: {0}, PointsCount: {4}, leftIndex: {1}, rightIndex: {2}, toRightEnd: {3}", ActiveIndex, lastLeftIndex, lastRightIndex, layers[ActiveIndex].Length-lastRightIndex-1, pointsCount);
        }

        #region Additional Functions
        private PointD[] MakeConvolution(PointD[] src, int step, Action<double> callback = null)
        {
            PointD[] layer = new PointD[((src.Length)/step)*2+1];
            int c = 0;            
            for (int i = 0; i < src.Length - step; i += step)
            {
                PointD[] minmax = getMinAndMax(src, i, step);
                if (minmax[0].X > minmax[1].X)
                {
                    layer[c] = minmax[1];
                    layer[c + 1] = minmax[0];
                }
                else
                {
                    layer[c] = minmax[0];
                    layer[c + 1] = minmax[1];
                }
                c += 2;
            }
            if (src.Length % step != 0)
                layer[layer.Length - 1] = src.Last();
            return layer;
        }

        private PointD[] getMinAndMax(PointD[] ps, int index, int step)
        {
            PointD[] minmax = new PointD[] { new PointD(ps[index].X, ps[index].Y), new PointD(ps[index].X, ps[index].Y) }; //[0] - min, [1] - max            
            for (int i = index; i < index + step; i++)
            {
                PointD p = ps[i];
                if (minmax[0].Y > p.Y) minmax[0] = p;
                if (minmax[1].Y < p.Y) minmax[1] = p;
                //minmax[1].X = minmax[0].X;
            }
            minmax[0].X = ps[index].X;
            minmax[1].X = ps[index + step].X;

            return minmax;
        }
        #endregion

        //Some staff
        public static PointF[] ToPointFArr(PointD[] arr)
        {
            PointF[] output = new PointF[arr.Length];
            Parallel.For(0, arr.Length, (i) =>
            {
                output[i] = arr[i].ToPointF();
            });
            return output;
        }
    }
}

/*          BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerAsync();
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;    
            */
/*public DataManager(string path, int layersCount)
        {
            for (int layerI = 0; layerI < layersCount; layerI++)
            {
                List<PointD> layer = new List<PointD>();
                StreamReader sr = new StreamReader(string.Format("{1}_{2}.dat", path, layerI.ToString()));
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    string[] s = line.Split(':');
                    layer.Add(new PointD(double.Parse(s[0]), double.Parse(s[1])));
                }
                layers.Add(layer.ToArray());
                sr.Close();
            }
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("End");
        }
                
        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\temp");
            int layerI = 0;
            foreach (PointD[] a in layers) {
                StreamWriter sw = new StreamWriter(string.Format("{0}\\temp\\{1}_{2}.dat", Environment.CurrentDirectory, (id.Length > 1) ? id : "0"+id, layerI.ToString()));
                foreach (PointD p in a)
                    sw.WriteLine(p.X + ":" + p.Y);
                sw.Close();
                layerI++;
            }
        } */
