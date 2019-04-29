using UnityEngine;

namespace BToolkit
{
    public class TouchMove3DByTwoFingers : MonoBehaviour
    {

        public Camera attachCamera;
        public bool testInEditor;
        Vector3 previousPos;
        float dis;
        Camera AttachCamera
        {
            get
            {
                if (!attachCamera)
                {
                    Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
                    if (cameras.Length == 1)
                    {
                        attachCamera = cameras[0];
                    }
                }
                if (!attachCamera)
                {
                    Debug.LogError(gameObject.name + ":TouchMoveByTwoFingers >>> 请指定从属的摄像机");
                }
                return attachCamera;
            }
        }
        Transform cameraTrans;
        Transform CameraTrans
        {
            get
            {
                if (!cameraTrans)
                {
                    if (AttachCamera)
                    {
                        cameraTrans = AttachCamera.transform;
                    }
                }
                return cameraTrans;
            }
        }

        void Update()
        {
            if (Application.isEditor)
            {
                if (testInEditor)
                {
                    if (Input.GetMouseButtonDown(2))
                    {
                        OnDown(Input.mousePosition);
                    }
                    if (Input.GetMouseButton(2))
                    {
                        OnDrag(Input.mousePosition);
                    }
                }
            }
            else
            {
                if (Input.touchCount >= 2)
                {
                    Touch touch = Input.GetTouch(1);
                    if (touch.phase == TouchPhase.Began)
                    {
                        OnDown(touch.position);
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        OnDrag(touch.position);
                    }
                }
            }
        }

        void OnDown(Vector3 screenPos)
        {
            Ray ray = AttachCamera.ScreenPointToRay(screenPos);
            dis = Vector3.Distance(transform.position, ray.origin);
            previousPos = WorldPos(screenPos);
        }

        void OnDrag(Vector3 screenPos)
        {
            Vector3 worldPos = WorldPos(screenPos);
            Vector3 deltaPos = worldPos - previousPos;
            previousPos = worldPos;
            transform.position += deltaPos;
        }

        Vector3 WorldPos(Vector3 screenPos)
        {
            Ray ray = AttachCamera.ScreenPointToRay(screenPos);
            return ray.origin + ray.direction * dis;
        }
    }
}