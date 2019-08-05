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
    public class PropertyMarker : PropertyObject
    {

        private string imageFile = @"image\marker_0.ico";
        private MarkerTooltipMode toolTipMode = MarkerTooltipMode.Always;

        private MARKER_TYPE markerType = MARKER_TYPE.Green;

        public PropertyMarker Clone()
        {
            PropertyMarker p = new PropertyMarker();
            p.ID = this.ID;
            p.DefaultZoom = this.DefaultZoom;
            p.MinZoom = this.MinZoom;
            p.MaxZoom = this.MaxZoom;
            p.Name = this.Name;
            p.Type = this.Type;
            p.LocalPosition = this.LocalPosition;
            p.ImageFile = this.ImageFile;
            p.toolTipMode = this.toolTipMode;
            return p;
        }


        //[DisplayName("图片文件")]
        //[Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        //[Browsable(false)]
        [CategoryAttribute("图片"), DisplayName("文件路径"), DescriptionAttribute("文件路径.")]
        public string ImageFile
        {
            get
            {
                return imageFile;
            }
            set
            {
                imageFile = value;
            }
        }

        [CategoryAttribute("标签"), DisplayName("模式"), DescriptionAttribute("名称显示模式.")]
        public MarkerTooltipMode ToolTipMode
        {
            get
            {
                return toolTipMode;
            }
            set
            {
                toolTipMode = value;
                if (IsLoad && ehLableValueChanged != null)
                {
                    ehLableValueChanged("ToolTipMode", value);
                }
            }
        }

        [CategoryAttribute("图标"), DisplayName("类型"), DescriptionAttribute("图标类型.")]
        public MARKER_TYPE MarkerType
        {
            get
            {
                return this.markerType;
            }
            set
            {
                this.markerType = value;

                if (IsLoad && ehLableValueChanged != null)
                {
                    ehLableValueChanged("MarkerType", value);
                }
            }
        }
    }
}
