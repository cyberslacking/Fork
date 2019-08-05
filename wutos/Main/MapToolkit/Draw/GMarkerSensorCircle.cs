using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace MapToolkit.Draw
{
    public class GMarkerSensorCircle: GMapMarker
    {
        /// <summary>
        /// 距离，单位为米 半径
        /// </summary>
        public double Radius;

        private Pen stroke = new Pen(Color.MidnightBlue, 2.5f);
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

        public GMarkerSensorCircle(PointLatLng p, double radius)
            : base(p)
        {
            Radius = radius;
        }

        public override void OnRender(Graphics g)
        {
            // 将距离转换成像素长度
            float R =(float)(Radius / Overlay.Control.MapProvider.Projection.GetGroundResolution((int)Overlay.Control.Zoom, Position.Lat));

           /* if (IsFilled)
            {
                g.FillEllipse(Fill, new System.Drawing.Rectangle(LocalPosition.X - R / 2, LocalPosition.Y - R / 2, R, R));
            }*/
            g.DrawEllipse(Stroke, new RectangleF(LocalPosition.X - R, LocalPosition.Y - R, 2*R, 2*R));
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


    }
}
