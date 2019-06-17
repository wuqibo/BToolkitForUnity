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
            unityPlayer = GetComponent<VideoPlayer>();
            isAVProPlayer = false;
        }

        void Update()
        {
            if (!meshRenderer.enabled)
            {
                if (unityPlayer.frame > 1)
                {
                    meshRenderer.enabled = true;
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
            meshRenderer.enabled = false;
            unityPlayer.frame = 0;
            unityPlayer.source = VideoSource.Url;
            unityPlayer.url = videoUrl;
            unityPlayer.Play();
        }
    }
}