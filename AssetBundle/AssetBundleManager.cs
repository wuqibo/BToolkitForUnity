using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using LitJson;

namespace BToolkit
{
    public class AssetBundleManager
    {

        public enum ResuleType
        {
            NoNetwork,
            DownloadJsonFailed,
            DownloadJsonComplete,
            FirstTime,
            LocalVersionLess,
            LocalVersionSameOrLarge,
            DownloadingAsset,
            DownloadAssetFailed,
            DownloadAssetComplete
        }

        public delegate void DownloadDelegate(ResuleType assetResuleType, float progress);
        public bool hasDownloaded;
        DownloadDelegate DownloadCallback;
        const string KeyExtraForVersion = "version", KeyExtraForUrl = "url";
        string jsonFileURL;
        UnityWebRequest assetRequest;
        DownloadHandlerAssetBundle downloadHandlerAssetBundle;
        int localVersion, newVersion;
        string assetURL;
        const bool cleanCacheOnEditor = true;
        ResuleType jsonResultType;
        bool canProgressUpdate;

        public AssetBundleManager(string jsonFileURL)
        {
            this.jsonFileURL = jsonFileURL;
            if (Application.isEditor && cleanCacheOnEditor)
            {
                UnityEngine.PlayerPrefs.SetInt(jsonFileURL + KeyExtraForVersion, 0);
                UnityEngine.PlayerPrefs.SetString(jsonFileURL + KeyExtraForUrl, "");
                Caching.ClearCache();
            }
        }

        public void StartLoad(DownloadDelegate DownloadCallback)
        {
            hasDownloaded = false;
            this.DownloadCallback = DownloadCallback;

            localVersion = UnityEngine.PlayerPrefs.GetInt(jsonFileURL + KeyExtraForVersion, -1);
            assetURL = UnityEngine.PlayerPrefs.GetString(jsonFileURL + KeyExtraForUrl);

            Debug.Log(".--------------------------------.");
            Debug.Log("assetVersion: " + localVersion);
            Debug.Log("assetURL: " + assetURL);
            Debug.Log("`--------------------------------`");

            BUtils.Instance.StartCoroutine(DownloadJson());
        }

        IEnumerator DownloadJson()
        {
            if (!BUtils.IsNetWorkOK)
            {
                if (string.IsNullOrEmpty(assetURL))
                {
                    Debug.Log(">>>>>>>>>>>>>>>>> No network and no localed asset");
                    DownloadCallback(ResuleType.NoNetwork, 0f);
                    yield break;
                }
                else
                {
                    jsonResultType = ResuleType.NoNetwork;
                    Debug.Log(">>>>>>>>>>>>>>>>> No network but try to read local");
                    StartLoadAssetBundle();
                }
            }
            else
            {
                UnityWebRequest jsonRequest = UnityWebRequest.Get(jsonFileURL);
                yield return jsonRequest.Send();
                if (!jsonRequest.isNetworkError)
                {
                    if (string.IsNullOrEmpty(assetURL))
                    {
                        Debug.Log(">>>>>>>>>>>>>>>>> Download json failed and no localed asset");
                        DownloadCallback(ResuleType.DownloadJsonFailed, 0f);
                        yield break;
                    }
                    else
                    {
                        jsonResultType = ResuleType.DownloadJsonFailed;
                        Debug.Log(">>>>>>>>>>>>>>>>> Download json failed but try to read local");
                        StartLoadAssetBundle();
                    }
                }
                else
                {
                    jsonResultType = ResuleType.DownloadJsonComplete;
                    DownloadCallback(ResuleType.DownloadJsonComplete, 0f);
                    JsonData jsonData = JsonMapper.ToObject(jsonRequest.downloadHandler.text);
                    string urlKey = "ios_url";
                    string versionKey = "ios_version";
#if UNITY_ANDROID
                    urlKey = "android_url";
                    versionKey = "android_version";
#endif
                    assetURL = (string)jsonData[urlKey];
                    newVersion = (int)jsonData[versionKey];
                    UnityEngine.PlayerPrefs.SetString(jsonFileURL + KeyExtraForUrl, assetURL);
                    if (localVersion == -1)
                    {
                        DownloadCallback(ResuleType.FirstTime, 0f);
                    }
                    else if (localVersion < newVersion)
                    {
                        localVersion = newVersion;
                        DownloadCallback(ResuleType.LocalVersionLess, 0f);
                    }
                    else
                    {
                        DownloadCallback(ResuleType.LocalVersionSameOrLarge, 0f);
                    }
                    StartLoadAssetBundle();
                }
            }
        }

