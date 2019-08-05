using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using GMap.NET;

namespace MapToolkit
{
	/// <summary>
	/// Text tool
	/// </summary>
	class ToolText : ToolObject
	{
        public override void OnMouseDown(MapControl control, MouseEventArgs e, int zoom)
        {
            var obj = new DrawText(e.X, e.Y, zoom, TOOL_TYPE.text);
            AddNewObject(control, obj);
        }
	}
}
