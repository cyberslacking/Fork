using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace MapToolkit
{
	/// <summary>
	/// Image tool
	/// </summary>
	class ToolImage : ToolObject
	{
        public override void OnMouseDown(MapControl control, MouseEventArgs e, int zoom)
        {
            var obj = new DrawImage(e.X, e.Y, zoom, TOOL_TYPE.image);
            AddNewObject(control, obj);
        }
	}
}
