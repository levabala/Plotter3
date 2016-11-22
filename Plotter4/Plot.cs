using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;

namespace Plotter4
{
    abstract class Plot
    {
        protected List<DrawStruct> dms = new List<DrawStruct>();
        protected Matrix m = new Matrix();
        protected Control control = null;
        protected string PLOTTER_TRANSFORM_REGKEY = "";
        protected PointF lastMousePoint;
        protected PointF lastMousePointD;
        protected Timer wheelEndTimer = new Timer();

        protected double LeftBorderX, RightBorderX;

        public delegate void ParseFun(string path, string[] args, Action<double> progressCallBack = null);
        public ParseFun Parse;
        public delegate void VisualizeFun(object sender, PaintEventArgs e);
        public VisualizeFun Visualize;
        public delegate void NewDMLoadedFun(double minX, double maxX, double minY, double maxY);
        public NewDMLoadedFun NewDMLoaded;
        public delegate void MatrixUpdateFun(Matrix newMatrix);
        public MatrixUpdateFun MatrixUpdate;
        //public delegate void ControlFun(object sender, EventArgs e);
        //public ControlFun ControlF;      

        public List<Pen> pallete = new List<Pen>()
        {
            new Pen(Color.DarkGreen, 2),
            new Pen(Color.DarkRed, 2),
            new Pen(Color.DarkBlue, 2),
            new Pen(Color.Brown, 2),
            new Pen(Color.SandyBrown, 2)
        };

        public Plot(Control c)
        {
            Parse = ParseRaw;
            Visualize = Draw;
            MatrixUpdate += OnMatrixUpdate;                   

            SetTargetControl(c);

            wheelEndTimer.Interval = 30;
            wheelEndTimer.Tick += WheelEndTimer_Tick;
        }        

        public void SetTargetControl(Control c)
        {
            if (control != null) control.Paint -= Draw;
            control = c;
            PLOTTER_TRANSFORM_REGKEY = "plotter_transform_" + control.Name;

            c.Paint += Draw;
            c.MouseMove += C_MouseMove;
            c.MouseWheel += C_MouseWheel;
            c.MouseDown += C_MouseDown;
            c.MouseUp += C_MouseUp;
        }

