using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Plotter3
{
    class Axises
    {                
        Font smallFont = new Font("Calibri", 13, FontStyle.Bold);
        Pen xGridPen = new Pen(Brushes.Blue, 0.5f);
        Pen yGridPen = new Pen(Brushes.Green, 0.5f);

        public Axises()
        {
            xGridPen.DashStyle = DashStyle.Dot;
            yGridPen.DashStyle = DashStyle.Dot;
        }        

        public void DrawAxises(Matrix m, Graphics g, float width, float height)
        {
            PointF axes_step_pt = new PointF(0, 0);
            PointF axes_step_pt2 = new PointF(70, 50);
            PointF axes_step = DataPoint(axes_step_pt, m);
            PointF axes_step2 = DataPoint(axes_step_pt2, m);

            float dx = axes_step2.X - axes_step.X;
            float dy = axes_step2.Y - axes_step.Y;

            int power = 5;

            double lg_x = Math.Log(Math.Abs(dx), power);
            double lg_y = Math.Log(Math.Abs(dy), power);

            double pow_x = Math.Pow(power, Math.Floor(lg_x) + 1);
            double pow_y = Math.Pow(power, Math.Floor(lg_y) + 1);

            float step_x = (float)Math.Round(pow_x,3);     
            if (step_x < 0.125) step_x = 0.125f;

            float step_y = (float)Math.Round(pow_y, 3);
            if (step_y < 0.125) step_y = 0.125f;

            int kx = 0;
            PointF origin = DataPoint(new PointF(0, height), m);
            float x = ((int)(origin.X / step_x) + 1) * step_x;
            PointF xp = ScreenPoint(new PointF(x, 0), m);
            do
            {
                xp = ScreenPoint(new PointF(x, 0), m);
                //g.DrawLine(Pens.Blue, xp.X, xp.Y, xp.X, xp.Y + 20);
                g.DrawLine(Pens.Blue, xp.X, 0, xp.X, 10);
                g.DrawLine(xGridPen, xp.X, 10, xp.X, height);
                g.DrawString(Math.Round(x, 5).ToString(), smallFont, Brushes.Blue, xp.X + 3, -2);
                x += step_x;
                kx += 1;
            } while (xp.X < width && kx < 64);
            g.DrawString("msec", smallFont, Brushes.Blue, width - 60, 15);

            int ky = 0;
            float y = 0; // (int)origin.Y;
            PointF yp = ScreenPoint(new PointF(0, y), m);
            do
            {
                yp = ScreenPoint(new PointF(0, y), m);
                //g.DrawLine(Pens.Red, yp.X, yp.Y, yp.X-20, yp.Y);
                g.DrawLine(Pens.Red, 0, yp.Y, 10, yp.Y);
                g.DrawLine(yGridPen, 10, yp.Y, width, yp.Y);
                g.DrawString(Math.Round(y, 5).ToString(), smallFont, Brushes.Red, 0, yp.Y);
                y += step_y;
                ky += 1;
            } while (yp.Y > 0 && ky < 64);
            g.DrawString("rpm", smallFont, Brushes.Red, 25, height - 30);
        }

        private PointF DataPoint(PointF scr, Matrix m)
        {
            Matrix mr = m.Clone();
            mr.Invert();
            PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
            mr.TransformPoints(po);
            return po[0];
        }
                
        private PointF ScreenPoint(PointF scr, Matrix m)
        {
            PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
            m.TransformPoints(po);
            return po[0];
        }        
    }

    public enum Orientation
    {
        horizontal, vertical
    }
}
