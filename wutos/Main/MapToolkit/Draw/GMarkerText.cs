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
    public class GMarkerText: GMapMarker
    {

        private Font font = new Font("Arial", 8.25F);
        private Brush fill = Brushes.Red;
        private string text = "未命名";

        public Brush Fill
        {
            set
            {
                fill = value;
            }
        }

        public string Text
        {
            set
            {
                text = value;
            }
        }

        public Font Font
        {
            set
            {
                font = value;
            }
            get
            {
                return font;
            }
        }

        public GMarkerText(PointLatLng p)
            : base(p)
        {
        }

        public override void OnRender(Graphics g)
        {
            if (Size.Height <= 1)
                return;
            g.DrawString(text, new Font(font.FontFamily, Size.Height * 3 / 4, font.Style), fill, new System.Drawing.Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height));
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

        public void Adapter(GPoint gpLT,GPoint gpRB)
        {
            int Width = (int)(gpRB.X - gpLT.X);
            int Height = (int)(gpRB.Y - gpLT.Y);

            int xCenter = (int)gpLT.X + Width / 2;
            int yCenter = (int)gpLT.Y + Height / 2;

            Position = Global.control.FromLocalToLatLng(xCenter, yCenter);
            Size = new System.Drawing.Size(Width, Height);
            Offset = new Point(-Size.Width / 2, -Size.Height / 2);
  
        }


        public void Adapter(GPoint gpLT, int width, int height)
        {

            int xCenter = (int)gpLT.X + width / 2;
            int yCenter = (int)gpLT.Y + height / 2;

            Position = Global.control.FromLocalToLatLng(xCenter, yCenter);
            Size = new System.Drawing.Size(width, height);
            Offset = new Point(-Size.Width / 2, -Size.Height / 2);

        }

    }
}