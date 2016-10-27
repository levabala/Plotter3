using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plotter3
{
    public partial class InternalAxis : UserControl
    {
        public OrientationType orientation;
        Font smallFont = new Font("Calibri", 13, FontStyle.Bold);
        Pen xGridPen = new Pen(Brushes.Blue, 0.5f);
        Pen yGridPen = new Pen(Brushes.Green, 0.5f);
        Matrix m = new Matrix();

        PointF startP, endP;

        public InternalAxis()
        {
            InitializeComponent();
            xGridPen.DashStyle = DashStyle.Dot;
            yGridPen.DashStyle = DashStyle.Dot;
        }

        [DefaultValue(OrientationType.Horizontal), Description("Axis orientation")]
        public OrientationType Orientation
        {                        
            get
            {                
                return orientation;
            }

            set
            {
                if (orientation == OrientationType.Horizontal)
                    Paint -= PaintHorizontalAxis;
                else Paint -= PaintVerticalAxis;
                orientation = value;
                if (orientation == OrientationType.Horizontal)
                    Paint += PaintHorizontalAxis;
                else Paint += PaintVerticalAxis;
            }
        }

        private void InternalAxis_Load(object sender, EventArgs e)
        {            
            if (orientation == OrientationType.Horizontal)
            {                
                getPointsHor();
                Paint += PaintHorizontalAxis;
            }
            else
            {
                getPointsVert();
                Paint += PaintVerticalAxis;
            }
        }

        public void getPointsHor()
        {
            startP = new PointF(Left, Top + ClientRectangle.Height / 2);
            endP = new PointF(Left + ClientRectangle.Width, Top + ClientRectangle.Height / 2);

            if (startP.X > endP.X)
            {
                PointF buff = endP;
                endP = startP;
                startP = buff;
            }
        }

        public void getPointsVert()
        {
            startP = new PointF(Left + ClientRectangle.Width / 2, Top + ClientRectangle.Height);
            endP = new PointF(Left + ClientRectangle.Width / 2, Top);

            if (startP.Y < endP.Y)
            {
                PointF buff = endP;
                endP = startP;
                startP = buff;
            }
        }

        public void setMatrix(Matrix matr)
        {
            m = matr;
        }

        private void PaintHorizontalAxis(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PointF axes_step_pt = new PointF(0, 0);
            PointF axes_step_pt2 = new PointF(70, 50);
            PointF axes_step = DataPoint(axes_step_pt, m);
            PointF axes_step2 = DataPoint(axes_step_pt2, m);

            int power = 5;

            float dx = axes_step2.X - axes_step.X;
            double lg_x = Math.Log(Math.Abs(dx), power);
            double pow_x = Math.Pow(power, Math.Floor(lg_x) + 1);
            float step_x = (float)pow_x;
            if (step_x > 1) step_x = (float)Math.Round(pow_x);

            int kx = 0;
            PointF origin = DataPoint(startP, m);
            float x = ((int)(origin.X / step_x) + 1) * step_x;
            PointF xp = ScreenPoint(new PointF(x, 0), m);
            do
            {
                xp = ScreenPoint(new PointF(x, 0), m);
                //g.DrawLine(Pens.Blue, xp.X, xp.Y, xp.X, xp.Y + 20);
                g.DrawLine(Pens.Blue, xp.X, 0, xp.X, 10);
                g.DrawLine(xGridPen, xp.X, 10, xp.X, startP.Y);
                g.DrawString(Math.Round(x, 3).ToString(), smallFont, Brushes.Blue, xp.X + 3, -2);
                x += step_x;
                kx += 1;
            } while (xp.X < endP.X && kx < 64);
            g.DrawString("msec", smallFont, Brushes.Blue, endP.X - 60, 15);
        }

        private void PaintVerticalAxis(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PointF axes_step_pt = new PointF(0, 0);
            PointF axes_step_pt2 = new PointF(70, 50);
            PointF axes_step = DataPoint(axes_step_pt, m);
            PointF axes_step2 = DataPoint(axes_step_pt2, m);

            int power = 5;

            float dy = axes_step2.Y - axes_step.Y;            
            double lg_y = Math.Log(Math.Abs(dy), power);
            double pow_y = Math.Pow(power, Math.Floor(lg_y) + 1);            
            float step_y = (float)pow_y;            
            if (step_y > 1) step_y = (float)Math.Round(pow_y);            

            int ky = 0;
            PointF origin = DataPoint(startP, m);
            float y = ((int)(origin.Y / step_y) + 1) * step_y;
            PointF yp = ScreenPoint(startP, m);
            do
            {
                yp = ScreenPoint(new PointF(startP.X, y), m);
                //g.DrawLine(Pens.Red, yp.X, yp.Y, yp.X-20, yp.Y);
                g.DrawLine(Pens.Red, 0, yp.Y, 10, yp.Y);
                g.DrawLine(yGridPen, 10, yp.Y, startP.X, yp.Y);
                g.DrawString(Math.Round(y, 3).ToString(), smallFont, Brushes.Red, 0, yp.Y);
                y += step_y;
                ky += 1;
            } while (yp.Y > 0 && ky < 64);
            ky = 0;
            y = 0;
            yp = ScreenPoint(startP, m);
            do
            {
                yp = ScreenPoint(new PointF(startP.X, y), m);
                //g.DrawLine(Pens.Red, yp.X, yp.Y, yp.X-20, yp.Y);
                g.DrawLine(Pens.Red, 0, yp.Y, 10, yp.Y);
                g.DrawLine(yGridPen, 10, yp.Y, startP.X, yp.Y);
                g.DrawString(Math.Round(y, 5).ToString(), smallFont, Brushes.Red, 0, yp.Y);
                y -= step_y;
                ky += 1;
            } while (ky < 64);
            g.DrawString("rpm", smallFont, Brushes.Red, 25, endP.Y - 30);
        }

        public enum OrientationType
        {
            Horizontal, Vertical
        }

        private PointF DataPoint(PointF scr, Matrix m)
        {
            Matrix mr = m.Clone();
            mr.Invert();
            PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
            mr.TransformPoints(po);
            return po[0];
        }

        private void InternalAxis_SizeChanged(object sender, EventArgs e)
        {
            if (orientation == OrientationType.Horizontal)            
                getPointsHor();             
            else            
                getPointsVert();             
        }

        private PointF ScreenPoint(PointF scr, Matrix m)
        {
            PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
            m.TransformPoints(po);
            return po[0];
        }
    }
}
