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
    public class GMarkerEx: GMapMarker
    {
        Bitmap Bitmap;
        public GMarkerEx(PointLatLng p, Bitmap Bitmap)
            : base(p)
        {
            this.Bitmap = Bitmap;
            Size = new System.Drawing.Size(Bitmap.Width, Bitmap.Height);
            Offset = new Point(-Size.Width / 2, -Size.Height);
        }

        public override void OnRender(Graphics g)
        {
            g.DrawImage(Bitmap, LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
        }

        public void SetBitmap(Bitmap Bitmap)
        {
            this.Bitmap = Bitmap;
            Size = new System.Drawing.Size(Bitmap.Width, Bitmap.Height);
            Offset = new Point(-Size.Width / 2, -Size.Height);
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