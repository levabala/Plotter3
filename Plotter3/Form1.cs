using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plotter3
{
    public partial class Form1 : Form
    {
        PlotsManager plotsM,plotsM2;        
        List<PlotParams> plotsParams1 = new List<PlotParams>()
        {            
            new PlotParams(@"D:\work\test_6000_fast.raw", Color.DarkBlue, 242, true)
        };
        List<PlotParams> plotsParams2 = new List<PlotParams>()
        {
            new PlotParams(@"D:\work\test_6000_fast.raw", Color.DarkGreen, 245, false)            
        };

        public Form1()
        {            
            InitializeComponent();
            plotsM = new PlotsManager(PlotBox1);
            plotsM2 = new PlotsManager(PlotBox2);
            plotsM.HandleControlKeyDown(RestartMButton);
            plotsM.HandleControlKeyDown(SerializeAllButton);
            plotsM2.HandleControlKeyDown(RestartMButton);
            plotsM2.HandleControlKeyDown(SerializeAllButton);
        }  
                
        private void Form1_Load(object sender, EventArgs e)
        {
            plotsM.CreatePlotsFromFiles(plotsParams1);
            plotsM2.CreatePlotsFromFiles(plotsParams2);
            plotsM.MatrixSavingOn(this);
            plotsM2.MatrixSavingOn(this);
        }
        private void RestartM_Click(object sender, EventArgs e)
        {
            plotsM.ResetMatrix();
            plotsM2.ResetMatrix();
        }

        private void SerializeAll_Click(object sender, EventArgs e)
        {
            plotsM.SerializeAll();
            plotsM2.SerializeAll();
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
