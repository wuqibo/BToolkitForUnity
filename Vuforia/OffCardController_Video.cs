using UnityEngine;

namespace BToolkit
{
    public class OffCardController_Video : MonoBehaviour
    {
        public Material ARVideoBg;

        Transform trackableParent;
        Vector3 trackablePos, trackableAngle, trackableScale;
        Transform backgroundPlane;
        GameObject videoBg, videoBgQuad;
        ValueUpdate valueUpdate;
        float videoW, videoH;
        float scaleRatio { get { if (Application.isEditor) { return 2; } return 3.5f; } }
        bool hadToScreenOnce, isAVProPlayer;

        /// <summary>
        /// 切换到全屏
        /// </summary>
        public void ToScreen(float videoW, float videoH, bool isAVProPlayer)
        {
            this.videoW = videoW;
            this.videoH = videoH;
            this.isAVProPlayer = isAVProPlayer;
            if (!backgroundPlane)
            {
                backgroundPlane = GameObject.Find("BackgroundPlane").transform;
                trackableParent = transform.parent;
                trackablePos = transform.localPosition;
                trackableAngle = transform.localEulerAngles;
                trackableScale = transform.localScale;
            }
            ARVideoBg.SetColor("_Color", new Color(0, 0, 0, 0));
            valueUpdate = Tween.Value(0, 0, 0.8f, 0.5f, Tween.EaseType.ExpoEaseOut, (float v) =>
            {
                ARVideoBg.SetColor("_Color", new Color(0, 0, 0, v));
            });
            DoToScreen(true, isAVProPlayer);
            //打开UI控制
            UIController_Video.Show(this);
            hadToScreenOnce = true;
        }

        /// <summary>
        /// 切换到AR跟踪
        /// </summary>
        public void ToTracking()
        {
            if (hadToScreenOnce)
            {
                if (backgroundPlane)
                {
                    transform.SetParent(trackableParent);
                    transform.localPosition = trackablePos;
                    transform.localEulerAngles = trackableAngle;
                    transform.localScale = trackableScale;

                    if (videoBg)
                    {
                        Destroy(videoBg);
                    }
                    if (videoBgQuad)
                    {
                        Destroy(videoBgQuad);
                    }
                    if (valueUpdate)
                    {
                        valueUpdate.Destroy();
                    }
                }
                //关闭UI控制
                UIController_Video.Destroy();
            }
        }

        // Mesh必须挂载到当前节点，若用子物体做Mesh，须确保和当前对象相同的Transform参数
        void DoToScreen(bool useAnim, bool isAVProPlayer)
        {
            transform.SetParent(GameObject.Find("ARCamera").transform, true);
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
            //左右贴紧
            if (Screen.width < Screen.height)
            {
                //竖屏
                if (videoW / (float)videoH > Screen.width / (float)Screen.height)
                {
                    //左右贴紧
                    float scaleX = scaleRatio * backgroundPlane.localScale.z * Screen.width / (float)Screen.height;
                    float scaleY = scaleX * videoH / (float)videoW;
                    toScale = new Vector3(scaleX, scaleY, 1);
                }
                else
                {
                    //上下贴紧
                    float scaleY = scaleRatio * backgroundPlane.localScale.z;
                    float scaleX = scaleY * videoW / (float)videoH;
                    toScale = new Vector3(scaleX, scaleY, 1);
                }
            }
            else
            {
                //横屏
                if (videoW / (float)videoH > Screen.width / (float)Screen.height)
                {
                    //左右贴紧
                    float scaleX = scaleRatio * backgroundPlane.localScale.x;
                    float scaleY = scaleX * videoH / (float)videoW;
                    toScale = new Vector3(scaleX, scaleY, 1);
                }
                else
                {
                    //上下贴紧
                    float screenH = backgroundPlane.localScale.x * Screen.height / (float)Screen.width;
                    float scaleY = scaleRatio * screenH;
                    float scaleX = scaleY * videoW / (float)videoH;
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
                videoBg.transform.SetParent(GameObject.Find("ARCamera").transform);
                videoBgQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                videoBgQuad.GetComponent<MeshRenderer>().material = ARVideoBg;
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
                if (Screen.width < Screen.height)
                {
                    //竖屏
                    if (videoH / (float)videoW > Screen.width / (float)Screen.height)
                    {
                        //左右贴紧
                        float scaleY = scaleRatio * backgroundPlane.localScale.z * Screen.width / (float)Screen.height;
                        float scaleX = scaleY * videoW / (float)videoH;
                        transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                    else
                    {
                        //上下贴紧
                        float scaleX = scaleRatio * backgroundPlane.localScale.z;
                        float scaleY = scaleX * videoH / (float)videoW;
                        transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                }
                else
                {
                    //横屏
                    if (videoH / (float)videoW > Screen.width / (float)Screen.height)
                    {
                        //左右贴紧
                        float scaleY = scaleRatio * backgroundPlane.localScale.x;
                        float scaleX = scaleY * videoH / (float)videoW;
                        transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                    else
                    {
                        //上下贴紧
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
                DoToScreen(false, isAVProPlayer);
            }
        }
    }
}