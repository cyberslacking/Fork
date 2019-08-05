using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.CompilerServices;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;




namespace MapToolkit
{
    public class GMarkerEllipse: GMapMarker
    {

        private Pen stroke = new Pen(Color.FromArgb(155, Color.MidnightBlue));
        private Brush fill = Brushes.AliceBlue;

        public Brush Fill
        {
            set
            {
                fill = value;
            }
        }

        public Pen Stroke
        {
            set
            {
                stroke = value;
            }
            get
            {
                return stroke;
            }
        }

        public GMarkerEllipse(PointLatLng p)
            : base(p)
        {
        }

        public override void OnRender(Graphics g)
        {
            g.FillEllipse(fill, new System.Drawing.Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height));
            g.DrawEllipse(stroke, new System.Drawing.Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height));
        }


        public override void Dispose()
        {
            if (stroke != null)
            {
                stroke.Dispose();
                stroke = null;
            }
            base.Dispose();
        }

        public void Adapter(PointLatLng lt,PointLatLng rb)
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