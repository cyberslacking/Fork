using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapToolkit
{
    public class Global
    {
        public static MapControl control = null;
        public static string configFile = AppDomain.CurrentDomain.BaseDirectory + @"configMap.bin";

        public static long GuidToInt64()
        {
            byte[] bytes = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(bytes, 0);
        }
    }
}
