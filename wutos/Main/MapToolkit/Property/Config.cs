using System;
using System.Collections.Generic;
using System.Drawing;

using ProtoBuf;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace MapToolkit
{
    [ProtoContract]
    [ProtoInclude(201, typeof(ConfigMap))]
    [ProtoInclude(202, typeof(ConfigElement))]
    public class ConfigBase
    {
        [ProtoMember(1)]
        public long id;
        [ProtoMember(2)]
        public string name;
        [ProtoMember(3)]
        public Int32 defaultZoom = 10;
        [ProtoMember(4)]
        public Int32 minZoom = 1;
        [ProtoMember(5)]
        public Int32 maxZoom = 23;
        [ProtoMember(6)]
        public PointLatLng localPosition = new PointLatLng(30.9776090933487, 118.01513671875);
        public ConfigBase()
        {
            id = Global.GuidToInt64();
        }
    }


    [ProtoContract]
    public class ConfigMap: ConfigBase
    {
        [ProtoMember(1)]
        public AccessMode accessMode = AccessMode.ServerAndCache;
        [ProtoMember(2)]
        public MAP_PROVIDERS providers = MAP_PROVIDERS.GoogleChinaSatelliteMap;
        [ProtoMember(3)]
        public Int32 zoom = 10;
        [ProtoMember(4)]
        public string mapFile = AppDomain.CurrentDomain.BaseDirectory + @"MapData\TileDBv5\en\Data.gmdb";
        [ProtoMember(5)]
        public List<ConfigElement> listElement = new List<ConfigElement>();

        public ConfigMap()
        {
            name = "地图";
        }

    }

    [ProtoContract]
    [ProtoInclude(201, typeof(ConfigMarker))]
    [ProtoInclude(202, typeof(ConfigLine))]
    public class ConfigElement : ConfigBase
    {
        [ProtoMember(1)]
        public TOOL_TYPE type = TOOL_TYPE.defence;
    }

    [ProtoContract]
    public class ConfigMarker : ConfigElement
    {
        [ProtoMember(1)]
        private string imageFile = @"image\marker_0.ico";
        [ProtoMember(2)]
        private MarkerTooltipMode toolTipMode = MarkerTooltipMode.Always;
        public ConfigMarker()
        {
            name = "标签";
        }
    }

    [ProtoContract]
    public class ConfigLine : ConfigElement
    {
        [ProtoMember(1)]
        public Color color = Color.DeepSkyBlue;
        [ProtoMember(2)]
        public int penWidth = 2;
        [ProtoMember(3)]
        public double distance = 0.0;
        public ConfigLine()
        {
            name = "线条";
        }
    }
}
