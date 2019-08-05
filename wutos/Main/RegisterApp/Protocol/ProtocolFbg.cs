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
    #region tool
    using HCLIENT = System.Int32;
    using ERRORCODE = System.Int32;
    using DWORD = System.UInt32;
    using DATE = System.Double;
    class StringSize
    {
        public const int Short = 32;
        public const int Middle = 128;
        public const int Long = 512;
    };

    enum OIASEventClass
    {
        undef = 0,
        alarm = 1,
        fault = 2,
        faultRestore = 3,
        online = 4,
        offline = 5
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class LOGIN_INFO
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = StringSize.Short)]
        public String hostIP;
        public int hostPort;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = StringSize.Short)]
        public String userName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = StringSize.Short)]
        public String password;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    class NOTIFY_INFO
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = StringSize.Short)]
        public String objectType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = StringSize.Short)]
        public String objectId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = StringSize.Middle)]
        public String objectName;
        public DATE foundTime;
        public int eventClass;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = StringSize.Middle)]
        public string eventName;
        public DWORD eventData;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = StringSize.Long)]
        public String description;
    };//end NOTIFY_INFO

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class STRATEGY_INFO
    {
        public int id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = StringSize.Middle)]
        public String name;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class OBJECT_LIST
    {
        public int count;
        public IntPtr items;
    };

    class OIASClient : IDisposable
    {
        const string file = "dll\\FBGAlarm\\OIAS.NetSDK.dll";
        #region SDK Functions
        [DllImport(file)]
        public static extern ERRORCODE OIAS_GetSDKVersion();

        [DllImport(file)]
        public static extern ERRORCODE OIAS_Init();

        [DllImport(file)]
        public static extern ERRORCODE OIAS_Release();

        [DllImport(file)]
        public static extern HCLIENT OIAS_CreateClient(DWORD dwReseve);

        [DllImport(file)]
        public static extern ERRORCODE OIAS_DeleteClient(HCLIENT hClient);

        public delegate System.Int32 OIAS_NotifyCallBack(HCLIENT hClient, NOTIFY_INFO notifyInfo);
        [DllImport(file)]
        public static extern ERRORCODE OIAS_SetNotifyRecvProc(HCLIENT hClient, OIAS_NotifyCallBack notifyCB);

        [DllImport(file)]
        public static extern ERRORCODE OIAS_SetLoginInfo(HCLIENT hClient, LOGIN_INFO loginInfo);

        [DllImport(file)]
        public static extern ERRORCODE OIAS_Login(HCLIENT hClient);

        [DllImport(file)]
        public static extern ERRORCODE OIAS_Logout(HCLIENT hClient);

        [DllImport(file)]
        public static extern ERRORCODE OIAS_ListStrategy(HCLIENT hClient, OBJECT_LIST strategyList);


        [DllImport(file)]
        public static extern ERRORCODE OIAS_ListFault(HCLIENT hClient, OBJECT_LIST faultList);


        #endregion// SDK Functions end////////////////////////////////////
        protected static bool SDKInited = false;
        protected static List<OIASClient> RegClients = new List<OIASClient>();

        public static OIASClient FindClient(HCLIENT hClient)
        {
            int count = RegClients.Count;
            for (int i = 0; i < count; i++)
                if (RegClients[i].hClient == hClient)
                    return RegClients[i];
            return null;
        }

        static void PrintNotify(NOTIFY_INFO notifyInfo)
        {
        }

        protected static int OIASNotifyCB(HCLIENT hClient, NOTIFY_INFO notifyInfo)
        {
            OIASClient clientObj = FindClient(hClient);
            if (clientObj == null)
            {
                PrintNotify(notifyInfo);
                return 0;
            }
            return clientObj.OnOIASNotify(notifyInfo);
        }



        #region DateUtils
        struct tm
        {
            public int tm_year;
            public int tm_mon;
            public int tm_yday;
            public int tm_mday;
            public int tm_wday;
            public int tm_hour;
            public int tm_min;//minute
            public int tm_sec;//second
            public int tm_msec;//millisecond
        }

        static bool OleDateDecode(DATE dtSrc, ref tm tmDest, bool bRoundSecond = true)
        {
            const int MIN_DATE = -657434, MAX_DATE = 2958465;
            const double HALF_SECOND = 1.0 / 172800.0;
            // The legal range does not actually span year 0 to 9999.
            if (dtSrc > MAX_DATE || dtSrc < MIN_DATE) // about year 100 to about 9999
                return false;
            int[] _afxMonthDays = new int[13] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
            int nDays;             // Number of days since Dec. 30, 1899
            int nDaysAbsolute;     // Number of days since 1/1/0
            int nSecsInDay;        // Time in seconds since midnight
            int nMinutesInDay;     // Minutes in day

            int n400Years;         // Number of 400 year increments since 1/1/0
            int n400Century;       // Century within 400 year block (0,1,2 or 3)
            int n4Years;           // Number of 4 year increments since 1/1/0
            int n4Day;             // Day within 4 year block
            //  (0 is 1/1/yr1, 1460 is 12/31/yr4)
            int n4Yr;              // Year within 4 year block (0,1,2 or 3)
            bool bLeap4 = true;     // TRUE if 4 year block includes leap year

            double dblDate = dtSrc; // tempory serial date

            // If a valid date, then this conversion should not overflow
            nDays = (int)dblDate;

            // Round to the second
            if (bRoundSecond)
                dblDate += ((dtSrc > 0.0) ? HALF_SECOND : -HALF_SECOND);

            nDaysAbsolute = (int)dblDate + 693959; // Add days from 1/1/0 to 12/30/1899

            dblDate = Math.Abs(dblDate);
            int nMSecsInDay = (int)((dblDate - Math.Floor(dblDate)) * 86400000);
            nSecsInDay = nMSecsInDay / 1000;
            nMSecsInDay = nMSecsInDay % 1000;
            tmDest.tm_msec = nMSecsInDay;

            // Calculate the day of week (sun=1, mon=2...)
            //   -1 because 1/1/0 is Sat.  +1 because we want 1-based
            tmDest.tm_wday = (int)((nDaysAbsolute - 1) % 7L) + 1;

            // Leap years every 4 yrs except centuries not multiples of 400.
            n400Years = (int)(nDaysAbsolute / 146097L);

            // Set nDaysAbsolute to day within 400-year block
            nDaysAbsolute %= 146097;

            // -1 because first century has extra day
            n400Century = (int)((nDaysAbsolute - 1) / 36524);

            // Non-leap century
            if (n400Century != 0)
            {
                // Set nDaysAbsolute to day within century
                nDaysAbsolute = (nDaysAbsolute - 1) % 36524;

                // +1 because 1st 4 year increment has 1460 days
                n4Years = (int)((nDaysAbsolute + 1) / 1461);

                if (n4Years != 0)
                    n4Day = (int)((nDaysAbsolute + 1) % 1461);
                else
                {
                    bLeap4 = false;
                    n4Day = (int)nDaysAbsolute;
                }
            }
            else
            {
                // Leap century - not special case!
                n4Years = (int)(nDaysAbsolute / 1461L);
                n4Day = (int)(nDaysAbsolute % 1461L);
            }

            if (bLeap4)
            {
                // -1 because first year has 366 days
                n4Yr = (n4Day - 1) / 365;

                if (n4Yr != 0)
                    n4Day = (n4Day - 1) % 365;
            }
            else
            {
                n4Yr = n4Day / 365;
                n4Day %= 365;
            }

            // n4Day is now 0-based day of year. Save 1-based day of year, year number
            tmDest.tm_yday = (int)n4Day + 1;
            tmDest.tm_year = n400Years * 400 + n400Century * 100 + n4Years * 4 + n4Yr;

            // Handle leap year: before, on, and after Feb. 29.
            if (n4Yr == 0 && bLeap4)
            {
                // Leap Year
                if (n4Day == 59)
                {
                    /* Feb. 29 */
                    tmDest.tm_mon = 2;
                    tmDest.tm_mday = 29;
                    goto DoTime;
                }

                // Pretend it's not a leap year for month/day comp.
                if (n4Day >= 60)
                    --n4Day;
            }

            // Make n4DaY a 1-based day of non-leap year and compute
            //  month/day for everything but Feb. 29.
            ++n4Day;

            // Month number always >= n/32, so save some loop time */
            for (tmDest.tm_mon = (n4Day >> 5) + 1;
                n4Day > _afxMonthDays[tmDest.tm_mon]; tmDest.tm_mon++) ;

            tmDest.tm_mday = (int)(n4Day - _afxMonthDays[tmDest.tm_mon - 1]);

        DoTime:
            if (nSecsInDay == 0)
                tmDest.tm_hour = tmDest.tm_min = tmDest.tm_sec = 0;
            else
            {
                tmDest.tm_sec = (int)nSecsInDay % 60;
                nMinutesInDay = nSecsInDay / 60;
                tmDest.tm_min = (int)nMinutesInDay % 60;
                tmDest.tm_hour = (int)nMinutesInDay / 60;
            }
            return true;
        }

        public static DateTime OleDateDecode(DATE dtSrc)
        {
            tm tmDest = new tm();
            if (!OleDateDecode(dtSrc, ref tmDest, false))
                return new DateTime(1899, 12, 30);//0
            else if (dtSrc > 0.000001)
            {
                DateTime dtDest = new DateTime(tmDest.tm_year, tmDest.tm_mon, tmDest.tm_mday, tmDest.tm_hour, tmDest.tm_min, tmDest.tm_sec, tmDest.tm_msec);
                return dtDest;
            }
            else
            {
                DateTime dtDest = DateTime.Now;
                return dtDest;
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////        
        void Create()
        {
            if (hClient != 0)
                return;
            if (!SDKInited)
                SDKInited = OIAS_Init() == 0;
            hClient = SDKInited ? OIAS_CreateClient(0) : 0;
            if (hClient != 0)
            {
                RegClients.Add(this);
                if (notifyCB == null)
                    notifyCB = new OIAS_NotifyCallBack(OIASNotifyCB);
                OIAS_SetNotifyRecvProc(hClient, notifyCB);
            }
        }

        void Destroy()
        {
            if (hClient != 0)
            {
                OIAS_DeleteClient(hClient);
                hClient = 0;
                int index = RegClients.IndexOf(this);
                if (index >= 0)
                    RegClients.RemoveAt(index);
            }
        }

        protected bool hasLogin = false;
        protected bool wishLogin = false;//希望登录
        protected bool hasSessiontionFault = false;
        protected HCLIENT hClient;
        OIAS_NotifyCallBack notifyCB;
        public delegate int OIASNotifyEventHandler(OIASClient sender, NOTIFY_INFO notifyInfo);

        public ERRORCODE SetLoginInfo(LOGIN_INFO loginInfo)
        {
            ERRORCODE nErrorCode = OIAS_SetLoginInfo(hClient, loginInfo);;
            return nErrorCode;
        }

        public ERRORCODE Login()
        {
            ERRORCODE result = OIAS_Login(hClient);
            hasLogin = result == 0;
            wishLogin = true;
            hasSessiontionFault = result != 0;
            return result;
        }

        public ERRORCODE Logout()
        {
            ERRORCODE result = OIAS_Logout(hClient);
            wishLogin = false;
            hasLogin = false;
            hasSessiontionFault = false;
            return result;
        }

        public ERRORCODE ListFault(OBJECT_LIST faultList)
        {
            ERRORCODE result = OIAS_ListFault(hClient, faultList);
            wishLogin = false;
            hasLogin = false;
            hasSessiontionFault = false;
            return result;
        }

        public OIASClient()
        {
            Create();
        }

        public void Dispose()
        {
            Destroy();
        }

        ~OIASClient()
        {
            Dispose();
        }

        public event OIASNotifyEventHandler Notify;

        protected virtual int OnOIASNotify(NOTIFY_INFO notifyInfo)
        {
            if (notifyInfo.objectType == "client" && wishLogin)
            {
                if (notifyInfo.eventName == "loginFault.restore" || notifyInfo.eventName == "connectBreak.restore")
                {
                    hasSessiontionFault = false;
                    hasLogin = true;
                }
                else if (notifyInfo.eventName == "loginFault" || notifyInfo.eventName == "connectBreak")
                {
                    hasSessiontionFault = true;
                    hasLogin = false;
                }
            }
            if (Notify != null)
                return Notify(this, notifyInfo);
            else
            {
                PrintNotify(notifyInfo);
                return 0;
            }
        }

        public bool HasLogin
        {
            get { return hasLogin; }
        }

        public bool HasSessionFault { get { return hasSessiontionFault; } }

    }
    #endregion

    public class ProtocolFbg : ProtocolDriver
    {
        int cbOIASNotifyRecvProc(OIASClient sender, NOTIFY_INFO info)
        {
            if (info.eventName == "groupNotify" && info.eventData != 0)
            {
                IntPtr listPtr = new IntPtr(info.eventData);
                OBJECT_LIST notifyList = new OBJECT_LIST();
                Marshal.PtrToStructure(listPtr, notifyList);
                int[] itemAddresses = new int[notifyList.count];
                Marshal.Copy(notifyList.items, itemAddresses, 0, notifyList.count);
                for (int i = 0; i < notifyList.count; i++)
                {
                    NOTIFY_INFO subNotifyInfo = new NOTIFY_INFO();
                    IntPtr itemPtr = new IntPtr(itemAddresses[i]);
                    Marshal.PtrToStructure(itemPtr, subNotifyInfo);
                    cbOIASNotifyRecvProc(sender, subNotifyInfo);
                }
            }
            else
            {

                if (info.objectType == "client")
                {
                    if (info.eventName == "loginFault" || info.eventName == "connectBreak")
                    {
                        this.Notify(1,"","");
                    }
                    else if (info.eventName == "connectBreak.restore" || info.eventName == "loginFault.restore")
                    {
                        this.Notify(1,"","");
                    }
                }
                else if (info.objectType == "sensor")
                {
                    ParseSensorEvent(info.objectName, info.eventName);
                }
            }
            if (DataReceived != null)
            {
                string msg = string.Format("IP {0}, name: {1} event: {2}, description: {3}", device.ip, info.objectName, info.eventName, info.description);
                DataReceived(msg);
            }

            return 0;
        }

        public delegate void DataReceivedProc(string msg);
        public DataReceivedProc DataReceived = null;


        public string LibFile { get; set; }


        public DateTime LastLive = DateTime.Now;

        /// <summary>
        /// 通道最多个数
        /// </summary>
        public int MaxChannelCount = 24;

        Dictionary<long, OIASClient> mapDriver = new Dictionary<long, OIASClient>();
        public ProtocolFbg()
        {

        }

        ~ProtocolFbg()
        {

        }

        public void ParseSensorEvent(string objectName, string eventName)
        {
            string[] array = objectName.Split(new char[3] { 'D', 'C', 'S' });
            int chan = 1, id = 1;
            if (!int.TryParse(array[2], out chan) || !int.TryParse(array[3], out id))
                return;

            var sensor = FindSensorBySID(chan, id);
            if (sensor == null)
                return;

            switch (eventName)
            {
                case "signalBreak":
                    {
                        ERROR_INFO info = new ERROR_INFO();
                        info.id = sensor.id;
                        info.name = sensor.name;
                        info.status = STATUS_TYPE.ERROR;
                        info.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        info.type = OBJECT_TYPE.sensor;
                        OnObjectErrorEH(info);
                    }
                    break;
                case "signalBreak.restore":
                    {
                        ERROR_INFO info = new ERROR_INFO();
                        info.id = sensor.id;
                        info.name = sensor.name;
                        info.status = STATUS_TYPE.NORMAL;
                        info.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        info.type = OBJECT_TYPE.sensor;
                        OnObjectErrorEH(info);
                    }
                    break;
                case "alarm":
                    {
                        STATUS_INFO info = new STATUS_INFO();
                        info.no = Function.GuidToInt64();
                        info.id = sensor.id;
                        info.name = sensor.name;
                        info.status = STATUS_TYPE.ALARM;
                        info.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        info.type = MONITOR_TYPE.vibration;
                        info.confidence = 100;
                        info.classification = CLASSIFICATION.human_recognition;
                        OnObjectStatusEH(info);
                    }
                    break;
            }
        }

        public override void RegisterDevice(Device device)
        {
            if (mapDriver.ContainsKey(device.id))
                return;
            OIASClient oiasClient = new OIASClient();
            oiasClient.Notify += new OIASClient.OIASNotifyEventHandler(cbOIASNotifyRecvProc);
            mapDriver.Add(device.id, oiasClient);

            LOGIN_INFO loginInfo = new LOGIN_INFO();
            loginInfo.hostIP = device.ip;
            loginInfo.hostPort = device.port;
            loginInfo.userName = device.user;
            loginInfo.password = device.pwd;
            oiasClient.SetLoginInfo(loginInfo);
            if (!oiasClient.HasSessionFault && oiasClient.Login() == 0)
            {
                OBJECT_LIST list = new OBJECT_LIST();
                IntPtr listPtr = new IntPtr(Marshal.SizeOf(list));
                    
                if (oiasClient.ListFault(list) == 0)
                {
                    if (list.count > 0)
                    {
                        int[] itemAddresses = new int[list.count];
                        Marshal.Copy(list.items, itemAddresses, 0, list.count);
                        for (int i = 0; i < list.count; i++)
                        {
                            NOTIFY_INFO subNotifyInfo = new NOTIFY_INFO();
                            IntPtr itemPtr = new IntPtr(itemAddresses[i]);
                            Marshal.PtrToStructure(itemPtr, subNotifyInfo);
                            if (subNotifyInfo.objectType == "sensor")
                            {
                                ParseSensorEvent(subNotifyInfo.objectName, subNotifyInfo.eventName);
                            }

                            if (DataReceived != null)
                            {
                                string msg = string.Format("历史事件：IP:{0},name:{1}, event:{2},description:{3}", device.ip, subNotifyInfo.objectName, subNotifyInfo.eventName, subNotifyInfo.description);
                                DataReceived(msg);
                            }
                        }
                    }
                }
                    
                if (DataReceived != null)
                    DataReceived("start success");
                OnConnectEH(device, 0);
            }
            else
            {
                if (DataReceived != null)
                    DataReceived("start failure");
                OnConnectEH(device, 1);
            }
        }

        public override void UnRegisterDevice(long id)
        {
            if (mapDriver.ContainsKey(id))
            {
                if (oiasClient.HasLogin)
                {
                    oiasClient.Logout();
                    OnConnectEH(device, 1);
                    if (DataReceived != null)
                        DataReceived("stop success");
                }
            }          
        }
    }
}