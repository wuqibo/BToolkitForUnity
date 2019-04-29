using UnityEngine;

namespace BToolkit
{
    [AddComponentMenu("BToolkit/TouchRay")]
    public class TouchRay : MonoBehaviour
    {

        public Camera attachCamera;
        public bool useTouchStay;
        public string touchPressMsg = "OnTouchPress";
        public string touchStayMsg = "OnTouchStay";
        public string touchClickMsg = "OnTouchClick";
        public float listenMove = 0.05f;
        Ray ray;
        RaycastHit hit;
        GameObject downTarget;
        Vector3 downPos;
        Camera AttachCamera
        {
            get
            {
                if (!attachCamera)
                {
                    Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
                    Debuger.Log("TouchRay >>> cameras.Length: " + cameras.Length);
                    for (int i = 0; i < cameras.Length; i++)
                    {
                        Debuger.Log(">>>>>>> " + cameras[i].name);
                    }
                    if (cameras.Length == 1)
                    {
                        attachCamera = cameras[0];
                    }
                }
                if (!attachCamera)
                {
                    Debuger.LogError("TouchRay >>> 请指定从属的摄像机");
                }
                return attachCamera;
            }
        }

        void Update()
        {
            if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnDown(Input.mousePosition);
                }
                if (useTouchStay)
                {
                    if (Input.GetMouseButton(0))
                    {
                        OnStay(Input.mousePosition);
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    OnUp(Input.mousePosition);
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        OnDown(touch.position);
                    }
                    if (useTouchStay)
                    {
                        if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                        {
                            OnStay(touch.position);
                        }
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        OnUp(touch.position);
                    }
                }
            }
        }

        void OnDown(Vector3 pos)
        {
            ray = AttachCamera.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit))
            {
                downTarget = hit.collider.gameObject;
                downPos = AttachCamera.ScreenToViewportPoint(pos);
                downTarget.SendMessage(touchPressMsg, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                downTarget = null;
            }
        }

        void OnStay(Vector3 pos)
        {
            if (downTarget)
            {
                downTarget.SendMessage(touchStayMsg, SendMessageOptions.DontRequireReceiver);
            }
        }

        void OnUp(Vector3 pos)
        {
            ray = AttachCamera.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == downTarget)
                {
                    if (Vector3.Distance(downPos, AttachCamera.ScreenToViewportPoint(pos)) < listenMove)
                    {
                        downTarget.SendMessage(touchClickMsg, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }
    }
}