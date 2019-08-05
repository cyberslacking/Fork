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
    public class PropertyText : PropertyObject
    {
        private Font font = new Font("Arial", 12);
        private Color color = Color.Red;
        private bool autosize = false;

        public PropertyText Clone()
        {
            PropertyText p = new PropertyText();
            p.ID = this.ID;
            p.DefaultZoom = this.DefaultZoom;
            p.MinZoom = this.MinZoom;
            p.MaxZoom = this.MaxZoom;
            p.Name = this.Name;
            p.LocalPosition = this.LocalPosition;
            p.Font = this.Font;
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


        [CategoryAttribute("显示"), DisplayName("自由大小"), DescriptionAttribute("根据图层改变自身大小")]
        public bool AutoSize
        {
            get
            {
                return autosize;
            }
            set
            {
                autosize = value;
                if (IsLoad && ehLableValueChanged != null)
                    ehLableValueChanged("AutoSize", value);
            }
        }
    }
}
