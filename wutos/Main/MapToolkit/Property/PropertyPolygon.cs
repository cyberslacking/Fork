using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;
 

namespace MapToolkit
{

    public class PropertyPolygon : PropertyLine
    {

        private Color fill = Color.IndianRed;

        public new PropertyPolygon Clone()
        {
            PropertyPolygon p = new PropertyPolygon();
            p.ID = this.ID;
            p.DefaultZoom = this.DefaultZoom;
            p.MinZoom = this.MinZoom;
            p.MaxZoom = this.MaxZoom;
            p.Name = this.Name;
            p.LocalPosition = this.LocalPosition;
            p.Color = this.Color;
            p.PenWidth = this.PenWidth;
            p.Distance = this.Distance;
            p.Fill = this.Fill;
            p.Type = this.Type;
            return p;
        }

        [CategoryAttribute("填充"), DisplayName("颜色"), DescriptionAttribute("填充一种颜色，默认透明.")]
        public Color Fill
        {
            get
            {
                return fill;
            }
            set
            {
                fill = value;
                if (IsLoad && ehLableValueChanged != null)
                    ehLableValueChanged("Fill", value);
            }
        }
    }
}
