﻿using System.Drawing;

namespace Plotter4
{
    class DrawStruct
    {
        public Pen pen;
        public ViewStyle drawStyle = ViewStyle.Lines;
        public PointF[] outputArr;
        public string log; //debug

        public virtual void UpdateOutputArray(double leftX, double rightX)
        {

        }

        public DrawStruct(Pen p)
        {
            pen = p;
        }                

        public DrawStruct(Pen p, PointF[] array)
        {
            pen = p;
            outputArr = array;
        }
    }    
}
