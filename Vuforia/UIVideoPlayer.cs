using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BToolkit
{
    public class UIVideoPlayer : MonoBehaviour
    {
        public VideoPlayer unityPlayer;
        public BButton btnClose;
        public BButton btnDirection;
        public GameObject loading;
        bool hadShowImage;

        public static UIVideoPlayer Show(string videoUrl, long startFrame)
        {
            UIVideoPlayer player = Instantiate(Resources.Load<UIVideoPlayer>("Prefabs/UI/UIVideoPlayer"));
            player.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            player.unityPlayer.GetComponent<RawImage>().enabled = false;
            player.hadShowImage = false;
            player.Play(videoUrl, startFrame);
            return player;
        }

        void OnDestroy()
        {
            unityPlayer.prepareCompleted -= OnVideoPlayerPrepareCompleted;
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
        }

        void Update()
        {
            if (!hadShowImage)
            {
                if (unityPlayer.frame > 5)
                {
                    unityPlayer.GetComponent<RawImage>().enabled = true;
                }
            }
        }

        private void OnVideoPlayerPrepareCompleted(VideoPlayer source)
        {
            SetPlayerTransSize();
            if (loading)
            {
                Destroy(loading);
            }
        }

        private void Play(string videoUrl, long startFrame)
        {
            unityPlayer.url = videoUrl;
            if (startFrame > 0)
            {
                unityPlayer.frame = startFrame;
            }
            if (!unityPlayer.isPlaying)
            {
                unityPlayer.Play();
            }
        }

        void SetPlayerTransSize()
        {
            if (unityPlayer.transform.localEulerAngles.z == 0)
            {
                //视频
                RectTransform playerTrans = unityPlayer.transform as RectTransform;
                float screenUIWidth = 1080;
                float videoHeight = screenUIWidth * unityPlayer.texture.height / (float)unityPlayer.texture.width;
                playerTrans.sizeDelta = new Vector2(screenUIWidth, videoHeight);
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
                //视频
                RectTransform playerTrans = unityPlayer.transform as RectTransform;
                float screenUIWidth = 1080;
                float videoWidth = screenUIWidth * unityPlayer.texture.width / (float)unityPlayer.texture.height;
                playerTrans.sizeDelta = new Vector2(videoWidth, screenUIWidth);
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

        void ChangeDirection()
        {
            unityPlayer.transform.localEulerAngles = new Vector3(0, 0, unityPlayer.transform.localEulerAngles.z == 0 ? -90 : 0);
            SetPlayerTransSize();
        }

    }
}