using UnityEngine;
using UnityEngine.Video;
using Vuforia;
using System.Collections;

namespace BToolkit
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(VideoPlayer))]
    public class BVideoPlayer : BImageTarget
    {
        public Material arVideoBg;
        public BVideoPlayerUICtrl uICtrlPrefab;

        public VideoSource source { set { videoPlayer.source = value; } }
        public bool playOnAwake { set { videoPlayer.playOnAwake = value; } }
        public string url { set { videoPlayer.url = value; } }
        public long frame { get { return videoPlayer.frame; } }
        public void Play() { videoPlayer.Play(); }

        MeshRenderer currMeshRenderer;
        VideoPlayer vp;
        VideoPlayer videoPlayer { get { return vp ?? (vp = GetComponent<VideoPlayer>()); } }
        Transform arCamera, backgroundPlane;
        GameObject videoBg, videoBgQuad;
        bool videoPrepareCompleted;
        BVideoPlayerUICtrl uICtrl;
        bool hadFoundOnce;

        Transform trackableParent;
        Vector3 trackablePos, trackableAngle, trackableScale;
        bool hadFadeToScreen;
        float bgAlpha;

        void OnDestroy()
        {
            videoPlayer.prepareCompleted -= OnVideoPlayerPrepareCompleted;
        }

        void Awake()
        {
            currMeshRenderer = GetComponent<MeshRenderer>();
            videoPlayer.prepareCompleted += OnVideoPlayerPrepareCompleted;
            trackableParent = transform.parent;
            trackablePos = transform.localPosition;
            trackableAngle = transform.localEulerAngles;
            trackableScale = transform.localScale;
        }

        void Update()
        {
            if (videoBgQuad && bgAlpha < 0.8f)
            {
                bgAlpha += (0.81f - bgAlpha) * 0.02f;
                arVideoBg.SetColor("_Color", new Color(0, 0, 0, bgAlpha));
            }
            if (!currMeshRenderer.enabled)
            {
                if (videoPlayer.frame > 1)
                {
                    currMeshRenderer.enabled = true;
                }
            }
        }

        private void OnVideoPlayerPrepareCompleted(VideoPlayer source)
        {
            videoPrepareCompleted = true;
        }

        /// <summary>
        /// 切换到全屏
        /// </summary>
        public void ToFullScreen()
        {
            if (!videoPrepareCompleted)
            {
                Debuger.LogError(">>>>>>>>>>>视频初始化失败，无法全屏");
                return;
            }
            VuforiaHelper.StopTracker();
            if (!uICtrl && uICtrlPrefab)
            {
                uICtrl = Instantiate(uICtrlPrefab);
                uICtrl.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            }
            uICtrl.videoPlayer = this;
            if (!arCamera)
            {
                arCamera = FindObjectOfType<VuforiaBehaviour>().transform;
            }
            if (!backgroundPlane)
            {
                backgroundPlane = GameObject.Find("BackgroundPlane").transform;
            }
            TrackerToScreen(true);
        }

        /// <summary>
        /// 切换到AR跟踪
        /// </summary>
        public void ToTrackable()
        {
            VuforiaHelper.StartTracker();
            if (uICtrl)
            {
                Destroy(uICtrl.gameObject);
            }
            if (hadFadeToScreen)
            {
                transform.SetParent(trackableParent);
                transform.localPosition = trackablePos;
                transform.localEulerAngles = trackableAngle;
                transform.localScale = trackableScale;
                hadFadeToScreen = false;

                if (videoBg)
                {
                    Destroy(videoBg);
                }
                if (videoBgQuad)
                {
                    Destroy(videoBgQuad);
                }
                bgAlpha = 0;
                arVideoBg.SetColor("_Color", new Color(0, 0, 0, bgAlpha));
            }
        }

        // Mesh必须挂载到当前节点，若用子物体做Mesh，须确保和当前对象相同的Transform参数
        void TrackerToScreen(bool useAnim)
        {
            hadFadeToScreen = true;
            transform.SetParent(arCamera, true);
            transform.localEulerAngles = Vector3.zero;
            float time = 0.5f;
            if (useAnim)
            {
                Tween.Move(0, transform, backgroundPlane.localPosition, time, false, Tween.EaseType.ExpoEaseInOut);
            }
            else
            {
                transform.localPosition = backgroundPlane.localPosition;
            }
            Vector3 toScale = Vector3.zero;
            if (videoPlayer.texture.width / (float)videoPlayer.texture.height > Screen.width / (float)Screen.height)
            {
                //Debuger.LogError("左右贴紧");
                //左右贴紧
                if (Screen.width < Screen.height)
                {
                    //竖屏
                    float scaleX = 2 * backgroundPlane.localScale.z * Screen.width / (float)Screen.height;
                    float scaleY = scaleX * videoPlayer.texture.height / (float)videoPlayer.texture.width;
                    toScale = new Vector3(scaleX, scaleY, 1);
                }
                else
                {
                    //横屏
                    float scaleX = 2 * backgroundPlane.localScale.x;
                    float scaleY = scaleX * videoPlayer.texture.height / (float)videoPlayer.texture.width;
                    toScale = new Vector3(scaleX, scaleY, 1);
                }
            }
            else
            {
                //Debuger.LogError("上下贴紧");
                //上下贴紧
                if (Screen.width < Screen.height)
                {
                    //竖屏
                    float scaleY = 2 * backgroundPlane.localScale.z;
                    float scaleX = scaleY * videoPlayer.texture.width / (float)videoPlayer.texture.height;
                    toScale = new Vector3(scaleX, scaleY, 1);
                }
                else
                {
                    //横屏
                    float screenH = backgroundPlane.localScale.x * Screen.height / (float)Screen.width;
                    float scaleY = 2 * screenH;
                    float scaleX = scaleY * videoPlayer.texture.width / (float)videoPlayer.texture.height;
                    toScale = new Vector3(scaleX, scaleY, 1);
                }
            }
            if (useAnim)
            {
                Tween.Scale(0, transform, toScale, time, Tween.EaseType.ExpoEaseInOut);
            }
            else
            {
                transform.localScale = toScale;
            }
            CreateVideoBg();
        }

        void CreateVideoBg()
        {
            if (!videoBg)
            {
                videoBg = new GameObject("BlackBg");
                videoBg.transform.SetParent(arCamera);
                videoBgQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                videoBgQuad.GetComponent<MeshRenderer>().material = arVideoBg;
                videoBgQuad.transform.SetParent(videoBg.transform);
                Destroy(videoBgQuad.GetComponent<MeshCollider>());
            }
            videoBg.transform.localPosition = backgroundPlane.localPosition;
            videoBg.transform.localEulerAngles = backgroundPlane.localEulerAngles;
            videoBg.transform.localScale = backgroundPlane.localScale;
            videoBgQuad.transform.localPosition = new Vector3(0, -0.1f, 0);
            videoBgQuad.transform.localEulerAngles = new Vector3(90, 0, 0);
            videoBgQuad.transform.localScale = Vector3.one * 2;
        }

        /// <summary>
        /// 切换横屏竖屏方向
        /// </summary>
        public void SwitchDirection()
        {
            if (transform.localEulerAngles.z == 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, -90);
                if (videoPlayer.texture.height / (float)videoPlayer.texture.width > Screen.width / (float)Screen.height)
                {
                    //Debuger.LogError("左右贴紧");
                    //左右贴紧（分横屏和竖屏处理）
                    if (Screen.width < Screen.height)
                    {
                        float scaleY = 2 * backgroundPlane.localScale.z * Screen.width / (float)Screen.height;
                        float scaleX = scaleY * videoPlayer.texture.width / (float)videoPlayer.texture.height;
                        transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                    else
                    {
                        float scaleY = 2 * backgroundPlane.localScale.x;
                        float scaleX = scaleY * videoPlayer.texture.height / (float)videoPlayer.texture.width;
                        transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                }
                else
                {
                    //Debuger.LogError("上下贴紧");
                    //上下贴紧（分横屏和竖屏处理）
                    if (Screen.width < Screen.height)
                    {
                        float scaleX = 2 * backgroundPlane.localScale.z;
                        float scaleY = scaleX * videoPlayer.texture.height / (float)videoPlayer.texture.width;
                        transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                    else
                    {
                        float screenH = backgroundPlane.localScale.x * Screen.height / (float)Screen.width;
                        float scaleX = 2 * screenH;
                        float scaleY = scaleX * videoPlayer.texture.height / (float)videoPlayer.texture.width;
                        transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                }
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
                TrackerToScreen(false);
            }
        }

        protected override void OnTrackingFound()
        {
            hadFoundOnce = true;
            gameObject.SetActive(true);
            GetComponent<MeshRenderer>().enabled = false;
        }

        protected override void OnTrackingLost()
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
            }
        }
    }
}