using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Plotter3
{
    public partial class RawsViewer : UserControl
    {
        string LAST_FOLDER_REGKEY;
        List<FileInfo> filesInfo = new List<FileInfo>();
        public RawsViewer()
        {            
            InitializeComponent();
            LAST_FOLDER_REGKEY = "last_folder_" + Name;

            MouseLeave += RawsViewer_MouseLeave;
            panel1.MouseDown += RawsViewer_MouseDown;
            FileLoadBar.MouseDown += RawsViewer_MouseDown;
            //listView1.MouseDown += RawsViewer_MouseDown;
            Button.MouseDown += RawsViewer_MouseDown;
            panel1.Hide();                        
        }

        private void RawsViewer_MouseLeave(object sender, EventArgs e)
        {
            HideAll();
        }

        private void OpenFileChoosingBlockButton_MouseEnter(object sender, EventArgs e)
        {
            ShowAll();
        }      

        private void HideAll()
        {
            panel1.Hide();
            BackColor = Color.White;
            Width = Button.Width + 5;
            Height = Button.Height + 5;
        }

        private void ShowAll()
        {
            panel1.Show();
            BackColor = Color.White;
            Width = panel1.Width + 10;
            Height = panel1.Height + 10;
        }

        private void SaveLastPath(string path)
        {
            Microsoft.Win32.Registry.CurrentUser.SetValue(LAST_FOLDER_REGKEY, path);
        }

        private void RawsViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                HideAll();
                return;
            }
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.raw");
                filesInfo.Clear();
                //listView1.Clear();
                foreach (string path in files)
                {                    
                    FileInfo info = new FileInfo(path);
                    filesInfo.Add(info);
                                      
                }
            }
        }
    }
}
