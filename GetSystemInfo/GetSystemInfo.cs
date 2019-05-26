using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace BToolkit
{
    public class GetSystemInfo
    {
        public enum InternetStatus
        {
            NoNet,
            Wifi,
            Mobile
        }
        public enum InternetQuality
        {
            Low,
            Middle,
            Hight
        }

        /// <summary>
        /// 获取当前电量的百分比（0-100的整数）
        /// </summary>
        public static int GetBatteryPercent()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return AndroidHelper.CallAndroidStaticFunction<int>("cn.btoolkit.system.GetSystemInfo", "getBatteryPercent");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return (int)(_GetBatteryProgress() * 100);
            }
            return 100;
        }

        /// <summary>
        /// 获取当前网络类型
        /// </summary>
        public static InternetStatus GetInternetStatus()
        {
            InternetStatus status = InternetStatus.NoNet;
            if (Application.platform == RuntimePlatform.Android)
            {
                string statusStr = AndroidHelper.CallAndroidStaticFunction<string>("cn.btoolkit.system.GetSystemInfo", "getInternetType");
                switch (statusStr)
                {
                    case "Wifi":
                        status = InternetStatus.Wifi;
                        break;
                    case "Mobile":
                        status = InternetStatus.Mobile;
                        break;
                    case "NoNet":
                        status = InternetStatus.NoNet;
                        break;
                }
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                string statusStr = _GetInternetStatus();
                switch (statusStr)
                {
                    case "Wifi":
                        status = InternetStatus.Wifi;
                        break;
                    case "CellularNetwork":
                        status = InternetStatus.Mobile;
                        break;
                    case "NoNet":
                        status = InternetStatus.NoNet;
                        break;
                }
            }
            return status;
        }

        /// <summary>
        /// 获取当前网络信号强度
        /// </summary>
        public static void GetInternetQuality(Action<InternetQuality> Callback)
        {
            Ping.Instance.StartPing("www.baidu.com", (int time) =>
            {
                if (time <= 200)
                {
                    Callback(InternetQuality.Hight);
                }
                else if (time <= 300)
                {
                    Callback(InternetQuality.Middle);
                }
                else
                {
                    Callback(InternetQuality.Low);
                }
            });
        }

        /// <summary>
        /// 获取是否是Android模拟器
        /// </summary>
        public static bool IsAndroidSimulator()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return AndroidHelper.CallAndroidStaticFunction<bool>("cn.btoolkit.system.GetSystemInfo", "isSimulator");
            }
            return false;
        }

        /// <summary>
        /// 获取是否是平板设备
        /// </summary>
        public static bool IsPad()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                float physicscreen = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height) / Screen.dpi;
                if (physicscreen >= 7f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //string genneration = UnityEngine.iOS.Device.generation.ToString();
                string genneration = SystemInfo.deviceModel.ToString();
                if (genneration.Substring(0, 3) == "iPa")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取设备ID
        /// </summary>
        public static string GetDeviceId(string appPackgeName)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                string deviceId = AndroidHelper.CallAndroidStaticFunction<string>("cn.btoolkit.system.GetSystemInfo", "getDeviceId");
                return GetMD5_32(deviceId + appPackgeName, true);//MD5加密以防设备识别码泄露
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return GetMD5_32(_GetDeviceId(appPackgeName), true);//MD5加密以防设备识别码泄露
            }
            return SystemInfo.deviceUniqueIdentifier;//使用此值会随着应用卸载重装而改变
        }

        [DllImport("__Internal")]
        static extern float _GetBatteryProgress();
        [DllImport("__Internal")]
        static extern string _GetInternetStatus();
        [DllImport("__Internal")]
        static extern string _GetDeviceId(string appPackgeName);

        //MD5加密
        static string GetMD5_32(string inputStr, bool upperCase = true)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputStr);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                if (upperCase)
                {
                    sb.Append(hash[i].ToString("X2"));//字母大写
                }
                else
                {
                    sb.Append(hash[i].ToString("x2"));//字母小写
                }
            }
            return sb.ToString();
        }
    }
}