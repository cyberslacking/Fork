using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APP.Common;

namespace APP.Protocol
{
    class FactoryDriver
    {
        public static ProtocolDriver CreateInstance(Device device)
        {
            ProtocolDriver driver = null;
            switch (device.key)
            {
                case DEVICE_KEY.FBG:
                    driver =  new ProtocolFbg(device);
                    break;
                case DEVICE_KEY.DVS:
                case DEVICE_KEY.DTS:
                    driver =  new ProtocolDVS(device);
                    break;
                case DEVICE_KEY.INANTER:
                    driver = new ProtocolInanter(device);
                    break;
                case DEVICE_KEY.ES:
                    driver = new ProtocolEs(device);
                    break;
                case DEVICE_KEY.HK:
                    driver = new ProtocolHK(device);
                    break;
                case DEVICE_KEY.MOXA_2K:
                    driver = new ProtocolMoxa(device);
                    break;
                case DEVICE_KEY.JDEX:
                    driver = new ProtocolJDExpand(device);
                    break;
                case DEVICE_KEY.VIDEO:
                    driver = new ProtocolVideo(device);
                    break;
                case DEVICE_KEY.HKAI:
                    driver = new ProtocolHKAI(device);
                    break;
                case DEVICE_KEY.DHAlarm:
                    driver = new ProtocolDHAlarm(device);
                    break;
                default:
                    break;
            }
            return driver;
        }
    }
}
