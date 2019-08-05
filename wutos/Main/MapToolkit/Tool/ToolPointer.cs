using System;
using System.Windows.Forms;
using System.Drawing;
using GMap.NET;

namespace MapToolkit
{
	/// <summary>
	/// Pointer tool
	/// </summary>
	class ToolPointer : Tool
	{
        private enum SelectionMode
        {
            None,
            NetSelection,   // group selection is active
            Move,           // object(s) are moves
            Size            // object is resized
        }

        private SelectionMode selectMode = SelectionMode.None;

        // Object which is currently resized:
        private DrawObject resizedObject;
        private int resizedObjectHandle;

        // Keep state about last and current point (used to move and resize objects)
        private Point lastPoint = new Point(0,0);
        private Point startPoint = new Point(0, 0);

        private CommandChangeState commandChangeState;
        private bool wasMove;

		public ToolPointer()
		{
		}

        /// <summary>
        /// Left mouse button is pressed
        /// </summary>
        /// <param name="control"></param>
        /// <param name="e"></param>
        public override void OnMouseDown(MapControl control, MouseEventArgs e, int zoom)
        {
            commandChangeState = null;
            wasMove = false;

            selectMode = SelectionMode.None;
            Point point = new Point(e.X, e.Y);

            // Test for resizing (only if control is selected, cursor is on the handle)
            foreach (DrawObject o in control.Objects.Selection)
            {
                int handleNumber = o.HitTest(point);
                if (handleNumber > 0)
                {
                    selectMode = SelectionMode.Size;

                    // keep resized object in class member
                    resizedObject = o;
                    resizedObjectHandle = handleNumber;

                    // Since we want to resize only one object, unselect all other objects
                    control.Objects.UnselectAll();
                    o.Selected = true;

                    commandChangeState = new CommandChangeState(control.Objects);

                    break;
                }
            }

            // Test for move (cursor is on the object)
            if ( selectMode == SelectionMode.None )
            {
                int n1 = control.Objects.Count;
                DrawObject o = null;

                for ( int i = 0; i < n1; i++ )
                {
                    if ( control.Objects[i].HitTest(point) == 0 )
                    {
                        o = control.Objects[i];
                        break;
                    }
                }

                if ( o != null )
                {
                    selectMode = SelectionMode.Move;

                    // Unselect all if Ctrl is not pressed and clicked object is not selected yet
                    if ( ( Control.ModifierKeys & Keys.Control ) == 0  && !o.Selected )
                        control.Objects.UnselectAll();

                    // Select clicked object
                    o.Selected = true;
                    commandChangeState = new CommandChangeState(control.Objects);
                    control.Cursor = Cursors.Hand;
                }
            }

            // Net selection
            if ( selectMode == SelectionMode.None )
            {
                // click on background
                if ( ( Control.ModifierKeys & Keys.Control ) == 0 )
                    control.Objects.UnselectAll();
                selectMode = SelectionMode.NetSelection;
                control.Selected = true;
            }

            lastPoint.X = e.X;
            lastPoint.Y = e.Y;
            startPoint.X = e.X;
            startPoint.Y = e.Y;

            control.Refresh();

            if (selectMode == SelectionMode.NetSelection)
            {
                // Draw selection rectangle in initial position
                ControlPaint.DrawReversibleFrame(
                    control.RectangleToScreen(DrawRectangle.GetNormalizedRectangle(startPoint, lastPoint)),
                    Color.Black,
                    FrameStyle.Dashed);
            }
        }


        /// <summary>
        /// Mouse is moved.
        /// None button is pressed, or left button is pressed.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="e"></param>
        public override void OnMouseMove(MapControl control, MouseEventArgs e)
        {
            Point point = new Point(e.X, e.Y);
            Point oldPoint = lastPoint;

            wasMove = true;
            // set cursor when mouse button is not pressed
            if (e.Button == MouseButtons.None)
            {
                Cursor cursor = null;

                for (int i = 0; i < control.Objects.Count; i++)
                {
                    int n = control.Objects[i].HitTest(point);

                    if (n > 0)
                    {
                        cursor = control.Objects[i].GetHandleCursor(n);
                        break;
                    }
                    else if (n == 0)
                    {
                        cursor = Cursors.Hand;
                        break;
                    }
                }

                if (cursor == null)
                {
                    cursor = Cursors.Default;
                }

                control.Cursor = cursor;

                return;
            }

            if (e.Button != MouseButtons.Left)
                return;
            /// Left button is pressed

            // Find difference between previous and current position
            int dx = e.X - lastPoint.X;
            int dy = e.Y - lastPoint.Y;

            lastPoint.X = e.X;
            lastPoint.Y = e.Y;

            // resize
            if (selectMode == SelectionMode.Size)
            {
                if (resizedObject != null)
                {
                    resizedObject.MoveHandleTo(point, resizedObjectHandle);
                    control.Refresh();
                    control.SetDirty();
                }
            }

            // move
            if (selectMode == SelectionMode.Move)
            {
                foreach (DrawObject o in control.Objects.Selection)
                {
                    o.Move(dx, dy);
                }

                control.Cursor = Cursors.Hand;
                control.Refresh();
                control.SetDirty();
            }

            if (selectMode == SelectionMode.NetSelection)
            {
                // Remove old selection rectangle
                ControlPaint.DrawReversibleFrame(
                    control.RectangleToScreen(DrawRectangle.GetNormalizedRectangle(startPoint, oldPoint)),
                    Color.Black,
                    FrameStyle.Dashed);

                // Draw new selection rectangle
                ControlPaint.DrawReversibleFrame(
                    control.RectangleToScreen(DrawRectangle.GetNormalizedRectangle(startPoint, point)),
                    Color.Black,
                    FrameStyle.Dashed);

                return;
            }
        }

        /// <summary>
        /// Right mouse button is released
        /// </summary>
        /// <param name="control"></param>
        /// <param name="e"></param>
        public override void OnMouseUp(MapControl control, MouseEventArgs e)
        {
            if (selectMode == SelectionMode.NetSelection)
            {
                // Remove old selection rectangle
                ControlPaint.DrawReversibleFrame(
                    control.RectangleToScreen(DrawRectangle.GetNormalizedRectangle(startPoint, lastPoint)),
                    Color.Black,
                    FrameStyle.Dashed);

                // Make group selection
                control.Objects.SelectInRectangle(
                    DrawRectangle.GetNormalizedRectangle(startPoint, lastPoint));

            }
            selectMode = SelectionMode.None;


            if ( resizedObject != null )
            {
                // after resizing
                resizedObject.Normalize();
                resizedObject = null;
            }

            control.Refresh();

            if ( commandChangeState != null  && wasMove )
            {
                // Keep state after moving/resizing and add command to history
                commandChangeState.NewState(control.Objects);
                control.AddCommandToHistory(commandChangeState);

                commandChangeState = null;
            }
        }
	}
}
