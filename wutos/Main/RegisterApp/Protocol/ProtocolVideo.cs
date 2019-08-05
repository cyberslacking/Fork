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
    #region 视频播放控制
    public enum VIDOE_PLAYBACK
    {
        PLAYSTART = 1,//开始播放
        PLAYSTOP,//停止播放
        PLAYPAUSE,//暂停播放
        PLAYRESTART,//恢复播放
        PLAYFAST,//快放
        PLAYSLOW,//慢放
        PLAYNORMAL,//正常速度
        PLAYFRAME,//单帧放
        PLAYSTARTAUDIO,//打开声音
        PLAYSTOPAUDIO,//关闭声音
        PLAYAUDIOVOLUME,//调节音量
        PLAYSETPOS,//改变文件回放的进度
        PLAYGETPOS,//获取文件回放的进度
        PLAYGETTIME,//获取当前已经播放的时间
        PLAYGETFRAME,//获取当前已经播放的帧数
        GETTOTALFRAMES,//获取当前播放文件总的帧数
        GETTOTALTIME,//获取当前播放文件总的时间
        THROWBFRAME//丢B帧
    };
    #endregion

    #region 云台控制
    public enum VIDEO_PTZ
    {
        PAN_LIGHT_PWRON = 1, //接通灯光电源
        PAN_WIPER_PWRON,   //接通雨刷开关
        PAN_FAN_PWRON,     //接通风扇开关
        PAN_HEATER_PWRON,  //接通加热器开关
        PAN_AUX_PWRON,     //接通辅助设备开关
        PAN_ZOOM_IN,       //焦距以变大(倍率变大)
        PAN_ZOOM_OUT,      //焦距变小(倍率变小)
        PAN_FOCUS_IN,      //焦点前调
        PAN_FOCUS_OUT,     //焦点后调
        PAN_IRIS_ENLARGE,  //光圈扩大
        PAN_IRIS_SHRINK,   //光圈缩小
        PAN_TILT_UP,     //云台向上
        PAN_TILT_DOWN,   //云台向下
        PAN_TILT_LEFT,   //云台左转
        PAN_TILT_RIGHT,  //云台右转
        PAN_UP_LEFT,	 //云台以SS的速度上仰和左转
        PAN_UP_RIGHT,    //云台以SS的速度上仰和右转
        PAN_DOWN_LEFT,   //云台以SS的速度下俯和左转
        PAN_DOWN_RIGHT,  //云台以SS的速度下俯和右转
        PAN_PAN_AUTO     //云台以SS的速度左右自动扫描
    };

    public enum VIDEO_PRESET
    {
        MY_SET_PRESET = 1, //设置预置点
        MY_CLE_PRESET,    //清除预置点
        MY_GOTO_PRESET    //调用预置点
    };
    #endregion

    public enum OBJECT_CODE
    {
	    DEVICE,
	    CAMERA
    };

    public enum EVENT_CODE
    {
	    MOTION_ALARM,
	    VIDEOLOST,
	    BLIND_ALARM
    };


    ////通知信息
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class  VIDEO_EVENT_INFO
    {		
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
	    public string object_id;
        public OBJECT_CODE object_type;
        public EVENT_CODE event_code;
    };

    #region 视频导出类
    public static class VideoManager
    {
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateInstance(int nVideoType);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyInstance(int nKey);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Open(int nKey, string sIP, int nPort, string sName, string sPWD);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Close(int nKey);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RealPlay(int nKey, string cameraCode, int nChannel, IntPtr hWnd);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StopRealPlay(int nKey, int lPlay);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Preset(int nKey, string cameraCode, int nChannel, int nParam, int nPresetNO);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetRecordPath(int nKey, string sPath);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StartRecord(int nKey, int lPlay, string sName);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StopRecord(int nKey, int lPlay);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool OpenVideoFile(int nKey, string sFilePath);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CloseVideoFile(int nKey);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StopVideoFile(int nKey);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PlayVideoFile(int nKey, IntPtr hWnd);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PauseFlie(int nKey, System.UInt32 nPause);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PtzPan(int nKey, string cameraCode, int nChannel, int pan, int speed, int stop);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FastSlowPlay(int nKey, bool speed, string pscale);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Snapshot(int nKey, string cameraCode, int nChannel, string sFileName);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ScreenToWall(int nKey, string pIP, int nPort, string pDeviceId, int nAreaNumber);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AlarmInfo(int nKey, int alarmType, string puId, int nChannel, string UserData);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PlayBackByTime(int nKey, string cameraCode, int nChannel, string pStartTime, string pEndTime, IntPtr hWnd);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PlayBackControl(int nKey, int lPlay, int nControlCode, int nInValue, ref int nOutValue);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StopPlayBack(int nKey, int lPlay);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetMatrixScreen(int nKey, string monitorCode, string cameraCode, int nChannel, int mod);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetServerTime(int nKey, IntPtr time);
        [DllImport("DLL\\Video\\VideoSDK.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetServerTime(int nKey, IntPtr time);
    }
    #endregion

    public class ProtocolVideo: ProtocolDriver
    {
        private int key = -1;
        public static ProtocolVideo g_protocalVideo = null;

        public ProtocolVideo(Device device)
            : base(device)
        {
            g_protocalVideo = this;
        }

        public override void Start()
        {
            if (!device.enable)
                return;
            key = VideoManager.CreateInstance(device.type);
            if (VideoManager.Open(key, device.ip, device.port, device.user, device.pwd))
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
            if (!device.enable || key == -1)
                return;
            VideoManager.Close(key);
            VideoManager.DestroyInstance(key);
            key = -1;
            OnConnectEH(device, 1);
        }

        /// <summary>
        /// 输出信号控制 0,1 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        public override void DoOutput(byte DO, byte flag)
        {
        }

        public override bool GetServerTime(IntPtr time)
        {
            return VideoManager.GetServerTime(key,time);
        }

        public override bool SetServerTime(IntPtr time)
        {
            return VideoManager.SetServerTime(key, time);
        }

        public int RealPlay(string camera, int channel , IntPtr handle)
        {
            return VideoManager.RealPlay(key ,camera ,channel, handle);
        }

        public void StopRealPlay(int play)
        {
            VideoManager.StopRealPlay(key, play);
        }

        public bool Preset(string camera, int channel, int param, int preset)
        {
            return VideoManager.Preset(key, camera, channel, param, preset);
        }

        public bool PtzPan(string camera, int channel, int pan, int speed, int stop)
        {
            return VideoManager.PtzPan(key, camera, channel, pan, speed, stop);
        }

        public int PlayBackByTime(string camera, int channel, string begin, string end, IntPtr handle)
        {
            return VideoManager.PlayBackByTime(key, camera, channel, begin, end, handle);
        }

        public void StopPlayBack(int play)
        {
            VideoManager.StopPlayBack(key,play);
        }

        public bool PlayBackControl(int play, int param, int inValue, ref int outValue)
        {
           return VideoManager.PlayBackControl(key, play, param, inValue, ref outValue);
        }
    }
}
