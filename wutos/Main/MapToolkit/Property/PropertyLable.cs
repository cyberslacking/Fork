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
    [Serializable]
    public class PropertyLabel : PropertyObject
    {
        private Font font = new Font("Arial", 12);
        private Color color = Color.Red;

        public PropertyLabel Clone()
        {
            PropertyLabel p = new PropertyLabel();
            p.ID = this.ID;
            p.DefaultZoom = this.DefaultZoom;
            p.MinZoom = this.MinZoom;
            p.MaxZoom = this.MaxZoom;
            p.Name = this.Name;
            p.LocalPosition = this.LocalPosition;
            p.font = this.font;
            p.Type = this.Type;
            return p;
        }
        

        [CategoryAttribute("字体"), DisplayName("颜色"), DescriptionAttribute("填充一种颜色，默认透明.")]
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
                    ehLableValueChanged("Fill", value);
            }
        }

        [CategoryAttribute("字体"), DisplayName("属性"), DescriptionAttribute("属性.")]
        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
                if (IsLoad && ehLableValueChanged != null)
                    ehLableValueChanged("Font", value);
            }
        }
    }
}
