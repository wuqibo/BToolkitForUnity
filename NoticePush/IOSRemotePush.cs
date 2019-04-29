using UnityEngine;
#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
using System.Runtime.InteropServices;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#endif

namespace BToolkit
{
    //本地推送
    public class IOSRemotePush : MonoBehaviour
    {

        static IOSRemotePush instance;
        public static IOSRemotePush Instance
        {
            get
            {
                if (!instance)
                {
                    GameObject go = new GameObject("IOSRemotePush");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<IOSRemotePush>();
                }
                return instance;
            }
        }
        static string currDeviceToken;
        static bool hasTip;

        void OnDestroy()
        {
            instance = null;
        }

        void Awake()
        {
            instance = this;
        }

        public void Init()
        {
#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
		Debug.Log("Unity>>>>>>>>>>>>>>>>>>> IOSRemotePush_Init: ");
		_Init(gameObject.name,"GetedDeviceToken");
#endif
        }

        public void SetCurrBadgeNum(int badgeNum)
        {
#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
		_SetCurrBadgeNum(badgeNum);
#endif
        }

#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
	[DllImport("__Internal")]
	static extern bool _Init(string callbackGo,string callbackFun);
	[DllImport("__Internal")]
	static extern bool _SetCurrBadgeNum(int badgeNum);
#endif

        void GetedDeviceToken(string deviceToken)
        {
            Debug.Log("Unity>>>>>>>>>>>>>>>>>>> deviceToken: " + deviceToken);
            if (!string.IsNullOrEmpty(deviceToken))
            {
                currDeviceToken = deviceToken;
            }
            RegisterDevice(deviceToken);
            /*
            if (AccountManager.Instance.HasLogined)
            {
                RegisterDevice(deviceToken);
            }
            else
            {
                if (!hasTip)
                {
                    hasTip = true;
                    Invoke("Alert", 1);
                }
            }
            */
        }

        void RegisterDevice(string deviceToken)
        {
            //TODO:提交到服务器
        }

        void Alert()
        {
            DialogAlert.Show("提示", "登陆后可享受消息推送功能");
        }

        void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                TryToRegisterDevice();
            }
        }

        public void TryToRegisterDevice()
        {
            if (!string.IsNullOrEmpty(currDeviceToken))
            {
                RegisterDevice(currDeviceToken);
            }
            /*
            if (AccountManager.Instance.HasLogined && !string.IsNullOrEmpty(currDeviceToken))
            {
                UnreadManager.Instance.RegisterDevice(currDeviceToken);
            }
            */
        }
    }
}