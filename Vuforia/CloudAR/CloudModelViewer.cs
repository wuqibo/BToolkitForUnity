using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace BToolkit
{
    public class CloudModelViewer : CloudShowTarget
    {
        public static readonly Vector3 DefaultScale = Vector3.one * 0.75f;
        GameObject target;

        public override void PlayTarget(CloudImageTarget cloudImageTarget, MoJingTargetInfo info)
        {
            base.PlayTarget(cloudImageTarget, info);
            transform.localScale = DefaultScale;
            if (target)
            {
                Destroy(target);
            }
            //安卓苹果地址拆分
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
                if (Application.platform == RuntimePlatform.Android)
                {
                    httpUrl = httpUrlRoot + fileNameAndroid;
                }
                else
                {
                    httpUrl = httpUrlRoot + fileNameIOS;
                }
            }

            string parsedPath = CloudFileDownloader.ParsePath(httpUrl);
            Debug.Log("<color=yellow>下载Url:" + parsedPath + "</color>");
            StartCoroutine(LoadModel(parsedPath));
            CloudFileDownloader.Save(httpUrl);
        }
        IEnumerator LoadModel(string url)
        {
            //Debuger.LogError("url:" + url);
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.chunkedTransfer = false;
            yield return request.SendWebRequest();
            if (request.error != null)
            {
                Debuger.LogError("FileDownloader:" + request.error);
            }
            cloudImageTarget.loading.SetActive(false);
            AssetBundle bundle = AssetBundle.LoadFromMemory(request.downloadHandler.data);
            string[] names = bundle.GetAllAssetNames();
            int length = names.Length;
            for (int i = 0; i < length; i++)
            {
                string[] strArr = names[i].Split('.');
                if ("prefab".Equals(strArr[strArr.Length - 1]))
                {
                    GameObject prefab = bundle.LoadAsset<GameObject>(names[i]);
                    target = Instantiate(prefab);
                    target.transform.SetParent(transform, false);
                    break;
                }
            }
            bundle.Unload(false);
        }

        public override void OnTrackingLost()
        {
            base.OnTrackingLost();
            if (StorageManager.Instance.IsARHideWhenOffCard)
            {
                Show(false);
            }
            else
            {
                if (gameObject.activeInHierarchy)
                {
                    CloudOffCardCtrl showTarget = GetComponent<CloudOffCardCtrl>();
                    showTarget.ToScreen();
                    CloudUIShowCtrller.Show(showTarget);
                }
            }
        }

    }
}