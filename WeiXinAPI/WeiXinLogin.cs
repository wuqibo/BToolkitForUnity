using LitJson;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class WeiXinLogin : SingletonMonoBehaviour<WeiXinLogin>
    {
        public delegate void WeiXinLoginDelegate(string openId, string nickname, string gender, string headImgUrl);
        WeiXinLoginDelegate LoginCallbackEvent;

        public void Login(WeiXinLoginDelegate OnLoginCallback)
        {
            if (!WeiXin.IsAppInstalled())
            {
                Wait.Show(false);
                //TopDialog.Show("请先安装微信客户端");
                return;
            }
            this.LoginCallbackEvent = OnLoginCallback;
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidUtils.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXinLogin", "login", gameObject.name, "WeiXinLoginCallback");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _WeiXinLogin(gameObject.name, "WeiXinLoginCallback");
            }
        }

        void WeiXinLoginCallback(string msg)
        {
            //{"openid":"odRJkxOxv3IyEKjpaYPCmKQ6o06w","nickname":"[LO]","sex":1,"language":"zh_CN","city":"Nanning","province":"Guangxi","country":"CN","headimgurl":"http:\/\/wx.qlogo.cn\/mmopen\/vi_32\/LmMk0fAHOg2MBbVr2vIGzeAUozDO65UCnvVrpjz6BFQ1ZlPicTysxbicqXiaJVkPvWK6gBjzVtRYGu9W9j1ibpmaCg\/0","privilege":[],"unionid":"o1K470SYEWEoZ7xyZxwQIX3rsXCA"}
            Debuger.Log("WeiXinLoginCallback:" + msg);
            JsonData jsonData = JsonMapper.ToObject(msg);
            string openId = (string)jsonData["openid"];
            string nickname = (string)jsonData["nickname"];
            string gender = ((int)jsonData["sex"] == 0) ? "女" : "男";
            string headImgUrl = (string)jsonData["headimgurl"];
            if (LoginCallbackEvent != null)
            {
                LoginCallbackEvent(openId, nickname, gender, headImgUrl);
            }
        }

        [DllImport("__Internal")]
        static extern void _WeiXinLogin(string callbackGo, string callbackFun);
    }
}