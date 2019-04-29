using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class GyroCameraController : MonoBehaviour
    {

        public static GyroCameraController instance;
        public bool enableOnStart;
        public bool roundMode;
        public Transform roundTarget;
        public float roundRadius = 80;
        public bool IsGyroControling { get { return this.enabled; } }
        Gyroscope gyro;
        Quaternion rotFix;
        Transform guide, guideHolder;
        bool isMovingTo;

        void Awake()
        {
            instance = this;
            gyro = Input.gyro;
            gyro.enabled = true;
            this.enabled = enableOnStart;
            guideHolder = new GameObject("GyroGuideParent").transform;
            guideHolder.eulerAngles = new Vector3(90, 0, 0);
            guide = new GameObject("GyroGuide").transform;
            guide.SetParent(guideHolder, false);
            guide.localPosition = Vector3.zero;
            rotFix = new Quaternion(0, 0, 1f, 0f);
            Debug.Log(Screen.orientation);
        }

        void Start()
        {
            if (Screen.orientation != ScreenOrientation.LandscapeLeft)
            {
                Debug.LogWarning("当前算法只适用于屏幕方向为 LandscapeLeft 的配置");
            }
        }

        void OnDestroy()
        {
            instance = null;
            if (guideHolder)
            {
                Destroy(guideHolder.gameObject);
            }
        }

        void Update()
        {
            guide.localRotation = gyro.attitude * rotFix;
            transform.rotation = guide.rotation;
            if (roundMode)
            {
                Vector3 roundTargetPos = roundTarget.position - transform.forward * roundRadius;
                if (isMovingTo)
                {
                    transform.position += (roundTargetPos - transform.position) * Time.deltaTime * 5f;
                    if (Vector3.Distance(roundTargetPos, transform.position) < 0.3f)
                    {
                        isMovingTo = false;
                    }
                }
                else
                {
                    transform.position = roundTargetPos;
                }
            }
        }

        /// <summary>
        /// 开启陀螺仪控制模式
        /// </summary>
        public void StartGyroControl()
        {
            if (Application.isEditor)
            {
                guideHolder.eulerAngles = new Vector3(160, 180, 0);
            }
            else
            {
                guideHolder.eulerAngles = new Vector3(90, 0, 0);
            }
            this.enabled = true;
        }

        /// <summary>
        /// 关闭陀螺仪控制模式
        /// </summary>
        public void StopGyroControl()
        {
            this.enabled = false;
        }

        public void SwitchToRoundMode(Transform target, float radius)
        {
            roundMode = true;
            if (target)
            {
                this.roundTarget = target;
            }
            this.roundRadius = radius;
            isMovingTo = true;
        }

        public void SwitchToVRMode(Vector3 cameraPos)
        {
            roundMode = false;
            isMovingTo = false;
            Tween.Move(0, transform, cameraPos, 5f, true, Tween.EaseType.SineEaseInOut);
        }
    }
}