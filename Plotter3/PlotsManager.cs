using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Input;

namespace Plotter3
{
    [Serializable]
    class PlotsManager
    {
        public List<Plot> plots = new List<Plot>(); 
        public float absMin, absMax;
        public float xRange = 1;
        public float maxLeftX, maxRightX;
        public List<Axises> axises = new List<Axises>();

        private float leftX, rightX;        
        private Object thisLock = new Object();        
        private Control c;
        private Matrix m = new Matrix();
        private Timer wheelEndTimer = new Timer();
        private List<string> information = new List<string>();

        string PLOTTER_TRANSFORM_REGKEY;

        public PlotsManager(Control control)
        {            
            if (control.GetType().GetMethod("CreateGraphics") == null)            
                throw new CannotCreateGraphicsException("The control can't create a graphics object");

            c = control;
            PLOTTER_TRANSFORM_REGKEY = "plotter_transform_" + c.Name;            

            c.Paint += C_Paint;
            c.MouseMove += C_MouseMove;
            c.MouseUp += C_MouseUp;
            c.MouseWheel += C_MouseWheel;            
            c.KeyDown += C_KeyDown;            

            wheelEndTimer.Interval = 30;
            wheelEndTimer.Tick += WheelEndTimer_Tick;            
        }

        private void UpdatePlots()
        {
            UpdateViewRange(leftX, rightX);

            ReFillInformationBox();

            c.Invalidate();
        }

        private void ReFillInformationBox()
        {
            information.Clear();
            int pIndex = 0;
            foreach (Plot p in plots)
            {
                pIndex++;
                information.Add(String.Format("plot{0}:\n\tlayers: {1}\n\txRange: {2},\n\tyRange: {3},\n\tActiveLayer: {4}\n\tPointsDrawed: {5}", pIndex, p.layers.Count, p.xRange, p.yRange, p.ActiveLayerIndex, p.pointsToDraw.Length));
            }

        }

        Font infoFont = new Font("Arial", 40, FontStyle.Regular, GraphicsUnit.Document);
        private void C_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (Axises a in axises)
                a.DrawAxises(m, g, c.ClientRectangle.Width, c.ClientRectangle.Height);

            //draw information strings 
            float x = 10;
            float y = 10;
            foreach (string inf in information)
            {
                SizeF stringSize = g.MeasureString(inf, infoFont);
                g.DrawString(inf, infoFont, Brushes.Black, x, y);
                y += stringSize.Height + 3;
            }

            //drawing plots
            DrawPlots(g, m);
        }               

        public void HandleControlKeyDown(Control control)
        {
            control.KeyDown += C_KeyDown;
        }

        public void DrawPlots(Graphics g, Matrix m)
        {
            foreach (Plot p in plots)
            {
                if (p.pointsToDraw.Length < 2) continue;
                if (p.averagePointOn)
                {
                    PointF[] avps = p.averagePoints.ToArray();
                    m.TransformPoints(avps);
                    //avLinePen = new Pen(c, 0.5f);
                    //g.DrawBeziers(p.avLinePen, avps);
                }

                PointF[] ps = p.pointsToDraw.ToArray(); //clone it!                
                m.TransformPoints(ps);
                g.DrawLines(new Pen(p.color), ps);
            }
        }

        public void AddPlot(Plot p)
        {
            plots.Add(p);
        }

        public void UpdateViewRange(float left, float right)
        {
            Parallel.ForEach(plots, (p) =>
            {
                p.UpdateViewRange(left, right);
            });                
        }

        #region ControlOptions
        public void ResetM_Click(object sender, EventArgs e)
        {
            ResetMatrix();
        }
        public void ResetMatrix()
        {
            resetMatrix();
            ResetLeftAndRightX();
            UpdatePlots();
            c.Invalidate();
        }

        public void SerializeAll_Click(object sender, EventArgs e)
        {
            SerializeAll();
        }

        public void SerializeAll()
        {
            Serialize();
            Serialized += PlotsM_Serialized;
        }

        #endregion

