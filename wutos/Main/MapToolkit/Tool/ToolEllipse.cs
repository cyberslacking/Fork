using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace MapToolkit
{
	/// <summary>
	/// Ellipse tool
	/// </summary>
	class ToolEllipse : ToolRectangle
	{
        public override void OnMouseDown(MapControl control, MouseEventArgs e, int zoom)
        {
            var obj = new DrawEllipse(e.X, e.Y, zoom, TOOL_TYPE.ellipse);
            AddNewObject(control, obj);
        }
	}
}
