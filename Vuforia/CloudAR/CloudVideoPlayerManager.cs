using UnityEngine;

namespace BToolkit
{
    public class CloudVideoPlayerManager : MonoBehaviour
    {
        public CloudVideoPlayer unityPlayer;
        public CloudVideoPlayer avProPlayer;
        public CloudVideoPlayer CurrPlayer { get; private set; }
        CloudImageTarget cloudImageTarget;

        void Awake()
        {
            unityPlayer.RegisterPlayedEvent(OnPrepared);
            avProPlayer.RegisterPlayedEvent(OnPrepared);
        }

        public void Show(bool b)
        {
            gameObject.SetActive(b);
        }

        public void Play(CloudImageTarget cloudImageTarget, MoJingTargetInfo info)
        {
            this.cloudImageTarget = cloudImageTarget;
            Show(true);
            SetTranform(info);
            if ("webm".Equals(info.alphaType))
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
            CurrPlayer.Play(info.showFileUrl);
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
            string[] videoSizeArr = info.imgSize.Split('X');
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
    }
}