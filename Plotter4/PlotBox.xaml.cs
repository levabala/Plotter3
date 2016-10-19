using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Plotter4
{
    /// <summary>
    /// Interaction logic for PlotBox.xaml
    /// </summary>
    public partial class PlotBox : UserControl
    {
        public PlotBox()
        {
            InitializeComponent();
            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Arrange(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));             
        }

        public void AddChild(UIElement el)
        {
            Canvas.Children.Add(el);
        }

        public void Clear()
        {
            Canvas.Children.Clear();
        }
    }
}
