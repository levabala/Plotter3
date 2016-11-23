using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Plotter4
{
    public enum ViewStyle
    {
        Points, Lines
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();                        
            PlotsTreeView.AfterSelect += PlotsTreeView_AfterSelect;
        }

        private void PlotsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                
            }
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

            SimplePlot p = new SimplePlot(DrawBox, new ControlCollection(this));
            p.AddAxisControl(internalAxis1);
            p.AddAxisControl(internalAxis2);
            p.Parse(path, signals.ToArray(), ProgressCallBack);

            //init graphic changers                                    
            propertyGrid1.SelectedObject = p;     
        }        

        public void ProgressCallBack(double progress)
        {
            Text = (Math.Round(progress*100)).ToString()+"%";
            Application.DoEvents();
        }        
    }    
}
