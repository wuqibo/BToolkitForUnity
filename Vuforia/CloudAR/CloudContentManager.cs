using LitJson;
using System.Collections;
using UnityEngine;
using Vuforia;

namespace BToolkit
{
    public class CloudContentManager : MonoBehaviour
    {
        public Shader unlitTex, alphaTex;
        public CloudVideoPlayer videoPlayerPrefab;
        public CloudLoading loadingPrefab;
        public UnityEngine.UI.Button btnClose;
        public GameObject tipUIHolder;
        public static CloudContentManager instance;
        Transform _cameraTrans;
        public Transform CameraTrans
        {
            get
            {
                if (!_cameraTrans)
                {
                    _cameraTrans = FindObjectOfType<VuforiaBehaviour>().transform;
                }
                return _cameraTrans;
            }
        }
        const bool CanOffCard = true;
        CloudVideoPlayer videoPlayer = null;
        GameObject model = null;
        string currTargetId = "";
        bool hasDownloadSuccess;
        FileDownloader.ShowingFile.Type currFileType;

        void OnDestroy()
        {
            instance = null;
            CloudRecognition.OnScanedNewTarget -= OnScanedNewTarget;
            CloudImageTarget.OnTrackingFoundLost -= OnTrackingFoundLost;
        }

        void Awake()
        {
            instance = this;
            CloudRecognition.OnScanedNewTarget += OnScanedNewTarget;
            CloudImageTarget.OnTrackingFoundLost += OnTrackingFoundLost;
            if (btnClose)
            {
                btnClose.onClick.AddListener(() =>
                {
                    DestroyContent();
                    currTargetId = "";
                    CloudRecognition.instance.ResetRecognition();
                    btnClose.gameObject.SetActive(false);
                    if (tipUIHolder)
                    {
                        tipUIHolder.SetActive(true);
                    }
                });
                btnClose.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (videoPlayer)
                {
                    if (videoPlayer.isPlaying)
                    {
                        videoPlayer.Pause();
                    }
                    else
                    {
                        videoPlayer.Play();
                    }
                }
            }
        }

        void OnTrackingFoundLost(bool isFound)
        {
            if (isFound)
            {
                if (tipUIHolder)
                {
                    tipUIHolder.SetActive(false);
                }
            }
            else
            {
                ChangeContentToTracking(false);
                CloudLoading.Show(false);
            }
        }

        void OnScanedNewTarget(TargetFinder.TargetSearchResult targetSearchResult)
        {
            Debug.Log("<color=yellow>OnScanedNewTarget TargetId: " + targetSearchResult.UniqueTargetId + "</color>");
            if (currTargetId.Equals(targetSearchResult.UniqueTargetId))
            {
                Debug.Log("<color=yellow>TargetID没变</color>");
                ChangeContentToTracking(true);
            }
            else
            {
                Debug.Log("<color=yellow>新的TargetID</color>");
                currTargetId = targetSearchResult.UniqueTargetId;
                DestroyContent();
                string url = Config.ServerUrl + "admin/api/readshowing.php?targetId=" + targetSearchResult.UniqueTargetId;
                Debug.Log("url:" + url);
                Http.Get(url, HttpCallback);
            }
        }

        private void HttpCallback(Http.ResultType resultType, string result)
        {
            Debug.Log(">>>>>>>>>>> HttpCallback: " + resultType + " result: " + result);
            result = result.Replace("000", "");
            switch (resultType)
            {
                case Http.ResultType.NoNetwork:
                    DialogAlert.Show("请将手机连接网络!", 2f);
                    break;
                case Http.ResultType.LoadFailed:
                    DialogAlert.Show("读取网络数据失败!", 2f);
                    break;
                case Http.ResultType.Success:
                    JsonData jsonData = JsonMapper.ToObject(result);
                    int errorCode = (int)jsonData["error"];
                    if (errorCode == 0)
                    {
                        JsonData data = jsonData["data"];
                        string state = (string)data["state"];
                        if (state.Equals("completed"))
                        {
                            string videoSize = (string)data["targetImgSize"];
                            string showFile = (string)data["showFile"];
                            string showFileSize = (string)data["showFileSize"];
                            string alphaType = (string)data["videoAlphaType"];
                            Debug.Log("alphaType:" + alphaType);
                            CloudLoading.Show(true, showFileSize);
                            float showFilePercent = float.Parse((string)data["showFilePercent"]);
                            string videoRect = (string)data["showFileRect"];
                            Debug.Log("=================" + showFile);
                            FileDownloader.Instance.DownloadShowingFile(showFile, videoRect, videoSize, showFilePercent, alphaType, LoadedCallback);
                        }
                        else if (state.Equals("actived"))
                        {
                            DialogAlert.Show("该云图未绑定AR对象!", 2f);
                        }
                        else
                        {
                            DialogAlert.Show("该云图未生效!", 2f);
                        }
                    }
                    else
                    {
                        DialogAlert.Show("TargetId不存在!", 2f);
                    }
                    break;
            }
        }

