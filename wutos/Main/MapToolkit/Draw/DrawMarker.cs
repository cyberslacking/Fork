using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Collections.Generic;

using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;



namespace MapToolkit
{
	/// <summary>
	/// Line graphic object
	/// </summary>
	public class DrawMarker : DrawObject
	{
        private const string entryImageFile = "ImageFile";
        private const string entryToolTipMode = "ToolTipMode";
        private const string MarkerType = "MarkerType";

        /// <summary>
        ///  Graphic objects marker
        /// </summary>
        public GMarkerEx marker = null;
        public PropertyMarker property = null;    //标签元素属性

        public DrawMarker()
            : this(0, 0, 0, 0, TOOL_TYPE.unknow)
        {

        }

        public DrawMarker(long pid, int x, int y, int zoom, TOOL_TYPE type)
            : base()
        {
            PointLatLng latlng = Global.control.FromLocalToLatLng(x, y);
            PointsArray.Add(latlng);

            property = new PropertyMarker();
            property.DefaultZoom = zoom;
            property.LocalPosition = latlng;
            property.Type = type;
            if (type == TOOL_TYPE.camera)
            {
                property.ImageFile = @"image\camera_0.ico";
            }
            else if (type == TOOL_TYPE.marker)
            {
                property.ImageFile = @"image\marker_0.ico";
            }
            else if (type == TOOL_TYPE.model)
            {
                property.ImageFile = @"image\model.ico";
            }
        }

        public DrawMarker(long pid, int x, int y, int zoom, TOOL_TYPE type, MARKER_TYPE markerType)
            : base()
        {
            PointLatLng latlng = Global.control.FromLocalToLatLng(x, y);
            PointsArray.Add(latlng);

            property = new PropertyMarker();
            property.DefaultZoom = zoom;
            property.LocalPosition = latlng;
            property.Type = type;
            property.MarkerType = markerType;
            if (type == TOOL_TYPE.camera)
            {
                property.ImageFile = @"image\camera_0.ico";
            }
            else if (type == TOOL_TYPE.model)
            {
                property.ImageFile = @"image\model.ico";
            }
            else if (type == TOOL_TYPE.marker)
            {
                if (markerType == MARKER_TYPE.Green)
                {
                    property.ImageFile = @"image\marker_0.ico";
                }
                else if (markerType == MARKER_TYPE.Yellow)
                {
                    property.ImageFile = @"image\marker_2.ico";
                }
                else if (markerType == MARKER_TYPE.Red)
                {
                    property.ImageFile = @"image\marker_1.ico";
                }
                else if (markerType == MARKER_TYPE.Gray)
                {
                    property.ImageFile = @"image\marker_3.ico";
                }
            }
        }

	    public DrawMarker(long pid, PointLatLng latlng, int zoom, TOOL_TYPE type, MARKER_TYPE markerType)
	        : base()
	    {	  
	        PointsArray.Add(latlng);

	        property = new PropertyMarker();
	        property.DefaultZoom = zoom;
	        property.LocalPosition = latlng;
	        property.Type = type;
	        property.MarkerType = markerType;
	        if (type == TOOL_TYPE.camera)
	        {
	            property.ImageFile = @"image\camera_0.ico";
	        }
            else if (type == TOOL_TYPE.model)
            {
                property.ImageFile = @"image\model.ico";
            }
            else if (type == TOOL_TYPE.marker)
	        {
	            if (markerType == MARKER_TYPE.Green)
	            {
	                property.ImageFile = @"image\marker_0.ico";
	            }
	            else if (markerType == MARKER_TYPE.Yellow)
	            {
	                property.ImageFile = @"image\marker_2.ico";
	            }
	            else if (markerType == MARKER_TYPE.Red)
	            {
	                property.ImageFile = @"image\marker_1.ico";
	            }
	            else if (markerType == MARKER_TYPE.Gray)
	            {
	                property.ImageFile = @"image\marker_3.ico";
	            }
	        }
	    }


