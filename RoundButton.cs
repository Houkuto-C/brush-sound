using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BrushSound
{
    /// <summary>
    /// 圆角按钮。支持悬停/按下时颜色自动变亮/变暗。
    /// </summary>
    public class RoundButton : Button
    {
        public int CornerRadius { get; set; } = 8;

        private bool isHover = false;
        private bool isPressed = false;

        public RoundButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHover = false;
            isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            isPressed = true;
            Invalidate();
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            isPressed = false;
            Invalidate();
            base.OnMouseUp(mevent);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent != null ? Parent.BackColor : BackColor);

            Color fill = BackColor;
            if (!Enabled)
                fill = Blend(BackColor, Color.Gray, 0.5f);
            else if (isPressed)
                fill = Darken(BackColor, 0.88f);
            else if (isHover)
                fill = Lighten(BackColor, 1.12f);

            using (var path = GetRoundedPath(ClientRectangle, CornerRadius))
            using (var brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
            }

            // 文字
            Color textColor = Enabled ? ForeColor : Color.Gray;
            TextRenderer.DrawText(
                g, Text, Font, ClientRectangle, textColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis);
        }

        private static GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0) return path;

            int d = radius * 2;
            if (d > rect.Width) d = rect.Width;
            if (d > rect.Height) d = rect.Height;

            rect.Width -= 1;
            rect.Height -= 1;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static Color Lighten(Color c, float factor)
        {
            int r = (int)Math.Min(255, c.R * factor);
            int g = (int)Math.Min(255, c.G * factor);
            int b = (int)Math.Min(255, c.B * factor);
            return Color.FromArgb(c.A, r, g, b);
        }

        private static Color Darken(Color c, float factor)
        {
            int r = (int)(c.R * factor);
            int g = (int)(c.G * factor);
            int b = (int)(c.B * factor);
            return Color.FromArgb(c.A, r, g, b);
        }

        private static Color Blend(Color a, Color b, float t)
        {
            int r = (int)(a.R * (1 - t) + b.R * t);
            int g = (int)(a.G * (1 - t) + b.G * t);
            int bl = (int)(a.B * (1 - t) + b.B * t);
            return Color.FromArgb(a.A, r, g, bl);
        }
    }
}