        #region Serializing
        public void Serialize()
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream("temp.dat", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, this);
            }
        }

        public event EventHandler Serialized;
        protected virtual void OnSerialized(EventArgs e)
        {
            if (Serialized != null)
                Serialized(this, e);
        }

        [OnSerialized]
        public void OnSerialized(StreamingContext context)
        {
            OnSerialized(new EventArgs());
        }
        #endregion

        #region PrivateFunctions

        #endregion

        #region MatrixManipulations
        private PointF DataPoint(PointF scr)
        {
            Matrix mr = m.Clone();
            mr.Invert();
            PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
            mr.TransformPoints(po);
            return po[0];
        }

        private PointF ScreenPoint(PointF scr)
        {
            PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
            m.TransformPoints(po);
            return po[0];
        }


        private PointF GetMatrixScale()
        {
            PointF[] ps = new PointF[] { new PointF(1f, 1f) };
            m.TransformPoints(ps);
            return ps[0];
        }
        private void restartMatrix()
        {
            try
            {
                string ms = (string)Microsoft.Win32.Registry.CurrentUser.GetValue(PLOTTER_TRANSFORM_REGKEY);
                string[] mv = ms.Split(' ');
                float[] mf = mv.Select(a => float.Parse(a)).ToArray();
                m = new Matrix(mf[0], mf[1], mf[2], mf[3], mf[4], mf[5]);
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "Matrix Error",
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);

                resetMatrix();
            }
        }

        private void resetMatrix()
        {
            m = new Matrix();
            m.Scale(1, -1);
            m.Translate(0, -c.ClientSize.Height);
            float scalex = c.ClientSize.Width / xRange;
            float scaley = c.ClientSize.Height / (absMax - absMin);
            if (absMax - absMin == 0) scaley = 1;
            m.Scale(scalex, scaley);
        }

        private string matrixToString(Matrix m)
        {
            return m.Elements.Select(a => a.ToString()).Aggregate((a, b) => a + " " + b);
        }
        #endregion

        #region MouseScaling
        private void C_MouseWheel(object sender, MouseEventArgs e)
        {
            PointF pos = DataPoint(e.Location);
            bool inXscale = e.Location.Y < 50;
            bool inYscale = e.Location.X < 50;
            float z = e.Delta > 0 ? 1.1f : 1.0f / 1.1f;
            float kx = z;
            float ky = z;

            if (/*ModifierKeys.HasFlag(Keys.Control) || */inXscale) ky = 1;
            if (/*ModifierKeys.HasFlag(Keys.Shift) || */inYscale) kx = 1;
            PointF po = DataPoint(e.Location);
            m.Translate(po.X, po.Y);
            m.Scale(kx, ky);
            m.Translate(-po.X, -po.Y);

            //Text += " |" + scalingDirection.ToString() + "| ";

            c.Invalidate();

            wheelEndTimer.Stop();
            wheelEndTimer.Start();
        }

        private void WheelEndTimer_Tick(object sender, EventArgs e)
        {
            wheelEndTimer.Stop();

            PointF newlp = DataPoint(new PointF(0f, 0f));
            PointF newrp = DataPoint(new PointF(c.ClientSize.Width, c.ClientSize.Height));

            leftX = newlp.X;
            rightX = newrp.X;

            UpdatePlots();
            c.Invalidate();
        }


        #endregion

        #region plotsCreating
        public void CreatePlotsFromFiles(List<PlotParams> pars)
        {
            List<Action> actions = new List<Action>();
            foreach (PlotParams p in pars)
                actions.Add(new Action(() =>
                {
                    CreatePlotFromFile(p.path, p.color, p.signalCode);
                }));
            Parallel.Invoke(actions.ToArray());
            ResetLeftAndRightX();
            UpdateViewRange(leftX, rightX);
            ReFillInformationBox();
            c.Invalidate();
        }

        public void CreatePlotFromFile(PlotParams par)
        {
            // Ahah :)
            //it looks funny))
            CreatePlotFromFile(par.path, par.color, par.signalCode, par.averageLineOn);            
        }

        public void CreatePlotFromFile(string path, Color c, byte signalCode, bool averageLineOn = true)
        {
            Int64[] data = Parser.parseLM(path, signalCode);
            List<PointF> points = new List<PointF>();
            float max = 0;
            float min = data[1] - data[0];
            float lastrpm = 0;

            for (int i = 1; i < data.Length; i += 1)
            {
                double tt = data[i] - data[i - 1];
                double rpm = 60.0 / (tt * 1024 * 16e-9);
                float rpmf = (float)rpm;
                points.Add(new PointF((float)(data[i] * 16e-9), rpmf));
                if (rpm > max && tt != 0) max = rpmf;
                else if (rpm < min) min = rpmf;
                lastrpm = rpmf;
            }

            lock (thisLock)
            {
                if (min < absMin || plots.Count == 0) absMin = min;
                if (max > absMax || plots.Count == 0) absMax = max;

                float xrange = points[points.Count - 1].X - points[0].X;
                if (xrange > xRange || plots.Count == 0) xRange = xrange;
                if (points[0].X > maxLeftX || plots.Count == 0) maxLeftX = points[0].X;
                if (points[points.Count - 1].X > maxRightX || plots.Count == 0) maxRightX = points[points.Count - 1].X;

                Plot plot = new Plot(points, min, max, c);
                plot.averagePointOn = averageLineOn;
                plots.Add(plot);
                
                if (axises.Count == 0) axises.Add(new Axises());
                restartMatrix();
                this.c.Invalidate();                
            }            
        }
        #endregion

        #region MouseListeners
        PointF lastmousepos;
        private void C_MouseUp(object sender, MouseEventArgs e)
        {
            UpdatePlots();
        }

        private void C_MouseMove(object sender, MouseEventArgs e)
        {
            PointF dp = DataPoint(e.Location);
            //Text = String.Format("X:{0:F9} Y:{1:F9}", dp.X, dp.Y);
            if (e.Button == MouseButtons.Right)
            {
                float dx = DataPoint(lastmousepos).X - DataPoint(e.Location).X;
                float dy = DataPoint(lastmousepos).Y - DataPoint(e.Location).Y;

                //cam.Move(dx, dy);
                ResetLeftAndRightX();

                m.Translate(-dx, -dy);
                c.Invalidate();
            }
            lastmousepos = e.Location;
        }
        private void ResetLeftAndRightX()
        {
            PointF newlp = DataPoint(new PointF(0f, 0f));
            PointF newrp = DataPoint(new PointF(c.ClientSize.Width, c.ClientSize.Height));

            leftX = newlp.X;
            rightX = newrp.X;
        }
        #endregion

        #region KeyboardListener
        private void C_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.R:
                    resetMatrix();
                    c.Invalidate();
                    break;
            }
            //checking for a number
            int value = -1;
            if (e.KeyValue >= ((int)Keys.NumPad0) && e.KeyValue <= ((int)Keys.NumPad9))
                value = e.KeyValue - ((int)Keys.NumPad0);
            else if (e.KeyValue >= ((int)Keys.D0) && e.KeyValue <= ((int)Keys.D9))
                value = e.KeyValue - ((int)Keys.D0);
            //choosing a plot
            FocusOnPlot(value);
        }
        #endregion

        #region MatrixSaving
        public void MatrixSavingOn(Form form)
        {
            form.FormClosing += Form_FormClosing;
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Microsoft.Win32.Registry.CurrentUser.SetValue(PLOTTER_TRANSFORM_REGKEY, matrixToString(m));
        }
        #endregion

        #region SomeStaff        
        private void PlotsM_Serialized(object sender, EventArgs e)
        {
            MessageBox.Show("Serialized", "PlotsManager has been serialized");
        }

        public static bool IsKeyADigit(Keys key)
        {
            return (key >= Keys.D0 && key <= Keys.D9) || (key >= Keys.NumPad0 && key <= Keys.NumPad9);
        }

        private void FocusOnPlot(int index)
        {
            if (index > plots.Count - 1 || index < 0) return;

            Plot plot = plots[index];

            if (plot.xRange == 0 || plot.maxValue == plot.minValue) return;

            m = new Matrix();
            m.Scale(1, -1);
            m.Translate(0, -c.ClientSize.Height);
            m.Scale(c.ClientSize.Width / plot.xRange, c.ClientSize.Height / (plot.maxValue - plot.minValue));

            c.Invalidate();
        }
        #endregion
    }

    struct PlotParams
    {
        public string path;
        public Color color;
        public byte signalCode;
        public bool averageLineOn;
        public PlotParams(string path, Color color, byte signalCode, bool averageLineOn)
        {
            this.path = path;
            this.color = color;
            this.signalCode = signalCode;
            this.averageLineOn = averageLineOn;
        }
    }

    public class CannotCreateGraphicsException : Exception
    {
        public CannotCreateGraphicsException()
        {
        }

        public CannotCreateGraphicsException(string message)
            : base(message)
        {
        }

        public CannotCreateGraphicsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
