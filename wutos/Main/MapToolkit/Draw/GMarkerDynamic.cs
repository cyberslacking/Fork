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
    public class GMarkerDynamic: GMapMarker
    {
        private System.Timers.Timer _timerDnamic;//网络心跳Timer
        private Int32 diameterSmall = 14;
        private Int32 diameterBig = 20;
        private Brush b, s;

        public GMarkerDynamic(PointLatLng p, Brush big, Brush small)
            : base(p)
        {
            b = big;
            s = small;
            Size = new System.Drawing.Size(20, 20);
            Offset = new Point(-10,-10);
            _timerDnamic = new System.Timers.Timer();
            _timerDnamic.Interval = 400;
            _timerDnamic.Elapsed += new System.Timers.ElapsedEventHandler(_timerDnamic_Elapsed);
            _timerDnamic.Start();
        }

        bool small = true;
        void _timerDnamic_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (diameterSmall == 6)
            {
                small = true;
            }
            if (diameterSmall == 14)
            {
                small = false;
            }
            if (small)
            {
                diameterSmall += 2;
            }
            else
            {
                diameterSmall -= 2;
            }

            if (diameterBig == 70)
            {
                diameterBig = 15;
            }
            diameterBig += 5;
            Global.control.Invalidate();
        }

        public override void OnRender(Graphics g)
        {
            int x, y;
            x = LocalPosition.X - diameterBig / 2 - Offset.X;
            y = LocalPosition.Y - diameterBig / 2 - Offset.X;

            g.FillEllipse(b, x, y, diameterBig, diameterBig);
            x = LocalPosition.X - Size.Width / 2 - Offset.X;
            y = LocalPosition.Y - Size.Height / 2 - Offset.X;
            g.FillEllipse(Brushes.White, x, y, Size.Width, Size.Height);
            x = LocalPosition.X - diameterSmall / 2 - Offset.X;
            y = LocalPosition.Y - diameterSmall / 2 - Offset.X;
            g.FillEllipse(s, x, y, diameterSmall, diameterSmall);

        }

        public override void Dispose()
        {
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