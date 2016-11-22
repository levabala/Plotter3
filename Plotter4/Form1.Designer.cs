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
            this.ControlsGroup = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CompositingQualityCB = new System.Windows.Forms.ComboBox();
            this.CompositingModeCB = new System.Windows.Forms.ComboBox();
            this.InterpolationModeCB = new System.Windows.Forms.ComboBox();
            this.SmoothingModeCB = new System.Windows.Forms.ComboBox();
            this.DrawBox = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ViewStyleCB = new System.Windows.Forms.ComboBox();
            this.internalAxis2 = new Plotter4.InternalAxis();
            this.internalAxis1 = new Plotter4.InternalAxis();
            this.PlotsTreeView = new System.Windows.Forms.TreeView();
            this.ControlsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DrawBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ControlsGroup
            // 
            this.ControlsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlsGroup.Controls.Add(this.PlotsTreeView);
            this.ControlsGroup.Controls.Add(this.label5);
            this.ControlsGroup.Controls.Add(this.ViewStyleCB);
            this.ControlsGroup.Controls.Add(this.label4);
            this.ControlsGroup.Controls.Add(this.label3);
            this.ControlsGroup.Controls.Add(this.label2);
            this.ControlsGroup.Controls.Add(this.label1);
            this.ControlsGroup.Controls.Add(this.CompositingQualityCB);
            this.ControlsGroup.Controls.Add(this.CompositingModeCB);
            this.ControlsGroup.Controls.Add(this.InterpolationModeCB);
            this.ControlsGroup.Controls.Add(this.SmoothingModeCB);
            this.ControlsGroup.Location = new System.Drawing.Point(653, 34);
            this.ControlsGroup.Name = "ControlsGroup";
            this.ControlsGroup.Size = new System.Drawing.Size(188, 416);
            this.ControlsGroup.TabIndex = 2;
            this.ControlsGroup.TabStop = false;
            this.ControlsGroup.Text = "Controls";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(7, 301);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "CompositingQuality";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(7, 254);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "CompositingMode";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(7, 207);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "SmoothingMode";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(7, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "InterpolationMode";
            // 
            // CompositingQualityCB
            // 
            this.CompositingQualityCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CompositingQualityCB.BackColor = System.Drawing.SystemColors.Control;
            this.CompositingQualityCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CompositingQualityCB.FormattingEnabled = true;
            this.CompositingQualityCB.Location = new System.Drawing.Point(7, 321);
            this.CompositingQualityCB.Name = "CompositingQualityCB";
            this.CompositingQualityCB.Size = new System.Drawing.Size(159, 24);
            this.CompositingQualityCB.TabIndex = 4;
            // 
            // CompositingModeCB
            // 
            this.CompositingModeCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CompositingModeCB.BackColor = System.Drawing.SystemColors.Control;
            this.CompositingModeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CompositingModeCB.FormattingEnabled = true;
            this.CompositingModeCB.Location = new System.Drawing.Point(7, 274);
            this.CompositingModeCB.Name = "CompositingModeCB";
            this.CompositingModeCB.Size = new System.Drawing.Size(158, 24);
            this.CompositingModeCB.TabIndex = 3;
            // 
            // InterpolationModeCB
            // 
            this.InterpolationModeCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InterpolationModeCB.BackColor = System.Drawing.SystemColors.Control;
            this.InterpolationModeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.InterpolationModeCB.FormattingEnabled = true;
            this.InterpolationModeCB.Location = new System.Drawing.Point(7, 180);
            this.InterpolationModeCB.Name = "InterpolationModeCB";
            this.InterpolationModeCB.Size = new System.Drawing.Size(158, 24);
            this.InterpolationModeCB.TabIndex = 2;
            // 
            // SmoothingModeCB
            // 
            this.SmoothingModeCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SmoothingModeCB.BackColor = System.Drawing.SystemColors.Control;
            this.SmoothingModeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SmoothingModeCB.FormattingEnabled = true;
            this.SmoothingModeCB.Location = new System.Drawing.Point(7, 227);
            this.SmoothingModeCB.Name = "SmoothingModeCB";
            this.SmoothingModeCB.Size = new System.Drawing.Size(158, 24);
            this.SmoothingModeCB.TabIndex = 1;
            // 
            // DrawBox
            // 
            this.DrawBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DrawBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DrawBox.Location = new System.Drawing.Point(66, 34);
            this.DrawBox.Margin = new System.Windows.Forms.Padding(0);
            this.DrawBox.Name = "DrawBox";
            this.DrawBox.Size = new System.Drawing.Size(584, 562);
            this.DrawBox.TabIndex = 3;
            this.DrawBox.TabStop = false;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Location = new System.Drawing.Point(7, 348);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "ViewStyle";
            // 
            // ViewStyleCB
            // 
            this.ViewStyleCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ViewStyleCB.BackColor = System.Drawing.SystemColors.Control;
            this.ViewStyleCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ViewStyleCB.FormattingEnabled = true;
            this.ViewStyleCB.Location = new System.Drawing.Point(7, 368);
            this.ViewStyleCB.Name = "ViewStyleCB";
            this.ViewStyleCB.Size = new System.Drawing.Size(159, 24);
            this.ViewStyleCB.TabIndex = 9;
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
            this.internalAxis2.Size = new System.Drawing.Size(70, 562);
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
            // PlotsTreeView
            // 
            this.PlotsTreeView.Location = new System.Drawing.Point(7, 21);
            this.PlotsTreeView.Name = "PlotsTreeView";
            this.PlotsTreeView.Size = new System.Drawing.Size(159, 136);
            this.PlotsTreeView.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(840, 595);
            this.Controls.Add(this.DrawBox);
            this.Controls.Add(this.ControlsGroup);
            this.Controls.Add(this.internalAxis2);
            this.Controls.Add(this.internalAxis1);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ControlsGroup.ResumeLayout(false);
            this.ControlsGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DrawBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private InternalAxis internalAxis1;
        private System.Windows.Forms.GroupBox ControlsGroup;
        private System.Windows.Forms.ComboBox SmoothingModeCB;
        private System.Windows.Forms.ComboBox CompositingQualityCB;
        private System.Windows.Forms.ComboBox CompositingModeCB;
        private System.Windows.Forms.ComboBox InterpolationModeCB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox DrawBox;
        private InternalAxis internalAxis2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox ViewStyleCB;
        private System.Windows.Forms.TreeView PlotsTreeView;
    }
}

