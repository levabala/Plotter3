using System;
using System.Drawing;
using System.Windows.Forms;

class PlotBox : Panel
{
    public PlotBox()
    {
        this.SetStyle(ControlStyles.Selectable, true);
        this.TabStop = true;
        BackColor = Color.White;
        DoubleBuffered = true;
        
    }
    protected override void OnMouseDown(MouseEventArgs e)
    {
        this.Focus();
        base.OnMouseDown(e);
    }
    protected override bool IsInputKey(Keys keyData)
    {
        if (keyData == Keys.Up || keyData == Keys.Down) return true;
        if (keyData == Keys.Left || keyData == Keys.Right) return true;
        return base.IsInputKey(keyData);
    }
    protected override void OnMouseEnter(EventArgs e)
    {
        this.Invalidate();
        base.OnEnter(e);
        this.Focus();
    }
    protected override void OnLeave(EventArgs e)
    {
        this.Invalidate();
        base.OnLeave(e);
    }
    protected override void OnPaint(PaintEventArgs pe)
    {
        base.OnPaint(pe);
        if (this.Focused)
        {
            var rc = this.ClientRectangle;
            rc.Inflate(-2, -2);
            ControlPaint.DrawFocusRectangle(pe.Graphics, rc);
        }
    }
}