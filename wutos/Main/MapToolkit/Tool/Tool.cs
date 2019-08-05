using System;
using System.Windows.Forms;
using System.Drawing;
using GMap.NET;

namespace MapToolkit
{
	/// <summary>
	/// Base class for all drawing tools
	/// </summary>
	public abstract class Tool
	{

        public Tool()
        {

        }
        /// <summary>
        /// Left nous button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public virtual void OnMouseDown(MapControl control, MouseEventArgs e, int zoom)
        {
            
        }


        /// <summary>
        /// Mouse is moved, left mouse button is pressed or none button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public virtual void OnMouseMove(MapControl control, MouseEventArgs e)
        {
        }


        /// <summary>
        /// Left mouse button is released
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public virtual void OnMouseUp(MapControl control, MouseEventArgs e)
        {
        }
    }
}