        private void C_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                SetLeftXAndRightX();
                UpdateDMS();
                control.Invalidate();
            }
        }

        private void C_MouseDown(object sender, MouseEventArgs e)
        {
            lastMousePoint = e.Location;
            lastMousePointD = DataPoint(e.Location);
        }

        //Control functions
        protected void C_MouseMove(object sender, MouseEventArgs e)
        {
            //...dms and matrix manipulations...
            if (e.Button == MouseButtons.Right)
            {                
                PointF MouseDataPoint = DataPoint(e.Location);
                float dx = MouseDataPoint.X - lastMousePointD.X;
                float dy = MouseDataPoint.Y - lastMousePointD.Y;
                m.Translate(dx, dy);

                control.Invalidate();
            }                                   
        }

        protected void C_MouseWheel(object sender, MouseEventArgs e)
        {
            //...dms and matrix manipulations...
            PointF pos = DataPoint(e.Location);
            bool inXscale = e.Location.Y < 50;
            bool inYscale = e.Location.X < 50;
            float z = e.Delta > 0 ? 1.1f : 1.0f / 1.1f;
            float kx = z;
            float ky = z;

            if (/*ModifierKeys.HasFlag(Keys.Control) || */inXscale) ky = 1;
            if (/*ModifierKeys.HasFlag(Keys.Shift) || */inYscale) kx = 1;
            PointF po = DataPoint(e.Location);
            Matrix temp = m.Clone();
            temp.Translate(po.X, po.Y);
            temp.Scale(kx, ky);
            temp.Translate(-po.X, -po.Y);
            foreach (double el in temp.Elements)
                if (el < -1e+8 || el > 1e+8) return;
            if (temp.OffsetX < -1e+8 || temp.OffsetX > 1e+8 || temp.OffsetY < -1e+8 || temp.OffsetY > 1e+8 || !temp.IsInvertible) return;
            m = temp;                        

            wheelEndTimer.Stop();
            wheelEndTimer.Start();

            control.Invalidate();
        }

        private void WheelEndTimer_Tick(object sender, EventArgs e)
        {
            wheelEndTimer.Stop();

            SetLeftXAndRightX();
            UpdateDMS();
            control.Invalidate();
        }

        private void SetLeftXAndRightX()
        {
            PointF newlp = DataPoint(new PointF(0f, 0f));
            PointF newrp = DataPoint(new PointF(control.ClientSize.Width, control.ClientSize.Height));
            LeftBorderX = newlp.X;
            RightBorderX = newrp.X;
        }

        //Visualize functions
        protected void Draw(object sender, PaintEventArgs e)
        {
            Rectangle drawArea = e.ClipRectangle;
            Graphics g = e.Graphics;

            //((Form)control).Text = string.Format("LeftX: {0}  RightX: {1}", LeftBorderX, RightBorderX);

            foreach (DrawStruct dm in dms)
            {
                //PointF[] arr = dm.outputArr.Skip(dm.leftI).Take(dm.rightI-dm.leftI).ToArray();
                PointF[] arr = dm.outputArr.ToArray();
                if (arr.Length < 2) return;
                m.TransformPoints(arr);                
                g.DrawLines(dm.pen, arr);
                //g.FillEllipse(Brushes.Green, arr[0].X, arr[0].Y, 10, 10);
                //g.FillEllipse(Brushes.Blue, arr.Last().X, arr.Last().Y, 10, 10);

                ((Form)control).Text = dm.log;
            }            
        }

        //Parse functions
        public void ParseRaw(string path, string[] args, Action<double> progressCallBack = null)
        {
            byte[] signals = new byte[args.Length];
            for (int i = 0; i < args.Length; i++)
            {                
                //if (File.Exists(string.Format("{0}\\temp\\{1}{2}_0.dat", Environment.CurrentDirectory, Path.GetFileName(path), args[i])))
                signals[i] = byte.Parse(args[i]);
            }                                

            Func<long, long, long, long, long> bytesToLongTime = new Func<long, long, long, long, long>((b0, b1, b2, tc) => 
            {
                return (b0) | (b1 << 8) | (b2 << 16) | (tc << 24);
            });

            Func<byte[], int, uint> bytesToLowTime = new Func<byte[], int, uint>((buf, i) =>
            {
                return ((uint)buf[i]) | ((uint)buf[i + 1] << 8) | ((uint)buf[i + 2] << 16);
            });

            int time_code = 0;
            Dictionary<byte, List<long>> events = new Dictionary<byte, List<long>>();
            foreach (byte s in signals) events[s] = new List<long>();

            Action<byte[], double> callback = (byte[] buf, double progress) =>
            {
                for (int i = 0; i < buf.Length; i += 4)
                {
                    uint lo = bytesToLowTime(buf, i);
                    byte signal = buf[i + 3];
                    if (events.ContainsKey(signal))
                        events[signal].Add(bytesToLongTime(buf[i], buf[i + 1], buf[i + 2], time_code));
                    else if (buf[i + 3] == 0xf4) time_code++;
                }
                progressCallBack(progress);
            };

            FileLoadModule.Load(path, 1000000, callback);

            Dictionary<byte, long[]> eventsArr = new Dictionary<byte, long[]>();
            foreach (KeyValuePair<byte, List<long>> pair in events)
                eventsArr[pair.Key] = pair.Value.ToArray();

            events = null;

            foreach (KeyValuePair<byte, long[]> pair in eventsArr)
            {                
                long[] data = pair.Value;
                if (data.Length < 2) continue;
                PointD[] points = new PointD[data.Length];
                double max = 0;
                double min = (60.0 / (data[1] - data[0] * 1024 * 16e-9));
                double lastrpm = 0;

                for (int i = 1; i < data.Length; i += 1)
                {
                    double tt = data[i] - data[i - 1];
                    if (tt == 0) continue;
                    double rpm = 60.0 / (tt * 1024 * 16e-9);
                    points[i] = new PointD((data[i] * 16e-9), rpm); 
                    if (rpm > max) max = rpm;
                    else if (rpm < min) min = rpm;
                    lastrpm = rpm;
                }
                string id = Path.GetFileName(path) + pair.Key.ToString();
                DrawStruct dm = new LayersManager(pallete[dms.Count], points, min, max, id, progressCallBack);                
                dms.Add(dm);
                NewDMLoaded(points[0].X, points[points.Length-1].X, min, max);
                GC.Collect();
            }

            control.Invalidate();
        }

        //DataManagers Updating 
        protected void UpdateDMS()
        {
            foreach (DrawStruct dm in dms)
                dm.UpdateOutputArray(LeftBorderX, RightBorderX);
            /*Parallel.ForEach(dms, (dm) =>
            {
                dm.UpdateOutputArray(LeftBorderX, RightBorderX);
            });*/

            control.Invalidate();
        }        

        //Matrix
        protected void OnMatrixUpdate(Matrix newMatrix)
        {
            m = newMatrix;
            UpdateDMS();
            control.Invalidate();
        }

        protected PointF DataPoint(PointF scr)
        {
            Matrix mr = m.Clone();
            mr.Invert();
            PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
            mr.TransformPoints(po);
            return po[0];
        }

        protected PointF ScreenPoint(PointF scr)
        {
            PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
            m.TransformPoints(po);
            return po[0];
        }

        protected Matrix GenerateMatrix(double maxX, double maxY, double minX, double minY)
        {
            Matrix mm = new Matrix();
            mm.Scale(1, -1);
            mm.Translate(0, -control.ClientSize.Height);
            float scalex = (float)(control.ClientSize.Width / (maxX - minX));
            float scaley = (float)(control.ClientSize.Height / (maxY - minY));
            if (maxY - minY == 0) scaley = 1;
            mm.Scale(scalex, scaley);
            return mm;
        }
    }

    public struct PointD
    {
        public double X, Y;

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

    public struct DrawStyle
    {
        public Pen pen;
        public Color color;

        public DrawStyle(Pen pen, Color color)
        {
            this.pen = pen;
            this.color = color;
        }
    }
}
