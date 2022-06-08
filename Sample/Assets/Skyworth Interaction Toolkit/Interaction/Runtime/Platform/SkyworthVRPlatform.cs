using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyworthVRPlatform
{
    private readonly static string ACTION_SETSAFTYARE = "android.intent.action.SETSAFTYARE";
    private readonly static string PACKAGENAME = "com.ssnwt.newskyui";
    private static AndroidJavaObject mActivity;
    private static int mActionFlags = 0x10000000;
    static SkyworthVRPlatform()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityplayerclass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        mActivity = unityplayerclass.GetStatic<AndroidJavaObject>("currentActivity");

        mActionFlags = new AndroidJavaClass("android.content.Intent").GetStatic<int>("FLAG_ACTIVITY_NEW_TASK");
#endif
    }
    private static void StartIntent(string actionName)
    {
        AndroidJavaObject intetnobj = new AndroidJavaObject("android.content.Intent", ACTION_SETSAFTYARE);
        intetnobj.Call<AndroidJavaObject>("setPackage", PACKAGENAME);
        intetnobj.Call<AndroidJavaObject>("setFlags", mActionFlags);
        intetnobj.Call<AndroidJavaObject>("putExtra", "event", actionName);
        mActivity.Call("startActivity", intetnobj);
    }
    public static void ToSetGameArea()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        StartIntent("game");
#endif
    }
    public static void ToStationaryArea()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        StartIntent("stationary");
#endif
    }
}
