using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modules;


namespace Plotter4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Show();

            //Command Line Args Parsing
            string[] args = Environment.GetCommandLineArgs();            
            string path = args[1];
            List<string> signals = new List<string>();
            for (int i = 2; i < args.Length; i++)
                signals.Add(args[i]);

            Plot p = new Plot();
            p.Parse(path, signals.ToArray(), ProgressCallBack);
        }        

        public void ProgressCallBack(double progress)
        {
            Text = (Math.Round(progress*100)).ToString()+"%";
            Application.DoEvents();
        }
    }
}
