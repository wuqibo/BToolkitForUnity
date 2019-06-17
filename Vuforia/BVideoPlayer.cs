using UnityEngine;
using UnityEngine.Video;

namespace BToolkit
{
    [RequireComponent(typeof(OffCardController_Video))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(VideoPlayer))]
    public class BVideoPlayer : MonoBehaviour
    {
        VideoPlayer vp;
        public VideoPlayer videoPlayer { get { return vp ?? (vp = GetComponent<VideoPlayer>()); } }
        public OffCardController_Video offCardController { get; private set; }
        public MeshRenderer playerMesh { get; private set; }
        public VideoSource source { set { videoPlayer.source = value; } }
        public bool playOnAwake { set { videoPlayer.playOnAwake = value; } }
        public string url { set { videoPlayer.url = value; } }
        public long frame { get { return videoPlayer.frame; } }
        public void Play() { videoPlayer.Play(); }
        protected bool videoPrepareCompleted;
        bool hadFoundOnce;

        void OnDestroy()
        {
            videoPlayer.prepareCompleted -= OnVideoPlayerPrepareCompleted;
        }

        void Awake()
        {
            offCardController = GetComponent<OffCardController_Video>();
            playerMesh = GetComponent<MeshRenderer>();
            videoPlayer.prepareCompleted += OnVideoPlayerPrepareCompleted;
        }

        void Update()
        {
            if (!playerMesh.enabled && videoPlayer.isPlaying)
            {
                if (videoPlayer.frame > 1)
                {
                    playerMesh.enabled = true;
                }
            }
        }

        void OnVideoPlayerPrepareCompleted(VideoPlayer source)
        {
            videoPrepareCompleted = true;
        }

        public void OnTrackingFound()
        {
            hadFoundOnce = true;
            gameObject.SetActive(true);
            if (playerMesh)
            {
                playerMesh.enabled = false;
            }
            offCardController.ToTracking();
        }

        public void OnTrackingLost()
        {
            if (!hadFoundOnce)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (StorageManager.Instance.IsARHideWhenOffCard)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    offCardController.ToScreen(videoPlayer.texture.width, videoPlayer.texture.height, false);
                }
            }
        }

    }
}