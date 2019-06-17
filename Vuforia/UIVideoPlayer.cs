#define AVProPlayer

#if AVProPlayer
using RenderHeads.Media.AVProVideo;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BToolkit
{
    public class UIVideoPlayer : MonoBehaviour
    {
        public VideoPlayer unityPlayer;
#if AVProPlayer
        public MediaPlayer avProPlayer;
#endif
        public BButton btnClose;
        public BButton btnDirection;
        public GameObject loading;
        public static UIVideoPlayer instance;
        RectTransform currVideoPlayerTrans;
        bool hadShowImage;
        float videoW, videoH;

        public static void Show(string videoUrl, bool isAVProPlayer)
        {
            if (!instance)
            {
                instance = Instantiate(Resources.Load<UIVideoPlayer>("UIVideoPlayer"));
            }
            instance.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            instance.unityPlayer.GetComponent<RawImage>().enabled = false;
            instance.hadShowImage = false;
            instance.unityPlayer.gameObject.SetActive(!isAVProPlayer);
#if AVProPlayer
            instance.avProPlayer.GetComponent<DisplayUGUI>().enabled = false;
            instance.avProPlayer.gameObject.SetActive(isAVProPlayer);
            instance.currVideoPlayerTrans = isAVProPlayer ? instance.avProPlayer.transform as RectTransform : instance.unityPlayer.transform as RectTransform;
#else
            instance.currVideoPlayerTrans = instance.unityPlayer.transform as RectTransform;
#endif
            instance.Play(videoUrl);
        }

        void OnDestroy()
        {
            unityPlayer.prepareCompleted -= OnVideoPlayerPrepareCompleted;
#if AVProPlayer
            avProPlayer.Events.RemoveListener(OnAVProPlayerEvent);
#endif
        }

        void Awake()
        {
            if (btnClose)
            {
                btnClose.onTrigger.AddListener(() =>
                {
                    Destroy(gameObject);
                });
            }
            if (btnDirection)
            {
                btnDirection.onTrigger.AddListener(() =>
                {
                    ChangeDirection();
                });
            }
            unityPlayer.prepareCompleted += OnVideoPlayerPrepareCompleted;
#if AVProPlayer
            avProPlayer.Events.AddListener(OnAVProPlayerEvent);
#endif
        }

        void Update()
        {
            if (!hadShowImage)
            {
                if (unityPlayer.gameObject.activeSelf)
                {
                    if (unityPlayer.frame > 5)
                    {
                        unityPlayer.GetComponent<RawImage>().enabled = true;
                        hadShowImage = true;
                    }
                }
#if AVProPlayer
                else if (avProPlayer.gameObject.activeSelf)
                {
                    if (avProPlayer.Control != null && avProPlayer.Control.GetCurrentTimeMs() > 5)
                    {
                        avProPlayer.GetComponent<DisplayUGUI>().enabled = true;
                        hadShowImage = true;
                    }
                }
#endif
            }
            if (loading && loading.activeSelf)
            {
                loading.transform.Rotate(0, 0, -300 * Time.deltaTime);
            }
        }

        private void OnVideoPlayerPrepareCompleted(VideoPlayer source)
        {
            SetPlayerTransSize(unityPlayer.texture.width, unityPlayer.texture.height);
            if (loading)
            {
                Destroy(loading);
            }
        }

#if AVProPlayer
        private void OnAVProPlayerEvent(MediaPlayer player, MediaPlayerEvent.EventType eventType, ErrorCode code)
        {
            if (eventType == MediaPlayerEvent.EventType.Started)
            {
                SetPlayerTransSize(player.Info.GetVideoWidth(), player.Info.GetVideoHeight() * 0.5f);
                if (loading)
                {
                    Destroy(loading);
                }
            }
        }
#endif

        private void Play(string videoUrl)
        {
            if (loading)
            {
                loading.SetActive(true);
            }
            Debuger.Log("<color=yellow>播放Url: " + videoUrl + "</color>");

            if (unityPlayer.gameObject.activeSelf)
            {
                unityPlayer.url = videoUrl;
                unityPlayer.Play();
            }
            else
            {
#if AVProPlayer
                avProPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, videoUrl);
#endif
            }
        }

        void ChangeBtnsPos()
        {
            if (currVideoPlayerTrans.localEulerAngles.z == 0)
            {
                //按钮
                if (btnClose)
                {
                    RectTransform btnCloseTrans = btnClose.rectTransform;
                    btnCloseTrans.anchorMin = new Vector2(0, 1);
                    btnCloseTrans.anchorMax = new Vector2(0, 1);
                    btnCloseTrans.anchoredPosition = new Vector2(75, -70);
                    btnCloseTrans.localEulerAngles = new Vector3(0, 0, 0);
                }
                if (btnDirection)
                {
                    RectTransform btnDirectionTrans = btnDirection.rectTransform;
                    btnDirectionTrans.anchorMin = new Vector2(1, 1);
                    btnDirectionTrans.anchorMax = new Vector2(1, 1);
                    btnDirectionTrans.anchoredPosition = new Vector2(-85, -70);
                    btnDirectionTrans.localEulerAngles = new Vector3(0, 0, 0);
                }
            }
            else
            {
                //按钮
                if (btnClose)
                {
                    RectTransform btnCloseTrans = btnClose.rectTransform;
                    btnCloseTrans.anchorMin = new Vector2(1, 1);
                    btnCloseTrans.anchorMax = new Vector2(1, 1);
                    btnCloseTrans.anchoredPosition = new Vector2(-75, -80);
                    btnCloseTrans.localEulerAngles = new Vector3(0, 0, -90);
                }
                if (btnDirection)
                {
                    RectTransform btnDirectionTrans = btnDirection.rectTransform;
                    btnDirectionTrans.anchorMin = new Vector2(1, 0);
                    btnDirectionTrans.anchorMax = new Vector2(1, 0);
                    btnDirectionTrans.anchoredPosition = new Vector2(-75, 80);
                    btnDirectionTrans.localEulerAngles = new Vector3(0, 0, -90);
                }
            }
        }

        void SetPlayerTransSize(float videoW, float videoH)
        {
            this.videoW = videoW;
            this.videoH = videoH;
            if (currVideoPlayerTrans.localEulerAngles.z == 0)
            {
                //视频
                float screenUIWidth = 1080;
                float videoHeight = screenUIWidth * videoH / videoW;
                currVideoPlayerTrans.sizeDelta = new Vector2(screenUIWidth, videoHeight);
            }
            else
            {
                //视频
                float screenUIWidth = 1080;
                float videoWidth = screenUIWidth * videoW / videoH;
                currVideoPlayerTrans.sizeDelta = new Vector2(videoWidth, screenUIWidth);
            }
            ChangeBtnsPos();
        }

        void ChangeDirection()
        {
            currVideoPlayerTrans.localEulerAngles = new Vector3(0, 0, currVideoPlayerTrans.localEulerAngles.z == 0 ? -90 : 0);
            SetPlayerTransSize(this.videoW, this.videoH);
        }

    }
}