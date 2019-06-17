using LitJson;
using System;
using UnityEngine;

namespace BToolkit
{
    public class CloudServerAPI
    {
        private static CloudServerAPI instance;
        public static CloudServerAPI Instance { get { return instance ?? (instance = new CloudServerAPI()); } }

        public const string ServerUrl = "https://www.moon-ar.com:8091/cloudar";
#if UNITY_ANDROID
        public const string VersionConfigPath = "https://www.moon-ar.com:8091/cloudar/appdownload/androidversion.json";
#elif UNITY_IOS
    public const string VersionConfigPath = "https://www.moon-ar.com:8091/cloudar/appdownload/iosversion.json";
#endif

        /// <summary>
        /// 根据识别的TargetId读取对象详细信息
        /// </summary>
        public void LoadTargetInfo(string targetId, Action<CloudTargetInfo> Callback)
        {
            string url = ServerUrl + "/admin/api/readshowing.php?targetId=" + targetId;
            Http.Get(url, (Http.ResultType resultType, string result) =>
            {
                Debug.Log("<color=yellow>>>>>>>>>>>> HttpCallback: " + resultType + " result: " + result + "</color>");
                result = result.Replace("000", "");
                switch (resultType)
                {
                    case Http.ResultType.NoNetwork:
                        DialogAlert.Show("请将手机连接网络!", 2f);
                        Callback(null);
                        break;
                    case Http.ResultType.LoadFailed:
                        DialogAlert.Show("读取网络数据失败!", 2f);
                        Callback(null);
                        break;
                    case Http.ResultType.Success:
                        JsonData jsonData = JsonMapper.ToObject(result);
                        int errorCode = (int)jsonData["error"];
                        if (errorCode == 0)
                        {
                            JsonData data = jsonData["data"];
                            string state = (string)data["state"];
                            if ("completed".Equals(state))
                            {
                                CloudTargetInfo info = new CloudTargetInfo();
                                info.id = (string)data["id"];
                                info.targetId = (string)data["targetId"];
                                info.account = (string)data["account"];
                                info.imgName = (string)data["imgName"];
                                info.targetImgSize = (string)data["targetImgSize"];
                                info.showType = (string)data["showType"];
                                info.videoAlphaType = (string)data["videoAlphaType"];
                                info.showFile = (string)data["showFile"];
                                info.showFileSize = (string)data["showFileSize"];
                                info.showFilePercent = float.Parse((string)data["showFilePercent"]);
                                info.showFileRect = (string)data["showFileRect"];
                                Callback(info);
                            }
                            else if ("actived".Equals(state))
                            {
                                DialogAlert.Show("该云图未绑定AR对象!", 2f);
                                Callback(null);
                            }
                            else
                            {
                                DialogAlert.Show("该云图未生效!", 2f);
                                Callback(null);
                            }
                        }
                        else
                        {
                            DialogAlert.Show("TargetId不存在!", 2f);
                            Callback(null);
                        }
                        break;
                }
            });
        }
    }
}