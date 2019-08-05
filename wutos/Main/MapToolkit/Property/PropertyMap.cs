using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;

using GMap.NET;
using GMap.NET.WindowsForms;

namespace MapToolkit
{
    public class PropertyMap : PropertyBase
    {
        private AccessMode accessMode = AccessMode.ServerAndCache;
        private MAP_PROVIDERS providers = MAP_PROVIDERS.GoogleChinaSatelliteMap;
        private Int32 zoom = 10;
        private string mapFile = AppDomain.CurrentDomain.BaseDirectory + @"MapData\TileDBv5\en\Data.gmdb";

        public PropertyMap()
        {
            Name = "周界安防地图";
        }

        [CategoryAttribute("地图"), DisplayName("服务提供商"), DescriptionAttribute("选择一个适合的地图服务提供商.")]
        public MAP_PROVIDERS Providers
        {
            get
            {
                return providers;
            }
            set
            {
                providers = value;
                if (IsLoad && ehLableValueChanged != null)
                    ehLableValueChanged("Providers", value);
            }
        }

        [CategoryAttribute("地图"), DisplayName("数据访问模式"), DescriptionAttribute("CacheOnly模式下，必须指定数据文件.")]
        public AccessMode AccessMode
        {
            get
            {
                return accessMode;
            }
            set
            {
                accessMode = value;
                if (IsLoad && ehLableValueChanged != null)
                    ehLableValueChanged("AccessMode", value);
            }
        }

        [CategoryAttribute("地图"), DisplayName("离线地图文件"), DescriptionAttribute("离线地图文件路径.")]
        public string MapFile
        {
            get
            {
                return mapFile;
            }
            set
            {
                mapFile = value;
                if (IsLoad && ehLableValueChanged != null)
                    ehLableValueChanged("MapFile", value);
            }
        }

        [CategoryAttribute("显示"), DisplayName("当前图层"), ReadOnlyAttribute(true), DescriptionAttribute("当前显示的图层")]
        public Int32 Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                if ((value >= MinZoom) && (value <= MaxZoom))
                {
                    zoom = value;
                }
            }
        }
    }
}
