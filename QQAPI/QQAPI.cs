using LitJson;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class QQAPI : SingletonMonoBehaviour<QQAPI>
    {
        public delegate void QQLoginDelegate(string openId, string nickname, string gender, string headImgUrl);
        QQLoginDelegate LoginCallbackEvent;

        public void Init(string appId)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidUtils.CallAndroidStaticFunction("cn.btoolkit.qqapi.QQ", "init", appId);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _QQInit(appId);
            }
        }

        public bool IsAppInstalled()
        {
            if (Application.isEditor)
            {
                return true;
            }
            bool isAppInstalled = false;
            if (Application.platform == RuntimePlatform.Android)
            {
                isAppInstalled = AndroidUtils.CallAndroidStaticFunction<bool>("cn.btoolkit.qqapi.QQ", "isQQInstalled");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                isAppInstalled = _IsQQInstalled();
            }
            return isAppInstalled;
        }

        public void Login(QQLoginDelegate OnLoginCallback)
        {
            if (!IsAppInstalled())
            {
                Wait.Show(false);
                DialogAlert.Show("提示", "请先安装微信客户端");
                return;
            }
            this.LoginCallbackEvent = OnLoginCallback;
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidUtils.CallAndroidStaticFunction("cn.btoolkit.qqapi.QQLogin", "login", gameObject.name, "QQLoginCallback");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (!_IsQQInstalled())
                {
                    DialogAlert.Show("提示", "请先安装QQ客户端");
                    Wait.Show(false);
                    return;
                }
                _QQLogin(gameObject.name, "QQLoginCallback");
            }
        }

        void QQLoginCallback(string msg)
        {
            Debug.Log("QQLoginCallback:" + msg);
            JsonData jsonData = JsonMapper.ToObject(msg);
            string openId = (string)jsonData["openid"];
            string nickname = (string)jsonData["nickname"];
            string gender = (string)jsonData["gender"];
            string headImgUrl = (string)jsonData["figureurl_2"];
            if (LoginCallbackEvent != null)
            {
                LoginCallbackEvent(openId, nickname, gender, headImgUrl);
            }
        }

        [DllImport("__Internal")]
        static extern bool _IsQQInstalled();

        [DllImport("__Internal")]
        static extern void _QQInit(string appId);

        [DllImport("__Internal")]
        static extern void _QQLogin(string callbackGo, string callbackFun);

    }
}