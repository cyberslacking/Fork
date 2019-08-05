using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapToolkit
{
    public delegate void EHZoomChanged(int zoom);
    public delegate void EHObjectSelected(ConfigBase o);
    public delegate void EHObjectChanged(DrawObject obj, OBJ_OPERATE operate);
    public delegate void EHMouseRightClick(int x, int y);
    public delegate void EHPropertyChanged(ConfigBase o);
    public delegate void EHLableValueChanged(string label, object value);
    public delegate void EHDrawPolylineMouseEnter(DrawPolyline drawPolyline, int x, int y);
    public delegate void EHDrawPolylineMouseClick(DrawPolyline drawPolyline, int x, int y);
    public delegate void EHGMapMarkerMouseClick(DrawPolyline line, object obj, GMapMarker marker, MouseButtons button);
}
