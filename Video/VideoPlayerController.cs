using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BToolkit
{
    public class VideoPlayerController : MonoBehaviour
    {

        public VideoPlayer videoPlayer;
        public bool canDragProgress;
        public Slider slider;
        float previousX, playProgress;
        Vector2 previousPos;
        bool canProgressUpdate;

        void Awake()
        {
            if (!videoPlayer)
            {
                videoPlayer = GetComponent<VideoPlayer>();
            }
            videoPlayer.prepareCompleted += OnPrepareCompleted;
            videoPlayer.loopPointReached += OnPlayFinished;
        }

        void OnDestroy()
        {
            videoPlayer.prepareCompleted -= OnPrepareCompleted;
            videoPlayer.loopPointReached -= OnPlayFinished;
        }

        void OnPrepareCompleted(VideoPlayer player)
        {
            canProgressUpdate = true;
        }

        void OnPlayFinished(VideoPlayer player)
        {
        }

        public void Play()
        {
            videoPlayer.Play();
        }

        public void Play(VideoSource videoSource, string path)
        {
            videoPlayer.source = videoSource;
            if (videoPlayer.source == VideoSource.VideoClip)
            {

            }
            else if (videoPlayer.source == VideoSource.Url)
            {
                if (!path.Contains("http://") && !path.Contains("file://"))
                {
                    path = "file://" + path;
                }
                videoPlayer.url = path;
            }
            videoPlayer.Play();
        }

        public void Pause()
        {
            videoPlayer.Pause();
        }

        void Update()
        {
            if (canDragProgress)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    canProgressUpdate = false;
                    previousX = Input.mousePosition.x / (float)Screen.width;
                    previousPos = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
                }
                if (Input.GetMouseButton(0))
                {
                    float deltaX = Input.mousePosition.x / (float)Screen.width - previousX;
                    previousX = Input.mousePosition.x / (float)Screen.width;
                    playProgress += deltaX;
                    if (playProgress > 1f)
                    {
                        playProgress = 1f;
                    }
                    else if (playProgress < 0f)
                    {
                        playProgress = 0f;
                    }
                    return;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    Vector2 currPos = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
                    float distance = Vector2.Distance(currPos, previousPos);
                    if (distance < 0.01f)
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
                    else
                    {
                        int frame = Mathf.FloorToInt(videoPlayer.frameCount * playProgress);
                        videoPlayer.frame = frame;
                        videoPlayer.Play();
                        Invoke("SetPrepared", 0.2f);
                    }
                }
            }
            if (canProgressUpdate)
            {
                playProgress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
            }
            if (slider)
            {
                slider.value = playProgress;
            }
        }

        void SetPrepared()
        {
            canProgressUpdate = true;
        }

        public void Btn_Close()
        {
            Destroy(gameObject);
        }

    }
}