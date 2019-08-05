using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Drawing.Drawing2D;



using GMap.NET;
using GMap.NET.WindowsForms;

namespace MapToolkit
{
	/// <summary>
	/// Base class for all draw objects
	/// </summary>
	public abstract class DrawObject
	{
        #region Members
        private const string entryID = "ID";
        private const string entryName = "Name";
        private const string entryLengthPoint = "LengthPoint";
        private const string entryPoint = "Point";
        private const string entryOverlay = "Overlay";
        private const string entryDefaultZoom = "DefaultZoom";
        private const string entryMinZoom = "MinZoom";
        private const string entryMaxZoom = "MaxZoom";
        private const string entryLocalPosition = "LocalPosition";
        private const string entryLengthVideo = "LengthVideo";
        private const string entryVideo = "Video"; 
        private const string entryLengthLink = "LengthLink";
        private const string entryLink = "Link";
        private const string entryType = "toolType";


        // Object properties
        private bool copyed;
        private bool selected;
        private bool isHit;
        private int  tracker;
        private int  overlay;
        private bool visible;

        private List<PointLatLng> pointsArray = new List<PointLatLng>();

        private List<Object> linkArray = new List<Object>();

        // Allows to write Undo - Redo functions and don't care about
        // objects order in the list.
        public event EventHandler ImageChanged;

        #endregion

        public DrawObject()
        {
        }

