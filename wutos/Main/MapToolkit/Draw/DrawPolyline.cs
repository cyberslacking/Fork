using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Runtime.Serialization;

using GMap.NET;
using GMap.NET.WindowsForms;
using System.Linq;

namespace MapToolkit
{

    /// <summary>
    /// Line graphic object
    /// </summary>
    public class DrawPolyline : DrawObject
    {
        private const string entryColor = "Color";
        private const string entryPenWidth = "PenWidth";

        private Dictionary<object, GMapMarker> mapMarker = new Dictionary<object, GMapMarker>();
        private Dictionary<object, GMapRoute> mapRoute = new Dictionary<object, GMapRoute>();
        private List<PointLatLng> listPile = new List<PointLatLng>();

        /// <summary>
        ///  Graphic objects route
        /// </summary>
        public GMapRoute route = null;

        public PropertyLine property = null;

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
                    if (o.Routes.Contains(route))
                    {
                        base.Overlay = index;
                        break;
                    }
                    index++;
                }
                return base.Overlay;
            }
        }

        public Dictionary<object, GMapMarker> MapMarker
        {
            get
            {
                return this.mapMarker;
            }
        }

        public DrawPolyline()
            : this(0, 0, 0, TOOL_TYPE.unknow)
        {

        }

        public DrawPolyline(int x, int y, int zoom, TOOL_TYPE type)
            : base()
        {
            property = new PropertyLine();
            property.LocalPosition = Global.control.FromLocalToLatLng(x, y);
            property.DefaultZoom = zoom;
            property.Type = type;

            Overlay = Global.control.Overlays.Count - 1;

            PointLatLng p;
            p = Global.control.FromLocalToLatLng(x, y);
            PointsArray.Add(p);
            p = Global.control.FromLocalToLatLng(x + 15, y + 15);
            PointsArray.Add(p);
        }

        public DrawPolyline(PointLatLng p1, PointLatLng p2, int zoom, TOOL_TYPE type)
            : base()
        {
            property = new PropertyLine();
            property.LocalPosition = p1;
            property.DefaultZoom = zoom;
            property.Type = type;
            Overlay = Global.control.Overlays.Count - 1;
            PointsArray.Add(p1);
            PointsArray.Add(p2);
        }

        public void SetObjectDisplay(object obj, bool flag)
        {
            if (mapMarker.ContainsKey(obj))
            {
                mapMarker[obj].IsVisible = flag;
            }

            if (mapRoute.ContainsKey(obj))
            {
                mapRoute[obj].IsVisible = flag;
            }
        }

        public void DestroyLines()
        {
            foreach (var node in mapRoute)
            {
                Global.control.Overlays[Overlay].Routes.Remove(node.Value);
            }
            mapRoute.Clear();
        }

        public void DestroyLine(object obj)
        {
            if (mapRoute.ContainsKey(obj))
            {
                Global.control.Overlays[Overlay].Routes.Remove(mapRoute[obj]);
                mapRoute.Remove(obj);
            }
        }

        public void CreateLine(object objBegin, object objEnd, Color color, bool isShowCenter = true)
        {
            DestroyLine(objBegin);

            var listPoint = new List<PointLatLng>();
            GetObjectsRoute(objBegin, objEnd, ref listPoint);
            if (listPoint.Count > 1)
            {
                GMapRoute route = new GMapRoute(listPoint, "LINE");
                route.Stroke = new Pen(color, property.PenWidth);
                Global.control.Overlays[Overlay].Routes.Add(route);
                mapRoute.Add(objBegin, route);

                if (isShowCenter)
                {
                    Global.control.Position = listPoint[0];
                    Global.control.Zoom = property.DefaultZoom;
                }
            }
        }

        public void CreateLine(object obj, Color color, bool isShowCenter = true, bool isShowLine = true)
        {
            DestroyLine(obj);

            var listPoint = new List<PointLatLng>();
            GetPositions(obj, ref listPoint);
            if (listPoint.Count > 1)
            {
                GMapRoute route = new GMapRoute(listPoint, "LINE");
                route.Stroke = new Pen(color, property.PenWidth);

                if (isShowLine)
                {
                    Global.control.Overlays[Overlay].Routes.Add(route);

                    mapRoute.Add(obj, route);

                    if (isShowCenter)
                    {
                        Global.control.Position = listPoint[0];
                        Global.control.Zoom = property.DefaultZoom;
                    }
                }               
            }
        }

        static private string ConvertDigitalToDegrees(double digitalDegree)
        {
            const double num = 60;
            int degree = (int)digitalDegree;
            double tmp = (digitalDegree - degree) * num;
            int minute = (int)tmp;
            double second = (tmp - minute) * num;
            string degrees = "" + degree + "°" + minute + "′" + second.ToString("f2") + "″";
            return degrees;
        }

        public GMapMarker GetMarker(object obj)
        {
            if (mapMarker.ContainsKey(obj))
                return mapMarker[obj];
            return null;
        }

        private delegate void InvokeCallback(object obj);

        public void DestroyMarker(object obj)
        {
            if (Global.control.InvokeRequired)
            {
                InvokeCallback func = new InvokeCallback(DestroyMarker);
                Global.control.Invoke(func, new object[] {obj});
            }
            else
            {
                if (mapMarker.ContainsKey(obj))
                {
                    Global.control.Overlays[Overlay].Markers.Remove(mapMarker[obj]);
                    mapMarker.Remove(obj);
                }
            }
        }

        public void CreateMarker(object obj, Bitmap bmp, string text = "")
        {
            DestroyMarker(obj);

            var listPoint = new List<PointLatLng>();
            GetPositions(obj, ref listPoint);
            if (listPoint.Count > 0)
            {
                GMarkerImage marker = new GMarkerImage(listPoint[0]);
                marker.ToolTipText = String.Format("{0} Position:{1},{2}", text, Math.Round(listPoint[0].Lat, 8), Math.Round(listPoint[0].Lng, 8));
                marker.Bmp = bmp;
                Global.control.Overlays[Overlay].Markers.Add(marker);
                mapMarker.Add(obj, marker);
                Global.control.Position = listPoint[0];
                Global.control.Zoom = property.DefaultZoom;
            }
        }

        public string CreateMarker(object obj, Brush big, Brush small, MarkerTooltipMode mode, ref string remark, ref string reference, bool isShowCenter = true, bool isShowMarker = true)
        {
            string position = string.Empty;
            DestroyMarker(obj);

            List<PointLatLng> listTmp = new List<PointLatLng>();
            var listPoint = new List<PointLatLng>();
            GetPositions(obj, ref listPoint);
            if (listPoint.Count > 0)
            {
                double distance = 0.0;
                int index = -1;

                bool first = true;
                for (int i = 0; i < Global.control.Objects.Count; i++)
                {
                    if (Global.control.Objects[i].GetProperty().Type == TOOL_TYPE.marker)
                    {
                        listTmp.Clear();
                        listTmp.Add(listPoint[0]);
                        listTmp.Add(Global.control.Objects[i].GetProperty().LocalPosition);
                        GMapRoute route = new GMapRoute(listTmp, "TMP");
                        if (first)
                        {
                            distance = route.Distance;
                            index = i;
                            first = false;
                        }
                        else if (route.Distance < distance)
                        {
                            distance = route.Distance;
                            index = i;
                        }
                    }
                }

                string positive = string.Empty;

                #region 长庆版本 参照物正负
                if (index >= 0 && listTmp.Count > 1)
                {
                    PointLatLng p1 = PointsArray[0];
                    PointLatLng p2 = PointsArray[PointsArray.Count - 1];

                    // 西侧为负，东侧为正                  
                    if (p2.Lng > p1.Lng)
                    {
                        positive = "正";
                    }
                    else
                    {
                        positive = "负";
                    }
                }
                /*                if (index >= 0)
                                {
                                    string[] opposite = Global.control.Objects[index].GetProperty().Name.Split('.');
                                    if (opposite.Length > 1 && opposite[opposite.Length - 1] == "1")
                                    {
                                        if (LinkArray.Count / 2 >= LinkArray.IndexOf(obj))
                                        {
                                            positive = "负";
                                        }
                                        else
                                        {
                                            positive = "正";
                                        }
                                    }
                                    else
                                    {
                                        if (LinkArray.Count / 2 < LinkArray.IndexOf(obj))
                                        {
                                            positive = "负";
                                        }
                                        else
                                        {
                                            positive = "正";
                                        }
                                    }
                                }*/
                #endregion

                GMarkerDynamic marker = new GMarkerDynamic(listPoint[0], big, small);
                marker.ToolTipMode = mode;
                position = string.Format("{0},{1}", Math.Round(listPoint[0].Lat, 8), Math.Round(listPoint[0].Lng, 8));
                if (index != -1)
                {
                    remark += String.Format(" 距离:{0} {1} {2}km",
                        Global.control.Objects[index].GetProperty().Name, positive, distance.ToString("f3"));
                    reference = Global.control.Objects[index].GetProperty().Name;
                }
                marker.ToolTipText = remark;

                if (isShowMarker)
                {
                    Global.control.Overlays[Overlay].Markers.Add(marker);

                    mapMarker.Add(obj, marker);

                    if (isShowCenter)
                    {
                        Global.control.Position = listPoint[0];
                        Global.control.Zoom = property.DefaultZoom;
                    }
                }                
            }

            return position;
        }

        public void ShowCurrentMarker(long sensorId)
        {
            foreach (KeyValuePair<object, GMapMarker> kvp in this.mapMarker)
            {
                long id = -1;

                if (long.TryParse(kvp.Key.ToString(), out id))
                {
                    if (id == sensorId)
                    {
                        if (!Global.control.Overlays[Overlay].Markers.Contains(kvp.Value))
                        {
                            Global.control.Overlays[Overlay].Markers.Add(kvp.Value);
                        }
                    }
                }
            }

            foreach (KeyValuePair<object, GMapRoute> kvp in this.mapRoute)
            {
                long id = -1;

                if (long.TryParse(kvp.Key.ToString(), out id))
                {
                    if (id == sensorId)
                    {
                        if (!Global.control.Overlays[Overlay].Routes.Contains(kvp.Value))
                        {
                            Global.control.Overlays[Overlay].Routes.Add(kvp.Value);
                        }
                    }
                }
            }
        }

        public void HideCurrentMarker(long sensorId)
        {
            foreach (KeyValuePair<object, GMapMarker> kvp in this.mapMarker)
            {
                long id = -1;

                if (long.TryParse(kvp.Key.ToString(), out id))
                {
                    if (id == sensorId)
                    {
                        if (Global.control.Overlays[Overlay].Markers.Contains(kvp.Value))
                        {
                            Global.control.Overlays[Overlay].Markers.Remove(kvp.Value);
                        }
                    }
                }
            }

            foreach (KeyValuePair<object, GMapRoute> kvp in this.mapRoute)
            {
                long id = -1;

                if (long.TryParse(kvp.Key.ToString(), out id))
                {
                    if (id == sensorId)
                    {
                        if (Global.control.Overlays[Overlay].Routes.Contains(kvp.Value))
                        {
                            Global.control.Overlays[Overlay].Routes.Remove(kvp.Value);
                        }
                    }
                }
            }

        }

        public override PropertyObject GetProperty()
        {
            return property;
        }

        public override void IsZoomVisible(int zoom)
        {
            if (route != null)
            {
                if (property.MaxZoom >= zoom && property.MinZoom <= zoom)
                {
                    foreach (var obj in mapMarker)
                    {
                        obj.Value.IsVisible = true;
                    }
                    foreach (var obj in mapRoute)
                    {
                        obj.Value.IsVisible = true;
                    }
                    route.IsVisible = true;
                    Visible = true;
                }
                else
                {
                    foreach (var obj in mapMarker)
                    {
                        obj.Value.IsVisible = false;
                    }
                    foreach (var obj in mapRoute)
                    {
                        obj.Value.IsVisible = false;
                    }
                    route.IsVisible = false;
                    Visible = false;
                }
            }
        }

        public override void Show()
        {
            if (route == null)
            {
                base.Show();
                route = new GMapRoute(PointsArray, "Line");
                route.IsHitTestVisible = true;
                route.Stroke = new Pen(property.Color, property.PenWidth);
                Global.control.Overlays[Overlay].Routes.Add(route);

                property.ehLableValueChanged += new EHLableValueChanged(OnLableValueChanged);
                property.IsLoad = true;
                property.Distance = route.Distance;
                Global.control.ObjectSelected(property);
                IsZoomVisible((int)Global.control.Zoom);

            }
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawPolyline drawLine = new DrawPolyline();
            drawLine.property = this.property.Clone();
            drawLine.PointsArray = new List<PointLatLng>(this.PointsArray);

            FillDrawObjectFields(drawLine);
            return drawLine;
        }

        public override void Clear()
        {
            if (route != null)
            {
                foreach (GMapOverlay overlay in Global.control.Overlays)
                {
                    if (overlay.Routes.Contains(route))
                    {
                        overlay.Routes.Remove(route);
                        route = null;
                        break;
                    }
                }
            }
        }

        /*
        public override void ClearLinkSelected()
        {
            List<string> listRemove = new List<string>();
            foreach (KeyValuePair<string, LinkObject> o in mapLink)
            {
                if (o.Value.type == ACTION_TYPE.selected)
                {
                    Global.control.Overlays[Overlay].Routes.Remove(o.Value.route);
                    o.Value.marker.Clear();
                    listRemove.Add(o.Key);
                }
            }
            foreach (string o in listRemove)
            {
                mapLink.Remove(o);
            }
        }
         */

        public override void AddPoint(Point point)
        {
            if (route == null)
                return;
            PointLatLng p = Global.control.FromLocalToLatLng((int)point.X, (int)point.Y);
            PointsArray.Add(p);
            route.Points.Add(p);
            Global.control.UpdateRouteLocalPosition(route);
            property.Distance = route.Distance;
            Global.control.ObjectSelected(property);
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
            route.Points.Insert(index + 1, p);
            Global.control.UpdateRouteLocalPosition(route);
            property.Distance = route.Distance;
            Global.control.ObjectSelected(property);
        }

        public override void DeletePoint(int n)
        {
            if (PointsArray.Count == 2)
                return;
            PointsArray.RemoveAt(n);
            route.Points.RemoveAt(n);
            Global.control.UpdateRouteLocalPosition(route);
            property.Distance = route.Distance;
            Global.control.ObjectSelected(property);
        }

        public override void Draw(Graphics g)
        {
            /*
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Pen pen = new Pen(Color, PenWidth);

            g.DrawLine(pen, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);

            pen.Dispose();
            */
        }

        public override bool Selected
        {
            set
            {
                base.Selected = value;
                if (value)
                {
                    Global.control.ObjectSelected(property);
                }
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
                if (route == null)
                    return 0;
                return route.Points.Count;
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
            if (route == null)
                return;
            if (handleNumber < 1)
                handleNumber = 1;

            if (handleNumber > PointsArray.Count)
                handleNumber = PointsArray.Count;
            PointLatLng p = Global.control.FromLocalToLatLng(point.X, point.Y);
            PointsArray[handleNumber - 1] = p;
            route.Points[handleNumber - 1] = p;
            Global.control.UpdateRouteLocalPosition(route);
            property.Distance = route.Distance;
            Global.control.ObjectSelected(property);
        }

        public override void Move(int deltaX, int deltaY)
        {
            if (route == null)
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
                route.Points[i] = p;
            }
            Global.control.UpdateRouteLocalPosition(route);
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

            Show();
        }

        protected void OnLableValueChanged(string lable, object value)
        {
            switch (lable)
            {
                case "PenWidth":
                    route.Stroke.Width = (int)value;
                    break;
                case "Color":
                    route.Stroke.Color = (Color)value;
                    break;
            }
            Global.control.UpdateRouteLocalPosition(route);
            Global.control.PropertyChanged(property);
        }

        /// <summary>
        /// get ojbects route 
        /// </summary>
        private void GetObjectsRoute(object oBegin, object oEnd, ref List<PointLatLng> listPoint)
        {
            if (LinkArray.Contains(oBegin) && LinkArray.Contains(oEnd) && listPoint != null)
            {
                int begin = LinkArray.IndexOf(oBegin);
                int end = LinkArray.IndexOf(oEnd);
                double interval = route.Distance / LinkArray.Count;
                double distanceB = interval * (begin) - 0.0001;
                double distanceE = interval * (end) - 0.0001;
                List<PointLatLng> listGo = new List<PointLatLng>();
                listGo.Add(PointsArray[0]);
                for (int i = 1; i < PointsArray.Count; i++)
                {
                    listGo.Add(PointsArray[i]);
                    GMapRoute routeGo = new GMapRoute(listGo, "go");
                    if (routeGo.Distance < distanceB)
                    {
                        listGo.RemoveAt(0);
                        distanceB -= routeGo.Distance;
                    }
                    else
                    {
                        double rate = distanceB / routeGo.Distance;
                        double latOffset = (listGo[1].Lat - listGo[0].Lat) * rate;
                        double lngOffset = (listGo[1].Lng - listGo[0].Lng) * rate;
                        PointLatLng from = new PointLatLng();
                        from.Lat = listGo[0].Lat + latOffset;
                        from.Lng = listGo[0].Lng + lngOffset;
                        listPoint.Add(from);
                        List<PointLatLng> listBack = new List<PointLatLng>();
                        listBack.Add(from);
                        int j = i;
                        for (; j < PointsArray.Count; j++)
                        {
                            listBack.Add(PointsArray[j]);
                            GMapRoute routeBack = new GMapRoute(listBack, "back");
                            if (routeBack.Distance < distanceE)
                            {
                                listPoint.Add(PointsArray[j]);
                                if (j == 0)
                                {
                                    break;
                                }
                                listBack.RemoveAt(0);
                                distanceE -= routeBack.Distance;
                            }
                            else
                            {
                                rate = interval / routeBack.Distance;
                                latOffset = (listBack[1].Lat - listBack[0].Lat) * rate;
                                lngOffset = (listBack[1].Lng - listBack[0].Lng) * rate;
                                PointLatLng to = new PointLatLng();
                                to.Lat = listBack[0].Lat + latOffset;
                                to.Lng = listBack[0].Lng + lngOffset;
                                listPoint.Add(to);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        public static double PointToSegDist(double x, double y, double x1, double y1, double x2, double y2)
        {
            double cross = (x2 - x1) * (x - x1) + (y2 - y1) * (y - y1);

            if (cross <= 0)
            {
                return Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1));
            }

            double d2 = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);

            if (cross >= d2)
            {
                return Math.Sqrt((x - x2) * (x - x2) + (y - y2) * (y - y2));
            }

            double r = cross / d2;
            double px = x1 + (x2 - x1) * r;
            double py = y1 + (y2 - y1) * r;
            return Math.Sqrt((x - px) * (x - px) + (py - y) * (py - y));
        }

        public int GetPointIndex(MapControl mapControl, GMapRoute gMapRoute, PointLatLng pointLatLng)
        {
            if (gMapRoute == null)
            {
                return -1;
            }

            if (gMapRoute.Points.Count == 0)
            {
                return 0;
            }

            if (gMapRoute.Points.Count == 1)
            {
                return 1;
            }

            int index = 0;
            double dis = 0;
            GPoint point = mapControl.FromLatLngToLocal(pointLatLng);

            for (int i = 0; i < gMapRoute.Points.Count - 1; i++)
            {
                GPoint p1 = mapControl.FromLatLngToLocal(gMapRoute.Points[i]);
                GPoint p2 = mapControl.FromLatLngToLocal(gMapRoute.Points[i + 1]);
                double distance = PointToSegDist(point.X, point.Y, p1.X, p1.Y, p2.X, p2.Y);

                if (i == 0)
                {
                    dis = distance;
                }
                else
                {
                    if (dis > distance)
                    {
                        dis = distance;
                        index = i;
                    }
                }
            }

            return index + 1;
        }

        public object GetObject(MapControl mapControl, PointLatLng pointLatLng)
        {
            if (this.PointsArray == null || this.PointsArray.Count == 0 || this.LinkArray == null || this.LinkArray.Count == 0)
            {
                return null;
            }

            int index = this.GetPointIndex(mapControl, this.route, pointLatLng);
            List<PointLatLng> routes = new List<PointLatLng>();

            for (int i = 0; i < index; i++)
            {
                routes.Add(this.route.Points[i]);
            }

            routes.Add(pointLatLng);
            GMapRoute routeDis = new GMapRoute(routes, "dis");
            double interval = this.route.Distance / LinkArray.Count;

            if (interval == 0)
            {
                return null;
            }

            int targetIndex = (int)(routeDis.Distance / interval);

            if (targetIndex == -1)
            {
                targetIndex = 0;
            }

            if (targetIndex >= this.LinkArray.Count || targetIndex < 0)
            {
                return null;
            }

            return this.LinkArray[targetIndex];
        }

        public double GetDistance(object obj)
        {
            int targetIndex = this.LinkArray.IndexOf(obj);

            if (targetIndex >= 0)
            {
                return this.route.Distance / LinkArray.Count * targetIndex;
            }

            return 0;
        }

        public double GetDistance(long id)
        {
            int targetIndex = this.LinkArray.IndexOf((object)id);

            if (targetIndex >= 0)
            {
                return this.route.Distance / LinkArray.Count * targetIndex;
            }

            return 0;
        }

        /// <summary>
        /// get obj range points
        /// </summary>
        public void GetPositions(object o, ref List<PointLatLng> listPoint)
        {
            if (LinkArray.Contains(o))
            {
                int index = LinkArray.IndexOf(o);
                double interval = route.Distance / LinkArray.Count;
                double distance = interval * (index) - 0.0001;
                List<PointLatLng> listGo = new List<PointLatLng>();
                listGo.Add(PointsArray[0]);
                for (int i = 1; i < PointsArray.Count; i++)
                {
                    listGo.Add(PointsArray[i]);
                    GMapRoute routeGo = new GMapRoute(listGo, "go");
                    if (routeGo.Distance < distance || routeGo.Distance < 0.000000001)
                    {
                        listGo.RemoveAt(0);
                        distance -= routeGo.Distance;
                    }
                    else
                    {
                        double rate = distance / routeGo.Distance;
                        double latOffset = (listGo[1].Lat - listGo[0].Lat) * rate;
                        double lngOffset = (listGo[1].Lng - listGo[0].Lng) * rate;
                        PointLatLng from = new PointLatLng();
                        from.Lat = listGo[0].Lat + latOffset;
                        from.Lng = listGo[0].Lng + lngOffset;
                        List<PointLatLng> listBack = new List<PointLatLng>();
                        listBack.Add(from);
                        listPoint.Add(from);
                        int j = i - 1;
                        for (; j >= 0; j--)
                        {
                            listBack.Add(PointsArray[j]);
                            GMapRoute routeBack = new GMapRoute(listBack, "back");
                            if (routeBack.Distance < interval)
                            {
                                listPoint.Add(PointsArray[j]);
                                if (j == 0)
                                {
                                    break;
                                }
                                listBack.RemoveAt(0);
                                interval -= routeBack.Distance;
                            }
                            else
                            {
                                rate = interval / routeBack.Distance;
                                latOffset = (listBack[1].Lat - listBack[0].Lat) * rate;
                                lngOffset = (listBack[1].Lng - listBack[0].Lng) * rate;
                                PointLatLng to = new PointLatLng();
                                to.Lat = listBack[0].Lat + latOffset;
                                to.Lng = listBack[0].Lng + lngOffset;
                                listPoint.Add(to);
                                break;
                            }
                        }
                        break;
                        /*
                        List<PointLatLng> listTmp = new List<PointLatLng>(listPoint);
                        while (j >= 0)
                        {
                            listTmp.Add(PointsArray[j]);
                            j--;
                        }
                        GMapRoute routeReal = new GMapRoute(listTmp, "real");
                        return (double.Parse(routeReal.Distance.ToString("0.0000")) * 1000).ToString() + "米";
                        */
                    }
                }
            }
        }


        /// <summary>
        /// get obj range points
        /// </summary>
        public bool GetCenterPosition(object o, out PointLatLng center)
        {
            center = new PointLatLng();

            if (LinkArray.Contains(o))
            {
                int index = LinkArray.IndexOf(o);
                double interval = route.Distance / (LinkArray.Count*2);
                double distance = interval * (2*index + 1) - 0.0001;
                List<PointLatLng> list = new List<PointLatLng>();           

                list.Add(PointsArray[0]);
                int i = 1;
                
                for (; i < PointsArray.Count; i++)
                {
                    list.Add(PointsArray[i]);
                    GMapRoute route2 = new GMapRoute(list, "end");
                    if (route2.Distance < distance)
                    {
                        continue;
                    }

                    double rate;
                    if (i == 1)
                    {
                        rate = distance / route2.Distance;
                    }
                    else
                    {
                        list.RemoveAt(list.Count - 1);
                        GMapRoute route1 = new GMapRoute(list, "start");
                        rate = (distance - route1.Distance) / (route2.Distance - route1.Distance);
                    }

                    center.Lat = PointsArray[i - 1].Lat + (PointsArray[i].Lat - PointsArray[i - 1].Lat) * rate;
                    center.Lng = PointsArray[i - 1].Lng + (PointsArray[i].Lng - PointsArray[i - 1].Lng) * rate;

                    return true;
                }                
            }

            return false;
        }

    }
}
