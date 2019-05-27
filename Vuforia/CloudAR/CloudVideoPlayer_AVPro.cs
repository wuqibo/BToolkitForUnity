using RenderHeads.Media.AVProVideo;
using System;
using UnityEngine;

namespace BToolkit
{
    public class CloudVideoPlayer_AVPro : CloudVideoPlayer
    {
        MediaPlayer avProPlayer;

        void Awake()
        {
            isAVProPlayer = true;
            avProPlayer = GetComponent<MediaPlayer>();
        }

        void Update()
        {
            if (canListenPlayed)
            {
                if (avProPlayer.Control != null && avProPlayer.Control.GetCurrentTimeMs() > 5)
                {
                    canListenPlayed = false;
                    avProPlayer.GetComponent<MeshRenderer>().enabled = true;
                    //TODO:AVPro获取视频尺寸方法不详，暂用缩放比例代替
                    videoW = transform.parent.localScale.x;
                    videoH = transform.parent.localScale.y;
                    if (PlayedAction != null)
                    {
                        PlayedAction();
                    }
                }
            }
        }

        public override void Play(string videoUrl)
        {
            avProPlayer.GetComponent<MeshRenderer>().enabled = false;
            avProPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, videoUrl);
            canListenPlayed = true;
        }
    }
}