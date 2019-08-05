using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using GMap.NET;

namespace MapToolkit
{
	/// <summary>
	/// Polyline tool
	/// </summary>
	class ToolDefence : ToolObject
	{
        private int lastX;
        private int lastY;
        private const int minDistance = 15*15;
        private DrawPolyline obj;

        /// <summary>
        /// Left mouse button is pressed
        /// </summary>
        /// <param name="control"></param>
        /// <param name="e"></param>
        public override void OnMouseDown(MapControl control, MouseEventArgs e, int zoom)
        {
            // Create new polygon, add it to the list
            // and keep reference to it
            obj = new DrawPolyline(e.X, e.Y, zoom, TOOL_TYPE.defence);
            AddNewObject(control, obj);
            lastX = e.X;
            lastY = e.Y;
        }

        /// <summary>
        /// Mouse move - resize new polygon
        /// </summary>
        /// <param name="control"></param>
        /// <param name="e"></param>
        public override void OnMouseMove(MapControl control, MouseEventArgs e)
        {
            base.OnMouseMove(control, e);
            if ( e.Button != MouseButtons.Left )
                return;

            if (obj == null)
                return;                 // precaution

            Point point = new Point(e.X, e.Y);
            int distance = (e.X - lastX)*(e.X - lastX) + (e.Y - lastY)*(e.Y - lastY);

            if ( distance > minDistance )
            {
                // Add new point
                obj.AddPoint(point);
                lastX = e.X;
                lastY = e.Y;
            }

            control.Refresh();
        }
	}
}
