using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

public class ShapeButton : Button
{
    protected override void OnPaint(PaintEventArgs pevent)
    {
        var graphicsPath = new GraphicsPath();
        int radius = 20; // 角丸の半径を設定します
        graphicsPath.AddLine(radius, 0, Width - radius, 0);
        graphicsPath.AddArc(Width - radius, 0, radius, radius, 270, 90);
        graphicsPath.AddLine(Width, radius, Width, Height - radius);
        graphicsPath.AddArc(Width - radius, Height - radius, radius, radius, 0, 90);
        graphicsPath.AddLine(Width - radius, Height, radius, Height);
        graphicsPath.AddArc(0, Height - radius, radius, radius, 90, 90);
        graphicsPath.AddLine(0, Height - radius, 0, radius);
        graphicsPath.AddArc(0, 0, radius, radius, 180, 90);
        this.Region = new Region(graphicsPath);

        base.OnPaint(pevent);
    }
}
