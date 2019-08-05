/**********************************************************************
 * 
 * 文 件 名: ProtocolDVS.cs
 * 创 建 人: 刘简
 * 创建时间: 2017-03-01
 * 描    述: WUTOS DVS，接收报警、故障信息 针对设备DVS
 * 
 * 修 改 人: 
 * 修改时间: 
 * 修改内容: 
 * 
 **********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Timers;
using System.Windows.Forms;
using System.Data.SqlClient;


using System.Runtime.InteropServices;

using APP.Common;

namespace APP.Protocol
{


    public class ProtocolDVS : ProtocolDriver
    {
        public delegate void NETMSGPROC_G(int handle, int nType, IntPtr pointer);

        enum CMD_CODE
        {
            R_EVENT,
            R_ERROR,
            Q_VALUE,
            Q_WAVE,
            R_CONNECT,
            R_DISCONNECT
        };


        public struct EVENT_PACK
        {
            public int no;
            public int channel;
            public int id;
            public CLASSIFICATION classification;
            public int range1;
            public int range2;
            public int alarmneed;
            public int confidence;
            public SYSTEMTIME time;
            public int flag;
        };

        public struct ERROR_PACK
        {
            public int channel;
            public int id;
            public int error;
            public SYSTEMTIME time;
        };

        private static void cbNetMsgProc(int handle, int nType, IntPtr pointer)
        {
            if (GlobalMain.from == null)
                return;
            ProtocolDVS driver = GlobalMain.from._listDriver.Find(x => x.instance == handle) as ProtocolDVS;
            if (driver == null)
                return;
            switch ((CMD_CODE)nType)
            {
                case CMD_CODE.R_CONNECT:
                    driver.OnConnectEH(driver.device, 0);
                    break;
                case CMD_CODE.R_DISCONNECT:
                    driver.OnConnectEH(driver.device, 1);
                    break;
                case CMD_CODE.R_EVENT:
                    {
                        EVENT_PACK pack = (EVENT_PACK)Marshal.PtrToStructure(pointer, typeof(EVENT_PACK));
                        if (pack.alarmneed == 0)
                        {
                            break;
                        }
                        Sensor sensor = driver.FindSensorBySID(pack.channel, pack.id);
                        if (sensor != null)
                        {
                            STATUS_INFO info = new STATUS_INFO();
                            info.no = pack.no;
                            info.id = sensor.id;
                            info.type = (MONITOR_TYPE)sensor.type;
                            info.name = sensor.name;
                            info.range1 = pack.range1.ToString();
                            info.range2 = pack.range2.ToString();
                            info.confidence = pack.confidence/100;
                            info.classification = (CLASSIFICATION)pack.classification;
                            info.status = STATUS_TYPE.ALARM;
                            
                            DateTime vDateTime = new DateTime(
                                pack.time.vYear, pack.time.vMonth, pack.time.vDay, // 月日年
                                pack.time.vHour, pack.time.vMinute, pack.time.vSecond, // 时分秒
                                pack.time.vMiliseconds); // 毫秒
                            info.time = vDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                            driver.OnObjectStatusEH(info);
                        }
                    }
                    break;
                case CMD_CODE.R_ERROR:
                    {
                        ERROR_PACK pack = (ERROR_PACK)Marshal.PtrToStructure(pointer, typeof(ERROR_PACK));
                        Sensor sensor = driver.FindSensorBySID(pack.channel, pack.id);
                        if (sensor != null)
                        {
                            DateTime vDateTime = new DateTime(
                                pack.time.vYear, pack.time.vMonth, pack.time.vDay, // 月日年
                                pack.time.vHour, pack.time.vMinute, pack.time.vSecond, // 时分秒
                                pack.time.vMiliseconds); // 毫秒
                            Channel channel = driver.device.listChannel.Find(x => x.number == pack.channel);
                            if (channel != null)
                            {
                                List<Sensor> listSensorFront =  channel.listSensor.FindAll(x => (x.number != sensor.number && x.status == STATUS_TYPE.ERROR));
                                foreach (var front in listSensorFront)
                                {
                                    ERROR_INFO infoFront = new ERROR_INFO();
                                    infoFront.id = front.id;
                                    infoFront.name = front.name;
                                    infoFront.status = STATUS_TYPE.NORMAL;
                                    infoFront.starttime = vDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                    infoFront.type = OBJECT_TYPE.sensor;
                                    driver.OnObjectErrorEH(infoFront);
                                }
                            }

                            ERROR_INFO info = new ERROR_INFO();
                            info.id = sensor.id;
                            info.name = sensor.name;
                            if (pack.error == 1)
                            {
                                info.status = STATUS_TYPE.ERROR;
                            }
                            else if (pack.error == 0)
                            {
                                info.status = STATUS_TYPE.NORMAL;
                            }
                            info.starttime = vDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                            info.type = OBJECT_TYPE.sensor;
                            driver.OnObjectErrorEH(info);
                        }
                    }
                    break;
            }
        }

        [DllImport("DLL\\WUTOS\\WSDK.dll")]
        private static extern int Init();
        [DllImport("DLL\\WUTOS\\WSDK.dll")]
        private static extern void Destroy(int id);
        [DllImport("DLL\\WUTOS\\WSDK.dll")]
        private static extern int Connect(int id, string ip, ushort port, NETMSGPROC_G cbNetMsgProc);
        [DllImport("DLL\\WUTOS\\WSDK.dll")]
        private static extern void DisConnect(int id);

        NETMSGPROC_G cbNetMsg = new NETMSGPROC_G(cbNetMsgProc);

        public ProtocolDVS(Device device)
            : base(device)
        {
            instance = Init();
            if (instance == -1)
            {
                OnConnectEH(device, 2);
            }
        }
        ~ProtocolDVS()
        {
            Destroy(instance);
        }

        public override void Start()
        { 
            if (!device.enable)
                return;
            if (Connect(instance, device.ip, (ushort)device.port, cbNetMsg) == 0)
            {
                OnConnectEH(device, 0);
            }
            else
            {
                OnConnectEH(device, 1);
            }
        }

        public override void Stop()
        {
            OnConnectEH(device, 1);
            DisConnect(instance);
        }

        public override void DoOutput(byte DO,byte flag)
        {
        }
    }
}
