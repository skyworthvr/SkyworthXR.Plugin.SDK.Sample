using UnityEngine;
using UnityEditor;

namespace Svr
{
    public class SvrLog
    {
        public static void Log(object logs)
        {
            if (SvrSetting.svrLogLevel <= SvrLogLevel.Log)
            {
                Debug.Log(logs);
            }
        }
        public static void LogFormat(string format, params object[] value)
        {
            if (SvrSetting.svrLogLevel <= SvrLogLevel.Log)
            {
                Debug.LogFormat(format, value);
            }
        }
        public static void LogWarning(object logs)
        {
            if (SvrSetting.svrLogLevel <= SvrLogLevel.Warning)
            {
                Debug.LogWarning(logs);
            }
        }
        public static void LogWarningFormat(string format, params object[] value)
        {
            if (SvrSetting.svrLogLevel <= SvrLogLevel.Warning)
            {
                Debug.LogWarningFormat(format, value);
            }
        }
        public static void LogError(object logs)
        {
            if (SvrSetting.svrLogLevel <= SvrLogLevel.Error)
            {
                Debug.LogError(logs);
            }
        }
        public static void LogErrorFormat(string format, params object[] value)
        {
            if (SvrSetting.svrLogLevel <= SvrLogLevel.Error)
            {
                Debug.LogFormat(format, value);
            }
        }
    }
}