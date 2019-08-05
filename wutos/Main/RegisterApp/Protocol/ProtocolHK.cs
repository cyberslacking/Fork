/**********************************************************************
 * 
 * 文 件 名: ProtocolHK.cs
 * 创 建 人: 刘简
 * 创建时间: 2014-7-14
 * 描    述: 海康网络模块，接收报警、故障信息
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
using System.Xml;
using System.Timers;


using System.Runtime.InteropServices;

using APP.Common;

namespace APP.Protocol
{
    public class ProtocolHK : ProtocolDriver
    {
        public delegate void HK_ReadStat(string ID, int nType, IntPtr StatVary, string strState, int nPort, string strCode);
        public delegate void HK_ReadReport(string ID, string ActId, string SubId, string ZoneId, string UserId, string CID, int Port, string Ip, string Value);

        private List<STATUS_INFO> listAlarm = new List<STATUS_INFO>();
        private List<ERROR_INFO> listError = new List<ERROR_INFO>();
        public static void cbHK_ReadReport(string ID, string ActId, string SubId, string ZoneId, string UserId, string CID, int Port, string Ip, string Value)
        {
            ProtocolHK instrument = listInstrument.Find(x => x.device.ip == Ip);
            if (instrument == null)
                return;

            switch (CID)
            {
                case "E998":
                    instrument.OnConnectEH(instrument.device, 0);
                    break;
                case "E997":
                    instrument.OnConnectEH(instrument.device, 1);
                    break;
                case "E570":
                case "E806":
                    {
                        if (ZoneId == "")
                            return;
                        Sensor sensor = instrument.FindSensorBySID(1, int.Parse(ZoneId));
                        if (sensor != null)
                        {
                            ERROR_INFO old = instrument.listError.Find(x => x.id == sensor.id);
                            if (old == null)
                            {
                                ERROR_INFO info = new ERROR_INFO();
                                info.id = sensor.id;
                                info.name = sensor.name;
                                info.status = STATUS_TYPE.ERROR;
                                info.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                info.type = OBJECT_TYPE.sensor;
                                instrument.OnObjectErrorEH(info);
                                instrument.listError.Add(info);
                            }
                        }
                    }
                    break;
                case "R570":
                case "R806":
                    {
                        if (ZoneId == "")
                            return;
                        Sensor sensor = instrument.FindSensorBySID(1, int.Parse(ZoneId));
                        if (sensor != null)
                        {
                            ERROR_INFO old = instrument.listError.Find(x => x.id == sensor.id);
                            if (old != null)
                            {
                                ERROR_INFO info = new ERROR_INFO();
                                info.id = sensor.id;
                                info.name = sensor.name;
                                info.status = STATUS_TYPE.NORMAL;
                                info.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                info.type = OBJECT_TYPE.sensor;
                                instrument.OnObjectErrorEH(info);
                                instrument.listError.Remove(old);
                            }
                        }
                    }
                    break;
                case "E134":
                case "E103":
                    {
                        if (ZoneId == "")
                            return;
                        Sensor sensor = instrument.FindSensorBySID(1, int.Parse(ZoneId));
                        if (sensor != null)
                        {
                            STATUS_INFO old = instrument.listAlarm.Find(x => x.id == sensor.id);
                            if (old == null)
                            {
                                STATUS_INFO info = new STATUS_INFO();
                                info.no = (int)sensor.id;
                                info.id = sensor.id;
                                info.name = sensor.name;
                                info.status = STATUS_TYPE.ALARM;
                                info.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                info.type = MONITOR_TYPE.io;
                                info.confidence = 100;
                                instrument.OnObjectStatusEH(info);
                                instrument.listAlarm.Add(info);
                            }
                        }
                    }
                    break;
                case "R134":
                case "R103":
                    {
                        if (ZoneId == "")
                            return;
                        Sensor sensor = instrument.FindSensorBySID(1, int.Parse(ZoneId));
                        if (sensor != null)
                        {
                            STATUS_INFO old = instrument.listAlarm.Find(x => x.id == sensor.id);
                            if (old != null)
                            {
                                instrument.listAlarm.Remove(old);
                            }
                        }
                    }
                    break;
            }
        }

        public static void cbHK_ReadStat(string ID, int nType, IntPtr StatVary, string strState, int nPort, string strCode)
        {

        }


        [DllImport("DLL\\HK\\Mta_Sdk.dll", EntryPoint = "mta001_Init")]
        private static extern int mta001_Init(HK_ReadReport cbReadReport, HK_ReadStat cbReadStat);
        [DllImport("DLL\\HK\\Mta_Sdk.dll", EntryPoint = "mta001_StartPort")]
        private static extern int mta001_StartPort(int locport);
        [DllImport("DLL\\HK\\Mta_Sdk.dll", EntryPoint = "mta001_StopPort")]
        private static extern int mta001_StopPort(int locport);
        [DllImport("DLL\\HK\\Mta_Sdk.dll", EntryPoint = "mta001_Exit")]
        private static extern int mta001_Exit();
        [DllImport("DLL\\HK\\Mta_Sdk.dll", EntryPoint = "mta001_CountModules")]
        private static extern int mta001_CountModules();

        HK_ReadReport cbReadReport = new HK_ReadReport(cbHK_ReadReport);
        HK_ReadStat cbReadStat = new HK_ReadStat(cbHK_ReadStat);

        public ProtocolHK(Device device)
            : base(device)
        {
            if (mta001_Init(cbReadReport, null) != 1)
            {
                OnConnectEH(device,2);
            }
        }

        public override void Start()
        { 
            if (!device.enable)
                return;
            if (mta001_StartPort(device.port) != 1)
            {
                OnConnectEH(device, 2);
            }
            if (!listInstrument.Contains(this))
            {
                listInstrument.Add(this);
            }
        }

        public override void Stop()
        {
            if (!listInstrument.Contains(this))
            {
                listInstrument.Remove(this);
            }

            if (mta001_StopPort(device.port) != 1)
            {
                OnConnectEH(device, 2);
            }
            else
            {
                OnConnectEH(device, 1);
            }
        }

        public override void DoOutput(byte DO,byte flag)
        {
        }

        public static List<ProtocolHK> listInstrument = new List<ProtocolHK>();
    }
}
