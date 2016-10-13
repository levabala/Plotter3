using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plotter3
{
    public partial class Form1 : Form
    {
        string PLOTTER_TRANSFORM_REGKEY = "plotter_transform";
        Timer wheelEndTimer = new Timer();
        PlotsManager plotsM;        
        PointF lastmousepos = new PointF();
        Matrix m;
        List<string> information = new List<string>(); 
        float leftX, rightX;

        List<Axises> axises = new List<Axises>();
        List<PlotParams> plotsParams = new List<PlotParams>()
        {
            new PlotParams(@"D:\work\test_6000_fast.raw", Color.DarkGreen, 245, false),
            new PlotParams(@"D:\work\test_6000_fast.raw", Color.DarkBlue, 242, true),
            new PlotParams(@"D:\work\test_6000_fast.raw", Color.DarkOrange, 244, true)
        };

        public Form1()
        {            
            InitializeComponent();
            m = new Matrix();
            plotsM = new PlotsManager(this);
                        

            /*Paint += Form1_Paint;
            FormClosing += Form1_FormClosing;
            MouseMove += Form1_MouseMove;
            MouseUp += Form1_MouseUp;
            MouseWheel += Form1_MouseWheel;
            PreviewKeyDown += Form1_PreviewKeyDown;*/

            wheelEndTimer.Interval = 30;
            wheelEndTimer.Tick += WheelEndTimer_Tick;
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.R:
                    resetMatrix();
                    Invalidate();
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

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (Axises a in axises)
                a.DrawAxises(m, g, ClientRectangle.Width, ClientRectangle.Height);

            //draw information strings 
            float x = 10;
            float y = 10;
            foreach (string inf in information)
            {
                SizeF stringSize = g.MeasureString(inf, Font);
                g.DrawString(inf, Font, Brushes.Black, x, y);
                y += stringSize.Height + 3;
            }

            //drawing plots
            plotsM.DrawPlots(g, m);           
        }        
                
        private void Form1_Load(object sender, EventArgs e)
        {
            //axises.Add(new Axis(new PointF(20, 0), new PointF(20, ClientRectangle.Height), 25));
            axises.Add(new Axises());

            plotsM.CreatePlotsFromFiles(plotsParams);
            plotsM.MatrixSavingOn(this);
            //not works :))
            

            leftX = plotsM.maxLeftX;
            rightX = plotsM.maxRightX;

            UpdatePlots();

            restartMatrix();
            resetMatrix();
            Invalidate();
        }

        private void UpdatePlots()
        {
            plotsM.UpdateViewRange(leftX, rightX);

            information.Clear();
            int pIndex = 0;
            foreach (Plot p in plotsM.plots)
            {
                pIndex++;
                information.Add(String.Format("plot{0}:\n\tlayers: {1}\n\txRange: {2},\n\tyRange: {3},\n\tActiveLayer: {4}", pIndex, p.layers.Count, p.xRange, p.yRange, p.ActiveLayerIndex));
            }

            Invalidate();
        }

        #region MouseListeners
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            UpdatePlots();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            PointF dp = DataPoint(e.Location);
            Text = String.Format("X:{0:F9} Y:{1:F9}", dp.X, dp.Y);
            if (e.Button == MouseButtons.Right)
            {
                float dx = DataPoint(lastmousepos).X - DataPoint(e.Location).X;
                float dy = DataPoint(lastmousepos).Y - DataPoint(e.Location).Y;

                //cam.Move(dx, dy);
                ResetLeftAndRightX();

                m.Translate(-dx, -dy);
                Invalidate();
            }
            lastmousepos = e.Location;
        }
        private void ResetLeftAndRightX()
        {
            PointF newlp = DataPoint(new PointF(0f, 0f));
            PointF newrp = DataPoint(new PointF(ClientSize.Width, ClientSize.Height));

            leftX = newlp.X;
            rightX = newrp.X;
        }
        #endregion

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
            float scalex = ClientSize.Width / plotsM.xRange;
            float scaley = ClientSize.Height / (plotsM.absMax - plotsM.absMin);
            if (plotsM.absMax - plotsM.absMin == 0) scaley = 1;            
            m.Scale(scalex, scaley);
        }        

        private string matrixToString(Matrix m)
        {
            return m.Elements.Select(a => a.ToString()).Aggregate((a, b) => a + " " + b);
        }
        #endregion

        #region SomeStaff
        private void RestartM_Click(object sender, EventArgs e)
        {
            plotsM.ResetMatrix();
        }

        private void SerializeAll_Click(object sender, EventArgs e)
        {
            plotsM.SerializeAll();
        }

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
            if (index > plotsM.plots.Count - 1 || index < 0) return;

            Plot plot = plotsM.plots[index];

            if (plot.xRange == 0 || plot.maxValue == plot.minValue) return;

            m = new Matrix();
            m.Scale(1, -1);
            m.Translate(0, -ClientSize.Height);
            m.Scale(ClientSize.Width / plot.xRange, ClientSize.Height / (plot.maxValue - plot.minValue));

            Invalidate();
        }
        #endregion
    }
}

/*Some code
FileStream fs = new FileStream("temp.dat", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
                plotsM = (PlotsManager)formatter.Deserialize(fs);
            }
            catch (SerializationException ex)
            {
                MessageBox.Show("Failed to deserialize","Reason: " + ex.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
*/
