/**********************************************************************
 * 
 * 文 件 名: ProtocolHKAI.cs
 * 创 建 人: 刘简
 * 创建时间: 2017-9-27
 * 描   述: 海康智能分析
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
    public class ProtocolHKAI : ProtocolDriver
    {

        public delegate void NETMSGPROC_AI(int command, int id, int nType);
        public static void cbNETMSGPROC_AI(int command, int id, int nType)
        {
            switch (command)
            {
                case 0x1102: //COMM_ALARM_RULE: //行为分析信息
                    {
                        switch (nType)
                        {
                            case 2:  //ENUM_VCA_EVENT_ENTER_AREA
                            case 4:  //ENUM_VCA_EVENT_INTRUSION
                                {
                                    Sensor sensor = gProtocol.FindSensorBySID(1, id);
                                    if (sensor != null)
                                    {
                                        STATUS_INFO info = new STATUS_INFO();
                                        info.id = sensor.id;
                                        info.name = sensor.name;
                                        info.status = STATUS_TYPE.ALARM;
                                        info.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        info.type = MONITOR_TYPE.camera;
                                        info.classification = CLASSIFICATION.human_recognition;
                                        info.confidence = 100;
                                        info.remark = "区域入侵";
                                        gProtocol.OnObjectStatusEH(info);
                                    }
                                }
                                break;
                            case 13: //ENUM_VCA_EVENT_LEFT
                                {
                                    Sensor sensor = gProtocol.FindSensorBySID(1, id);
                                    if (sensor != null)
                                    {
                                        STATUS_INFO info = new STATUS_INFO();
                                        info.id = sensor.id;
                                        info.name = sensor.name;
                                        info.status = STATUS_TYPE.ALARM;
                                        info.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        info.type = MONITOR_TYPE.camera;
                                        info.classification = CLASSIFICATION.goods_left;
                                        info.confidence = 100;
                                        info.remark = "物品遗留";
                                        gProtocol.OnObjectStatusEH(info);
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case 0x4521: //COMM_VCA_ALARM: //智能检测通用报警
                    {
                        //gProtocol.OnDebugEH(string.Format("智能检测通用报警, Json数据内容：{0}", msg));
                    }
                    break;
                default:
                    //gProtocol.OnDebugEH(string.Format("其他报警，报警信息类型：{0} {1}", command, msg));
                    break;
            }
        }

        private const string lib = @"dll\VIDEO\HKAI\AIAlarm.dll";//DLL\\HKAI\\AIAlarm.dll
        //const string lib = @"DLL\HKAI\AIAlarm.dll";//DLL\\HKAI\\AIAlarm.dll

        [DllImport(lib, EntryPoint = "Init")]
        private static extern bool Init();
        [DllImport(lib, EntryPoint = "Destroy")]
        private static extern void Destroy();
        [DllImport(lib, EntryPoint = "Connect")]
        private static extern int Connect(string server, UInt16 port, string name, string pwd, NETMSGPROC_AI pProc);
        [DllImport(lib, EntryPoint = "DisConnect")]
        private static extern void DisConnect();

        private static ProtocolHKAI gProtocol = null;

        public NETMSGPROC_AI CallBack = new NETMSGPROC_AI(cbNETMSGPROC_AI);

        public ProtocolHKAI(Device device)
            : base(device)
        {
            if (!Init())
            {
                OnConnectEH(device,2);
            }
            gProtocol = this;
        }
        ~ProtocolHKAI()
        {
            Destroy();
        }

        public override void Start()
        {
            if (!device.enable)
                return;

            int handle = Connect(device.ip, (UInt16)device.port, device.user, device.pwd, CallBack);
            if (handle != 0)
            {
                OnConnectEH(device, 1);
                return;
            }
            OnConnectEH(device, 0);
        }

        public override void Stop()
        {
            DisConnect();
            OnConnectEH(device, 1);
        }
    }
}
