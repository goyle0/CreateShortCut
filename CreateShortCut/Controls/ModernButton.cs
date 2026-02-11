using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CreateShortCut.Styles;

namespace CreateShortCut.Controls
{
    public class ModernButton : Button
    {
        private int _cornerRadius = 8;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private Color _baseColor = AppColors.Primary;

        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value; Invalidate(); }
        }

        public ModernButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor = Color.White;
            Cursor = Cursors.Hand;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            _baseColor = BackColor;
        }

        private Color Darken(Color color, float factor)
        {
            return Color.FromArgb(color.A,
                (int)Math.Max(color.R * factor, 0),
                (int)Math.Max(color.G * factor, 0),
                (int)Math.Max(color.B * factor, 0));
        }

        private Color GetCurrentBackColor()
        {
            if (_isPressed) return Darken(_baseColor, 0.7f);
            if (_isHovered) return Darken(_baseColor, 0.85f);
            return _baseColor;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? SystemColors.Control);

            using (var path = CreateRoundedRectangle(new Rectangle(0, 0, Width - 1, Height - 1), _cornerRadius))
            {
                using (var brush = new SolidBrush(GetCurrentBackColor()))
                {
                    g.FillPath(brush, path);
                }
            }

            // テキスト描画
            TextRenderer.DrawText(g, Text, Font, new Rectangle(0, 0, Width, Height), ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            _isPressed = true;
            Invalidate();
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(mevent);
        }
    }
}
