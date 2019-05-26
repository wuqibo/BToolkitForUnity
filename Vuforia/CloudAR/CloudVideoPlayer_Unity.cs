using System;
using UnityEngine;
using UnityEngine.Video;

namespace BToolkit
{
    public class CloudVideoPlayer_Unity : CloudVideoPlayer
    {
        VideoPlayer unityPlayer;

        void Awake()
        {
            isAVProPlayer = false;
            unityPlayer = GetComponent<VideoPlayer>();
        }

        void Update()
        {
            if (canListenPlayed)
            {
                if (unityPlayer.frame > 1)
                {
                    canListenPlayed = false;
                    unityPlayer.GetComponent<MeshRenderer>().enabled = true;
                    videoW = unityPlayer.texture.width;
                    videoH = unityPlayer.texture.height;
                    if (PlayedAction != null)
                    {
                        PlayedAction();
                    }
                }
            }
        }

        public override void Play(string videoUrl)
        {
            unityPlayer.GetComponent<MeshRenderer>().enabled = false;
            unityPlayer.frame = 0;
            unityPlayer.source = VideoSource.Url;
            unityPlayer.url = videoUrl;
            unityPlayer.Play();
            canListenPlayed = true;
        }
    }
}