using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Security.Permissions;

using ProtoBuf;

namespace APP.Common
{
    [ProtoContract]
    public class ObjectConfig
    {
        [ProtoMember(1)]
        public CfgVoice voiceCfg = null;
        [ProtoMember(2)]
        public CfgOrganize organizeCfg = null;
        [ProtoMember(3)]
        public CfgUser userCfg = null;
        [ProtoMember(4)]
        public MonitorDevice device = null;
    }


    [ProtoContract]
    public class CfgVoice
    {
        [ProtoMember(1)]
        public string name = "语音";
        [ProtoMember(2)]
        public string filePath = AppDomain.CurrentDomain.BaseDirectory + @"alarm.wav";
        [ProtoMember(3)]
        public bool file = true;
        [ProtoMember(4)]
        public string speakLang = "Language = 804";
        [ProtoMember(5)]
        public bool speak = false;
    }

    [ProtoContract]
    public class MonitorDevice
    {
        [ProtoMember(1)]
        public string name = "监测设备";
        [ProtoMember(2)]
        public List<Cfg> listCfg = new List<Cfg>();
    }


    #region 设备配置
    [ProtoContract,ProtoInclude(100, typeof(CfgFBG))]
    [ProtoInclude(101, typeof(CfgDVS))]
    [ProtoInclude(102, typeof(CfgDTS))]
    [ProtoInclude(103, typeof(CfgHK))]
    [ProtoInclude(104, typeof(CfgMoxa2k))]
    [ProtoInclude(105, typeof(CfgJdex))]
    [ProtoInclude(106, typeof(CfgVideo))]
    [ProtoInclude(107, typeof(CfgInanter))]
    [ProtoInclude(108, typeof(CfgHKAI))]
    [ProtoInclude(109, typeof(CfgDHAlarm))]
    public class Cfg
    {
        [ProtoMember(1)]
        public long id;
        [ProtoMember(2)]
        public string name = "";
        [ProtoMember(3)]
        public string mark = "";
        [ProtoMember(4)]
        public bool enable = false;
        [ProtoMember(5)]
        public List<Device> listDev = new List<Device>();
        public Cfg()
        {
            id = Function.GuidToInt64();
        }
    }

    [ProtoContract]
    public class CfgVideo : Cfg
    {
        [ProtoMember(6)]
        public int winWigth = 800;
        [ProtoMember(7)]
        public int winHeight = 600;

        public CfgVideo()
        {
            name = "视频设备";
            mark = "video";
        }
    }

    [ProtoContract]
    public class CfgFBG : Cfg
    {
        public CfgFBG()
        {
            name = "光栅报警器";
            mark = "FBG";
        }
    }

    [ProtoContract]
    public class CfgDVS : Cfg
    {
        public CfgDVS()
        {
            name = "分布式测震主机";
            mark = "DVS";
        }
    }

    [ProtoContract]
    public class CfgDTS : Cfg
    {
        public CfgDTS()
        {
            name = "分布式测温主机";
            mark = "DTS";
        }
    }

    [ProtoContract]
    public class CfgInanter : Cfg
    {
        public CfgInanter()
        {
            name = "英安特报警主机";
            mark = "INANTER";
        }
    }

    [ProtoContract]
    public class CfgMoxa2k : Cfg
    {
        public CfgMoxa2k()
        {
            name = "MOXA-2K";
            mark = "moxa2k";
        }
    }

    [ProtoContract]
    public class CfgJdex : Cfg
    {
        public CfgJdex()
        {
            name = "继电扩展器";
            mark = "jdex";
        }
    }

    [ProtoContract]
    public class CfgHK : Cfg
    {
        public CfgHK()
        {
            name = "海康报警模块";
            mark = "HK";
        }
    }

    [ProtoContract]
    public class CfgHKAI : Cfg
    {
        public CfgHKAI()
        {
            name = "海康智能分析";
            mark = "HKAI";
        }
    }

    [ProtoContract]
    public class CfgDHAlarm: Cfg
    {
        public CfgDHAlarm()
        {
            name = "大华报警主机";
            mark = "DHAlarm";
        }
    }
    #endregion

    #region 设备属性
    [ProtoContract]
    [ProtoInclude(200, typeof(DevVideo))]
    [ProtoInclude(201, typeof(DevFBG))]
    [ProtoInclude(202, typeof(DevDVS))]
    [ProtoInclude(203, typeof(DevDTS))]
    [ProtoInclude(204, typeof(DevHK))]
    [ProtoInclude(205, typeof(DevMoxa))]
    [ProtoInclude(206, typeof(DevJdex))]
    [ProtoInclude(207, typeof(DevInanter))]
    [ProtoInclude(208, typeof(DevHKAI))]
    [ProtoInclude(209, typeof(DevDHAlarm))]
    public class Device
    {
        [ProtoMember(1)]
        public long id;
        [ProtoMember(2)]
        public string name = "device";
        [ProtoMember(3)]
        public int port = 8000;
        [ProtoMember(4)]
        public string ip = "172.16.99.8";
        [ProtoMember(5)]
        public string user = "admin";
        [ProtoMember(6)]
        public string pwd = "12345";
        [ProtoMember(7)]
        public string version = "";
        [ProtoMember(8)]
        public int type;
        [ProtoMember(9)]
        public DEVICE_KEY key;
        [ProtoMember(10)]
        public bool enable = false;
        [ProtoMember(11)]
        public bool modify = false;
        [ProtoMember(12)]
        public List<Channel> listChannel = new List<Channel>();