        #region Properties
        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;
            }
        }

        /// <summary>
        /// Hit flag
        /// </summary>
        public bool IsHit
        {
            get
            {
                return isHit;
            }
            set
            {
                isHit = value;
            }
        }

        /// <summary>
        /// Selection flag
        /// </summary>
        public virtual bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
            }
        }

        public virtual int Overlay
        {
            get { return overlay; }
            set
            {
                overlay = value;
            }
        }
        
        /// <summary>
        /// Number of handles
        /// </summary>
        public virtual int HandleCount
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Hover Tracker
        /// </summary>
        public int Tracker
        {
            get { return tracker; }
            set { tracker = value; }
        }

        /// <summary>
        /// List Point
        /// </summary>
        public List<PointLatLng> PointsArray
        {
            get
            {
                return pointsArray;
            }
            set
            {
                pointsArray = value;
            }
        }

        /// <summary>
        /// List Links
        /// </summary>
        public List<Object> LinkArray
        {
            get
            {
                return linkArray;
            }
            set
            {
                linkArray = value;
            }
        }
        #endregion

        #region Virtual Functions

        protected void NotifyImageChanged(object sender, EventArgs e)
        {
            if (this.ImageChanged != null)
            {
                this.ImageChanged(sender, e);
            }
        }

        /// <summary>
        /// get this property.
        /// </summary>
        public abstract PropertyObject GetProperty();

        /// <summary>
        /// this instance is visible in current zoom?
        /// </summary>
        public abstract void IsZoomVisible(int zomm);

        /// <summary>
        /// Clone this instance.
        /// </summary>
        public abstract DrawObject Clone();

        /// <summary>
        /// Clear this instance.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Show this instance.
        /// </summary>
        public virtual void Show()
        {
            int n = overlay - Global.control.Overlays.Count + 1;
            for (int i = 0; i < n; i++)
            {
                GMapOverlay Objects = new GMapOverlay("Objects");
                Global.control.Overlays.Add(Objects);
            }
        }


        /// <summary>
        /// Draw object
        /// </summary>
        /// <param name="g"></param>
        public virtual void Draw(Graphics g)
        {
        }

        /// <summary>
        /// 添加一个控制点到对象
        /// </summary>
        /// <param name="point"></param>
        public virtual void AddPoint(Point point)
        {
        }

        /// <summary>
        /// 添加一个控制点到对象
        /// </summary>
        /// <param name="point"></param>
        public virtual void InsertPoint(Point point)
        {
        }

        /// <summary>
        /// 从对象里删除一个控制点
        /// </summary>
        /// <param name="point"></param>
        public virtual void DeletePoint(int n)
        {

        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public virtual Point GetHandle(int handleNumber)
        {
            return new Point(0, 0);
        }

        /// <summary>
        /// Get handle rectangle by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public virtual Rectangle GetHandleRectangle(int handleNumber)
        {
            Point point = GetHandle(handleNumber);

            return new Rectangle(point.X - 3, point.Y - 3, 7, 7);
        }

        /// <summary>
        /// Draw tracker for selected object
        /// </summary>
        /// <param name="g"></param>
        public virtual void DrawTracker(Graphics g)
        {
            if (!Selected || !visible)
            {
                return;
            }
            Brush brush = Brushes.Black;
            for (int i = 1; i <= HandleCount; i++)
            {
                g.FillRectangle(brush, GetHandleRectangle(i));
            }
        }

        /// <summary>
        /// Hit test.
        /// Return value: -1 - no hit
        ///                0 - hit anywhere
        ///                > 1 - handle number
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual int HitTest(Point point)
        {
            tracker = -1;
            if (Selected)
            {
                for (int i = 1; i <= HandleCount; i++)
                {
                    if (GetHandleRectangle(i).Contains(point))
                    {
                        tracker = i;
                        return i;
                    }
                }
            }

            if (IsHit)
                return 0;

            return -1;
        }

        /// <summary>
        /// Get cursor for the handle
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public virtual Cursor GetHandleCursor(int handleNumber)
        {
            return Cursors.Default;
        }

        /// <summary>
        /// Test whether object intersects with rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public virtual bool IntersectsWith(Rectangle rectangle)
        {
            bool ret = false;
            Region AreaRegion = new Region(rectangle);
            for (int i = 0; i < PointsArray.Count; i++)
            {
                GPoint gp = Global.control.FromLatLngToLocal(PointsArray[i]);
                if (AreaRegion.IsVisible(new PointF(gp.X,gp.Y)))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Move object
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public virtual void Move(int deltaX, int deltaY)
        {
        }

        /// <summary>
        /// Move handle to the point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="handleNumber"></param>
        public virtual void MoveHandleTo(Point point, int handleNumber)
        {
        }

        /// <summary>
        /// Dump (for debugging)
        /// </summary>
        public virtual void Dump()
        {
        }

        /// <summary>
        /// Normalize object.
        /// Call this function in the end of object resizing.
        /// </summary>
        public virtual void Normalize()
        {
        }

        /// <summary>
        /// Save object to serialization stream
        /// </summary>
        /// <param name="info"></param>
        /// <param name="orderNumber"></param>
        public virtual void SaveToStream(SerializationInfo info, int orderNumber)
        {
            info.AddValue(
                 String.Format(CultureInfo.InvariantCulture,
                 "{0}{1}",
                 entryID, orderNumber),
                 GetProperty().ID);
            info.AddValue(
                 String.Format(CultureInfo.InvariantCulture,
                 "{0}{1}",
                 entryName, orderNumber),
                 GetProperty().Name);
            info.AddValue(
                 String.Format(CultureInfo.InvariantCulture,
                 "{0}{1}",
                 entryOverlay, orderNumber),
                 overlay);
            info.AddValue(
                 String.Format(CultureInfo.InvariantCulture,
                 "{0}{1}",
                 entryType, orderNumber),
                 GetProperty().Type);
            info.AddValue(
                 String.Format(CultureInfo.InvariantCulture,
                 "{0}{1}",
                 entryMinZoom, orderNumber),
                 GetProperty().MinZoom);
            info.AddValue(
                 String.Format(CultureInfo.InvariantCulture,
                 "{0}{1}",
                 entryMaxZoom, orderNumber),
                 GetProperty().MaxZoom);
            info.AddValue(
                 String.Format(CultureInfo.InvariantCulture,
                 "{0}{1}",
                 entryDefaultZoom, orderNumber),
                 GetProperty().DefaultZoom);
            info.AddValue(
                 String.Format(CultureInfo.InvariantCulture,
                 "{0}{1}",
                 entryLocalPosition, orderNumber),
                 GetProperty().LocalPosition);

            info.AddValue(
                 String.Format(CultureInfo.InvariantCulture,
                 "{0}{1}",
                 entryLengthPoint, orderNumber),
                 PointsArray.Count);
            int i = 0;
            foreach (PointLatLng p in PointsArray)
            {
                info.AddValue(
                    String.Format(CultureInfo.InvariantCulture,
                    "{0}{1}-{2}",
                    entryPoint, orderNumber, i++),
                    p);
            }
            
            info.AddValue(
                 String.Format(CultureInfo.InvariantCulture,
                 "{0}{1}",
                 entryLengthLink, orderNumber),
                 linkArray.Count);
            
            i = 0;
            foreach (Object link in linkArray)
            {
                info.AddValue(
                    String.Format(CultureInfo.InvariantCulture,
                    "{0}{1}-{2}",
                    entryLink, orderNumber, i++),
                    link);
            }
            
        }

        /// <summary>
        /// Load object from serialization stream
        /// </summary>
        /// <param name="info"></param>
        /// <param name="orderNumber"></param>
        public virtual void LoadFromStream(SerializationInfo info, int orderNumber)
        {
            GetProperty().ID = info.GetInt64(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryID, orderNumber));
            GetProperty().Name = info.GetString(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryName, orderNumber));
            overlay = info.GetInt32(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryOverlay, orderNumber));
            GetProperty().Type = (TOOL_TYPE)info.GetInt32(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryType, orderNumber));
            GetProperty().MaxZoom = info.GetInt32(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryMaxZoom, orderNumber));
            GetProperty().MinZoom = info.GetInt32(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryMinZoom, orderNumber));
            GetProperty().DefaultZoom = info.GetInt32(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryDefaultZoom, orderNumber));

            GetProperty().LocalPosition = (PointLatLng)info.GetValue(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryLocalPosition, orderNumber),
                typeof(PointLatLng));

            PointsArray.Clear();
            PointLatLng point;
            int n = info.GetInt32(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryLengthPoint, orderNumber));

            for (int i = 0; i < n; i++)
            {
                point = (PointLatLng)info.GetValue(
                    String.Format(CultureInfo.InvariantCulture,
                    "{0}{1}-{2}",
                    entryPoint, orderNumber, i),
                    typeof(PointLatLng));

                PointsArray.Add(point);
            }

            n = info.GetInt32(
                String.Format(CultureInfo.InvariantCulture,
                "{0}{1}",
                entryLengthLink, orderNumber));
            linkArray.Clear();
            for (int i = 0; i < n; i++)
            {
                Object link = (Object)info.GetValue(
                    String.Format(CultureInfo.InvariantCulture,
                    "{0}{1}-{2}",
                    entryLink, orderNumber, i),
                    typeof(Object));
                linkArray.Add(link);             
            }
        }

        #endregion

        #region Other functions

        /// <summary>
        /// Copy fields from this instance to cloned instance drawObject.
        /// Called from Clone functions of derived classes.
        /// </summary>
        /// 
        protected void FillDrawObjectFields(DrawObject drawObject)
        {
            drawObject.selected = this.selected;
            drawObject.copyed = this.copyed;
            drawObject.isHit = this.isHit;
            drawObject.tracker = this.tracker;
        }
        #endregion
    }
}
