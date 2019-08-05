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
	/// Image graphic object
	/// </summary>
	class DrawImage : DrawObject
	{
        private const string entryImageFile = "ImageFile";

        public GMarkerImage marker = null;
        public PropertyImage property = null;

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

        public DrawImage()
            : this(0, 0, 0, TOOL_TYPE.unknow)
        {

        }

        public DrawImage(int x, int y, int zoom, TOOL_TYPE type)
            : base()
        {

            property = new PropertyImage();
            property.DefaultZoom = zoom;
            property.Type = type;
            property.LocalPosition = Global.control.FromLocalToLatLng(x, y);

            Overlay = Global.control.Overlays.Count - 1;
  
            Bitmap bmp = new Bitmap(property.ImageFile);
            Rectangle rect = new Rectangle(x, y, (int)bmp.Width * 3 / 4, (int)bmp.Height * 3 / 4);
            PointsArray.Add(Global.control.FromLocalToLatLng(x, y));
            PointsArray.Add(Global.control.FromLocalToLatLng(x + rect.Width, y));
            PointsArray.Add(Global.control.FromLocalToLatLng(x + rect.Width, y + rect.Height));
            PointsArray.Add(Global.control.FromLocalToLatLng(x, y + rect.Height));

            SetRectangle(rect);
        }

        public override PropertyObject GetProperty()
        {
            return property;
        }

        protected void SetRectangle(int x, int y, int width, int height)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
            SetRectangle(rectangle);
        }

        protected void SetRectangle(Rectangle rectangle)
        {
            PointLatLng p;
            p = Global.control.FromLocalToLatLng(rectangle.Left, rectangle.Top);
            PointsArray[0] = p;
            p = Global.control.FromLocalToLatLng(rectangle.Left, rectangle.Bottom);
            PointsArray[1] = p;
            p = Global.control.FromLocalToLatLng(rectangle.Right, rectangle.Bottom);
            PointsArray[2] = p;
            p = Global.control.FromLocalToLatLng(rectangle.Right, rectangle.Top);
            PointsArray[3] = p;

            if (marker != null)
            {
                marker.Adapter(PointsArray[0], PointsArray[2]);
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
                Rectangle rectangle = Rectangle.Round(GetRectangle());
                SetRectangle(rectangle);
                int xCenter = rectangle.X + rectangle.Width / 2;
                int yCenter = rectangle.Y + rectangle.Height / 2;
                PointLatLng postion = Global.control.FromLocalToLatLng(xCenter,yCenter);
                marker = new GMarkerImage(postion);
                marker.Adapter(PointsArray[0], PointsArray[2]);
                marker.Bmp = new Bitmap(property.ImageFile);
                marker.IsHitTestVisible = true;
                Global.control.Overlays[Overlay].Markers.Add(marker);
                property.ehLableValueChanged += new EHLableValueChanged(OnLableValueChanged);
                property.IsLoad = true;
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
                marker = null;
            }
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawImage drawImage = new DrawImage();
            drawImage.property = this.property.Clone();
            drawImage.PointsArray = new List<PointLatLng>(PointsArray);
            FillDrawObjectFields(drawImage);
            return drawImage;
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
            Rectangle rectangle = Rectangle.Round(GetRectangle()); 

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
            Rectangle rectangle = Rectangle.Round(GetRectangle()); 

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
            Rectangle rectangle = Rectangle.Round(GetRectangle()); 
            rectangle.X += deltaX;
            rectangle.Y += deltaY;
            SetRectangle(rectangle);
        }

        public override void Normalize()
        {
            if (marker != null)
            {
                marker.Adapter(PointsArray[0], PointsArray[2]);
            }
        }

        private void OnLableValueChanged(string lable, object value)
        {
            switch (lable)
            {
                case "ImageFile":
                    marker.Bmp = new Bitmap((string)value);
                    GPoint gp = Global.control.FromLatLngToLocal(property.LocalPosition);
                    Rectangle rect = new Rectangle((int)gp.X, (int)gp.Y, (int)marker.Bmp.Width, (int)marker.Bmp.Height * 3 / 4);
                    SetRectangle(rect);
                    break;
            }
            Global.control.UpdateMarkerLocalPosition(marker);
            Global.control.PropertyChanged(property); 
            Global.control.Refresh();
        }

        private Rectangle GetRectangle()
        {
            Rectangle rectangle = new Rectangle();
            GPoint gpLT = Global.control.FromLatLngToLocal(PointsArray[0]);
            rectangle.X = (int)gpLT.X;
            rectangle.Y = (int)gpLT.Y;
            GPoint gpRB = Global.control.FromLatLngToLocal(PointsArray[2]);
            rectangle.Width = (int)(gpRB.X - gpLT.X);
            rectangle.Height = (int)(gpRB.Y - gpLT.Y);
            return rectangle;
        }

        public override void SaveToStream(System.Runtime.Serialization.SerializationInfo info, int orderNumber)
        {
            base.SaveToStream(info, orderNumber);

            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryImageFile, orderNumber),
                property.ImageFile);
        }

        public override void LoadFromStream(SerializationInfo info, int orderNumber)
        {
            base.LoadFromStream(info, orderNumber);

            property.ImageFile = (string)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryImageFile, orderNumber),
                typeof(string));

            Show();
        }
	}
}
