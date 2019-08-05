using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace MapToolkit
{
    /// <summary>
    /// Model tool
    /// </summary>
    class ToolModel: ToolObject
    {
        public override void OnMouseDown(MapControl control, MouseEventArgs e, int zoom)
        {
            var obj = new DrawMarker(control.Objects.Property.ID, e.X, e.Y, zoom, TOOL_TYPE.model, control.MarkerType);
            AddNewObject(control, obj);
        }
    }
}
