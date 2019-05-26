using UnityEngine;

namespace BToolkit
{
    public class CloudOffCardCtrl : MonoBehaviour
    {
        public Material arVideoBg;
        Transform defaultParent, arCamera, backgroundPlane;
        bool hadToScreen;
        GameObject videoBg, videoBgQuad;
        float bgAlpha;
        float scaleRatio
        {
            get
            {
                if (Application.isEditor)
                {
                    return 2;
                }
                return 3.5f;
            }
        }

        void Update()
        {
            if (videoBgQuad && bgAlpha < 0.8f)
            {
                bgAlpha += (0.81f - bgAlpha) * 0.02f;
                arVideoBg.SetColor("_Color", new Color(0, 0, 0, bgAlpha));
            }
            if (hadToScreen)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ToTrackable();
                    GetComponent<CloudVideoPlayerManager>().Show(false);
                    VuforiaHelper.StartTracker();
                    FindObjectOfType<CloudRecognition>().RestartScan();
                }
            }
        }

        /// <summary>
        /// ÇÐ»»µ½È«ÆÁ
        /// </summary>
        public void ToFullScreen(float videoW, float videoH, bool isAVProPlayer)
        {
            if (!defaultParent)
            {
                defaultParent = transform.parent;
            }
            if (!arCamera)
            {
                arCamera = GameObject.Find("ARCamera").transform;
            }
            if (!backgroundPlane)
            {
                backgroundPlane = arCamera.Find("BackgroundPlane");
            }
            TrackerToScreen(true, videoW, videoH, isAVProPlayer);
        }

        /// <summary>
        /// ÇÐ»»µ½AR¸ú×Ù
        /// </summary>
        public void ToTrackable()
        {
            if (hadToScreen)
            {
                transform.SetParent(defaultParent);
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = new Vector3(90, 0, 0);
                transform.localScale = Vector3.one;
                hadToScreen = false;
                if (videoBg)
                {
                    Destroy(videoBg);
                }
                bgAlpha = 0;
                arVideoBg.SetColor("_Color", new Color(0, 0, 0, bgAlpha));
            }
        }

        void TrackerToScreen(bool useAnim, float videoW, float videoH, bool isAVProPlayer)
        {
            hadToScreen = true;
            transform.SetParent(arCamera, true);
            float time = 0.5f;
            if (useAnim)
            {
                Tween.Move(0, transform, backgroundPlane.localPosition, time, false, Tween.EaseType.ExpoEaseOut);
            }
            else
            {
                transform.localPosition = backgroundPlane.localPosition;
            }
            transform.localEulerAngles = Vector3.zero;
            Vector3 toScale = Vector3.zero;
            if (Screen.width < Screen.height)
            {
                //ÊúÆÁ
                if (videoW / videoH > Screen.width / (float)Screen.height)
                {
                    //×óÓÒÌù½ô
                    float scaleX = scaleRatio * backgroundPlane.localScale.z * Screen.width / (float)Screen.height;
                    float scaleY = scaleX * videoH / (float)videoW;
                    toScale = new Vector3(scaleX, scaleY, 1);
                }
                else
                {
                    //ÉÏÏÂÌù½ô
                    float scaleY = scaleRatio * backgroundPlane.localScale.z;
                    float scaleX = scaleY * videoW / videoH;
                    toScale = new Vector3(scaleX, scaleY, 1);
                }
            }
            else
            {
                //ºáÆÁ
                if (videoW / videoH > Screen.width / (float)Screen.height)
                {
                    //×óÓÒÌù½ô
                    float scaleX = scaleRatio * backgroundPlane.localScale.x;
                    float scaleY = scaleX * videoH / (float)videoW;
                    toScale = new Vector3(scaleX, scaleY, 1);
                }
                else
                {
                    //ÉÏÏÂÌù½ô
                    float screenH = backgroundPlane.localScale.x * Screen.height / (float)Screen.width;
                    float scaleY = scaleRatio * screenH;
                    float scaleX = scaleY * videoW / videoH;
                    toScale = new Vector3(scaleX, scaleY, 1);
                }
            }
            if (useAnim)
            {
                Tween.Scale(0, transform, toScale, time, Tween.EaseType.ExpoEaseOut);
            }
            else
            {
                transform.localScale = toScale;
            }
            if (!isAVProPlayer)
            {
                CreateVideoBg();
            }
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
        /// ÇÐ»»ºáÆÁÊúÆÁ·½Ïò
        /// </summary>
        public void SwitchDirection(float videoW, float videoH, bool isAVProPlayer)
        {
            if (transform.localEulerAngles.z == 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, -90);
                if (Screen.width < Screen.height)
                {
                    //ÊúÆÁ
                    if (videoH / (float)videoW > Screen.width / (float)Screen.height)
                    {
                        //×óÓÒÌù½ô
                        float scaleY = scaleRatio * backgroundPlane.localScale.z * Screen.width / (float)Screen.height;
                        float scaleX = scaleY * videoW / videoH;
                        transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                    else
                    {
                        //ÉÏÏÂÌù½ô
                        float scaleX = scaleRatio * backgroundPlane.localScale.z;
                        float scaleY = scaleX * videoH / (float)videoW;
                        transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                }
                else
                {
                    //ºáÆÁ
                    if (videoH / (float)videoW > Screen.width / (float)Screen.height)
                    {
                        //×óÓÒÌù½ô
                        float scaleY = scaleRatio * backgroundPlane.localScale.x;
                        float scaleX = scaleY * videoH / (float)videoW;
                        transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                    else
                    {
                        //ÉÏÏÂÌù½ô
                        float screenH = backgroundPlane.localScale.x * Screen.height / (float)Screen.width;
                        float scaleX = scaleRatio * screenH;
                        float scaleY = scaleX * videoH / (float)videoW;
                        transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                }
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
                TrackerToScreen(false, videoW, videoH, isAVProPlayer);
            }
        }
    }
}