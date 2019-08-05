/**********************************************************************
 * 
 * 文 件 名: ProtocolCIAS.cs
 * 创 建 人: 刘简
 * 创建时间: 2017-07-01
 * 描    述: INANTER，接收报警、故障信息 针对吉斯设备
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
    public class ProtocolInanter : ProtocolDriver
    {
        public delegate void NETMSGPROC_G(int id, int nType, IntPtr pointer);

        enum CMD_CODE
        {
            R_EVENT,
            R_STATUS,
            Q_SYSTEMTIME,
            R_SYSTEMTIME,
            R_CONNECT,
            R_DISCONNECT,
            R_ARM,
            R_DISARM
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

        public struct STATUS_PACK
        {
            public int channel;
            public int id;
            public int status; //0:正常 1:故障 2:撤防 3:离线 4:防拆
            public SYSTEMTIME time;
        };

        public static void cbNetMsgProc(int id, int nType, IntPtr pointer)
        {
            switch ((CMD_CODE)nType)
            {
                case CMD_CODE.R_CONNECT:
                    g_protocalInanter.OnConnectEH(g_protocalInanter.device, 0);
                    break;
                case CMD_CODE.R_DISCONNECT:
                    g_protocalInanter.OnConnectEH(g_protocalInanter.device, 1);
                    break;
                case CMD_CODE.R_EVENT:
                    {
                        EVENT_PACK pack = (EVENT_PACK)Marshal.PtrToStructure(pointer, typeof(EVENT_PACK));
                        Sensor sensor = g_protocalInanter.FindSensorBySID(pack.channel, pack.id);
                        if (sensor != null)
                        {
                            STATUS_INFO info = new STATUS_INFO();
                            info.no = (int)sensor.id;
                            info.id = sensor.id;
                            info.type = (MONITOR_TYPE)sensor.type;
                            info.name = sensor.name;
                            info.range1 = pack.range1.ToString();
                            info.range2 = pack.range2.ToString();
                            info.confidence = pack.confidence;
                            info.classification = (CLASSIFICATION)pack.classification;
                            if (pack.alarmneed == 0)
                            {
                                info.status = STATUS_TYPE.WARNING;
                            }
                            else
                            {
                                info.status = STATUS_TYPE.ALARM;
                            }
                            DateTime vDateTime = new DateTime(
                                pack.time.vYear, pack.time.vMonth, pack.time.vDay, // 月日年
                                pack.time.vHour, pack.time.vMinute, pack.time.vSecond, // 时分秒
                                pack.time.vMiliseconds); // 毫秒
                            info.time = vDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                            g_protocalInanter.OnObjectStatusEH(info);
                        }
                    }
                    break;
                case CMD_CODE.R_STATUS:
                    {
                        STATUS_PACK pack = (STATUS_PACK)Marshal.PtrToStructure(pointer, typeof(STATUS_PACK));
                        Sensor sensor = g_protocalInanter.FindSensorBySID(pack.channel, pack.id);
                        if (sensor != null)
                        {
                            DateTime vDateTime = new DateTime(
                                pack.time.vYear, pack.time.vMonth, pack.time.vDay, // 月日年
                                pack.time.vHour, pack.time.vMinute, pack.time.vSecond, // 时分秒
                                pack.time.vMiliseconds); // 毫秒
                            Channel channel = g_protocalInanter.device.listChannel.Find(x => x.number == pack.channel);
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
                                    g_protocalInanter.OnObjectErrorEH(infoFront);
                                }
                            }

                            ERROR_INFO info = new ERROR_INFO();
                            info.id = sensor.id;
                            info.name = sensor.name;
                            if (pack.status == 0)
                            {
                                info.status = STATUS_TYPE.NORMAL;
                            }
                            else if (pack.status == 1)
                            {
                                info.status = STATUS_TYPE.ERROR;
                            }
                            else if (pack.status == 2)
                            {
                                info.status = STATUS_TYPE.DISABLE;
                            }
                            else if (pack.status == 3)
                            {
                                info.status = STATUS_TYPE.OFFLINE;
                            }
                            else if (pack.status == 4)
                            {
                                info.status = STATUS_TYPE.UNCOVER;
                            }
                            info.starttime = vDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                            info.type = OBJECT_TYPE.sensor;
                            g_protocalInanter.OnObjectErrorEH(info);
                        }
                    }
                    break;
            }
        }

        [DllImport("DLL\\INANTER\\Inanter_SDK.dll", EntryPoint = "Begin", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Begin(string ip, int port, NETMSGPROC_G cbNetMsgProc);
        [DllImport("DLL\\INANTER\\Inanter_SDK.dll", EntryPoint = "End")]
        private static extern void End();

        NETMSGPROC_G cbNetMsg = new NETMSGPROC_G(cbNetMsgProc);

        private List<STATUS_INFO> listAlarm = new List<STATUS_INFO>();
        private List<ERROR_INFO> listError = new List<ERROR_INFO>();

        public ProtocolInanter(Device device)
            : base(device)
        {
            g_protocalInanter = this;
        }

        public override void Start()
        { 
            if (!device.enable)
                return;
            if (Begin(device.ip, device.port, cbNetMsg))
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
            End();
        }

        public override void DoOutput(byte DO,byte flag)
        {
        }

        public static ProtocolInanter g_protocalInanter = null;
    }
}
