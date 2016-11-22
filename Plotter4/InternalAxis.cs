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

namespace Plotter4
{
    public partial class InternalAxis : UserControl
    {
        public OrientationType orientation;
        Font smallFont = new Font("Calibri", 13, FontStyle.Bold);        
        Pen xGridPen = new Pen(Brushes.Blue, 0.5f);
        Pen yGridPen = new Pen(Brushes.Green, 0.5f);
        Matrix m = new Matrix();
        Control TargetControl;

        PointF startP, endP;

        public InternalAxis()
        {
            InitializeComponent();

            TargetControl = Parent;

            xGridPen.DashStyle = DashStyle.Dot;
            yGridPen.DashStyle = DashStyle.Dot;

            if (orientation == OrientationType.Horizontal)
            {
                getPointsHor();
                Cursor = Cursors.SizeWE;
                Paint += PaintHorizontalAxis;
            }
            else
            {
                getPointsVert();
                Cursor = Cursors.SizeNS;
                Paint += PaintVerticalAxis;
            }
        }

        public void SetTargetControl(Control c)
        {
            TargetControl = c;
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

            Margin = new Padding(0, 0, 0, 0);
        }
        
        public void FitFont()
        {
            if (ParentForm == null) return;
            float w = ClientRectangle.Width - 15;
            float h = ClientRectangle.Height;
            float size = smallFont.Size;

            Graphics g = ParentForm.CreateGraphics();            

            if (orientation == OrientationType.Horizontal)
            {
                float baseH = g.MeasureString("123", smallFont).Height;                                
                if (baseH > h / 1.5)
                    do
                    {
                        size--;
                        smallFont = new Font("Calibri", size, FontStyle.Bold);
                        baseH = g.MeasureString("123", smallFont).Height;
                    } while (baseH > h / 1.5);
                else
                    do
                    {
                        size++;
                        smallFont = new Font("Calibri", size, FontStyle.Bold);
                        baseH = g.MeasureString("123", smallFont).Height;
                    } while (baseH < h / 1.5);
            }            
            else
            {
                float baseW = g.MeasureString("12345", smallFont).Width;
                if (baseW > w)
                    do
                    {
                        size--;
                        smallFont = new Font("Calibri", size, FontStyle.Bold);
                        baseW = g.MeasureString("1235", smallFont).Width;
                    } while (baseW > w);
                else
                    do
                    {
                        size++;
                        smallFont = new Font("Calibri", size, FontStyle.Bold);
                        baseW = g.MeasureString("1235", smallFont).Width;
                    } while (baseW < w);
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
            Update();
            Invalidate();
        }

        private void PaintHorizontalAxis(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PointF axes_step_pt = new PointF(0, 0);
            PointF axes_step_pt2 = new PointF(110, 60);
            PointF axes_step = DataPoint(axes_step_pt, m);
            PointF axes_step2 = DataPoint(axes_step_pt2, m);

            int power = 2;

            float dx = axes_step2.X - axes_step.X;
            double lg_x = Math.Log(Math.Abs(dx), power);
            double pow_x = Math.Pow(power, Math.Floor(lg_x) + 1);
            float step_x = (float)pow_x;
            if (step_x > 1) step_x = (float)Math.Round(pow_x);

            int kx = 0;
            PointF origin = DataPoint(startP, m);
            float x = ((int)(origin.X / step_x) + 1) * step_x - step_x * 3;
            PointF xp = ScreenPoint(new PointF(x, 0), m);
            List<ValPos> labels = new List<ValPos>();
            do
            {
                xp = ScreenPoint(new PointF(x, 0), m);
                //g.DrawLine(Pens.Blue, xp.X, xp.Y, xp.X, xp.Y + 20);
                g.DrawLine(Pens.Blue, xp.X, 0, xp.X, 10);
                g.DrawLine(xGridPen, xp.X, 10, xp.X, ClientRectangle.Height / 1.5f);
                //g.DrawString(string.Format("{0, 5}", Math.Round(x, 3)), smallFont, Brushes.Blue, xp.X, ClientRectangle.Height - g.MeasureString(x.ToString(), smallFont).Height);
                labels.Add(new ValPos((float)Math.Round(x, 3), xp.X));
                x += step_x;
                kx += 1;
            } while (xp.X < endP.X && kx < 64);

            float biggestValue = 0;
            foreach (ValPos vp in labels)
                if (Math.Abs(Math.Round(vp.value)) > biggestValue)
                    biggestValue = (float)Math.Round(Math.Abs(vp.value));
            float log = (float)Math.Floor(Math.Log10(biggestValue));
            if (biggestValue < 10) log = 1;
            log--;
            float coeff = (float)Math.Pow(10, log);
            if (coeff < 1000) coeff = 1;

            foreach (ValPos vp in labels)
                g.DrawString(string.Format("{0, 4}", Math.Round(vp.value / coeff, 4)), smallFont, Brushes.Blue, vp.pos, ClientRectangle.Height - g.MeasureString(vp.value.ToString(), smallFont).Height);

            Font titleFont = GetAxisHorTitleFont(ClientRectangle.Height, "msec x" + coeff.ToString());
            //g.DrawString("rpm x" + coeff.ToString(), titleFont, Brushes.Red, ClientRectangle.Width - g.MeasureString("rpm x" + coeff.ToString(), titleFont).Width, endP.Y - 30);


            g.DrawString("msec x" + coeff.ToString(), titleFont, Brushes.Blue, ClientRectangle.Width - g.MeasureString("msec x" + coeff.ToString(), titleFont).Width - 5, -g.MeasureString("m", titleFont).Height / 5f);
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
            List<ValPos> labels = new List<ValPos>();
            do
            {
                yp = ScreenPoint(new PointF(startP.X, y), m);
                //g.DrawLine(Pens.Red, yp.X, yp.Y, yp.X-20, yp.Y);
                g.DrawLine(Pens.Red, 0, yp.Y, 10, yp.Y);
                g.DrawLine(yGridPen, 10, yp.Y, startP.X, yp.Y);
                //g.DrawString(string.Format("{0, 5}", Math.Round(y, 3)), smallFont, Brushes.Red, 0, yp.Y);
                labels.Add(new ValPos((float)Math.Round(y, 4), yp.Y));
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
                //g.DrawString(string.Format("{0, 5}", Math.Round(y, 5)), smallFont, Brushes.Red, 0, yp.Y);
                labels.Add(new ValPos((float)Math.Round(y, 4), yp.Y));
                y -= step_y;
                ky += 1;
            } while (ky < 64);            

            float biggestValue = 0;
            foreach (ValPos vp in labels)
                if (Math.Abs(Math.Round(vp.value)) > biggestValue)
                    biggestValue = (float)Math.Round(Math.Abs(vp.value));
            float log = (float)Math.Floor(Math.Log10(biggestValue));
            if (biggestValue < 10) log = 1;
            log--;
            float coeff = (float)Math.Pow(10, log);
            if (coeff < 1000) coeff = 1;

            foreach (ValPos vp in labels)
                g.DrawString(string.Format("{0, 4}", Math.Round(vp.value / coeff, 4)), smallFont, Brushes.Red, 0, vp.pos);

            Font titleFont = GetAxisVertTitleFont(ClientRectangle.Width, "rpm x" + coeff.ToString());
            g.DrawString("rpm x"+coeff.ToString(), titleFont, Brushes.Red, ClientRectangle.Width - g.MeasureString("rpm x"+coeff.ToString(), titleFont).Width, endP.Y - 30);
        }

        public Font GetAxisHorTitleFont(float height, string title)
        {
            float size = 4;
            height -= 10;
            Font font = new Font("Calibri", size, FontStyle.Bold);
            if (title.Length < 1) return font;
            Graphics g = CreateGraphics();
            float titleH = g.MeasureString(title, font).Height / 2;
            if (titleH < height / 2)
            {
                do
                {
                    size++;
                    font = new Font("Calibri", size, FontStyle.Bold);
                    titleH = g.MeasureString(title, font).Height / 2;
                } while (titleH < height / 2);
            }
            font = new Font("Calibri", size-1, FontStyle.Bold);
            return font;
        }

        public Font GetAxisVertTitleFont(float width, string title)
        {
            float size = 13;            
            Font font = new Font("Calibri", size, FontStyle.Bold);
            if (title.Length < 1) return font;
            Graphics g = CreateGraphics();
            float titleW = g.MeasureString(title, font).Width;
            if (titleW > width)
            {
                do
                {
                    size--;
                    font = new Font("Calibri", size, FontStyle.Bold);
                    titleW = g.MeasureString(title, font).Width;
                } while (titleW > width);
            }
            else {
                title += "a";
                titleW = g.MeasureString(title, font).Width;
                if (titleW < width)
                {
                    title.Remove(title.Length - 2, 2);
                    do
                    {
                        size++;
                        font = new Font("Calibri", size, FontStyle.Bold);
                        titleW = g.MeasureString(title, font).Width;
                    } while (titleW < width);
                }
            }
            return font;
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
            FitFont();
        }

        private PointF ScreenPoint(PointF scr, Matrix m)
        {
            PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
            m.TransformPoints(po);
            return po[0];
        }
    }

    struct ValPos
    {
        public float value, pos;
        public ValPos(float value, float pos)
        {
            this.value = value;
            this.pos = pos;
        }
    }
}