        void StartLoadAssetBundle()
        {
            hasDownloaded = false;
            DownloadCallback(ResuleType.DownloadingAsset, 0f);
            BUtils.Instance.StartCoroutine(StartDownloadAsset());
        }

        IEnumerator StartDownloadAsset()
        {
            if (assetRequest == null)
            {
                BUtils.Instance.StartCoroutine(ProgressUpdate());
                Debug.Log("Download assets : url: " + assetURL + " version: " + localVersion);
                assetRequest = UnityWebRequestAssetBundle.GetAssetBundle(assetURL, (uint)localVersion, 0);
                downloadHandlerAssetBundle = (DownloadHandlerAssetBundle)assetRequest.downloadHandler;
                yield return assetRequest.Send();
                if (assetRequest.isNetworkError)
                {
                    if (jsonResultType == ResuleType.NoNetwork)
                    {
                        Debug.LogError("Load assets failed, because no network:" + assetRequest.error);
                        DownloadCallback(ResuleType.NoNetwork, 0f);
                    }
                    else if (jsonResultType == ResuleType.DownloadJsonFailed)
                    {
                        Debug.LogError("Load assets failed, because download json failed:" + assetRequest.error);
                        DownloadCallback(ResuleType.DownloadJsonFailed, 0f);
                    }
                    else
                    {
                        Debug.LogError("Load assets failed:" + assetRequest.error);
                        DownloadCallback(ResuleType.DownloadAssetFailed, 0f);
                    }
                    assetRequest = null;
                }
                else
                {
                    Debug.Log("Download assets complete!");
                    UnityEngine.PlayerPrefs.SetInt(jsonFileURL + KeyExtraForVersion, localVersion);
                    hasDownloaded = true;
                    DownloadCallback(ResuleType.DownloadingAsset, 1f);
                    DownloadCallback(ResuleType.DownloadAssetComplete, 1f);
                }
                canProgressUpdate = false;
            }
            else
            {
                DownloadCallback(ResuleType.DownloadAssetComplete, 1f);
            }
        }

        IEnumerator ProgressUpdate()
        {
            canProgressUpdate = true;
            while (canProgressUpdate)
            {
                if (assetRequest != null)
                {
                    DownloadCallback(ResuleType.DownloadingAsset, assetRequest.downloadProgress);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        public AssetBundle AssetBundle
        {
            get
            {
                if (assetRequest != null && downloadHandlerAssetBundle != null)
                {
                    return downloadHandlerAssetBundle.assetBundle;
                }
                return null;
            }
        }

        public T LoadAsset<T>(string fullPath) where T : Object
        {
            if (assetRequest != null && downloadHandlerAssetBundle != null)
            {
                return downloadHandlerAssetBundle.assetBundle.LoadAsset<T>(fullPath);
            }
            Debug.LogError("Not loaded resources!");
            return default(T);
        }

        public void UnloadAllAssetBundle()
        {
            if (assetRequest != null)
            {
                if (downloadHandlerAssetBundle != null)
                {
                    downloadHandlerAssetBundle.assetBundle.Unload(true);
                    downloadHandlerAssetBundle = null;
                }
                assetRequest = null;
            }
        }

        public void Cancel()
        {
            UnloadAllAssetBundle();
        }
    }
}