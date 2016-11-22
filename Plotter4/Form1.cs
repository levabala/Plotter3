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
            /*InterpolationModeCB.SelectedValueChanged += InterpolationModeCB_SelectedValueChanged;
            CompositingModeCB.SelectedValueChanged += CompositingModeCB_SelectedValueChanged;
            CompositingQualityCB.SelectedValueChanged += CompositingQualityCB_SelectedValueChanged;
            SmoothingModeCB.SelectedValueChanged += SmoothingModeCB_SelectedValueChanged;
            ViewStyleCB.SelectedValueChanged += ViewStyleCB_SelectedValueChanged;*/

            PlotsTreeView.AfterSelect += PlotsTreeView_AfterSelect;
        }

        private void PlotsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                SmoothingModeCB.SelectedValue = ((Plot)e.Node.Tag).graphicsProps.SmoothingMode;
                CompositingQualityCB.SelectedValue = ((Plot)e.Node.Tag).graphicsProps.SmoothingMode;
                SmoothingModeCB.SelectedValue = ((Plot)e.Node.Tag).graphicsProps.SmoothingMode;
                SmoothingModeCB.SelectedValue = ((Plot)e.Node.Tag).graphicsProps.SmoothingMode;
            }
        }

        private void ViewStyleCB_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        /*private void SmoothingModeCB_SelectedValueChanged(object sender, EventArgs e)
        {
            
            if ((SmoothingMode)SmoothingModeCB.SelectedValue != SmoothingMode.Invalid)
                ((Plot)DrawStructsList.SelectedValue).graphicsProps.SmoothingMode = (SmoothingMode)SmoothingModeCB.SelectedValue;
        }

        private void CompositingQualityCB_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((CompositingQuality)CompositingQualityCB.SelectedValue != CompositingQuality.Invalid)
                ((Plot)DrawStructsList.SelectedValue).graphicsProps.CompositingQuality = (CompositingQuality)CompositingQualityCB.SelectedValue;
        }

        private void CompositingModeCB_SelectedValueChanged(object sender, EventArgs e)
        {
            ((Plot)DrawStructsList.SelectedValue).graphicsProps.CompositingMode = (CompositingMode)CompositingModeCB.SelectedValue;
        }

        private void InterpolationModeCB_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((InterpolationMode)InterpolationModeCB.SelectedValue != InterpolationMode.Invalid)
                ((Plot)DrawStructsList.SelectedValue).graphicsProps.InterpolationMode = (InterpolationMode)InterpolationModeCB.SelectedValue;
        }*/

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
            //PlotsTreeView.DataSource = new List<Plot>() { p };
            ViewStyleCB.DataSource = Enum.GetValues(typeof(ViewStyle));            

            CompositingModeCB.DataSource = Enum.GetValues(typeof(CompositingMode));
            CompositingQualityCB.DataSource = Enum.GetValues(typeof(CompositingQuality));
            SmoothingModeCB.DataSource = Enum.GetValues(typeof(SmoothingMode));
            InterpolationModeCB.DataSource = Enum.GetValues(typeof(InterpolationMode));
        }        

        public void ProgressCallBack(double progress)
        {
            Text = (Math.Round(progress*100)).ToString()+"%";
            Application.DoEvents();
        }        
    }    
}
