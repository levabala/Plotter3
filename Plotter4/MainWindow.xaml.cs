using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Plotter4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Color> colors = new List<Color>()
        {
            Colors.Red,
            Colors.Blue,
            Colors.Green,
            Colors.Orange,
            Colors.Brown
        };
        public MainWindow()
        {
            InitializeComponent();
            window.Loaded += Window_Loaded;            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length < 2)
            {
                MessageBox.Show("input params are absent");
                Close();
                return;
            }

            List<PlotParams> p1 = new List<PlotParams>();
            for (int i = 2; i < args.Length; i++)
                p1.Add(new PlotParams(colors[i-2], byte.Parse(args[i]), false));

            PlotsView pv = new PlotsView(PlotBox1);
            Dictionary<byte, Plot> ps = Plot.CreatePlotsFromFile(p1, args[1]);
            pv.AddPlots(ps.Select(kvp => kvp.Value).ToList());
        }
    }
}
