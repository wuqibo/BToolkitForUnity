using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Text;
using System.Net.Sockets;
using UnityEngine.Networking;

namespace BToolkit
{
    public class Http
    {

        public enum ResultType
        {
            NoNetwork, LoadFailed, Success
        }
        public delegate void HttpCallback(ResultType resultType, string result);
        HttpCallback CurrHttpCallback;
        static Dictionary<string, Http> https = new Dictionary<string, Http>();

        /// <summary>
        /// HttpCallback(ResultType resultType, string result)
        /// </summary>
        public static void Post(string url, Dictionary<string, string> formData, HttpCallback HttpCallback)
        {
            Post(url, formData, null, null, HttpCallback);
        }
        public static void Post(string url, Dictionary<string, string> formData, Dictionary<string, byte[]> bytes, HttpCallback HttpCallback)
        {
            Post(url, formData, bytes, null, HttpCallback);
        }
        public static void Post(string url, Dictionary<string, string> formData, Dictionary<string, string> headers, HttpCallback HttpCallback)
        {
            Post(url, formData, null, headers, HttpCallback);
        }
        public static void Post(string url, Dictionary<string, string> formData, Dictionary<string, byte[]> bytes, Dictionary<string, string> headers, HttpCallback HttpCallback)
        {
            Http http = GetHttpFromPool(url);
            http.CurrHttpCallback = HttpCallback;
            BUtils.Instance.StartCoroutine(http.PostIEnumerator(url, formData, bytes, headers));
        }
        IEnumerator PostIEnumerator(string url, Dictionary<string, string> formData, Dictionary<string, byte[]> bytes, Dictionary<string, string> headers)
        {
            if (!BUtils.IsNetWorkOK)
            {
                if (CurrHttpCallback != null)
                {
                    CurrHttpCallback(ResultType.NoNetwork, "");
                }
            }
            else
            {
                //防止Http缓存，附带一个随机数
                if (url.Contains("?"))
                {
                    url += "&random=" + UnityEngine.Random.Range(0, 100) + Time.time;
                }
                else
                {
                    url += "?random=" + UnityEngine.Random.Range(0, 100) + Time.time;
                }
                WWWForm form = new WWWForm();
                if (formData != null)
                {
                    foreach (KeyValuePair<string, string> data in formData)
                    {
                        form.AddField(data.Key, data.Value);
                    }
                }
                if (bytes != null)
                {
                    foreach (KeyValuePair<string, byte[]> bs in bytes)
                    {
                        if (bs.Value != null)
                        {
                            form.AddBinaryData(bs.Key, bs.Value);
                        }
                    }
                }
                UnityWebRequest request = UnityWebRequest.Post(url, form);
                request.chunkedTransfer = false;
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> data in headers)
                    {
                        request.SetRequestHeader(data.Key, data.Value);
                    }
                }
                yield return request.Send();
                if (request.isNetworkError)
                {
                    if (CurrHttpCallback != null)
                    {
                        CurrHttpCallback(ResultType.LoadFailed, request.error);
                    }
                }
                else
                {
                    if (CurrHttpCallback != null)
                    {
                        CurrHttpCallback(ResultType.Success, request.downloadHandler.text.Trim());
                    }
                }
            }
        }

        /// <summary>
        /// HttpCallback(ResultType resultType, string result)
        /// </summary>
        public static void Get(string url, HttpCallback HttpCallback)
        {
            Get(url, null, HttpCallback);
        }
        public static void Get(string url, Dictionary<string, string> headers, HttpCallback HttpCallback)
        {
            Http http = GetHttpFromPool(url);
            http.CurrHttpCallback = HttpCallback;
            BUtils.Instance.StartCoroutine(http.GetIEnumerator(url, headers));
        }
        IEnumerator GetIEnumerator(string url, Dictionary<string, string> headers)
        {
            if (!BUtils.IsNetWorkOK)
            {
                if (CurrHttpCallback != null)
                {
                    CurrHttpCallback(ResultType.NoNetwork, "");
                }
            }
            else
            {
                //防止Http缓存，附带一个随机数
                if (url.Contains("?"))
                {
                    url += "&random=" + UnityEngine.Random.Range(0, 100) + Time.time;
                }
                else
                {
                    url += "?random=" + UnityEngine.Random.Range(0, 100) + Time.time;
                }
                UnityWebRequest request = UnityWebRequest.Get(url);
                request.chunkedTransfer = false;
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> data in headers)
                    {
                        request.SetRequestHeader(data.Key, data.Value);
                    }
                }
                yield return request.Send();
                if (request.isNetworkError)
                {
                    if (CurrHttpCallback != null)
                    {
                        CurrHttpCallback(ResultType.LoadFailed, request.error);
                    }
                }
                else
                {
                    if (CurrHttpCallback != null)
                    {
                        CurrHttpCallback(ResultType.Success, request.downloadHandler.text.Trim());
                    }
                }
            }
        }

        static Http GetHttpFromPool(string url)
        {
            if (https.ContainsKey(url))
            {
                return https[url];
            }
            Http http = new Http();
            https.Add(url, http);
            return http;
        }

        /// <summary>
        /// 获取本机内网IP
        /// </summary>
        public static string GetInternalIP()
        {
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
        /// 获取本机外网IP
        /// </summary>
        public static string GetExternalIP()
        {
            string ip = "";
            try
            {
                WebClient MyWebClient = new WebClient();
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                Byte[] pageData = MyWebClient.DownloadData("http://www.net.cn/static/customercare/yourip.asp");
                string pageHtml = Encoding.Default.GetString(pageData);
                //string pageHtml = Encoding.UTF8.GetString(pageData);
                int beginSub = pageHtml.IndexOf("<h2>") + 4;
                int endSub = pageHtml.IndexOf("</h2>");
                string[] ipArr = pageHtml.Substring(beginSub, endSub - beginSub).Split(',');
                ip = ipArr[0];
            }
            catch { }
            return ip;
        }

        public static bool IsIPv6
        {
            get
            {
                IPAddress[] address = Dns.GetHostAddresses("www.baidu.com");
                return address[0].AddressFamily == AddressFamily.InterNetworkV6;
            }
        }

    }
}