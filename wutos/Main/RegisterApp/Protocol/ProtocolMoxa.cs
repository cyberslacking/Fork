/**********************************************************************
 * 
 * 文 件 名: ProtocolMoxa.cs
 * 创 建 人: 刘简
 * 创建时间: 2014-11-24
 * 描    述: MOXA，接收报警、故障信息 针对设备ioLogik I/O e2000
 * 
 * 修 改 人: 
 * 修改时间: 2018-5-2
 * 修改内容: 增加检测连接状态线程
 * 
 **********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading;

using APP.Common;
namespace APP.Protocol
{
    public class ProtocolMoxa : ProtocolDriver
    {
        [DllImport("DLL\\MOXA\\MXIO.dll", EntryPoint = "MXEIO_Connect")]
        private static extern int MXEIO_Connect(string ip, ushort port,int timeout,ref int handle);
        [DllImport("DLL\\MOXA\\MXIO.dll", EntryPoint = "MXEIO_CheckConnection")]
        private static extern int MXEIO_CheckConnection(int handle,int timeout,ref byte value);
        [DllImport("DLL\\MOXA\\MXIO.dll", EntryPoint = "MXEIO_Init")]
        private static extern int MXEIO_Init();
        [DllImport("DLL\\MOXA\\MXIO.dll", EntryPoint = "MXEIO_Disconnect")]
        private static extern int MXEIO_Disconnect(int handle);
        [DllImport("DLL\\MOXA\\MXIO.dll", EntryPoint = "MXEIO_Exit")]
        private static extern void MXEIO_Exit();

        [DllImport("DLL\\MOXA\\MXIO.dll", EntryPoint = "DI_Read")]
        private static extern int DI_Read(int handle,byte slot,byte channel,ref byte value);
        [DllImport("DLL\\MOXA\\MXIO.dll", EntryPoint = "DO_Write")]
        private static extern int DO_Write(int handle, byte slot, byte channel, byte value);
        [DllImport("DLL\\MOXA\\MXIO.dll", EntryPoint = "DO2K_SetModes")]
        private static extern int DO2K_SetModes(int handle, byte start, byte count, int[] value);

        private static object lc = new object();
        private Thread diRead;
        private Thread CheckConnect;

        private bool IsRun = false;

        private int handle = -1;
        private List<STATUS_INFO> listAlarm = new List<STATUS_INFO>();

        public ProtocolMoxa(Device device)
            : base(device)
        {
            ParameterizedThreadStart readStart = new ParameterizedThreadStart(ThreadMethod);

            diRead = new Thread(readStart);
            diRead.IsBackground = true;
            diRead.Start(this);

            ParameterizedThreadStart checkStart = new ParameterizedThreadStart(ThreadCheckConnection);

            CheckConnect = new Thread(checkStart);
            CheckConnect.IsBackground = true;
            CheckConnect.Start(this);
        }

        public override void Start()
        {
            if (!device.enable)
                return;
            if (MXEIO_Init() != 0)
            {
                OnConnectEH(device, 2);
            }
            else
            {
                if (MXEIO_Connect(device.ip, (UInt16)device.port, 500, ref handle) != 0)
                {
                    OnConnectEH(device, 1);
                }
                else
                {
                    OnConnectEH(device, 0);
                }
                IsRun = true;
            }
        }

        public override void Stop()
        {
            if (!device.enable )
                return;
            if (handle != -1)
            {
                MXEIO_Disconnect(handle);
                handle = -1;
            }
            IsRun = false;
            OnConnectEH(device, 1);
            MXEIO_Exit();        
        }

        /// <summary>
        /// 输出信号控制 0,1 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        public override void DoOutput(byte DO, byte flag)
        {
            if (!device.enable )
                return;

            if (DO > 7)
                return;

            if (handle == -1)
                return;
           
            lock (lc)
            {
                int[] wSetModes = { 0, 0, 0, 0, 0, 0, 0, 0 };
                //set ch0 ~ ch7 DO modes
                DO2K_SetModes(handle,			//the handle for a connection 
                                        0,				//starting channel
                                        8,				//channel count
                                        wSetModes);
                DO_Write(handle, 0, DO, flag);
            }
        }

        private static void ThreadCheckConnection(object o)
        {
            ProtocolMoxa moxa = (ProtocolMoxa)o;
            Device device = moxa.device;
            while (true)
            {
                Thread.Sleep(3000);
                if (!device.enable)
                    continue;
                if (!moxa.IsRun)
                {
                    continue;
                }
                lock (lc)
                {
                    byte value = 100;
                    if (MXEIO_CheckConnection(moxa.handle, 300, ref value) == 0)
                    {
                        if (value != 0)
                        {
                            MXEIO_Connect(device.ip, (UInt16)device.port, 300, ref moxa.handle);
                            /*
                            if (MXEIO_Connect(device.ip, (UInt16)device.port, 300, ref moxa.handle) != 0)
                            {
                                moxa.OnConnectEH(device, 1);
                            }
                            else
                            {
                                moxa.OnConnectEH(device, 0);
                            }
                            */
                        }
                    }
                }
            }
        }

        private static void ThreadMethod(object o)
        {
            ProtocolMoxa moxa = (ProtocolMoxa)o;
            Device device = moxa.device;

            while (true)
            {
                Thread.Sleep(1000);
                if (!device.enable)
                    continue;               
                if (moxa.handle == -1)
                    continue;
                lock (lc)
                {          
                    foreach (var group in device.listChannel)
                    {
                        if (group.number != 1)
                            continue;
                        foreach (var sensor in group.listSensor)
                        {
                            byte value = 100;
                            byte channel = (byte)sensor.number;
                            if (DI_Read(moxa.handle, 1, channel, ref value) == 0)
                            {
                                if (value == 0)
                                {
                                    STATUS_INFO old = moxa.listAlarm.Find(x => x.id == sensor.id);
                                    if (old != null)
                                        continue;
                                    STATUS_INFO info = new STATUS_INFO();
                                    info.id = sensor.id;
                                    info.name = sensor.name;
                                    info.status = STATUS_TYPE.ALARM;
                                    info.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    info.type = MONITOR_TYPE.io;
                                    info.confidence = 100;
                                    moxa.listAlarm.Add(info);
                                    moxa.OnObjectStatusEH(info);
                                }
                                else
                                {
                                    STATUS_INFO old = moxa.listAlarm.Find(x => x.id == sensor.id);
                                    if (old != null)
                                    {
                                        moxa.listAlarm.Remove(old);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
