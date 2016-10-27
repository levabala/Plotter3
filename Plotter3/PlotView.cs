using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plotter3
{
    class PlotsView
    {
        private Control c;
        private Matrix m = new Matrix();
        private Timer wheelEndTimer = new Timer();
        public List<Plot> plots = new List<Plot>();
        private float leftX, rightX;
        public float absMin, absMax;
        public float xRange = 1;
        public float maxLeftX, maxRightX;
        private bool setted = false;
        public List<Axis> axes = new List<Axis>();
        string PLOTTER_TRANSFORM_REGKEY = "";

        List<Matrix> matrixHistory = new List<Matrix>();

        PointF lastmousepos;
        PointF mousedownpos;
        PointF mousedownposS;
        PointF rectP1, rectP2;

        public PlotsView(PlotBox control)
        {
            if (control.GetType().GetMethod("CreateGraphics") == null)
                throw new CannotCreateGraphicsException("The control can't create a graphics object");
            c = control;            

            PLOTTER_TRANSFORM_REGKEY = "plotter_transform_" + control.Name;            

            c.Paint += C_Paint;
            c.MouseMove += C_MouseMove;
            c.MouseUp += C_MouseUp;
            c.MouseWheel += C_MouseWheel;
            c.MouseDown += C_MouseDown;
            c.KeyDown += C_KeyDown;            

            wheelEndTimer.Interval = 30;
            wheelEndTimer.Tick += WheelEndTimer_Tick;
        }        

        public void AddPlots(List<Plot> plots)
        {
            this.plots.AddRange(plots);
            UpdateAbsProps();
            restartMatrix();            
            ResetLeftAndRightX();
            //Application.DoEvents();
            //DrawPlots(c.CreateGraphics(), m);
            UpdatePlots();
            c.Invalidate();
        }

        public void AddAxis(Orientation orientation, PointF startP, PointF endP)
        {
            axes.Add(new Axis(orientation));
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
        }

        public void UpdateViewRange(float left, float right)
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

            c.Invalidate();
        }

        HatchBrush htb = new HatchBrush(HatchStyle.BackwardDiagonal, Color.LightBlue, Color.Transparent);
        private void C_Paint(object sender, PaintEventArgs e)
        {

            foreach (Control cont in c.Controls)
            {
                if (cont is InternalAxis)
                {                    
                    ((InternalAxis)cont).setMatrix(m);
                    cont.Update();
                }                                                
            }
            Graphics g = e.Graphics;                                   
            //foreach (Axis a in axes)
              //  a.Paint(m, g, c.ClientRectangle.Width, c.ClientRectangle.Height);

            //draw information strings 
            /*float x = 10;
            float y = 10;
            foreach (string inf in information)
            {
                SizeF stringSize = g.MeasureString(inf, Font);
                g.DrawString(inf, Font, Brushes.Black, x, y);
                y += stringSize.Height + 3;
            }*/

            //drawing plots
            DrawPlots(g, m);
            g.FillRectangle(htb, rectP1.X, rectP1.Y, rectP2.X - rectP1.X, rectP2.Y - rectP1.Y);
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

        #region MouseListeners        
        Matrix downMatrix = new Matrix();
        private void C_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointF lt = mousedownpos;
                PointF rb = DataPoint(e.Location);                
                float buff = 0;
                if (lt.X > rb.X)
                {
                    buff = lt.X;
                    lt.X = rb.X;
                    rb.X = buff;
                }
                if (lt.Y < rb.Y)
                {
                    buff = lt.Y;
                    lt.Y = rb.Y;
                    rb.Y = buff;
                }
                float dx = rb.X - lt.X;
                float dy = lt.Y - rb.Y;

                Matrix temp = new Matrix();
                temp.Scale(1, -1);
                temp.Translate(0, -c.ClientSize.Height);
                float scalex = c.ClientSize.Width / dx;
                float scaley = c.ClientSize.Height / dy;
                if (absMax - absMin == 0) scaley = 1;
                temp.Scale(scalex, scaley);
                temp.Translate(-lt.X, -rb.Y);

                bool wrong = false;
                foreach (float el in temp.Elements)
                    if (el < -1e+8 || el > 1e+8) wrong = true;
                if (temp.OffsetX < -1e+8 || temp.OffsetX > 1e+8 || temp.OffsetY < -1e+8 || temp.OffsetY > 1e+8 || !temp.IsInvertible) wrong = true;

                if (!wrong)
                {
                    m = temp;
                    RememberMatrix();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                float delta = Math.Abs(lastmousepos.X - mousedownposS.X) + Math.Abs(lastmousepos.Y - mousedownposS.Y);
                if (delta > 100) RememberMatrix();
            }
            rectP1 = rectP2;
            UpdatePlots();
        }

        private void C_MouseDown(object sender, MouseEventArgs e)
        {
            mousedownpos = DataPoint(e.Location);
            mousedownposS = e.Location;
            if (e.Button == MouseButtons.Left)
                rectP1 = e.Location;
        }

        private void C_MouseMove(object sender, MouseEventArgs e)
        {
            PointF dp = DataPoint(e.Location);
            //Text = String.Format("X:{0:F9} Y:{1:F9}", dp.X, dp.Y);
            if (e.Button == MouseButtons.Right)
            {
                float dx = dp.X - mousedownpos.X;//DataPoint(e.Location).X;
                float dy = dp.Y - mousedownpos.Y;//DataPoint(e.Location).Y;

                //cam.Move(dx, dy);
                ResetLeftAndRightX();                
                m.Translate(dx, dy);
                c.Invalidate();
            }
            else if (e.Button == MouseButtons.Left)
            {
                rectP2 = e.Location;
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
                case Keys.Space:
                    loadPreviousMatrix();
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

        #region MatrixManipulations
        private void loadPreviousMatrix()
        {
            if (matrixHistory.Count < 2) return;
            m = matrixHistory[matrixHistory.Count - 2];
            matrixHistory.RemoveAt(matrixHistory.Count - 1);

            ResetLeftAndRightX();
            UpdatePlots();
            c.Invalidate();
        }

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
        private Matrix StringToMatrix(string ms)
        {
            string[] mv = ms.Split(' ');
            float[] mf = mv.Select(a => float.Parse(a)).ToArray();
            return new Matrix(mf[0], mf[1], mf[2], mf[3], mf[4], mf[5]);
        }
        private void restartMatrix()
        {
            try
            {
                string ms = (string)Microsoft.Win32.Registry.CurrentUser.GetValue(PLOTTER_TRANSFORM_REGKEY);
                m = StringToMatrix(ms);                
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "Matrix Error",
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);

                resetMatrix();
            }
            if (!m.IsInvertible) resetMatrix();
            if (!m.IsInvertible) throw new Exception();
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

            RememberMatrix();
        }

        private void RememberMatrix()
        {
            matrixHistory.Add(m);
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
            Matrix temp = m.Clone();
            temp.Translate(po.X, po.Y);
            temp.Scale(kx, ky);
            temp.Translate(-po.X, -po.Y);
            foreach (float el in temp.Elements)
                if (el < -1e+8 || el > 1e+8) return;
            if (temp.OffsetX < -1e+8 || temp.OffsetX > 1e+8 || temp.OffsetY < -1e+8 || temp.OffsetY > 1e+8 || !temp.IsInvertible) return;
            m = temp;

            RememberMatrix();

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

        /*public void SerializeAll_Click(object sender, EventArgs e)
        {
            SerializeAll();
        }

        public void SerializeAll()
        {
            Serialize();
            Serialized += PlotsM_Serialized;
        }*/

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
