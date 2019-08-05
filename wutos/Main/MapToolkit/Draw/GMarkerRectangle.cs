using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace DrawTools
{
    public class GMarkerRectangle: GMapMarker
    {
        private Pen stroke = new Pen(Color.FromArgb(155, Color.MidnightBlue));
        private Brush fill = new SolidBrush(Color.FromArgb(155, Color.AliceBlue));

        public GMarkerRectangle(PointLatLng p, Rectangle rect)
            : base(p)
        {
            Size = new System.Drawing.Size(rect.Width, rect.Height);
            Offset = new System.Drawing.Point(-Size.Width / 2, -Size.Height / 2);
        }

        public override void OnRender(Graphics g)
        {
            g.FillRectangle(fill, new System.Drawing.Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height));
            g.DrawRectangle(stroke, new System.Drawing.Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height));
        }


        public override void Dispose()
        {
            if (Bitmap != null)
            {
                Bitmap.Dispose();
                Bitmap = null;
            }
            base.Dispose();
        }

    }
}