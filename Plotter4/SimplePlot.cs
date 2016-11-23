using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Windows.Forms.Form.ControlCollection;

namespace Plotter4
{
    class SimplePlot : Plot
    {
        double? minX, maxX, minY, maxY;        
        public SimplePlot(Control c) : base(c)
        {
            NewDMLoaded += OnDMLoaded;
            control.Paint += Control_Paint;
        }

        private void Control_Paint(object sender, PaintEventArgs e)
        {
            foreach (InternalAxis axis in axes)
                axis.setMatrix(m);
        }

        private void OnDMLoaded(double minX, double maxX, double minY, double maxY)
        {
            if (this.minX == null || minX < this.minX) this.minX = minX;
            if (this.maxX == null || maxX > this.maxX) this.maxX = maxX;
            if (this.minY == null || minY < this.minY) this.minY = minY;
            if (this.maxY == null || maxY > this.maxY) this.maxY = maxY;

            m = GenerateMatrix(maxX, maxY, minX, minY);
            control.Invalidate();
        }        
    }
}
