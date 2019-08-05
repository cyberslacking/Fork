using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapToolkit
{
    /// <summary>
    /// 此枚举插值必须插在倒数第二的位置，即numberOfObjects的前面
    /// </summary>
    public enum TOOL_TYPE  
    {
        pointer,
        marker,
        defence,
        camera,
        rectangle,
        ellipse,
        polygon,
        text,
        image,
        unknow,
        model,
        numberOfObjects
    };

    public enum MARKER_TYPE
    {
        Green,
        Yellow,
        Red,
        Gray
    }


    public enum MAP_MODE
    {
        edit,
        none
    }

    public enum OBJ_OPERATE
    {
        insert,
        remove,
        modify
    }

    public enum MAP_PROVIDERS
    {
        GoogleChinaMap,
        GoogleTerrainMap,
        GoogleChinaSatelliteMap,
        GoogleChinaHybridMap,
        BingMap,
        BingSatelliteMap,
        OpenStreetMap
    };
}
