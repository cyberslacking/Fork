#region Using directives

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Runtime.Serialization;
#endregion


using GMap.NET;
using GMap.NET.WindowsForms;



namespace MapToolkit
{
    /// <summary>
    /// Polygon graphic object
    /// </summary>
    class DrawPolygon : DrawObject
    {

        private const string entryPenWidth = "PenWidth";
        private const string entryColor = "Color";
        private const string entryFill = "Fill";
        public GMapPolygon   polygon = null;
        public PropertyPolygon property = null;

        public override int Overlay
        {
            set
            {
                base.Overlay = value;
            }
            get
            {
                int index = 0;
                foreach (GMapOverlay o in Global.control.Overlays)
                {
                    if (o.Polygons.Contains(polygon))
                    {
                        base.Overlay = index;
                        break;
                    }
                    index++;
                }
                return base.Overlay;
            }
        }

        public DrawPolygon()
            : this(0, 0, 0,TOOL_TYPE.unknow)
        {

        }

        public DrawPolygon(int x, int y, int zoom, TOOL_TYPE type)
            : base()
        {
            property = new PropertyPolygon();
            property.DefaultZoom = zoom;
            property.LocalPosition = Global.control.FromLocalToLatLng(x, y);
            property.Type = type;
            Overlay = Global.control.Overlays.Count - 1;

            PointsArray.Add(Global.control.FromLocalToLatLng(x, y));
            PointsArray.Add(Global.control.FromLocalToLatLng(x + 30, y));  
            PointsArray.Add(Global.control.FromLocalToLatLng(x + 30, y + 30));
            PointsArray.Add(Global.control.FromLocalToLatLng(x , y + 30));
        }

        public override PropertyObject GetProperty()
        {
            return property;
        }

        public override void IsZoomVisible(int zoom)
        {
            if (polygon != null)
            {
                if (property.MaxZoom >= zoom && property.MinZoom <= zoom)
                {
                    polygon.IsVisible = true;
                    Visible = true;
                }
                else
                {
                    polygon.IsVisible = false;
                    Visible = false;

                }
            }
        }

        public override void Show()
        {
            if (polygon == null)
            {
                base.Show();
                polygon = new GMapPolygon(PointsArray, "Polygon");
                polygon.IsHitTestVisible = true;
                Global.control.Overlays[Overlay].Polygons.Add(polygon);
                property.ehLableValueChanged += new EHLableValueChanged(OnLableValueChanged);
                property.IsLoad = true;
                polygon.Stroke = new Pen(property.Color, property.PenWidth);
                polygon.Fill = new System.Drawing.SolidBrush(property.Fill);
                property.Distance = polygon.Distance;
                Global.control.ObjectSelected(property);
                IsZoomVisible((int)Global.control.Zoom);
            }
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawPolygon drawPolygon = new DrawPolygon();
            drawPolygon.property = this.property.Clone();
            drawPolygon.PointsArray = new List<PointLatLng>(PointsArray);
            FillDrawObjectFields(drawPolygon);
            return drawPolygon;
        }

        public override void Clear()
        {
            if (polygon != null)
            {
                foreach (GMapOverlay overlay in Global.control.Overlays)
                {
                    if (overlay.Polygons.Contains(polygon))
                    {
                        overlay.Polygons.Remove(polygon);
                        polygon = null;
                        break;
                    }
                }
            }
        }

        public override bool Selected
        {
            set
            {
                if (value)
                {
                    Global.control.ObjectSelected(property);
                }
                base.Selected = value;
            }
            get
            {
                return base.Selected;
            }
        }