        public override PropertyObject GetProperty()
        {
            return property;
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

                marker = new GMarkerEx(PointsArray[0], new Bitmap(AppDomain.CurrentDomain.BaseDirectory + property.ImageFile));
                marker.IsHitTestVisible = true;
                marker.ToolTipMode = property.ToolTipMode;
                marker.ToolTipText = property.Name;
                property.ehLableValueChanged += new EHLableValueChanged(OnLableValueChanged);
                property.LocalPosition = PointsArray[0];
                property.IsLoad = true;
                Global.control.Overlays[Overlay].Markers.Add(marker);
                IsZoomVisible((int)Global.control.Zoom);
            }
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawMarker drawMarker = new DrawMarker();
            drawMarker.property =  this.property.Clone();
            drawMarker.PointsArray = new List<PointLatLng>(this.PointsArray);
            FillDrawObjectFields(drawMarker);
            return drawMarker;
        }

        /// <summary>
        /// Clear this instance
        /// </summary>
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

        public override void Draw(Graphics g)
        {

        }

        public override bool Selected
        {
            set
            {
                if (marker != null)
                {
                    if (value)
                    {
                        Global.control.ObjectSelected(property);
                    }
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
                return 1;
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

        public void Move(PointLatLng p)
        {
            if (marker == null)
                return;
            property.LocalPosition = p;
            marker.Position = property.LocalPosition;
        }

        public override void Move(int deltaX, int deltaY)
        {
            if (marker == null)
                return;
            GPoint point = Global.control.FromLatLngToLocal(property.LocalPosition);
            point.X += deltaX;
            point.Y += deltaY;
            property.LocalPosition = Global.control.FromLocalToLatLng((int)point.X, (int)point.Y);
            marker.Position = property.LocalPosition;
            PointsArray[0] = property.LocalPosition;
        }

        public override void SaveToStream(System.Runtime.Serialization.SerializationInfo info, int orderNumber)
        {
            base.SaveToStream(info, orderNumber);

            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryToolTipMode, orderNumber),
                property.ToolTipMode);
            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryImageFile, orderNumber),
                property.ImageFile);

            try
            {
                info.AddValue(
                    String.Format(CultureInfo.InvariantCulture,
                    "{0}{1}",
                    MarkerType, orderNumber),
                    property.MarkerType);
            }
            catch (Exception ex)
            { }
        }

        public override void LoadFromStream(SerializationInfo info, int orderNumber)
        {
            base.LoadFromStream(info, orderNumber);
            property.ToolTipMode = (MarkerTooltipMode)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryToolTipMode, orderNumber),
                typeof(MarkerTooltipMode));

            // test 暂时处理为直接赋值
            property.ToolTipMode = MarkerTooltipMode.OnMouseOver;

            property.ImageFile = (string)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryImageFile, orderNumber),
                typeof(string));

            try
            {
                property.MarkerType = (MARKER_TYPE)info.GetValue(
                    String.Format(CultureInfo.InvariantCulture,
                    "{0}{1}",
                    MarkerType, orderNumber),
                    typeof(MARKER_TYPE));
            }
            catch (Exception ex)
            { }

            Show(); 
        }

        private void OnLableValueChanged(string lable, object value)
        {
            CommandChangeState c = new CommandChangeState(Global.control.Objects);
            switch (lable)
            {
                case "Name":
                    marker.ToolTipText = (string)value;
                    break;
                case "ToolTipMode":
                    marker.ToolTipMode = (MarkerTooltipMode)value;
                    break;
                case "Lat":
                case "Lng":
                    marker.Position = property.LocalPosition;
                    PointsArray[0] = property.LocalPosition;
                    break;
                case "MarkerType":
                    MARKER_TYPE markerType = (MARKER_TYPE)value;
                    if (markerType == MARKER_TYPE.Green)
                    {
                        property.ImageFile = @"image\marker_0.ico";
                    }
                    else if (markerType == MARKER_TYPE.Yellow)
                    {
                        property.ImageFile = @"image\marker_2.ico";
                    }
                    else if (markerType == MARKER_TYPE.Red)
                    {
                        property.ImageFile = @"image\marker_1.ico";
                    }
                    else if (markerType == MARKER_TYPE.Gray)
                    {
                        property.ImageFile = @"image\marker_3.ico";
                    }
                    marker.SetBitmap(new Bitmap(AppDomain.CurrentDomain.BaseDirectory + property.ImageFile));
                    this.NotifyImageChanged(this, new EventArgs());
                    break;
                    
            }
            Global.control.UpdateMarkerLocalPosition(marker);
            Global.control.PropertyChanged(property);
            Global.control.Refresh();
        }
	}
}
