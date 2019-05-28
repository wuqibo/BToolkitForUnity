using UnityEngine;

namespace BToolkit
{
    public class CloudModelOffCardCtrl : CloudOffCardCtrl
    {
        Transform defaultParent, arCamera;
        Vector3 defaultPos, defaultAngle, defaultScale;
        float scaleRatio
        {
            get
            {
                if (Application.isEditor)
                {
                    return 1f;
                }
                return 1f;
            }
        }

        /// <summary>
        /// ÇÐ»»µ½È«ÆÁ
        /// </summary>
        public override void ToScreen(float videoW = 1, float videoH = 1, bool isAVProPlayer = false)
        {
            if (!defaultParent)
            {
                defaultParent = transform.parent;
                defaultPos = transform.localPosition;
                defaultAngle = transform.localEulerAngles;
                defaultScale = transform.localScale;
            }
            if (!arCamera)
            {
                arCamera = GameObject.Find("ARCamera").transform;
            }
            hadToScreen = true;
            transform.SetParent(arCamera, true);
            float time = 0.5f;
            Tween.Move(0, transform, new Vector3(0, -0.5f, 3.5f), time, false, Tween.EaseType.ExpoEaseOut);
            transform.localEulerAngles = new Vector3(-20, 0, 0);
            Tween.Scale(0, transform, Vector3.one * scaleRatio, time, Tween.EaseType.ExpoEaseOut);
            //Ðý×ª
            TouchRotate3DByOneFinger touchRotate = GetComponent<TouchRotate3DByOneFinger>();
            if (!touchRotate)
            {
                touchRotate = gameObject.AddComponent<TouchRotate3DByOneFinger>();
            }
            touchRotate.attachCamera = arCamera.GetComponent<Camera>();
            //Ëõ·Å
            TouchScaleByTwoFingers touchScale = GetComponent<TouchScaleByTwoFingers>();
            if (!touchScale)
            {
                touchScale = gameObject.AddComponent<TouchScaleByTwoFingers>();
            }
            touchScale.max = 2;
        }

        /// <summary>
        /// ÇÐ»»µ½AR¸ú×Ù
        /// </summary>
        public override void ToTrackable()
        {
            if (hadToScreen)
            {
                TouchRotate3DByOneFinger touchRotate = GetComponent<TouchRotate3DByOneFinger>();
                if (touchRotate)
                {
                    Destroy(touchRotate);
                }
                TouchScaleByTwoFingers touchScale = GetComponent<TouchScaleByTwoFingers>();
                if (touchScale)
                {
                    Destroy(touchScale);
                }

                transform.SetParent(defaultParent);
                transform.localPosition = defaultPos;
                transform.localEulerAngles = defaultAngle;
                transform.localScale = defaultScale;
                hadToScreen = false;
                CloudUIShowCtrller.Destroy();
            }
        }

        public override void CloseFromUI()
        {
            if (hadToScreen) {
                base.CloseFromUI();
                GetComponent<CloudModelViewer>().Show(false);
            }
        }
    }
}