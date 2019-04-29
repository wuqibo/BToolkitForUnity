using UnityEngine;

namespace BToolkit
{
    public class WanderUGUI : MonoBehaviour
    {
        public float moveSpeed = 50;
        public float angleSpeed = 20;
        public float angleAddMax = 2f;
        public bool forward = true;
        public bool limitInScreen = true;
        public Vector2 limitCenter;
        public Vector2 limitSize;
        float radian, radianAdd;
        RectTransform rectTrans;

        void Awake()
        {
            rectTrans = GetComponent<RectTransform>();
            radian = rectTrans.localEulerAngles.z * Mathf.Deg2Rad;
        }

        void Update()
        {
            Vector2 velocity = new Vector2(Mathf.Cos(radian) * moveSpeed, Mathf.Sin(radian) * moveSpeed);
            rectTrans.anchoredPosition += velocity * Time.deltaTime;
            radianAdd += Random.Range(-angleSpeed, angleSpeed) * Time.deltaTime;
            radianAdd = Mathf.Clamp(radianAdd, -angleAddMax, angleAddMax);
            radian += radianAdd * Time.deltaTime;
            if (forward)
            {
                transform.localEulerAngles = new Vector3(0, 0, radian * 180f / Mathf.PI);
            }
            Wrap();
        }

        void Wrap()
        {
            Vector2 pos = rectTrans.anchoredPosition;
            if (limitInScreen)
            {
                if (pos.x < -BUtils.ScreenUISize.x * 0.5f)
                {
                    radian = 0f;
                    radianAdd = 0f;
                }
                if (pos.x > BUtils.ScreenUISize.x * 0.5f)
                {
                    radian = Mathf.PI;
                    radianAdd = 0f;
                }
                if (pos.y < -BUtils.ScreenUISize.y * 0.5f)
                {
                    radian = Mathf.PI * 0.5f;
                    radianAdd = 0f;
                }
                if (pos.y > BUtils.ScreenUISize.y * 0.5f)
                {
                    radian = -Mathf.PI * 0.5f;
                    radianAdd = 0f;
                }
            }
            else
            {
                if (pos.x < limitCenter.x - limitSize.x * 0.5f)
                {
                    radian = 0f;
                    radianAdd = 0f;
                }
                if (pos.x > limitCenter.x + limitSize.x * 0.5f)
                {
                    radian = Mathf.PI;
                    radianAdd = 0f;
                }
                if (pos.y < limitCenter.y - limitSize.y * 0.5f)
                {
                    radian = Mathf.PI * 0.5f;
                    radianAdd = 0f;
                }
                if (pos.y > limitCenter.y + limitSize.y * 0.5f)
                {
                    radian = -Mathf.PI * 0.5f;
                    radianAdd = 0f;
                }
            }
        }

        double Linear(double t, double b, double c, double d)
        {
            return c * t / d + b;
        }
    }
}