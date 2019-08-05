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
 
    public class PropertyImage: PropertyObject
    {
        private string imageFile = "image\\background.jpg";
        public PropertyImage Clone()
        {
            PropertyImage p = new PropertyImage();
            p.ID = this.ID;
            p.DefaultZoom = this.DefaultZoom;
            p.MinZoom = this.MinZoom;
            p.MaxZoom = this.MaxZoom;
            p.Name = this.Name;
            p.LocalPosition = this.LocalPosition;
            p.ImageFile = this.imageFile;
            p.Type = this.Type;
            return p;
        }

        [CategoryAttribute("图片"), DisplayName("文件"), DescriptionAttribute("图片文件.")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))] 
        public string ImageFile
        {
            get
            {
                return imageFile;
            }
            set
            {
                imageFile = value;
                if (IsLoad && ehLableValueChanged != null)
                {
                    ehLableValueChanged("ImageFile", value);
                }
            }
        }
    }
}
