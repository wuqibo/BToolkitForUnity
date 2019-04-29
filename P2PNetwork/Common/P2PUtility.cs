using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace BToolkit.P2PNetwork
{
    public class P2PUtility {

        /// <summary>
        /// 是否当前在Unity编辑器模式下运行
        /// </summary>
        public static bool isEditor { get; private set; }
        /// <summary>
        /// 是否当前在Windows平台下运行
        /// </summary>
        public static bool isWindowPlantform { get; private set; }
        /// <summary>
        /// 局域网本地IP
        /// </summary>
        public static string internalIP { get; private set; }
        /// <summary>
        /// 外网本地IP
        /// </summary>
        public static string externalIP { get; private set; }
        /// <summary>
        /// 设备唯一ID,在iOS上该ID会因为APP重装而改变
        /// </summary>
        public static string deviceId { get; private set; }
        /// <summary>
        /// 设备唯一ID,在iOS上该ID会因为APP重装而改变
        /// </summary>
        public static bool isIPV6 { get; private set; }
        /// <summary>
        /// Unity主线程ID
        /// </summary>
        private static int mainThreadId;

        /// <summary>
        /// 程序启动时初始化一遍，APP从后台切换回来时再调用一遍，已及时更新参数改变，比如用户切换了Wifi环境等
        /// </summary>
        public static void Init() {
            mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            isEditor = Application.isEditor;
            isWindowPlantform = Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer;
            internalIP = GetInternalIP();
            isIPV6 = internalIP.Contains(":");
            externalIP = GetExternalIP();
            deviceId = SystemInfo.deviceUniqueIdentifier;
        }

        /// <summary>
        /// 判断当前是否在Unity的主线程
        /// </summary>
        public static bool CurrIsMainThread()
        {
            return (System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId);
        }

        /// <summary>
        /// 获取内网IP
        /// </summary>
        private static string GetInternalIP() {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        /// <summary>
        /// 获取外网IP
        /// </summary>
        private static string GetExternalIP() {
            string ip = "";
            try {
                WebClient MyWebClient = new WebClient();
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                Byte[] pageData = MyWebClient.DownloadData("http://www.net.cn/static/customercare/yourip.asp");
                string pageHtml = Encoding.Default.GetString(pageData);
                //string pageHtml = Encoding.UTF8.GetString(pageData);
                int beginSub = pageHtml.IndexOf("<h2>") + 4;
                int endSub = pageHtml.IndexOf("</h2>");
                string[] ipArr = pageHtml.Substring(beginSub,endSub - beginSub).Split(',');
                ip = ipArr[0];
            } catch { }
            return ip;
        }

        /// <summary>
        /// 当前网络是否可用,必须在主线程调用
        /// </summary>
        public static bool IsNetWorkOK {
            get {
                if(Application.internetReachability == NetworkReachability.NotReachable) {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 类数据转byte[]
        /// </summary>
        public static byte[] ObjectToBytes(object data) {
            string json = JsonUtility.ToJson(data);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            return bytes;
        }

        /// <summary>
        /// Json字符串转类数据
        /// </summary>
        public static T JsonToObject<T>(string json) {
            return JsonUtility.FromJson<T>(json);
        }

        /// <summary>
        /// Json字符串转类数据
        /// </summary>
        public static object JsonToObject(string json,Type type) {
            return JsonUtility.FromJson(json,type);
        }

        /// <summary>
        /// 类数据转Json字符串
        /// </summary>
        public static string ObjectToJson(object data) {
            return JsonUtility.ToJson(data);
        }

        /// <summary>
        /// Json字符串转byte[]
        /// </summary>
        public static byte[] JsonToBytes(string json) {
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            return bytes;
        }

        /// <summary>
        /// byte[]转Json字符串
        /// </summary>
        public static string BytesToJson(byte[] bytes) {
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// byte[]转Json字符串
        /// </summary>
        public static string BytesToJson(byte[] bytes,int index,int count) {
            return Encoding.UTF8.GetString(bytes,index,count);
        }

        public static string ParseCustomData(object customData) {
            string customJson = "";
            if(customData.GetType() == typeof(Int32)) {
                customJson = MsgDivid.IsInt + customData;
            } else if(customData.GetType() == typeof(Boolean)) {
                customJson = MsgDivid.IsBool + customData;
            } else if(customData.GetType() == typeof(Single)) {
                customJson = MsgDivid.IsFloat + customData;
            } else if(customData.GetType() == typeof(String)) {
                customJson = MsgDivid.IsString + customData;
            } else {
                customJson = P2PUtility.ObjectToJson(customData);
            }
            return customJson;
        }

        /// <summary>
        /// 生成32位MD5
        /// </summary>
        public static string GetMD5_32(string inputStr,bool upperCase) {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputStr);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for(int i = 0;i < hash.Length;i++) {
                if(upperCase) {
                    sb.Append(hash[i].ToString("X2"));//字幕大写
                } else {
                    sb.Append(hash[i].ToString("x2"));//字幕小写
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 读取特定两个字符串中间的字符串
        /// </summary>
        public static string GetStrBetween(string originStr,string leftStr,string rightStr) {
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(originStr,"(?<=" + leftStr + ").*?(?=" + rightStr + ")");
            if(match.Success) {
                return match.Value;
            }
            P2PDebug.LogError("无法分析字符串: " + originStr);
            return null;
        }

        /// <summary>
        /// 判断字符串是否可以转为整型
        /// </summary>
        public static bool IsInt(string value) {
            int result;
            return int.TryParse(value,out result);
        }
        /// <summary>
        /// 判断字符串是否可以转为浮点型
        /// </summary>
        public static bool IsFloat(string value) {
            float result;
            return float.TryParse(value,out result);
        }
    }
}