        void LoadedCallback(FileDownloader.ShowingFile.Type type, string url, float leftRatio, float topRatio, float wRatio, float hRatio, Vector3 showScale, string alphaType, bool readFromLocal)
        {
            if (readFromLocal)
            {
                CloudLoading.Show(false);
            }
            hasDownloadSuccess = true;
            this.currFileType = type;
            Debug.Log("LoadedCallback:" + url);
            DestroyContent();
            if (currFileType == FileDownloader.ShowingFile.Type.Video)
            {
                videoPlayer = Instantiate(videoPlayerPrefab);
                videoPlayer.SetVideoAndPlay(url, leftRatio, topRatio, wRatio, hRatio, showScale, alphaType);
            }
            else if (currFileType == FileDownloader.ShowingFile.Type.Model)
            {
                model = new GameObject("ModelsHolder");
                model.transform.localScale = showScale;
                StartCoroutine(ShowModel(url, showScale));
            }
            ChangeContentToTracking(CloudImageTarget.isFound);
        }

        IEnumerator ShowModel(string localPath, Vector3 showScale)
        {
            WWW www = new WWW("file:///" + localPath);
            yield return www;
            if (www.error != null)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string[] names = www.assetBundle.GetAllAssetNames();
                int length = names.Length;
                for (int i = 0; i < length; i++)
                {
                    string[] strArr = names[i].Split('.');
                    if ("prefab".Equals(strArr[strArr.Length - 1]))
                    {
                        GameObject prefab = www.assetBundle.LoadAsset<GameObject>(names[i]);
                        GameObject child = Instantiate(prefab);
                        child.transform.SetParent(model.transform, false);
                        child.transform.localPosition = Vector3.zero;
                        child.transform.localEulerAngles = Vector3.zero;
                        child.transform.localScale = Vector3.one;
                        child.AddComponent<Model>();
                        TouchRotate3DByOneFinger touchRotate3DByOneFinger = child.AddComponent<TouchRotate3DByOneFinger>();
                        touchRotate3DByOneFinger.attachCamera = GameObject.Find("ARCamera").GetComponent<Camera>();
                        touchRotate3DByOneFinger.autoRotate = false;
                        touchRotate3DByOneFinger.xAxisEnable = false;
                        touchRotate3DByOneFinger.dragSpeed = -1;
                        child.AddComponent<TouchScaleByTwoFingers>();
                        www.assetBundle.Unload(false);
                        break;
                    }
                }
            }
        }

        void ChangeContentToTracking(bool b)
        {
            if (b)
            {
                if (videoPlayer)
                {
                    videoPlayer.transform.SetParent(CloudImageTarget.instance.transform, false);
                    videoPlayer.transform.localPosition = Vector3.zero;
                    videoPlayer.transform.localEulerAngles = Vector3.zero;
                    videoPlayer.Play();
                }
                if (model)
                {
                    model.transform.SetParent(CloudImageTarget.instance.transform, false);
                    model.transform.localPosition = Vector3.zero;
                    model.transform.localEulerAngles = Vector3.zero;
                }
                if (btnClose)
                {
                    btnClose.gameObject.SetActive(false);
                }
            }
            else
            {
                if (videoPlayer)
                {
                    videoPlayer.transform.SetParent(CameraTrans, false);
                    videoPlayer.transform.localPosition = new Vector3(0, 0, CloudVideoPlayer.DistanceFromCamera);
                    videoPlayer.transform.localEulerAngles = new Vector3(-90, 0, 0);
                }
                if (model)
                {
                    model.transform.SetParent(CameraTrans, false);
                    model.transform.localPosition = new Vector3(0, 0, 3);
                    model.transform.localEulerAngles = new Vector3(-90, 0, 0);
                }
                if (hasDownloadSuccess)
                {
                    if (btnClose)
                    {
                        btnClose.gameObject.SetActive(true);
                    }
                }
            }
        }

        void DestroyContent()
        {
            if (videoPlayer)
            {
                Destroy(videoPlayer.gameObject);
            }
            if (model)
            {
                Destroy(model);
            }
        }

    }
}