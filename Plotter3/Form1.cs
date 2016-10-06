using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plotter3
{
    public partial class Form1 : Form
    {
        string PLOTTER_TRANSFORM_REGKEY = "plotter_transform";
        Timer wheelEndTimer = new Timer();
        PlotsManager plotsM = new PlotsManager();
        Matrix m;
        PointF lastmousepos = new PointF();
        float leftX, rightX;
        public Form1()
        {
            m = new Matrix();            

            InitializeComponent();
            Paint += Form1_Paint;
            FormClosing += Form1_FormClosing;
            MouseMove += Form1_MouseMove;
            MouseUp += Form1_MouseUp;
            MouseWheel += Form1_MouseWheel;

            wheelEndTimer.Interval = 30;
            wheelEndTimer.Tick += WheelEndTimer_Tick;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            UpdatePlots();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            PointF dp = DataPoint(e.Location);
            //Text = String.Format("X:{0:F9} Y:{1:F9}", dp.X, dp.Y);
            if (e.Button == MouseButtons.Right)
            {
                float dx = DataPoint(lastmousepos).X - DataPoint(e.Location).X;
                float dy = DataPoint(lastmousepos).Y - DataPoint(e.Location).Y;

                //cam.Move(dx, dy);
                leftX += dx;
                rightX += dx;
                m.Translate(-dx, -dy);
                Invalidate();
            }
            lastmousepos = e.Location;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //g.Transform = m;

            foreach (Plot p in plotsM.plots)
            {
                if (p.pointsToDraw.Length < 2) continue;
                PointF[] ps = p.pointsToDraw.ToArray(); //clone it!
                m.TransformPoints(ps);
                g.DrawLines(new Pen(p.color), ps);
                Text = p.pointsToDraw.Length.ToString();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Microsoft.Win32.Registry.CurrentUser.SetValue(PLOTTER_TRANSFORM_REGKEY, matrixToString(m));
        }                

        private void Form1_Load(object sender, EventArgs e)
        {
            plotsM.CreatePlotFromFile(@"D:\work\test_6000_fast.raw", Color.DarkGreen);
            Plot plot1 = plotsM.plots[0];

            plotsM.UpdateViewRange(plotsM.maxLeftX, plotsM.maxRightX);

            leftX = plotsM.maxLeftX;
            rightX = plotsM.maxRightX;

            restartMatrix();
            resetMatrix();
            Invalidate();
        }

        private void UpdatePlots()
        {
            plotsM.UpdateViewRange(leftX, rightX);                           
            Invalidate();
        }

        #region MouseScaling
        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            PointF pos = DataPoint(e.Location);
            bool inXscale = e.Location.Y < 50;
            bool inYscale = e.Location.X < 50;
            float z = e.Delta > 0 ? 1.1f : 1.0f / 1.1f;            
            float kx = z;
            float ky = z;
            if (ModifierKeys.HasFlag(Keys.Control) || inXscale) ky = 1;
            if (ModifierKeys.HasFlag(Keys.Shift) || inYscale) kx = 1;
            PointF po = DataPoint(e.Location);
            m.Translate(po.X, po.Y);
            m.Scale(kx, ky);
            m.Translate(-po.X, -po.Y);

            //Text += " |" + scalingDirection.ToString() + "| ";

            Invalidate();

            wheelEndTimer.Stop();
            wheelEndTimer.Start();
        }

        private void WheelEndTimer_Tick(object sender, EventArgs e)
        {
            wheelEndTimer.Stop();

            PointF newlp = DataPoint(new PointF(0f, 0f));
            PointF newrp = DataPoint(new PointF(ClientSize.Width, ClientSize.Height));

            leftX = newlp.X;
            rightX = newrp.X;

            UpdatePlots();
            Invalidate();
        }


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
                MessageBox.Show(e.Message, "Matrix Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                resetMatrix();
            }
        }

        private void resetMatrix()
        {
            m = new Matrix();
            m.Scale(1, -1);
            m.Translate(0, -ClientSize.Height);
            m.Scale(ClientSize.Width / plotsM.xRange, ClientSize.Height / (plotsM.absMax - plotsM.absMin));
        }

        private string matrixToString(Matrix m)
        {
            return m.Elements.Select(a => a.ToString()).Aggregate((a, b) => a + " " + b);
        }
        #endregion
    }
}
