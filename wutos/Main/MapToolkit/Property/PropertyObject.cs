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
    public class PropertyObject : PropertyBase
    {
        private TOOL_TYPE type = TOOL_TYPE.defence;

        public PropertyObject()
        {
            Name = "未命名";
        }

        [CategoryAttribute("对象"), DisplayName("定义"), DescriptionAttribute("监测对象类型.")]
        public TOOL_TYPE Type
        {
            get { return type; }
            set
            {
                type = value;
                if (IsLoad && ehLableValueChanged != null)
                    ehLableValueChanged("Type", value);
            }
        }
    }
}
