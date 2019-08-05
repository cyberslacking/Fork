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
    public class GMarkerImage: GMapMarker
    {

        private Bitmap bmp = new Bitmap(1, 1);

        public Bitmap Bmp
        {
            set
            {
                bmp = value;
            }
            get
            {
                return bmp;
            }
        }

        public GMarkerImage(PointLatLng p)
            : base(p)
        {
        }

        public override void OnRender(Graphics g)
        {
            g.DrawImage(bmp, LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
        }


        public override void Dispose()
        {
            if (bmp != null)
            {
                bmp.Dispose();
                bmp = null;
            }
            base.Dispose();
        }

        public void Adapter(PointLatLng lt, PointLatLng rb)
        {
            GPoint gpLT = Global.control.FromLatLngToLocal(lt);
            GPoint gpRB = Global.control.FromLatLngToLocal(rb);
            int Width = (int)(gpRB.X - gpLT.X);
            int Height = (int)(gpRB.Y - gpLT.Y);

            int xCenter = (int)gpLT.X + Width / 2;
            int yCenter = (int)gpLT.Y + Height / 2;

            Position = Global.control.FromLocalToLatLng(xCenter, yCenter);
            Size = new System.Drawing.Size(Width, Height);
            Offset = new Point(-Size.Width / 2, -Size.Height / 2);
        }
    }
}