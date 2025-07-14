using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

public class ShapeButton : Button
{
    private const int DefaultCornerRadius = 20;
    
    public int CornerRadius { get; set; } = DefaultCornerRadius;
    
    protected override void OnPaint(PaintEventArgs pevent)
    {
        using (var graphicsPath = new GraphicsPath())
        {
            int radius = CornerRadius;
            graphicsPath.AddLine(radius, 0, Width - radius, 0);
            graphicsPath.AddArc(Width - radius, 0, radius, radius, 270, 90);
            graphicsPath.AddLine(Width, radius, Width, Height - radius);
            graphicsPath.AddArc(Width - radius, Height - radius, radius, radius, 0, 90);
            graphicsPath.AddLine(Width - radius, Height, radius, Height);
            graphicsPath.AddArc(0, Height - radius, radius, radius, 90, 90);
            graphicsPath.AddLine(0, Height - radius, 0, radius);
            graphicsPath.AddArc(0, 0, radius, radius, 180, 90);
            
            // 既存のRegionを破棄してから新しいRegionを設定
            this.Region?.Dispose();
            this.Region = new Region(graphicsPath);
        }

        base.OnPaint(pevent);
    }
}
