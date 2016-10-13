namespace Plotter3
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
            this.ControlsBox = new System.Windows.Forms.GroupBox();
            this.RestartM = new System.Windows.Forms.Button();
            this.SerializeAll = new System.Windows.Forms.Button();
            this.ControlsBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ControlsBox
            // 
            this.ControlsBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlsBox.AutoSize = true;
            this.ControlsBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.ControlsBox.Controls.Add(this.SerializeAll);
            this.ControlsBox.Controls.Add(this.RestartM);
            this.ControlsBox.Font = new System.Drawing.Font("Monospac821 BT", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ControlsBox.Location = new System.Drawing.Point(778, 12);
            this.ControlsBox.Name = "ControlsBox";
            this.ControlsBox.Size = new System.Drawing.Size(248, 801);
            this.ControlsBox.TabIndex = 0;
            this.ControlsBox.TabStop = false;
            this.ControlsBox.Text = "Controls";
            this.ControlsBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
            // 
            // RestartM
            // 
            this.RestartM.Location = new System.Drawing.Point(6, 28);
            this.RestartM.Name = "RestartM";
            this.RestartM.Size = new System.Drawing.Size(236, 49);
            this.RestartM.TabIndex = 0;
            this.RestartM.Text = "Restart Matrix";
            this.RestartM.UseVisualStyleBackColor = true;
            this.RestartM.Click += new System.EventHandler(this.RestartM_Click);
            this.RestartM.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
            // 
            // SerializeAll
            // 
            this.SerializeAll.Location = new System.Drawing.Point(6, 83);
            this.SerializeAll.Name = "SerializeAll";
            this.SerializeAll.Size = new System.Drawing.Size(236, 49);
            this.SerializeAll.TabIndex = 1;
            this.SerializeAll.Text = "Serialize Objects";
            this.SerializeAll.UseVisualStyleBackColor = true;
            this.SerializeAll.Click += new System.EventHandler(this.SerializeAll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 825);
            this.Controls.Add(this.ControlsBox);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Plotter3";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ControlsBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox ControlsBox;
        private System.Windows.Forms.Button RestartM;
        private System.Windows.Forms.Button SerializeAll;
    }
}