        public Device()
        {
            id = Function.GuidToInt64();
        }
    }

    [ProtoContract]
    public class DevVideo : Device
    {
        public DevVideo()
        {
            key = DEVICE_KEY.VIDEO;
            type = (int)VIDEO_TYPE.HK;
        }
    }

    [ProtoContract]
    public class DevFBG : Device
    {
        public DevFBG()
        {
            key = DEVICE_KEY.FBG;
        }
    }

    [ProtoContract]
    public class DevDVS : Device
    {
        public DevDVS()
        {
            key = DEVICE_KEY.DVS;
        }
    }

    [ProtoContract]
    public class DevDTS : Device
    {
        public DevDTS()
        {
            key = DEVICE_KEY.DTS;
        }
    }

    [ProtoContract]
    public class DevInanter : Device
    {
        public DevInanter()
        {
            key = DEVICE_KEY.INANTER;
        }
    }

    [ProtoContract]
    public class DevHK : Device
    {
        public DevHK()
        {
            key = DEVICE_KEY.HK;
            port = 4001;
            name = "HK Module";
        }
    }

    [ProtoContract]
    public class DevHKAI : Device
    {
        public DevHKAI()
        {
            key = DEVICE_KEY.HKAI;
            port = 8000;
            name = "HK AI";
        }
    }

    [ProtoContract]
    public class DevDHAlarm : Device
    {
        public DevDHAlarm()
        {
            key = DEVICE_KEY.DHAlarm;
            ip = "192.168.1.108";
            pwd = "admin";
            port = 37777;
            name = "DHAlarm";
        }
    }

    [ProtoContract]
    public class DevMoxa : Device
    {
        public DevMoxa()
        {
            key = DEVICE_KEY.MOXA_2K;
        }
    }

    [ProtoContract]
    public class DevJdex : Device
    {
        public DevJdex()
        {
            key = DEVICE_KEY.JDEX;
        }
    }

    #endregion

    #region 通道属性
    [ProtoContract]
    public class Channel
    {
        [ProtoMember(1)]
        public long id;
        [ProtoMember(2)]
        public long devID;
        [ProtoMember(3)]
        public int number;
        [ProtoMember(4)]
        public string name = "0";
        [ProtoMember(5)]
        public List<Sensor> listSensor = new List<Sensor>();

        public Channel()
        {
            id = Function.GuidToInt64();
        }
    }
    #endregion

    #region 传感器属性
    [ProtoContract]
    public class Sensor
    {
        [ProtoMember(1)]
        public long id;
        [ProtoMember(2)]
        public long devID;
        [ProtoMember(3)]
        public long channelID;
        [ProtoMember(4)]
        public string organize;
        [ProtoMember(5)]
        public int number;
        [ProtoMember(6)]
        public string name = "0";
        [ProtoMember(7)]
        public string remark = "0";
        [ProtoMember(8)]
        public int type;
        [ProtoMember(9)]
        public STATUS_TYPE status = STATUS_TYPE.NORMAL;
        [ProtoMember(10)]
        public bool aid = false;
        [ProtoMember(11)]
        public int timeout = 10;//延时秒
        [ProtoMember(12)]
        public List<PLAN_DATE> listPlanDate = new List<PLAN_DATE>();
        [ProtoMember(13)]
        public List<Sensor> listSensor = new List<Sensor>();
        [ProtoMember(14)]
        public List<VideoLink> listLink = new List<VideoLink>();
        public Sensor()
        {
            id = Function.GuidToInt64();
        }
    }

    [ProtoContract]
    public class VideoLink
    {
        [ProtoMember(1)]
        public long id;
        [ProtoMember(2)]
        public int number; //预置位ID
        [ProtoMember(3)]
        public string name = "0";
        [ProtoMember(4)]
        public long device;
        [ProtoMember(5)]
        public long camera; //摄像头ID
        public VideoLink()
        {
            id = Function.GuidToInt64();
        }
    }
    #endregion

    #region 组
    [ProtoContract]
    public class CfgOrganize
    {
        [ProtoMember(1)]
        public string name = "组配置";
        [ProtoMember(2)]
        public List<Organize> listOrganize = new List<Organize>();
    }

    [ProtoContract]
    public class Organize
    {
        [ProtoMember(1)]
        public long id;
        [ProtoMember(2)]
        public string name = "Default organize";
        [ProtoMember(3)]
        public string type = "Default";

        public Organize()
        {
            id = Function.GuidToInt64();
        }
    }

    #endregion

    #region 用户
    [ProtoContract]
    public class CfgUser
    {
        [ProtoMember(1)]
        public string name = "用户配置";
        [ProtoMember(2)]
        public List<User> listUser = new List<User>();
    }

    [ProtoContract]
    public class User
    {
        [ProtoMember(1)]
        public long id;
        [ProtoMember(2)]
        public string name;
        [ProtoMember(3)]
        public string password;
        [ProtoMember(4)]
        public string remark = "Default user";
        [ProtoMember(5)]
        public int type;
        [ProtoMember(6)]
        public List<Organize> listOrganize = new List<Organize>();
        public User()
        {
            id = Function.GuidToInt64();
        }
    }
    #endregion

}
