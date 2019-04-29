using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//拖到摄影机
namespace BToolkit
{
    public class CameraLookAround : MonoBehaviour
    {

        public Transform target;
        public float distane = 30;
        public bool canCtrlHeightAngle = true;
        public float currHeightAngle = 15;
        public bool heightAngleLimit;
        public float heightAngleMin = 0;
        public float heightAngleMax = 90;
        public bool autoRotate;
        public float autoRotateSpeed = 0.2f;
        Vector3 previousPos;
        float r, speedXScale = 0.01f, speedYScale;
        bool isDraging;

        void Awake()
        {
            r = -Mathf.PI * 0.5f;
            CanvasScaler canvasScaler = GameObject.FindObjectOfType<CanvasScaler>();
            if (canvasScaler)
            {
                speedXScale = 0.006f * canvasScaler.referenceResolution.y / (float)Screen.height;
            }
            speedYScale = speedXScale * 50f;
        }

        void Update()
        {
            if (target)
            {
                if (autoRotate)
                {
                    r += autoRotateSpeed * Time.deltaTime;
                }
                Vector3 pos = new Vector3(target.position.x + Mathf.Cos(r) * distane, 10f, target.position.z + Mathf.Sin(r) * distane);
                transform.position = pos;
                Vector3 axis = Vector3.Cross(Vector3.up, target.position - transform.position);
                if (heightAngleLimit)
                {
                    if (currHeightAngle < heightAngleMin)
                    {
                        currHeightAngle = heightAngleMin;
                    }
                    else if (currHeightAngle > heightAngleMax)
                    {
                        currHeightAngle = heightAngleMax;
                    }
                }
                transform.RotateAround(target.position, axis, currHeightAngle);
                transform.LookAt(target);
            }
            if (Input.GetMouseButtonDown(0))
            {
                OnTouchDown(Camera.main.ScreenToViewportPoint(Input.mousePosition));
            }
            if (Input.GetMouseButton(0))
            {
                OnTouchMove(Camera.main.ScreenToViewportPoint(Input.mousePosition));
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnTouchUp();
            }
        }

        void OnTouchDown(Vector3 pos)
        {
            if (target)
            {
                isDraging = true;
                previousPos = pos;
            }
        }

        void OnTouchMove(Vector3 pos)
        {
            if (target && isDraging)
            {
                float deltaX = (pos.x - previousPos.x) * speedXScale;
                r -= deltaX;
                if (canCtrlHeightAngle)
                {
                    float deltaY = (pos.y - previousPos.y) * speedYScale;
                    currHeightAngle -= deltaY;
                }
                previousPos = pos;
            }
        }

        void OnTouchUp()
        {
            isDraging = false;
        }
    }
}