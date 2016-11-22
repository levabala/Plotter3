namespace Plotter4
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.internalAxis2 = new Plotter4.InternalAxis();
            this.internalAxis1 = new Plotter4.InternalAxis();
            this.SuspendLayout();
            // 
            // internalAxis2
            // 
            this.internalAxis2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.internalAxis2.BackColor = System.Drawing.Color.Khaki;
            this.internalAxis2.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.internalAxis2.Location = new System.Drawing.Point(0, 34);
            this.internalAxis2.Margin = new System.Windows.Forms.Padding(0);
            this.internalAxis2.Name = "internalAxis2";
            this.internalAxis2.Orientation = Plotter4.InternalAxis.OrientationType.Vertical;
            this.internalAxis2.Size = new System.Drawing.Size(70, 469);
            this.internalAxis2.TabIndex = 1;
            // 
            // internalAxis1
            // 
            this.internalAxis1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.internalAxis1.BackColor = System.Drawing.Color.Khaki;
            this.internalAxis1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.internalAxis1.Location = new System.Drawing.Point(0, 0);
            this.internalAxis1.Margin = new System.Windows.Forms.Padding(0);
            this.internalAxis1.Name = "internalAxis1";
            this.internalAxis1.Size = new System.Drawing.Size(842, 34);
            this.internalAxis1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(840, 502);
            this.Controls.Add(this.internalAxis2);
            this.Controls.Add(this.internalAxis1);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private InternalAxis internalAxis1;
        private InternalAxis internalAxis2;
    }
}

