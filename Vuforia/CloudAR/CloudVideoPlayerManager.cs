using UnityEngine;

namespace BToolkit
{
    public class CloudVideoPlayerManager : MonoBehaviour
    {
        public CloudVideoPlayer unityPlayer;
        public CloudVideoPlayer avProPlayer;
        public CloudVideoPlayer CurrPlayer { get; private set; }
        OffCardController_Video offCardController;
        string currPlayingVideoUrl = "";

        void Awake()
        {
            unityPlayer.RegisterPlayedEvent(OnPrepared);
            avProPlayer.RegisterPlayedEvent(OnPrepared);
            offCardController = GetComponent<OffCardController_Video>();
        }

        public void PlayTarget(CloudTargetInfo info)
        {
            if (gameObject.activeInHierarchy && info.showFile.Equals(currPlayingVideoUrl))
            {
                Debuger.Log("<color=yellow>PlayTarget：播放同一个地址且播放器在激活状态，不做处理直接返回</color>");
                return;
            }
            currPlayingVideoUrl = info.showFile;
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
            Debuger.Log("<color=yellow>下载Url:" + info.showFile + "</color>");
            string parsedPath = CloudFileDownloader.ParseURL(info.showFile);
            Debuger.Log("<color=yellow>播放Url:" + parsedPath + "</color>");
            CurrPlayer.Play(parsedPath);
            CloudFileDownloader.Save(info.showFile);
        }

        void SetTranform(CloudTargetInfo info)
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
            VuforiaHelper.LoadingActiveAction(false);
        }

        public void OnTrackingFound()
        {
            unityPlayer.meshRenderer.enabled = false;
            avProPlayer.meshRenderer.enabled = false;
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
                    offCardController.ToScreen(CurrPlayer.videoW, CurrPlayer.videoH, CurrPlayer.isAVProPlayer);
                }
            }
        }
    }
}