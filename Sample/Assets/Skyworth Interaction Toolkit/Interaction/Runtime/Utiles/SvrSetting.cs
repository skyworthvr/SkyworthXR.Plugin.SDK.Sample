#define ENABLE_PROFILER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Svr
{
    public enum SvrHandedness
    {
        Left = 1, Right = 2
    }
    public enum SvrControllerType
    {
        Ximmers = 1,
        Nolo = 2
    }
    public enum Svr6DOFHandedness
    {
        Left = 1, Right = 2
    }

    public enum UnityVersion
    {
        UNITY_2017_1,
        UNITY_2017_3,
        UNITY_2017_4
            
	}
    public enum SvrLogLevel
    {
        All = 0,
        Log = 1,
        Warning = 2,
        Error = 3,
        None = 4
    }
    public class SvrSetting
    {
//        public static void SetSdkSupport(UnityVersion version)
//        {
//#if UNITY_ANDROID
//            try
//            {
//                if (!IsVR9Device)
//                    SdkSupport(version);
//            }
//            catch (Exception)
//            {
//            }
//#endif
        //}
        private const string HANDEDNESSKEY = "persist.svr.setting.handedness";
        private const string NOLOHANDEDNESSKEY = "persist.svr.nolo.handedness";
        public readonly static string FOVKEY = "persist.svr.fov";
        public readonly static string IPDKEY = "persist.svr.ipd";
        private readonly static string SETNOLOCONNECTED = "persist.svr.nolo.connect";
        private readonly static string SVRLOGLEVEL = "persist.svrlog.debug.level";
        private readonly static string ANTIALIASING = "persist.ssnwt.antiAliasing";
        private readonly static string SCREENSCALE = "persist.ssnwt.stereoScreenScale";
        private readonly static string CONTROLLERTYPE = "persist.3dof_type";
        private readonly static string SHOWFPS = "persist.ssnwt.showfps";
        private readonly static string SVRCONTROLLERSERVICE_V2 = "com.ssnwt.vr.server";
        private static SvrHandedness mSvrHandedness = SvrHandedness.Right;
        private static SvrControllerType mSvrController = SvrControllerType.Ximmers;
        private static Svr6DOFHandedness mSvrNOloHandedness = Svr6DOFHandedness.Right;
        public static SvrLogLevel svrLogLevel = SvrLogLevel.None;
        private static int noloconnect = 0;

        private static jvalue[] SETNOLOCONNECTED_jvalues;
        private static IntPtr getInt_methodID;
        public static bool GetNoloConnected
        {
            get
            {
                //noloconnect = getIntProperty(SETNOLOCONNECTED, 0);
                noloconnect = CallGetNoloConnect();
                //Svr.SvrLog.Log("noloconnect:"+ noloconnect);
                return noloconnect == 1;
            }
        }
        internal static int CallGetNoloConnect()
        {
            if (getInt_methodID == IntPtr.Zero)
                initNoloNative();
            //Debug.Log("jc.GetRawClass()xxx " + jc.GetRawClass());
            int value = AndroidJNI.CallStaticIntMethod(jc.GetRawClass(), getInt_methodID, SETNOLOCONNECTED_jvalues);
            //Debug.Log("CallGetNoloConnect "+ value);
            return value;
        }
        public static SvrHandedness Handedness
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                string ness = getProperty(HANDEDNESSKEY, "2");
                int handed = int.Parse(ness);
                return (SvrHandedness)handed;
#else
                return mSvrHandedness;
#endif
            }
        }
        public static SvrControllerType ControllerType
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                string ness = getProperty(CONTROLLERTYPE, "1");
                int handed = int.Parse(ness);
                return (SvrControllerType)handed;
#else
                return mSvrController;
#endif
            }
        }
        public static bool ShowFPS
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                string ness = getProperty(SHOWFPS, "false");
                bool fps = false;
                bool.TryParse(ness,out fps);
                return fps;
#else
                return false;
#endif
            }
        }
        public static Svr6DOFHandedness NHandedness
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                string ness = getProperty(NOLOHANDEDNESSKEY, "2");
                int handed = int.Parse(ness);
                return (Svr6DOFHandedness)handed;
#else
                return Svr6DOFHandedness.Right;
#endif
            }
        }
        public static int AntiAliasing
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                string antis = getProperty(ANTIALIASING, "4");
                //SvrLog.Log("System antiAliasing is "+ antis);
                switch (antis)
                {
                    case "0":
                        return 0;
                    case "2":
                        return 2;
                    case "4":
                        return 4;
                    case "8":
                        return 8;
                    default:
                        return 4;
                }
#else
                return QualitySettings.antiAliasing;
#endif
            }
        }
        public static float StereoScreenScale
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                string scalestr = getProperty(SCREENSCALE,"1.2");
                //SvrLog.Log("System stereoScreenScale is " + scalestr);
                float scale = 1.2f;
                if (!float.TryParse(scalestr, out scale))
                {
                    //Svr.SvrLog.LogErrorFormat("{0} can't parse to float", scalestr);
                }
                return scale;
