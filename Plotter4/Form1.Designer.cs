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
            this.DrawBox = new System.Windows.Forms.PictureBox();
            this.PlotsTreeView = new System.Windows.Forms.TreeView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.internalAxis2 = new Plotter4.InternalAxis();
            this.internalAxis1 = new Plotter4.InternalAxis();
            this.ControlsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DrawBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ControlsGroup
            // 
            this.ControlsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlsGroup.Controls.Add(this.propertyGrid1);
            this.ControlsGroup.Controls.Add(this.PlotsTreeView);
            this.ControlsGroup.Location = new System.Drawing.Point(596, 34);
            this.ControlsGroup.Name = "ControlsGroup";
            this.ControlsGroup.Size = new System.Drawing.Size(245, 562);
            this.ControlsGroup.TabIndex = 2;
            this.ControlsGroup.TabStop = false;
            this.ControlsGroup.Text = "Controls";
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
            this.DrawBox.Size = new System.Drawing.Size(534, 562);
            this.DrawBox.TabIndex = 3;
            this.DrawBox.TabStop = false;
            // 
            // PlotsTreeView
            // 
            this.PlotsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlotsTreeView.Location = new System.Drawing.Point(7, 21);
            this.PlotsTreeView.Name = "PlotsTreeView";
            this.PlotsTreeView.Size = new System.Drawing.Size(225, 136);
            this.PlotsTreeView.TabIndex = 4;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.propertyGrid1.Location = new System.Drawing.Point(7, 163);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(225, 393);
            this.propertyGrid1.TabIndex = 4;
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
            ((System.ComponentModel.ISupportInitialize)(this.DrawBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private InternalAxis internalAxis1;
        private System.Windows.Forms.GroupBox ControlsGroup;
        private System.Windows.Forms.PictureBox DrawBox;
        private InternalAxis internalAxis2;
        private System.Windows.Forms.TreeView PlotsTreeView;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}

