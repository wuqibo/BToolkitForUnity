using UnityEngine;

//拖到摄影机
namespace BToolkit
{
    public class CameraLookAround : MonoBehaviour
    {
        public Transform target;
        public float angle;
        public float distance = 5;
        public float cameraHeight = 2;
        public float cameraHeightMin = 0.1f;
        public float cameraHeightMax = 7f;
        public float viewPointHeight = 0f;

        Vector3 previousPos, dragDelta;

        void Update()
        {
            if (target)
            {
                Vector3 pos = target.position + new Vector3(0, cameraHeight, 0);
                float radian = angle * Mathf.Deg2Rad;
                pos.x += Mathf.Sin(radian) * distance;
                pos.z += Mathf.Cos(radian) * distance;
                transform.position = pos;
                transform.LookAt(target.position + new Vector3(0, viewPointHeight, 0));
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
            previousPos = pos;
            dragDelta = Vector2.zero;
        }

        void OnTouchMove(Vector3 pos)
        {
            dragDelta = pos - previousPos;
            previousPos = pos;
            angle += dragDelta.x * 100;
            cameraHeight -= dragDelta.y * 10;
            if (cameraHeight < cameraHeightMin)
            {
                cameraHeight = cameraHeightMin;
            }
            else if (cameraHeight > cameraHeightMax)
            {
                cameraHeight = cameraHeightMax;
            }
        }

        void OnTouchUp()
        {

        }
    }
}