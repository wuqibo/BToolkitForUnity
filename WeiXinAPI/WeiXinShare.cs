using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class WeiXinShare : SingletonMonoBehaviour<WeiXinShare>
    {
        /// <summary>
        /// 分享Text给微信好友或群
        /// </summary>
        public void ShareTextToFriend(string title, string content)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXinShare", "share", 0, 0, "", title, content, "");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _Share(0, 0, "", title, content, "");
            }
        }

        /// <summary>
        /// 分享Text到朋友圈
        /// </summary>
        public void ShareTextToMoments(string title, string content)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXinShare", "share", 0, 1, "", title, content, "");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _Share(0, 1, "", title, content, "");
            }
        }

        /// <summary>
        /// 添加Text到微信收藏夹
        /// </summary>
        public void AddTextToFavorite(string title, string content)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXinShare", "share", 0, 2, "", title, content, "");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _Share(0, 2, "", title, content, "");
            }
        }

        /// <summary>
        /// 分享Image给微信好友或群
        /// </summary>
        public void ShareImageToFriend(string title, string content, string imgLocalPath)
        {
            Debuger.Log(imgLocalPath);
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXinShare", "share", 1, 0, "", title, content, imgLocalPath);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _Share(1, 0, "", title, content, imgLocalPath);
            }
        }

        /// <summary>
        /// 分享Image到朋友圈
        /// </summary>
        public void ShareImageToMoments(string title, string content, string imgLocalPath)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXinShare", "share", 1, 1, "", title, content, imgLocalPath);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _Share(1, 1, "", title, content, imgLocalPath);
            }
        }

        /// <summary>
        /// 添加Image到微信收藏夹
        /// </summary>
        public void AddImageToFavorite(string title, string content, string imgLocalPath)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXinShare", "share", 1, 2, "", title, content, imgLocalPath);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _Share(1, 2, "", title, content, imgLocalPath);
            }
        }

        /// <summary>
        /// 分享Web给微信好友或群
        /// </summary>
        public void ShareWebToFriend(string url, string title, string content, string imgLocalPath)
        {
            Debuger.Log("ShareWebToFriend:" + url + "|" + title + "|" + content + "|" + imgLocalPath);
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXinShare", "share", 2, 0, url, title, content, imgLocalPath);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _Share(2, 0, url, title, content, imgLocalPath);
            }
        }

        /// <summary>
        /// 分享Web到朋友圈
        /// </summary>
        public void ShareWebToMoments(string url, string title, string content, string imgLocalPath)
        {
            Debuger.Log("ShareWebToMoments:" + url + "|" + title + "|" + content + "|" + imgLocalPath);
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXinShare", "share", 2, 1, url, title, content, imgLocalPath);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _Share(2, 1, url, title, content, imgLocalPath);
            }
        }

        /// <summary>
        /// 添加Web到微信收藏夹
        /// </summary>
        public void AddWebToFavorite(string url, string title, string content, string imgLocalPath)
        {
            Debuger.Log("AddWebToFavorite:" + url + "|" + title + "|" + content + "|" + imgLocalPath);
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXinShare", "share", 2, 2, url, title, content, imgLocalPath);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _Share(2, 2, url, title, content, imgLocalPath);
            }
        }

        [DllImport("__Internal")]
        static extern void _Share(int contentType, int toTarget, string url, string title, string content, string imgLocalPath);

    }
}