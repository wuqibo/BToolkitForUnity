using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace BToolkit
{
    [RequireComponent(typeof(OffCardController_Model))]
    public class CloudModelViewer : MonoBehaviour
    {
        public static readonly Vector3 DefaultScale = Vector3.one * 0.75f;
        OffCardController_Model offCardController;
        GameObject target;

        void Awake()
        {
            offCardController = GetComponent<OffCardController_Model>();
        }

        public void PlayTarget(CloudTargetInfo info)
        {
            transform.localScale = DefaultScale;
            if (target)
            {
                Destroy(target);
            }
            string httpUrl = GetModelUrlFromAndroidIOSBlend(info);
            string parsedPath = CloudFileDownloader.ParseURL(httpUrl);
            Debug.Log("<color=yellow>下载Url:" + parsedPath + "</color>");
            StartCoroutine(LoadModel(parsedPath, (GameObject prefab) =>
            {
                VuforiaHelper.LoadingActiveAction(false);
                target = Instantiate(prefab);
                target.transform.SetParent(transform, false);
            }));
            CloudFileDownloader.Save(httpUrl);
        }
        public static IEnumerator LoadModel(string url, Action<GameObject> CallbackPrefab)
        {
            //Debuger.LogError("url:" + url);
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.chunkedTransfer = false;
            yield return request.SendWebRequest();
            if (request.error != null)
            {
                Debuger.LogError("FileDownloader:" + request.error);
            }
            AssetBundle bundle = AssetBundle.LoadFromMemory(request.downloadHandler.data);
            string[] names = bundle.GetAllAssetNames();
            int length = names.Length;
            for (int i = 0; i < length; i++)
            {
                string[] strArr = names[i].Split('.');
                if ("prefab".Equals(strArr[strArr.Length - 1]))
                {
                    GameObject prefab = bundle.LoadAsset<GameObject>(names[i]);
                    CallbackPrefab(prefab);
                    break;
                }
            }
            bundle.Unload(false);
        }

        public static string GetModelUrlFromAndroidIOSBlend(CloudTargetInfo info)
        {
            string httpUrl = info.showFile;
            if (httpUrl.Contains("|"))
            {
                string[] httpUrlArr = httpUrl.Split('/');
                string httpUrlRoot = "";
                for (int i = 0; i < httpUrlArr.Length - 1; i++)
                {
                    httpUrlRoot += httpUrlArr[i] + "/";
                }
                string[] fileNameArr = httpUrlArr[httpUrlArr.Length - 1].Split('|');
                string fileNameAndroid = fileNameArr[0];
                string fileNameIOS = fileNameArr[1];
#if UNITY_ANDROID
                httpUrl = httpUrlRoot + fileNameAndroid;
#elif UNITY_IOS
                httpUrl = httpUrlRoot + fileNameIOS;
#endif
            }
            return httpUrl;
        }

        public void OnTrackingFound()
        {
            offCardController.ToTracking();
        }

        public void OnTrackingLost()
        {
            if (StorageManager.Instance.IsARHideWhenOffCard)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (gameObject.activeInHierarchy)
                {
                    offCardController.ToScreen(true);
                }
            }
        }

    }
}