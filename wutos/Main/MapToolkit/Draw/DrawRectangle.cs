using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Collections.Generic;

using GMap.NET;
using GMap.NET.WindowsForms;

namespace MapToolkit
{
	/// <summary>
	/// Rectangle graphic object
	/// </summary>
	class DrawRectangle : DrawPolygon
	{
        public DrawRectangle()
            : this(0, 0, 0, TOOL_TYPE.unknow)
        {

        }

        public DrawRectangle(int x, int y, int zoom, TOOL_TYPE type)
            : base(x, y, zoom, type)
        {
            SetRectangle(x,y,40,40);
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawRectangle drawRectangle = new DrawRectangle();
            drawRectangle.property = this.property.Clone();
            drawRectangle.PointsArray = new List<PointLatLng>(PointsArray);
            FillDrawObjectFields(drawRectangle);
            return drawRectangle;
        }

        private void SetRectangle(int x, int y, int width, int height)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;

            SetRectangle(rectangle);

        }

        private void SetRectangle(Rectangle rectangle)
        {
            PointLatLng p;
            p = Global.control.FromLocalToLatLng((int)rectangle.Left, (int)rectangle.Top);
            PointsArray[0] = p;
            p = Global.control.FromLocalToLatLng((int)rectangle.Left, (int)rectangle.Bottom);
            PointsArray[1] = p;
            p = Global.control.FromLocalToLatLng((int)rectangle.Right, (int)rectangle.Bottom);
            PointsArray[2] = p;
            p = Global.control.FromLocalToLatLng((int)rectangle.Right, (int)rectangle.Top);
            PointsArray[3] = p;
            if (polygon != null)
            {
                polygon.Points[0] = PointsArray[0];
                polygon.Points[1] = PointsArray[1];
                polygon.Points[2] = PointsArray[2];
                polygon.Points[3] = PointsArray[3];
                Global.control.UpdatePolygonLocalPosition(polygon);
            }
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
            Rectangle rectangle = GetRectangle();

            int x, y, xCenter, yCenter;
            xCenter = rectangle.X + rectangle.Width/2;
            yCenter = rectangle.Y + rectangle.Height/2;
            x = rectangle.X;
            y = rectangle.Y;

            switch ( handleNumber )
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
            switch ( handleNumber )
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
            Rectangle rectangle = GetRectangle();

            int left = rectangle.Left;
            int top = rectangle.Top;
            int right = rectangle.Right;
            int bottom = rectangle.Bottom;

            switch ( handleNumber )
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
            if (polygon == null)
                return;
            Rectangle rectangle = GetRectangle();
            rectangle.X += deltaX;
            rectangle.Y += deltaY;
            SetRectangle(rectangle);
        }

        /// <summary>
        /// Normalize rectangle
        /// </summary>
        public override void Normalize()
        {

        }

        #region Helper Functions

        public static Rectangle GetNormalizedRectangle(int x1, int y1, int x2, int y2)
        {
            if ( x2 < x1 )
            {
                int tmp = x2;
                x2 = x1;
                x1 = tmp;
            }

            if ( y2 < y1 )
            {
                int tmp = y2;
                y2 = y1;
                y1 = tmp;
            }

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public static Rectangle GetNormalizedRectangle(Point p1, Point p2)
        {
            return GetNormalizedRectangle(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static Rectangle GetNormalizedRectangle(Rectangle r)
        {
            return GetNormalizedRectangle(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
        }

        #endregion

    }
}
