using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Globalization;

using GMap.NET;
using GMap.NET.WindowsForms;

namespace MapToolkit
{
	/// <summary>
	/// graphic object
	/// </summary>
	class DrawText : DrawObject
	{
        private const string entryColor = "Color";
        private const string entryFont = "Font";

        private Rectangle rectangle;

        public GMarkerText marker = null;
        public PropertyText property = null;

        public DrawText()
            : this(0, 0, 0, TOOL_TYPE.text)
        {

        }

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
                    if (o.Markers.Contains(marker))
                    {
                        base.Overlay = index;
                        break;
                    }
                    index++;
                }
                return base.Overlay;
            }
        }

        public DrawText(int x, int y, int zoom, TOOL_TYPE type)
            : base()
        {
            property = new PropertyText();
            property.DefaultZoom = zoom;
            property.LocalPosition = Global.control.FromLocalToLatLng(x, y);
            property.Type = type;

            Overlay = Global.control.Overlays.Count - 1;

            Graphics g = Graphics.FromImage(new Bitmap(1, 1));
            SizeF size = g.MeasureString(property.Name, property.Font);

            rectangle = new Rectangle(x, y, (int)(size.Width*1.5), (int)size.Height);

            PointsArray.Add(Global.control.FromLocalToLatLng(x, y));
            PointsArray.Add(Global.control.FromLocalToLatLng(x + rectangle.Width, y));
            PointsArray.Add(Global.control.FromLocalToLatLng(x + rectangle.Width, y + rectangle.Height));
            PointsArray.Add(Global.control.FromLocalToLatLng(x, y + rectangle.Height));

            //SetRectangle(rectangle);
        }

        public override PropertyObject GetProperty()
        {
            return property;
        }

        protected void SetRectangle(int x, int y, int width, int height)
        {
            Rectangle rect = new Rectangle();
            rect.X = x;
            rect.Y = y;
            rect.Width = width;
            rect.Height = height;
            SetRectangle(rect);
        }

        protected void SetRectangle(Rectangle rect)
        {
            if (marker != null)
            {
                PointLatLng p;
                p = Global.control.FromLocalToLatLng(rect.Left, rect.Top);
                PointsArray[0] = p;

                if (property.AutoSize)
                {
                    p = Global.control.FromLocalToLatLng(rect.Left, rect.Bottom);
                    PointsArray[1] = p;                    
                    p = Global.control.FromLocalToLatLng(rect.Right, rect.Bottom);
                    PointsArray[2] = p;
                    p = Global.control.FromLocalToLatLng(rect.Right, rect.Top);
                    PointsArray[3] = p;
                    marker.Adapter(Global.control.FromLatLngToLocal(PointsArray[0]),
                        Global.control.FromLatLngToLocal(PointsArray[2]));

                }
                else
                {
                    marker.Adapter(Global.control.FromLatLngToLocal(PointsArray[0]),
                    rectangle.Width, rectangle.Height);
                }
            }
        }

        public override void IsZoomVisible(int zoom)
        {
            if (marker != null)
            {
                if (property.MaxZoom >= zoom && property.MinZoom <= zoom)
                {
                    marker.IsVisible = true;
                    Visible = true;
                }
                else
                {
                    marker.IsVisible = false;
                    Visible = false;
                }
            }
        }

        public override void Show()
        {
            if (marker == null)
            {
                base.Show();

                UpdateRectangle();

                int xCenter = rectangle.X + rectangle.Width / 2;
                int yCenter = rectangle.Y + rectangle.Height / 2;
                PointLatLng postion = Global.control.FromLocalToLatLng(xCenter,yCenter);
                marker = new GMarkerText(postion);
                marker.Adapter(Global.control.FromLatLngToLocal(PointsArray[0]),
                    Global.control.FromLatLngToLocal(PointsArray[2]));
                marker.IsHitTestVisible = true;
                Global.control.Overlays[Overlay].Markers.Add(marker);
                property.ehLableValueChanged += new EHLableValueChanged(OnLableValueChanged);
                property.IsLoad = true;
                marker.Text = property.Name;
                marker.Font = property.Font;
                marker.Fill = new System.Drawing.SolidBrush(property.Color);
                IsZoomVisible((int)Global.control.Zoom);
            }
        }

        public override void Clear()
        {
            if (marker != null)
            {
                foreach (GMapOverlay overlay in Global.control.Overlays)
                {
                    if (overlay.Markers.Contains(marker))
                    {
                        overlay.Markers.Remove(marker);
                        marker = null;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawText drawText = new DrawText();
            drawText.property = this.property.Clone();
            drawText.PointsArray = new List<PointLatLng>(PointsArray);
            FillDrawObjectFields(drawText);
            return drawText;
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

        /// <summary>
        /// Get number of handles
        /// </summary>
        public override int HandleCount
        {
            get
            {
                return 8;
            }
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override Point GetHandle(int handleNumber)
        {
            UpdateRectangle();

            int x, y, xCenter, yCenter;
            xCenter = rectangle.X + rectangle.Width / 2;
            yCenter = rectangle.Y + rectangle.Height / 2;
            x = rectangle.X;
            y = rectangle.Y;

            switch (handleNumber)
            {
                case 1:
                    x = rectangle.X;
                    y = rectangle.Y;
                    break;
                case 2:
                    x = xCenter;
                    y = rectangle.Y;
                    break;
                case 3:
                    x = rectangle.Right;
                    y = rectangle.Y;
                    break;
                case 4:
                    x = rectangle.Right;
                    y = yCenter;
                    break;
                case 5:
                    x = rectangle.Right;
                    y = rectangle.Bottom;
                    break;
                case 6:
                    x = xCenter;
                    y = rectangle.Bottom;
                    break;
                case 7:
                    x = rectangle.X;
                    y = rectangle.Bottom;
                    break;
                case 8:
                    x = rectangle.X;
                    y = yCenter;
                    break;
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Get cursor for the handle
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override Cursor GetHandleCursor(int handleNumber)
        {
            switch (handleNumber)
            {
                case 1:
                    return Cursors.SizeNWSE;
                case 2:
                    return Cursors.SizeNS;
                case 3:
                    return Cursors.SizeNESW;
                case 4:
                    return Cursors.SizeWE;
                case 5:
                    return Cursors.SizeNWSE;
                case 6:
                    return Cursors.SizeNS;
                case 7:
                    return Cursors.SizeNESW;
                case 8:
                    return Cursors.SizeWE;
                default:
                    return Cursors.Default;
            }
        }

        /// <summary>
        /// Move handle to new point (resizing)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="handleNumber"></param>
        public override void MoveHandleTo(Point point, int handleNumber)
        {

            UpdateRectangle();

            int left = rectangle.Left;
            int top = rectangle.Top;
            int right = rectangle.Right;
            int bottom = rectangle.Bottom;

            switch (handleNumber)
            {
                case 1:
                    left = point.X;
                    top = point.Y;
                    break;
                case 2:
                    top = point.Y;
                    break;
                case 3:
                    right = point.X;
                    top = point.Y;
                    break;
                case 4:
                    right = point.X;
                    break;
                case 5:
                    right = point.X;
                    bottom = point.Y;
                    break;
                case 6:
                    bottom = point.Y;
                    break;
                case 7:
                    left = point.X;
                    bottom = point.Y;
                    break;
                case 8:
                    left = point.X;
                    break;
            }

            SetRectangle(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Move object
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public override void Move(int deltaX, int deltaY)
        {
            if (marker == null)
                return;

            UpdateRectangle();
            rectangle.X += deltaX;
            rectangle.Y += deltaY;
            SetRectangle(rectangle);
        }

        public override void Normalize()
        {
            if (marker != null)
            {
                if (property.AutoSize)
                {
                    marker.Adapter(Global.control.FromLatLngToLocal(PointsArray[0]),
                    Global.control.FromLatLngToLocal(PointsArray[2]));
                }
                else
                {
                    marker.Adapter(Global.control.FromLatLngToLocal(PointsArray[0]),
                    rectangle.Width,rectangle.Height);
                }
            }
        }

        private void OnLableValueChanged(string lable, object value)
        {
            switch (lable)
            {
                case "Name":
                    marker.Text = (string)value;
                    break;
                case "Font":
                    marker.Font = (Font)value;
                    break;
                case "Fill":
                    marker.Fill = new System.Drawing.SolidBrush((Color)value);
                    break;
            }
            Global.control.UpdateMarkerLocalPosition(marker);
            Global.control.PropertyChanged(property); 
            Global.control.Refresh();
        }

        private void UpdateRectangle()
        {
            GPoint gpLT = Global.control.FromLatLngToLocal(PointsArray[0]);
            rectangle.X = (int)gpLT.X;
            rectangle.Y = (int)gpLT.Y;
            if (property.AutoSize)
            {
                GPoint gpRB = Global.control.FromLatLngToLocal(PointsArray[2]);
                rectangle.Width = (int)(gpRB.X - gpLT.X);
                rectangle.Height = (int)(gpRB.Y - gpLT.Y);
            }
        }

        public override void SaveToStream(System.Runtime.Serialization.SerializationInfo info, int orderNumber)
        {
            base.SaveToStream(info, orderNumber);

            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryFont, orderNumber),
                property.Font);

            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryColor, orderNumber),
                property.Color);
        }

        public override void LoadFromStream(SerializationInfo info, int orderNumber)
        {
            base.LoadFromStream(info, orderNumber);

            property.Color = (Color)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryColor, orderNumber),
                typeof(Color));

            property.Font = (Font)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryFont, orderNumber),
                typeof(Font));

            Show();
        }

	}
}
