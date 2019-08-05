using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;
 
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace MapToolkit
{

    public class PropertyLine : PropertyObject
    {
        private Color color = Color.DeepSkyBlue;
        private int penWidth = 2;
        private double distance = 0.0;

        public PropertyLine Clone()
        {
            PropertyLine p = new PropertyLine();
            p.ID = this.ID;
            p.DefaultZoom = this.DefaultZoom;
            p.MinZoom = this.MinZoom;
            p.MaxZoom = this.MaxZoom;
            p.Name = this.Name;
            p.LocalPosition = this.LocalPosition;
            p.color = this.color;
            p.penWidth = this.penWidth;
            p.distance = this.distance;
            p.Type = this.Type;
            return p;
        }

        [CategoryAttribute("直线属性"), DisplayName("宽度"), DescriptionAttribute("填写一个适合的名称.")]
        public int PenWidth
        {
            get
            {
                return penWidth;
            }
            set
            {
                penWidth = value;
                if (IsLoad && ehLableValueChanged != null)
                ehLableValueChanged("PenWidth", value);
            }
        }

        [CategoryAttribute("直线属性"), DisplayName("颜色"), DescriptionAttribute("直线颜色.")]
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                if (IsLoad && ehLableValueChanged != null)
                ehLableValueChanged("Color", value);
            }
        }

        [CategoryAttribute("直线属性"), DisplayName("长度(km)"), ReadOnlyAttribute(true), DescriptionAttribute("名称显示模式.")]
        public double Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
            }
        }
    }
}
