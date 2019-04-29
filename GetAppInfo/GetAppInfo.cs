using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class GetAppInfo
    {
        public static string GetAppPackageName()
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR && UNITY_ANDROID
                return UnityEditor.PlayerSettings.GetApplicationIdentifier(UnityEditor.BuildTargetGroup.Android);
#elif UNITY_EDITOR && UNITY_IOS
                return UnityEditor.PlayerSettings.GetApplicationIdentifier(UnityEditor.BuildTargetGroup.iOS);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                return AndroidUtils.CallAndroidStaticFunction<string>("cn.btoolkit.getappinfo.GetAppInfo", "getAppPackageName");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return _GetAppPackageName();
            }
            return "";
        }

        public static string GetAppVersionName()
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                return UnityEditor.PlayerSettings.bundleVersion;
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                return AndroidUtils.CallAndroidStaticFunction<string>("cn.btoolkit.getappinfo.GetAppInfo", "getAppVersionName");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return _GetAppVersionName();
            }
            return "1.0";
        }

        public static int GetAppVersionCode()
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR && UNITY_ANDROID
                return UnityEditor.PlayerSettings.Android.bundleVersionCode;
#elif UNITY_EDITOR && UNITY_IOS
                try
                {
                    return int.Parse(UnityEditor.PlayerSettings.iOS.buildNumber);
                }
                catch
                {
                    return 1;
                }
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                return AndroidUtils.CallAndroidStaticFunction<int>("cn.btoolkit.getappinfo.GetAppInfo", "getAppVersionCode");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return _GetAppVersionCode();
            }
            return 1;
        }

        [DllImport("__Internal")]
        static extern string _GetAppPackageName();

        [DllImport("__Internal")]
        static extern string _GetAppVersionName();

        [DllImport("__Internal")]
        static extern int _GetAppVersionCode();

    }
}