using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class WeiXin
    {
        public static string AppId { private set; get; }
        public static string AppSecret { private set; get; }

        public static void Init(string appId, string appSecret)
        {
            AppId = appId;
            AppSecret = appSecret;
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXin", "init", appId, appSecret);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _WeiXinInit(appId, appSecret);
            }
        }

        public static bool IsAppInstalled()
        {
            if (Application.isEditor)
            {
                return true;
            }
            bool isAppInstalled = false;
            if (Application.platform == RuntimePlatform.Android)
            {
                isAppInstalled = AndroidHelper.CallAndroidStaticFunction<bool>("cn.btoolkit.weixinapi.WeiXin", "isWeiXinInstalled");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                isAppInstalled = _IsWeiXinInstalled();
            }
            return isAppInstalled;
        }

        [DllImport("__Internal")]
        static extern void _WeiXinInit(string appId, string secret);
        [DllImport("__Internal")]
        static extern bool _IsWeiXinInstalled();

    }
}