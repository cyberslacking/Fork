using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace MapToolkit
{
    public class GMarkerLabel: GMapMarker
    {
        private Font font;
        private Pen pen;
        private string text = "value";
        private Brush fill = Brushes.Red;
        public GMarkerLabel(PointLatLng p, string text, Font font)
            : base(p)
        {
            this.font = font;
            Graphics g = Graphics.FromImage(new Bitmap(1, 1));
            SizeF size = g.MeasureString(text, font);
            Offset = new Point(-Size.Width / 2, -Size.Height);
        }

        public override void OnRender(Graphics g)
        {
            Size = g.MeasureString(text, font).ToSize();
            g.DrawRectangle(pen, LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
            g.DrawString(text, new Font(font.FontFamily, Size.Height * 3 / 4, font.Style), fill, new System.Drawing.Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height));
        }

        public void SetString(string text)
        {
            this.text = text;
        }

        public override void Dispose()
        {
            if (font != null)
            {
                font.Dispose();
                font = null;
            }
            base.Dispose();
        }
    }
}