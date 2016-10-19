using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Plotter4
{
    class PlotsView
    {
        private PlotBox c;
        private Matrix m = new Matrix();
        private DispatcherTimer wheelEndTimer = new DispatcherTimer();
        public List<Plot> plots = new List<Plot>();
        private double leftX, rightX;
        public double absMin, absMax;
        public double xRange = 1;
        public double maxLeftX, maxRightX;
        private bool setted = false;
        public List<Axis> axises = new List<Axis>();
        string PLOTTER_TRANSFORM_REGKEY = "";

        public PlotsView(PlotBox control)
        {            
            c = control;

            PLOTTER_TRANSFORM_REGKEY = "plotter_transform_" + control.Name;
            
            c.MouseMove += C_MouseMove;
            c.MouseUp += C_MouseUp;
            c.MouseWheel += C_MouseWheel;
            c.KeyDown += C_KeyDown;
            

            wheelEndTimer.Interval = new TimeSpan(0, 0, 01);
            wheelEndTimer.Start();
            wheelEndTimer.Tick += WheelEndTimer_Tick;
        }

        public void AddPlots(List<Plot> plots)
        {
            this.plots.AddRange(plots);
            UpdateAbsProps();
            //restartMatrix();
            resetMatrix();
            ResetLeftAndRightX();
            UpdatePlots();

            foreach (Plot p in plots)
                DrawPlot(p);
            //c.Invalidate();
        }

        private void UpdateAbsProps()
        {
            absMin = plots[0].minValue;
            absMax = plots[0].maxValue;
            xRange = plots[0].xRange;
            foreach (Plot p in plots)
            {
                if (p.minValue < absMin || !setted) absMin = p.minValue;
                if (p.maxValue > absMax || !setted) absMax = p.maxValue;

                if (p.xRange > xRange || !setted) xRange = p.xRange;
                if (p.layers[0][0].X > maxLeftX || !setted) maxLeftX = p.layers[0][0].X;
                if (p.layers[0][p.layers[0].Length - 1].X > maxRightX || !setted) maxRightX = p.layers[0][p.layers[0].Length - 1].X;
                setted = true;
            }
            if (axises.Count == 0) axises.Add(new Axis());
        }

        public void UpdateViewRange(double left, double right)
        {
            Parallel.ForEach(plots, (p) =>
            {
                p.UpdateViewRange(left, right);
            });
        }

        private void UpdatePlots()
        {
            UpdateViewRange(leftX, rightX);

            /*information.Clear();
            int pIndex = 0;
            foreach (Plot p in plotsM.plots)
            {
                pIndex++;
                information.Add(String.Format("plot{0}:\n\tlayers: {1}\n\txRange: {2},\n\tyRange: {3},\n\tActiveLayer: {4}", pIndex, p.layers.Count, p.xRange, p.yRange, p.ActiveLayerIndex));
            }*/

            //c.Invalidate();
        }


        private void DrawPlot(Plot p)
        {
            c.Clear();
            Polyline poly = new Polyline();
            poly.Points = new PointCollection(p.pointsToDraw);
            poly.Stroke = Brushes.Red;            
            c.Canvas.RenderTransform = new MatrixTransform(m);
            c.AddChild(poly);                        


            //foreach (Axises a in axises)
              //  a.DrawAxises(m, g, c.ClientRectangle.ActualWidth , c.ClientRectangle.ActualHeight);

            //draw information strings 
            /*double x = 10;
            double y = 10;
            foreach (string inf in information)
            {
                SizeF stringSize = g.MeasureString(inf, Font);
                g.DrawString(inf, Font, Brushes.Black, x, y);
                y += stringSize.ActualHeight + 3;
            }*/

            //drawing plots            
        }

        public void HandleControlKeyDown(Control control)
        {
            control.KeyDown += C_KeyDown;
        }

        /*public void DrawPlots(Graphics g, Matrix m)
        {
            foreach (Plot p in plots)
            {
                if (p.pointsToDraw.Length < 2) continue;
                if (p.averagePointOn)
                {
                    Point[] avps = p.averagePoints.ToArray();
                    m.TransformPoints(avps);
                    //avLinePen = new Pen(c, 0.5f);
                    //g.DrawBeziers(p.avLinePen, avps);
                }

                Point[] ps = p.pointsToDraw.ToArray(); //clone it!                
                m.TransformPoints(ps);
                g.DrawLines(new Pen(p.color), ps);
            }
        }*/

        #region MouseListeners
        Point lastmousepos;
        private void C_MouseUp(object sender, MouseEventArgs e)
        {
            UpdatePlots();
        }

        private void C_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousepos = e.GetPosition(c);
            Point dp = DataPoint(mousepos);            
            //Text = String.Format("X:{0:F9} Y:{1:F9}", dp.X, dp.Y);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double dx = DataPoint(lastmousepos).X - dp.X;
                double dy = DataPoint(lastmousepos).Y - dp.Y;
                
                ResetLeftAndRightX();                
                m.Translate(-dx, -dy);                
                c.Canvas.RenderTransform = new MatrixTransform(m);
                //c.Invalidate();
            }
            lastmousepos = e.GetPosition(c);
        }
        private void ResetLeftAndRightX()
        {
            Point newlp = DataPoint(new Point(0f, 0f));
            Point newrp = DataPoint(new Point(c.ActualWidth , c.ActualHeight));

            leftX = newlp.X;
            rightX = newrp.X;
        }
        #endregion

        #region KeyboardListener
        private void C_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Application.Current.Shutdown();
                    break;
                case Key.R:
                    resetMatrix();
                    //c.Invalidate();
                    break;
            }
            //checking for a number
            /*int value = -1;
            if (e.KeyValue >= ((int)Key.NumPad0) && e.KeyValue <= ((int)Key.NumPad9))
                value = e.KeyValue - ((int)Key.NumPad0);
            else if (e.KeyValue >= ((int)Key.D0) && e.KeyValue <= ((int)Key.D9))
                value = e.KeyValue - ((int)Key.D0);
            //choosing a plot
            FocusOnPlot(value);*/
        }
        #endregion

        #region SomeStaff        
        private void PlotsM_Serialized(object sender, EventArgs e)
        {
            MessageBox.Show("Serialized", "PlotsManager has been serialized");
        }

        public static bool IsKeyADigit(Key key)
        {
            return (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9);
        }

        private void FocusOnPlot(int index)
        {
            if (index > plots.Count - 1 || index < 0) return;

            Plot plot = plots[index];

            if (plot.xRange == 0 || plot.maxValue == plot.minValue) return;

            m = new Matrix();
            m.Scale(1, -1);
            m.Translate(0, -c.ActualHeight);
            m.Scale(c.ActualWidth  / plot.xRange, c.ActualHeight / (plot.maxValue - plot.minValue));

            //c.Invalidate();
        }
        #endregion
        #region MatrixSaving
        public void MatrixSavingOn()
        {
            Application.Current.MainWindow.Closing += MainWindow_Closing;            
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Microsoft.Win32.Registry.CurrentUser.SetValue(PLOTTER_TRANSFORM_REGKEY, matrixToString(m));
        }        
        #endregion

        #region MatrixManipulations
        private Point DataPoint(Point scr)
        {
            Matrix mr = m;
            mr.Invert();            
            return mr.Transform(new Point(scr.X, scr.Y));
        }

        private Point ScreenPoint(Point scr)
        {            
            return m.Transform(new Point(scr.X, scr.Y));
        }


        private Point GetMatrixScale()
        {            
            return m.Transform(new Point(1f, 1f));
        }
        private void restartMatrix()
        {
            try
            {
                string ms = (string)Microsoft.Win32.Registry.CurrentUser.GetValue(PLOTTER_TRANSFORM_REGKEY);
                string[] mv = ms.Split(' ');
                double[] mf = mv.Select(a => double.Parse(a)).ToArray();
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
            //m.Translate(0, c.ActualHeight/2);
            //m.Translate(0, c.ActualHeight);
            //m.Scale(1, -1);            
            double scalex = c.ActualWidth  / xRange;
            double scaley = c.ActualHeight / (absMax - absMin);
            if (absMax - absMin == 0) scaley = 1;            
            m.Scale(scalex, scaley);            
        }

        private string matrixToString(Matrix m)
        {
            return string.Format("{0} {1} {2} {3} {4} {5}", m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);           
        }
        #endregion

        #region MouseScaling
        private void C_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point mousepos = e.GetPosition(c);
            Point pos = DataPoint(mousepos);
            bool inXscale = mousepos.Y < 50;
            bool inYscale = mousepos.X < 50;
            double z = e.Delta > 0 ? 1.1f : 1.0f / 1.1f;
            double kx = z;
            double ky = z;

            if (/*ModifierKey.HasFlag(Key.Control) || */inXscale) ky = 1;
            if (/*ModifierKey.HasFlag(Key.Shift) || */inYscale) kx = 1;
            Point po = DataPoint(mousepos);
            m.Translate(po.X, po.Y);
            m.Scale(kx, ky);
            m.Translate(-po.X, -po.Y);            

            //c.Invalidate();

            wheelEndTimer.Stop();
            wheelEndTimer.Start();
        }

        private void WheelEndTimer_Tick(object sender, EventArgs e)
        {
            wheelEndTimer.Stop();

            Point newlp = DataPoint(new Point(0f, 0f));
            Point newrp = DataPoint(new Point(c.ActualWidth , c.ActualHeight));

            leftX = newlp.X;
            rightX = newrp.X;

            UpdatePlots();
            //c.Invalidate();
        }


        #endregion

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
            //c.Invalidate();
        }
        #endregion
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

    struct PlotParams
    {
        //public string path;
        public Color color;
        public byte signalCode;
        public bool averageLineOn;
        public PlotParams(Color color, byte signalCode, bool averageLineOn)
        {
            //this.path = path;
            this.color = color;
            this.signalCode = signalCode;
            this.averageLineOn = averageLineOn;
        }
    }
}
