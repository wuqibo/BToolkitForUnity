﻿using LitJson;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace BToolkit
{
    public class VersionChecker
    {
        /// <summary>
        /// 有版本更新则回调下载地址url,没有更新回调null
        /// </summary>
        public static void CheckVersion(string jsonUrl, Action<string> Callback)
        {
            BUtils.Instance.StartCoroutine(DoDownload(jsonUrl, Callback));
        }

        static IEnumerator DoDownload(string jsonUrl, Action<string> Callback)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(jsonUrl);
            yield return webRequest.SendWebRequest();
            if (webRequest.error != null)
            {
                Debuger.LogError("CheckVersion Error:" + webRequest.error);
                yield break;
            }
            try
            {
                JsonData jsonData = JsonMapper.ToObject(webRequest.downloadHandler.text);
                string version = (string)jsonData["versionName"];

                Debuger.Log("版本比较："+GetAppInfo.GetAppVersionName() + " = " + version + "url:" + jsonUrl);

                if (!GetAppInfo.GetAppVersionName().Equals(version))
                {
                    string url = (string)jsonData["url"];
                    Callback(url);
                }
                else
                {
                    Callback(null);
                }
            }
            catch (Exception err)
            {
                Callback(null);
                Debuger.LogError("CheckVersion Error:" + err);
            }
        }

    }
}