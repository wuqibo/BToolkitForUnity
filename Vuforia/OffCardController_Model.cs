using UnityEngine;

namespace BToolkit
{
    public class OffCardController_Model : MonoBehaviour
    {
        public Transform arParent;
        public Vector3 arPos = Vector3.zero;
        public Vector3 arAngle = Vector3.zero;
        public Vector3 arScale = Vector3.one;
        [Space]
        public Camera arCamera;
        public Vector3 screenPos = new Vector3(0, -0.2f, 4);
        public Vector3 screenAngle = new Vector3(-35, 0, 0);
        public Vector3 screenScale = Vector3.one;

        bool hadToScreenOnce;
        bool isOffCard;

        /// <summary>
        /// 切换到全屏
        /// </summary>
        public void ToScreen(bool isOffCard)
        {
            if (!arCamera)
            {
                arCamera = GameObject.Find("ARCamera").GetComponent<Camera>();
            }
            if (!arParent)
            {
                arParent = transform.parent;
            }
            this.isOffCard = isOffCard;
            arCamera.enabled = true;
            transform.SetParent(this.arCamera.transform);

            if (this.isOffCard)
            {
                Tween.Move(0, transform, this.screenPos, 0.5f, false, Tween.EaseType.ExpoEaseOut);
                Tween.Scale(0, transform, this.screenScale, 0.5f, Tween.EaseType.ExpoEaseOut);
            }
            else
            {
                transform.localPosition = this.screenPos;
                transform.localScale = this.screenScale;
            }
            transform.localEulerAngles = this.screenAngle;
            //旋转
            TouchRotate3DByOneFinger rotate = GetComponent<TouchRotate3DByOneFinger>();
            if (!rotate)
            {
                rotate = gameObject.AddComponent<TouchRotate3DByOneFinger>();
            }
            rotate.attachCamera = arCamera;
            rotate.autoRotate = false;
            //缩放
            TouchScaleByTwoFingers scale = GetComponent<TouchScaleByTwoFingers>();
            if (!scale)
            {
                gameObject.AddComponent<TouchScaleByTwoFingers>();
            }
            //移动
            TouchMove3DByTwoFingers move = GetComponent<TouchMove3DByTwoFingers>();
            if (!move)
            {
                move = gameObject.AddComponent<TouchMove3DByTwoFingers>();
            }
            move.attachCamera = arCamera;
            //打开UI控制
            UIController_Model.Show(this, isOffCard);
            hadToScreenOnce = true;
        }

        /// <summary>
        /// 切换到AR跟踪
        /// </summary>
        public void ToTracking()
        {
            Tween.StopMove(transform);
            Tween.StopScale(transform);
            if (arParent)
            {
                transform.SetParent(arParent);
                transform.localPosition = arPos;
                transform.localEulerAngles = arAngle;
                transform.localScale = arScale;
            }
            if (hadToScreenOnce)
            {
                //旋转
                TouchRotate3DByOneFinger rotate = GetComponent<TouchRotate3DByOneFinger>();
                if (rotate)
                {
                    Destroy(rotate);
                }
                //缩放
                TouchScaleByTwoFingers scale = GetComponent<TouchScaleByTwoFingers>();
                if (scale)
                {
                    Destroy(scale);
                }
                //移动
                TouchMove3DByTwoFingers move = GetComponent<TouchMove3DByTwoFingers>();
                if (move)
                {
                    Destroy(move);
                }

                //ARCamera
                if (!isOffCard && arCamera)
                {
                    arCamera.enabled = false;
                }

                //关闭UI控制
                UIController_Model.Destroy();
            }
        }
    }
}