#else
                return 1;
#endif
            }
        }
        private static AndroidJavaClass jc;
        private static IntPtr property_global;
        //public static bool IsVR9Device { get; private set; }
        /// <summary>
        /// 当前运行的环境是否是支持的VR设备
        /// </summary>
        //public static bool IsVRDevice { get; private set; }
        internal static bool UnityVRSupport { get; private set; }
        //internal static bool isPONE { get; private set; }
        private static long currentTicks = 0;
        private static jvalue SETNOLOCONNECTED_jvalue;
        private static jvalue zero_jvalue;
        static SvrSetting()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            jc = new AndroidJavaClass("android.os.SystemProperties");
            string product = getProperty("ro.build.product", "");
            svrLogLevel = (SvrLogLevel)int.Parse(getProperty(SVRLOGLEVEL, "3"));
            Debug.Log("Product:" + product);
            //isPONE = product.Equals("PX_VR9");
            //IsVR9Device = product.Equals("neptune-y2") 
            //    || isPONE;
            //IsVRDevice = IsVR9Device || product.Equals("hmd8895") || SVR.AtwAPI.CheckAppExist(SVRCONTROLLERSERVICE_V2);
            ////IsVR9Device = true;
            //if (IsVR9Device)
            //{
            //    Application.targetFrameRate = 70;
            //}

            //SVR.AtwAPI.PostRender();

            property_global = AndroidJNI.NewGlobalRef(AndroidJNI.FindClass("android/os/SystemProperties"));
            SETNOLOCONNECTED_jvalue.l = AndroidJNI.NewGlobalRef(AndroidJNI.NewStringUTF(SETNOLOCONNECTED));
            zero_jvalue.i = 0;
#endif

        }

        private static void initNoloNative()
        {

            //Debug.Log("jc.GetRawClass() " + jc.GetRawClass());

            //object[] parms_object = new object[] { 0 };
            //try
            //{
            //SETNOLOCONNECTED_jvalue.l = AndroidJNI.NewStringUTF(SETNOLOCONNECTED);
            SETNOLOCONNECTED_jvalues = new jvalue[] { SETNOLOCONNECTED_jvalue , zero_jvalue };
            getInt_methodID = AndroidJNIHelper.GetMethodID(jc.GetRawClass(), "getInt", "(Ljava/lang/String;I)I", true);
            //}
            //finally
            //{
            //    AndroidJNIHelper.DeleteJNIArgArray(parms_object, SETNOLOCONNECTED_jvalue);
            //}

        }
        public static string getProperty(string key, string def)
        {
            string result = def;
#if UNITY_ANDROID && !UNITY_EDITOR
            UnityEngine.Profiling.Profiler.BeginSample(string.Format("getProperty:{0}",key));
            result = jc.CallStatic<string>("get", key, def);
            UnityEngine.Profiling.Profiler.EndSample();
#endif
            return result;
        }
       
        public static int getIntProperty(string key, int def)
        {
            long ticks = DateTime.Now.Ticks / 10000;
            if (noloconnect == 0 && ticks - currentTicks < 100)
            {
                return 0;
            }
            currentTicks = ticks;
            int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
            UnityEngine.Profiling.Profiler.BeginSample(string.Format("getIntProperty:{0}", key));
            result = jc.CallStatic<int>("getInt", key, def);
            UnityEngine.Profiling.Profiler.EndSample();
#endif
            return result;
        }

        public static void setProperty(string key, string value)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            UnityEngine.Profiling.Profiler.BeginSample("setProperty");
            jc.CallStatic("set", key, value);
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        public static void SetHandedness(SvrHandedness Handedness)
        {
            mSvrHandedness = Handedness;
            int handedness = (int)Handedness;
            setProperty(HANDEDNESSKEY, handedness.ToString());
        }

        public static void SetNoloHandedness(Svr6DOFHandedness Handedness)
        {
            mSvrNOloHandedness = Handedness;
            int handedness = (int)Handedness;
            setProperty(NOLOHANDEDNESSKEY, handedness.ToString());
        }

        //public static void SetNoloConnected(bool connected)
        //{
        //    setProperty(SETNOLOCONNECTED, connected ? "1" : "0");
        //}

        internal static Version GetVersion(string str)
        {
            if (str.Contains("f"))
            {
                str = str.Substring(0, str.IndexOf("f"));
            }
            else if (str.Contains("b"))
            {
                str = str.Substring(0, str.IndexOf("b"));
            }
            else if (str.Contains("p"))
            {
                str = str.Substring(0, str.IndexOf("b"));
            }

            return new Version(str);
        }
        public static void SetVRSupport(bool support)
        {
            UnityVRSupport = support;

        }

    }
}
