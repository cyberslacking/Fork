using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;


namespace MapToolkit
{
	/// <summary>
	/// Base class for all tools which create new graphic object
	/// </summary>
	abstract class ToolObject : Tool
	{
        private Cursor cursor;

        /// <summary>
        /// Tool cursor.
        /// </summary>
        protected Cursor Cursor
        {
            get
            {
                return cursor;
            }
            set
            {
                cursor = value;
            }
        }

        public ToolObject()
		{
            MemoryStream stream = new MemoryStream(Properties.Resources.Marker);
            Cursor = new Cursor(stream);
		}

        /// <summary>
        /// Left mouse is released.
        /// New object is created and resized.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="e"></param>
        public override void OnMouseUp(MapControl control, MouseEventArgs e)
        {
            control.Objects[0].Normalize();
            control.AddCommandToHistory(new CommandAdd(control.Objects[0]));
            control.ActiveTool = TOOL_TYPE.pointer;

            control.Refresh();
        }

        public override void OnMouseMove(MapControl control, MouseEventArgs e)
        {
            control.Cursor = Cursor;
        }

        /// <summary>
        /// Add new object to draw DrawArea.Control.
        /// Function is called when user left-clicks draw area,
        /// and one of ToolObject-derived tools is active.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="o"></param>
        protected void AddNewObject(MapControl control, DrawObject o)
        {
            control.Objects.UnselectAll();
            control.Add(o);
            control.SetDirty();
        }
	}
}
