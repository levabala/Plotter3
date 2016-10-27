using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Plotter3
{
    public partial class Form1 : Form
    {
        //LEVA'S VERSION
        PlotsView pv1, pv2;

        string[] args;

        public Form1()
        {
            InitializeComponent();
            pv1 = new PlotsView(PlotBox1);
            pv1.HandleControlKeyDown(RestartMButton);
            pv1.HandleControlKeyDown(SerializeAllButton);            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            args = Environment.GetCommandLineArgs();

            if (args.Length < 2)
            {
                MessageBox.Show("input params are absent");
                Close();
                return;
            }
           
            List<PlotParams> p1 = new List<PlotParams>()
            {
                new PlotParams(Color.Orange, byte.Parse(args[3]), false),
                new PlotParams(Color.Red, byte.Parse(args[2]), false)
            };

            Dictionary<byte, Plot> plots = Plot.CreatePlotsFromFile(p1,args[1]);

            pv1.AddPlots(new List<Plot>() { plots[byte.Parse(args[2])] });//plots.Select(kvp => kvp.Value).ToList());
            //pv2.AddPlots(new List<Plot>() { plots[byte.Parse(args[3])] });
            pv1.MatrixSavingOn(this);
            //pv2.MatrixSavingOn(this);            
        }

        void ProgressAction(long p)
        {
            Text = p.ToString();
            Application.DoEvents();
        }
        private void RestartM_Click(object sender, EventArgs e)
        {
            pv1.ResetMatrix();
            //pv2.ResetMatrix();            
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void internalAxis2_Load(object sender, EventArgs e)
        {

        }

        private void SerializeAll_Click(object sender, EventArgs e)
        {
                        
        }
    }
}

/*Some code
FileStream fs = new FileStream("temp.dat", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
                plotsM = (PlotsManager)formatter.Deserialize(fs);
            }
            catch (SerializationException ex)
            {
                MessageBox.Show("Failed to deserialize","Reason: " + ex.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
*/
