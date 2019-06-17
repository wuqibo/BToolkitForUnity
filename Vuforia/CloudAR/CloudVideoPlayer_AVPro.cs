#define AVProPlayer

#if AVProPlayer
using RenderHeads.Media.AVProVideo;
using UnityEngine;

namespace BToolkit
{
    [RequireComponent(typeof(MediaPlayer))]
    public class CloudVideoPlayer_AVPro : CloudVideoPlayer
    {
        MediaPlayer avProPlayer;

        void Awake()
        {
            avProPlayer = GetComponent<MediaPlayer>();
            isAVProPlayer = true;
        }

        void Update()
        {
            if (!meshRenderer.enabled)
            {
                if (avProPlayer.Control != null && avProPlayer.Control.GetCurrentTimeMs() > 5)
                {
                    meshRenderer.enabled = true;
                    videoW = avProPlayer.Info.GetVideoWidth();
                    videoH = avProPlayer.Info.GetVideoHeight() * 0.5f;//AVPro的视频是上下分屏遮罩效果
                    if (PlayedAction != null)
                    {
                        PlayedAction();
                    }
                }
            }
        }

        public override void Play(string videoUrl)
        {
            meshRenderer.enabled = false;
            avProPlayer.GetComponent<MeshRenderer>().enabled = false;
            avProPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, videoUrl);
        }
    }
}
#endif