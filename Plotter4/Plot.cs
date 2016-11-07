using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Plotter4
{
    class Plot
    {
        List<DataManager> dms = new List<DataManager>();        
        Matrix m;

        public delegate void ParseFun(string path, string[] args, Action<double> progressCallBack = null);
        public ParseFun Parse;
        public delegate void VisualizeFun(Graphics g, Rectangle drawArea);
        public VisualizeFun Visualize;
        public delegate void ControlFun(object sender, EventArgs e);
        public ControlFun ControlF;

        public Plot()
        {
            Parse = ParseRaw;
            Visualize = Draw;
            ControlF += MouseDrag;
        }        


        //Control functions
        private void MouseDrag(object sender, EventArgs e)
        {
            Control c = (Control)sender;

            //...dms and matrix manipulations...

            Draw(c.CreateGraphics(), c.ClientRectangle);
        }

        private void MouseScroll(object sender, EventArgs e)
        {
            Control c = (Control)sender;

            //...dms and matrix manipulations...

            Draw(c.CreateGraphics(), c.ClientRectangle);
        }

        //Visualize functions
        private void Draw(Graphics g, Rectangle drawArea)
        {
            
        }

        //Parse functions
        private void ParseRaw(string path, string[] args, Action<double> progressCallBack = null)
        {
            byte[] signals = new byte[args.Length];
            for (int i = 0; i < args.Length; i++)
                signals[i] = byte.Parse(args[i]);

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
