using System.Drawing;

namespace Plotter4
{
    abstract class DrawStruct
    {
        public Pen pen;
        public PointF[] outputArr;
        public string log; //debug

        public virtual void UpdateOutputArray(double leftX, double rightX)
        {

        }

        public DrawStruct(Pen p)
        {
            pen = p;
        }        
    }
}
