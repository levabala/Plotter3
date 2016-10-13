using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Plotter3
{
    class Vector
    {
        public float length, alpha, dx, dy;
        public PointF start, end;
        public Vector(PointF s, PointF e)
        {
            start = s;
            end = e;
            length = getLength();
            alpha = getAlpha();
        }
        public Vector(float len, float al, PointF s)
        {
            length = len;
            alpha = al;
            start = s;
            end = getEndPoint();
        }

        public PointF GetPointByLength(float l)
        {
            PointF output = new PointF();
            float coeff = l / length;
            output.X = dx * coeff;
            output.Y = dy * coeff;

            return output;
        }

        public void Translate(float dx, float dy)
        {
            start.X += dx;
            end.X += dx;

            start.Y += dy;
            end.Y += dy;
        }

        public void NextPoint()
        {
            start = end;
            end = new PointF(end.X + dx, end.Y + dy);
        }

        public void rotateTo(float al)
        {
            alpha = al;
            end = getEndPoint();
        }

        public void changeLength(float len)
        {
            length = len;
            end = getEndPoint();
            dx = end.X - start.X;
            dy = end.Y - start.Y;
        }
        public PointF getEndPoint()
        {
            PointF pf = new PointF((float)(this.length * Math.Cos(alpha) + start.X), (float)(this.length * Math.Sin(alpha) + start.Y));
            return pf;
        }

        public float getLength()
        {
            float lengthX = Math.Abs(end.X - start.X);
            float lengthY = Math.Abs(end.Y - start.Y);
            dx = lengthX;
            dy = lengthY;
            return (float)Math.Sqrt(lengthX * lengthX + lengthY * lengthY);
        }

        public float getAlpha()
        {
            float ddx = end.X - start.X;
            float ddy = end.Y - start.Y;
            if (ddx == 0f) return 0f;
            float angle = (float)Math.Atan(ddy / ddx);
            if ((ddx < 0 && ddy < 0) || (ddx < 0 && ddy >= 0)) angle = (float)(angle - Math.PI);         
            return angle;
        }
    }
}
