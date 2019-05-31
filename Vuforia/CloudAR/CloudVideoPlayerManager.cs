using UnityEngine;

namespace BToolkit
{
    public class CloudVideoPlayerManager : CloudShowTarget
    {
        public CloudVideoPlayer unityPlayer;
        public CloudVideoPlayer avProPlayer;
        public CloudVideoPlayer CurrPlayer { get; private set; }
        string currPlayingVideoUrl = "";

        void Awake()
        {
            unityPlayer.RegisterPlayedEvent(OnPrepared);
            avProPlayer.RegisterPlayedEvent(OnPrepared);
        }

        public override void PlayTarget(CloudImageTarget cloudImageTarget, MoJingTargetInfo info)
        {
            if (gameObject.activeInHierarchy && info.showFile.Equals(currPlayingVideoUrl))
            {
                return;
            }
            currPlayingVideoUrl = info.showFile;
            base.PlayTarget(cloudImageTarget, info);
            SetTranform(info);
            if ("webm".Equals(info.videoAlphaType))
            {
                avProPlayer.gameObject.SetActive(false);
                CurrPlayer = unityPlayer;
            }
            else
            {
                unityPlayer.gameObject.SetActive(false);
                CurrPlayer = avProPlayer;
            }
            CurrPlayer.gameObject.SetActive(true);
            string parsedPath = CloudFileDownloader.ParsePath(info.showFile);
            Debug.Log("<color=yellow>下载Url:" + parsedPath + "</color>");
            CurrPlayer.Play(parsedPath);
            CloudFileDownloader.Save(info.showFile);
        }

        void SetTranform(MoJingTargetInfo info)
        {
            string[] datasArr = info.showFileRect.Split('|');
            float leftRatio = 0, topRatio = 0, wRatio = 1, hRatio = 1;
            if (datasArr.Length == 4)
            {
                float.TryParse(datasArr[0], out leftRatio);
                float.TryParse(datasArr[1], out topRatio);
                float.TryParse(datasArr[2], out wRatio);
                float.TryParse(datasArr[3], out hRatio);
            }

            float scaleX = 1, scaleY = 1;
            string[] videoSizeArr = info.targetImgSize.Split('X');
            if (videoSizeArr.Length == 2 && !string.IsNullOrEmpty(videoSizeArr[0]) && !string.IsNullOrEmpty(videoSizeArr[1]))
            {
                float.TryParse(videoSizeArr[0], out scaleX);
                float.TryParse(videoSizeArr[1], out scaleY);
            }
            Vector3 scale = Vector3.one;
            if (scaleX > scaleY)
            {
                scale.y = scaleY / scaleX;
            }
            else
            {
                scale.x = scaleX / scaleY;
            }
            scale *= info.showFilePercent * 1.01f / 100f;
            scale.x *= wRatio;
            scale.y *= hRatio;
            transform.localScale = scale;
            Vector3 pos = transform.localPosition;
            pos.x += transform.localScale.x * leftRatio;
            pos.z -= transform.localScale.y * topRatio;
            transform.localPosition = pos;
        }

        void OnPrepared()
        {
            cloudImageTarget.loading.SetActive(false);
        }

        public override void OnTrackingFound()
        {
            base.OnTrackingFound();
            CloudUIShowCtrller.Destroy();
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
                    float videoW = CurrPlayer.videoW;
                    float videoH = CurrPlayer.videoH;
                    bool isAVProPlayer = CurrPlayer.isAVProPlayer;
                    CloudOffCardCtrl showTarget = GetComponent<CloudOffCardCtrl>();
                    showTarget.ToScreen(videoW, videoH, isAVProPlayer);
                    CloudUIShowCtrller.Show(cloudImageTarget, showTarget);
                }
            }
        }
    }
}