        public override int HandleCount
        {
            get
            {
                return polygon.Points.Count;
            }
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override Point GetHandle(int handleNumber)
        {
            Point point = new Point();
            if (handleNumber < 1)
                handleNumber = 1;

            if (handleNumber > PointsArray.Count)
                handleNumber = PointsArray.Count;
            GPoint gp = Global.control.FromLatLngToLocal(PointsArray[handleNumber - 1]);
            point.X = (int)gp.X;
            point.Y = (int)gp.Y;
            return point;
        }

        public override Cursor GetHandleCursor(int handleNumber)
        {
            return Cursors.Cross;
        }

        public override void MoveHandleTo(Point point, int handleNumber)
        {
            if (polygon == null)
                return;
            if (handleNumber < 1)
                handleNumber = 1;

            if (handleNumber > PointsArray.Count)
                handleNumber = PointsArray.Count;
            PointLatLng p = Global.control.FromLocalToLatLng(point.X, point.Y);
            PointsArray[handleNumber - 1] = p;
            polygon.Points[handleNumber - 1] = p;
            Global.control.UpdatePolygonLocalPosition(polygon);
        }

        public override void Move(int deltaX, int deltaY)
        {
            if (polygon == null)
                return;
            int n = PointsArray.Count;
            GPoint point;
            PointLatLng p;
            for (int i = 0; i < n; i++)
            {
                point = Global.control.FromLatLngToLocal(PointsArray[i]);
                point.X += deltaX;
                point.Y += deltaY;
                p = Global.control.FromLocalToLatLng((int)point.X, (int)point.Y);

                PointsArray[i] = p;
                polygon.Points[i] = p;
            }
            Global.control.UpdatePolygonLocalPosition(polygon);
        }

        public override void InsertPoint(Point point)
        {
            //查找最近的两点距离
            int n = PointsArray.Count;
            int index = 0;
            PointLatLng p = Global.control.FromLocalToLatLng((int)point.X, (int)point.Y);
            List<PointLatLng> routePoints = new List<PointLatLng>();
            routePoints.Add(PointsArray[0]);
            routePoints.Add(p);
            GMapRoute routeA = new GMapRoute(routePoints, "A");
            routePoints.Clear();
            routePoints.Add(PointsArray[1]);
            routePoints.Add(p);
            GMapRoute routeB = new GMapRoute(routePoints, "B");
            double MinDistance = routeA.Distance + routeB.Distance;
            routeA.Dispose();
            routeB.Dispose();
            for (int i = 1; i < n - 1; i++)
            {
                routePoints.Clear();
                routePoints.Add(PointsArray[i]);
                routePoints.Add(p);
                routeA = new GMapRoute(routePoints, "A");
                routePoints.Clear();
                routePoints.Add(PointsArray[i + 1]);
                routePoints.Add(p);
                routeB = new GMapRoute(routePoints, "B");
                double distance = routeA.Distance + routeB.Distance;
                routeA.Dispose();
                routeB.Dispose();
                if (distance < MinDistance)
                {
                    MinDistance = distance;
                    index = i;
                }
            }

            PointsArray.Insert(index + 1, p);
            polygon.Points.Insert(index + 1, p);
            Global.control.UpdatePolygonLocalPosition(polygon);
        }

        public override void DeletePoint(int n)
        {
            if (PointsArray.Count == 3)
                return;
            PointsArray.RemoveAt(n);
            polygon.Points.RemoveAt(n);
            Global.control.UpdatePolygonLocalPosition(polygon);
            Global.control.Refresh();
        }

        public override void SaveToStream(System.Runtime.Serialization.SerializationInfo info, int orderNumber)
        {
            base.SaveToStream(info, orderNumber);

            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryPenWidth, orderNumber),
                property.PenWidth);

            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryColor, orderNumber),
                property.Color);

            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryFill, orderNumber),
                property.Fill);
        }

        public override void LoadFromStream(SerializationInfo info, int orderNumber)
        {

            base.LoadFromStream(info, orderNumber);

            property.PenWidth = (int)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryPenWidth, orderNumber),
                typeof(int));

            property.Color = (Color)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryColor, orderNumber),
                typeof(Color));

            property.Fill = (Color)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryFill, orderNumber),
                typeof(Color));

            Show();
        }

        private void OnLableValueChanged(string lable, object value)
        {
            switch (lable)
            {
                case "PenWidth":
                    polygon.Stroke.Width = (int)value;
                    break;
                case "Color": 
                    polygon.Stroke.Color = (Color)value;
                    break;
                case "Fill":
                    polygon.Fill = new System.Drawing.SolidBrush((Color)value);
                    break;
            }
            Global.control.UpdatePolygonLocalPosition(polygon);
            Global.control.PropertyChanged(property);
            Global.control.Refresh();
        }
    }
}

