using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

using APP.Common;

using DelegateEvent;

namespace APP.Protocol
{
    /// <summary>
    /// 事件托管
    /// </summary>
    public delegate void ConfigResEH(Device info);
    public delegate void StrategyEH(string id, string level, string result);
    public delegate void ConnectEH(Device info, int type);
    public delegate void ObjectStatusEH(STATUS_INFO info);
    public delegate void ObjectErrorEH(ERROR_INFO info);
    public delegate void DebugEH(string msg);

    public abstract class  ProtocolDriver : ModelBase
    {
        #region 变量
        public int instance = -1;
        public bool connected;
        #endregion

        #region 事件变量
        public event ConfigResEH OnConfigRes;
        public event StrategyEH OnStrategy;
        public event ConnectEH OnConnect;
        public event ObjectStatusEH OnObjectStatus;
        public event ObjectErrorEH OnObjectError;
        public event DebugEH OnDebug;
        #endregion

        #region 构造
        public ProtocolDriver() 
        {
            connected = false;
        }
        #endregion

        #region 抽象方法
        public abstract void Start(); //服务启动
        public abstract void Stop();  //服务停止
        public abstract void RegisterDevice(Device d);
        public abstract void UnRegisterDevice(long id);
        #endregion

        public virtual void DoStrategy(string id, string value)
        {

        }
        public virtual void DoOutput(byte DO, byte flag)
        {

        }

        public virtual bool GetServerTime(IntPtr time)
        {
            return false;
        }

        public virtual bool SetServerTime(IntPtr time)
        {
            return false;
        }

        #region 公用事件
        public virtual void OnConnectEH(Device info, int type)
        {
            connected = type == 0;
            ConnectEH handler = OnConnect;
            if (handler != null)
            {
                handler(info,type);
            }
        }

        public virtual void OnConfigResEH(Device info)
        {
            ConfigResEH handler = OnConfigRes;
            if (handler != null)
            {
                handler(info);
            }
        }

        public virtual void OnObjectStatusEH(STATUS_INFO info)
        {
            ObjectStatusEH handler = OnObjectStatus;
            if (handler != null)
            {
                handler(info);
            }
        }

        public virtual void OnObjectErrorEH(ERROR_INFO info)
        {
            ObjectErrorEH handler = OnObjectError;
            if (handler != null)
            {
                handler(info);
            }
        }

        public virtual void OnDebugEH(string msg)
        {
            DebugEH handler = OnDebug;
            if (handler != null)
            {
                handler(msg);
            }
        }

        public virtual void OnStrategyEH(string ip, string id, string level, string result)
        {
            StrategyEH handler = OnStrategy;
            if (handler != null)
            {
                handler(id, level,result);
            }
        }

        #endregion

        protected Sensor FindSensorBySID(int channelNum, int sensorNum)
        {
            Channel channel = device.listChannel.Find(x => x.number == channelNum);
            if(channel != null)
            {
                return channel.listSensor.Find(x => x.number == sensorNum);
            }
            return null;
        }

        public Sensor FindSensorByID(int id)
        {
            Sensor sensor = null;
            foreach (var channel in device.listChannel)
            {
                sensor = channel.listSensor.Find(x => x.id == id);
                if (sensor != null)
                {
                    return sensor;
                }
            }
            return sensor;
        }
    }
}
