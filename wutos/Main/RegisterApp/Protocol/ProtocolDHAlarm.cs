/**********************************************************************
 * 
 * 文 件 名: ProtocolDHAlarm.cs
 * 创 建 人: 刘简
 * 创建时间: 2018-3-7
 * 描    述: 大华报警主机，接收报警信息
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
    public class ProtocolDHAlarm : ProtocolDriver
    {
        // 总线类型
        public enum NET_BUS_TYPE
        {
            NET_BUS_TYPE_UNKNOWN = 0,
            NET_BUS_TYPE_MBUS,                                      // M-BUS总线
            NET_BUS_TYPE_RS485,                                     // RS-485总线
        };

        public struct ALARM_MODULE_LOST_INFO
        {
            public uint dwSize;
            public NET_TIME stuTime;                        // 事件上报时间
            public int nSequence;                      // 扩展模块接的总线的序号(从0开始)
            public NET_BUS_TYPE emBusType;                      // 总线类型
            public int nAddr;                          // 掉线的扩展模块数目
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public int[] anAddr;// 掉线的扩展模块的序号(从0开始)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szDevType;	// 设备类型 "SmartLock",是级联设备,当设备类型"AlarmDefence"接口序号为报警序号
            public bool bOnline;						//在线情况   默认false。   false   不在线   true 在线
        };
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_TIME
        {
            uint dwYear;                  // 年
            uint dwMonth;                 // 月
            uint dwDay;                   // 日
            uint dwHour;                  // 时
            uint dwMinute;                // 分
            uint dwSecond;                // 秒
        };

        // 报警输入源事件详情(只要有输入就会产生改事件,不论防区当前的模式,无法屏蔽)
        public struct ALARM_INPUT_SOURCE_SIGNAL_INFO
        {
            public uint dwSize;
            public int nChannelID;                         // 通道号
            public int nAction;                            // 0:开始 1:停止
            public NET_TIME stuTime;                            // 报警事件发生的时间
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DEVICEINFO_EX
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            public byte[] sSerialNumber;
            public int byAlarmInPortNum;
            public int byAlarmOutPortNum;
            public int byDiskNum;
            public int byDVRType;
            public int byChanNum;
            public byte byLimitLoginTime;
            public byte byLeftLogTimes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] bReserved;
            public int nLockLeftTime;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public string Reserved;
        };

        public delegate void DisConnectFunc(int lLoginID, string pchDVRIP, int nDVRPort, IntPtr dwUser);
        public delegate void AutoReconnect(int lLoginID, string pchDVRIP, int nDVRPort, IntPtr dwUser);
        public delegate bool MessageCallBack(int lCommand, int lLoginID, IntPtr pBuf,
                           uint dwBufLen, string pchDVRIP, int nDVRPort, IntPtr dwUser);

        public static void cbAutoReconnect(int lLoginID, string pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            ProtocolDHAlarm instrument = listInstrument.Find(x => x.device.ip == pchDVRIP);
            if (instrument != null)
            {
                CLIENT_StartListenEx(lLoginID);
                instrument.OnConnectEH(instrument.device, 0);
            }
        }

        public static void cbDisConnectFunc(int lLoginID, string pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            ProtocolDHAlarm instrument = listInstrument.Find(x => x.device.ip == pchDVRIP);
            if (instrument != null)
            {
                CLIENT_StopListen(lLoginID);
                instrument.OnConnectEH(instrument.device, 1);
            }
        }

        public static bool cbMessageCallBack(int lCommand, int lLoginID, IntPtr pBuf,
                           uint dwBufLen, string pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            ProtocolDHAlarm instrument = listInstrument.Find(x => x.device.ip == pchDVRIP);
            if (instrument == null)
                return true;
            switch (lCommand)
            {
                case 0x2175: //DH_ALARM_ALARM_EX2
                    break;
                case 0x3183:              
                    ALARM_INPUT_SOURCE_SIGNAL_INFO alarmInput = (ALARM_INPUT_SOURCE_SIGNAL_INFO)Marshal.PtrToStructure(pBuf, typeof(ALARM_INPUT_SOURCE_SIGNAL_INFO));
                    Sensor sensor = instrument.FindSensorBySID(1, alarmInput.nChannelID);
                    if (sensor != null)
                    {
                        if (alarmInput.nAction == 0)
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
                        else if (alarmInput.nAction == 1)
                        {
                            STATUS_INFO old = instrument.listAlarm.Find(x => x.id == sensor.id);
                            if (old != null)
                            {
                                instrument.listAlarm.Remove(old);
                            }
                        }
                    }                 
                    break;
                /*
                case 0x3195:
                    ALARM_MODULE_LOST_INFO lostInfo = (ALARM_MODULE_LOST_INFO)Marshal.PtrToStructure(pBuf, typeof(ALARM_MODULE_LOST_INFO));
                    for (int i = 0; i < lostInfo.nAddr; i++)
                    {
                        Sensor sensorError = instrument.FindSensorBySID(1, lostInfo.anAddr[i]);
                        if (sensorError != null)
                        {
                            if (lostInfo.bOnline)
                            {
                                ERROR_INFO old = instrument.listError.Find(x => x.id == sensorError.id);
                                if (old != null)
                                {
                                    ERROR_INFO info = new ERROR_INFO();
                                    info.id = sensorError.id;
                                    info.name = sensorError.name;
                                    info.status = STATUS_TYPE.NORMAL;
                                    info.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    info.type = OBJECT_TYPE.sensor;
                                    instrument.OnObjectErrorEH(info);
                                    instrument.listError.Remove(old);
                                }
                            }
                            else
                            {
                                ERROR_INFO old = instrument.listError.Find(x => x.id == sensorError.id);
                                if (old == null)
                                {
                                    ERROR_INFO info = new ERROR_INFO();
                                    info.id = sensorError.id;
                                    info.name = sensorError.name;
                                    info.status = STATUS_TYPE.ERROR;
                                    info.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    info.type = OBJECT_TYPE.sensor;
                                    instrument.OnObjectErrorEH(info);
                                    instrument.listError.Add(info);
                                }
                            }
                        }
                    }
                    break;
                 */
            }

            return true;

        }

        const string file = "DLL\\DHAlarm\\dhnetsdk.dll";

        [DllImport(file)]
        private static extern bool CLIENT_Init(DisConnectFunc cb, IntPtr dwUser);
        [DllImport(file)]
        private static extern void CLIENT_SetDVRMessCallBack(MessageCallBack cb, IntPtr dwUser);
        [DllImport(file)]
        private static extern void CLIENT_SetAutoReconnect(AutoReconnect cb, uint dwUser);
        [DllImport(file)]
        private static extern int CLIENT_LoginEx2(string pchDVRIP, UInt16 wDVRPort, string pchUserName, string pchPassword, int mode, IntPtr param, ref NET_DEVICEINFO_EX lpDeviceInfo, IntPtr error);
        [DllImport(file)]
        private static extern bool CLIENT_StartListenEx(long handle);
        [DllImport(file)]
        private static extern bool CLIENT_StopListen(long handle);
        [DllImport(file)]
        private static extern bool CLIENT_Logout(long handle);
        [DllImport(file)]
        private static extern bool CLIENT_Cleanup();

        DisConnectFunc cbDisConnect = new DisConnectFunc(cbDisConnectFunc);
        MessageCallBack cbMessage = new MessageCallBack(cbMessageCallBack);
        AutoReconnect cbAuto = new AutoReconnect(cbAutoReconnect);

        private int lLoginHandle = 0;
        private List<STATUS_INFO> listAlarm = new List<STATUS_INFO>();
        private List<ERROR_INFO> listError = new List<ERROR_INFO>();

        public ProtocolDHAlarm(Device device)
            : base(device)
        {
            CLIENT_Init(cbDisConnect, (IntPtr)0);
            //设置报警回调函数
            CLIENT_SetDVRMessCallBack(cbMessage, (IntPtr)0);

            CLIENT_SetAutoReconnect(cbAuto, 0); 
        }

        public override void Start()
        { 
            if (!device.enable)
                return;
            IntPtr error = new IntPtr();
            IntPtr param = new IntPtr();
            NET_DEVICEINFO_EX stDevInfo = new NET_DEVICEINFO_EX();
            lLoginHandle = CLIENT_LoginEx2(device.ip, (UInt16)device.port, device.user, device.pwd, 0, param, ref stDevInfo, error);
            if (lLoginHandle != 0)
            {
                if (CLIENT_StartListenEx(lLoginHandle))
                {
                    OnConnectEH(device, 0);
                }
            }
            else
            {
                OnConnectEH(device, 1);
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
            if (lLoginHandle != 0)
            {
                CLIENT_StopListen(lLoginHandle);
                CLIENT_Logout(lLoginHandle);
            }
            OnConnectEH(device, 1);
        }

        public override void DoOutput(byte DO,byte flag)
        {
        }

        public static List<ProtocolDHAlarm> listInstrument = new List<ProtocolDHAlarm>();
    }
}
