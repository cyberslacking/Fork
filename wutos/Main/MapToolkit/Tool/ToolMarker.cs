using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace MapToolkit
{
    /// <summary>
    /// Marker tool
    /// </summary>
    class ToolMarker: ToolObject
    {
        public override void OnMouseDown(MapControl control, MouseEventArgs e, int zoom)
        {
            var obj = new DrawMarker(control.Objects.Property.ID, e.X, e.Y, zoom, TOOL_TYPE.marker, control.MarkerType);
            AddNewObject(control, obj);
        }
    }
}
