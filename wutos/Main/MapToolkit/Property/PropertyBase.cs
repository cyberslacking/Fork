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
    public class PropertyBase
    {
        private long id;
        private string name;
        private Int32 defaultZoom = 10;
        private Int32 minZoom = 1;
        private Int32 maxZoom = 23;
        private PointLatLng localPosition = new PointLatLng(30.9776090933487, 118.01513671875);


        [XmlIgnore]
        [Browsable(false)]
        public bool IsLoad = false;


        //[XmlIgnore]
        //[Browsable(false)]
        //public PointLatLng LocalPosition = new PointLatLng(30.9776090933487, 118.01513671875);


        [XmlIgnore]
        [Browsable(false)]
        public EHLableValueChanged ehLableValueChanged = null;


        public PropertyBase()
        {
            byte[] bytes = Guid.NewGuid().ToByteArray();
            id = BitConverter.ToInt64(bytes, 0);
        }

        [CategoryAttribute("其它"), DisplayName("ID"), ReadOnlyAttribute(true), DescriptionAttribute("唯一标识")]
        public long ID
        {
            get { return id; }
            set
            {
                id = value;
            }
        }

        [CategoryAttribute("其它"), DisplayName("名称"), DescriptionAttribute("名称")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (IsLoad && ehLableValueChanged != null)
                    ehLableValueChanged("Name", value);
            }
        }

        [CategoryAttribute("显示"), DisplayName("默认图层"), DescriptionAttribute("当前显示的图层")]
        public Int32 DefaultZoom
        {
            get
            {
                return defaultZoom;
            }
            set
            {
                if ((value >= MinZoom) && (value <= MaxZoom))
                {
                    defaultZoom = value;
                    if (IsLoad && ehLableValueChanged != null)
                        ehLableValueChanged("DefaultZoom", value);
                }
            }
        }

        [CategoryAttribute("显示"), DisplayName("最小图层"), DescriptionAttribute("当前允许的最大图层")]
        public Int32 MinZoom
        {
            get
            {
                return minZoom;
            }
            set
            {
                if ((value >= 0) && (value <= 24) && value < MaxZoom)
                {
                    minZoom = value;
                    if (IsLoad && ehLableValueChanged != null)
                        ehLableValueChanged("MinZoom", value);
                }
            }
        }

        [CategoryAttribute("显示"), DisplayName("最大图层"), DescriptionAttribute("当前允许的最大图层")]
        public Int32 MaxZoom
        {
            get
            {
                return maxZoom;
            }
            set
            {
                if ((value >= 0) && (value <= 24) && value > minZoom)
                {
                    maxZoom = value;
                    if (IsLoad && ehLableValueChanged != null)
                        ehLableValueChanged("MaxZoom", value);
                }
            }
        }
     
        [CategoryAttribute("位置"), DisplayName("经度"), DescriptionAttribute("当前对象显示的经度")]
        public PointLatLng LocalPosition 
        {
            get { return localPosition; }
            set {
                localPosition = value;
                if (IsLoad && ehLableValueChanged != null)
                    ehLableValueChanged("Position", value);
                
            }
        }


        /*
[CategoryAttribute("位置"), DisplayName("经度"), DescriptionAttribute("当前对象显示的经度")]
public double Lat
{
    get { return LocalPosition.Lat; }
    set {
        LocalPosition.Lat = value;
        if (IsLoad && ehLableValueChanged != null)
            ehLableValueChanged("Lat", value);
                
    }
}

[CategoryAttribute("位置"), DisplayName("纬度"), DescriptionAttribute("当前对象显示的纬度")]
public double Lng
{
    get { return LocalPosition.Lng; }
    set { 
        LocalPosition.Lng = value;
        if (IsLoad && ehLableValueChanged != null)
            ehLableValueChanged("Lng", value);
    }
}
*/
    }
}
