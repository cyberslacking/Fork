using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using GMap.NET;
namespace MapToolkit
{
	/// <summary>
	/// Rectangle tool
	/// </summary>
	class ToolPolygon : ToolObject
	{
        public override void OnMouseDown(MapControl control, MouseEventArgs e, int zoom)
        {
            var obj = new DrawPolygon(e.X, e.Y, zoom, TOOL_TYPE.polygon);
            AddNewObject(control, obj);
        }
	}
}
