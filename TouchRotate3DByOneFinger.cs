using UnityEngine;
using UnityEngine.EventSystems;

namespace BToolkit
{
    public class TouchRotate3DByOneFinger : MonoBehaviour
    {

        public Camera attachCamera;
        public bool freeRotation = false;
        public bool xAxisEnable = true;
        public bool yAxisEnable = true;
        public float dragSpeed = 1;
        public bool autoRotate = true;
        public float autoRotateSpeed = 10;
        public bool useUITrigger;
        public EventTrigger eventTrigger;
        public bool useRayTrigger;
        public LayerMask rayTriggerLayerMask;
        Vector3 previousPos;
        bool canDrag = false, canAutoRotate = true;
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
                    Debug.LogError(gameObject.name + ":TouchRotateByOneFinger >>> 请指定从属的摄像机");
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

        void OnEnable()
        {
            if (autoRotate)
            {
                canAutoRotate = true;
            }
        }

        void Start()
        {
            if (useUITrigger)
            {
                if (!eventTrigger)
                {
                    Debug.LogError("TouchRotateByOneFinger >>> 当勾选useUITrigger时，必须给UI添加EventTrigger,并拖动到EventTrigger但无需手动关联函数");
                }
                else
                {
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(UITriggerFunction));
                    eventTrigger.triggers.Add(entry);
                }
            }
            if (useRayTrigger)
            {
                if (!GetComponent<Collider>())
                {
                    Debug.LogError("TouchRotateByOneFinger >>> 当勾选rayTrigger时，必须给物体添加Collider,并设置RayTriggerLayerMask");
                }
            }
        }

        void Update()
        {
            if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!useUITrigger)
                    {
                        OnDown(Input.mousePosition);
                    }
                }
                if (Input.GetMouseButton(0))
                {
                    OnMove(Input.mousePosition);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    OnUp();
                }
            }
            else
            {
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        if (!useUITrigger)
                        {
                            OnDown(touch.position);
                        }
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        OnMove(touch.position);
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        OnUp();
                    }
                }
            }
            if (autoRotate)
            {
                if (canAutoRotate && CameraTrans)
                {
                    if (freeRotation)
                    {
                        transform.Rotate(CameraTrans.up, autoRotateSpeed * Time.deltaTime, Space.World);
                    }
                    else
                    {
                        transform.Rotate(transform.up, autoRotateSpeed * Time.deltaTime, Space.World);
                    }
                }
            }
        }

        void UITriggerFunction(BaseEventData eventData)
        {
            OnDown(Input.mousePosition);
        }

        public void OnDown(Vector3 pos)
        {
            if (!AttachCamera)
            {
                Debug.LogWarning("TouchRotateByOneFinger >>> 没有指定从属的摄像机 AttachCamrea");
                return;
            }
            if (useRayTrigger)
            {
                Ray ray = AttachCamera.ScreenPointToRay(pos);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, rayTriggerLayerMask.value))
                {
                    Debug.LogWarning(hit.collider.gameObject.name);
                    if (hit.collider.gameObject == gameObject)
                    {
                        canDrag = true;
                    }
                }
            }
            else
            {
                canDrag = true;
            }
            canAutoRotate = false;
            previousPos = AttachCamera.ScreenToViewportPoint(pos);
        }

        void OnMove(Vector3 pos)
        {
            if (canDrag)
            {
                Vector3 viewportPoint = AttachCamera.ScreenToViewportPoint(pos);
                Vector3 moveDelta = (viewportPoint - previousPos) * 250f * dragSpeed;
                previousPos = viewportPoint;
                //如果是手机前置涉嫌头，则旋转方向相反
                /*
                if (Vuforia.CameraDevice.Instance.GetCameraDirection() == Vuforia.CameraDevice.CameraDirection.CAMERA_FRONT)
                {
                    moveDelta.y *= -1;
                }
                */
                if (CameraTrans)
                {
                    if (freeRotation)
                    {
                        if (xAxisEnable)
                        {
                            transform.Rotate(CameraTrans.right, moveDelta.y, Space.World);
                        }
                        if (yAxisEnable)
                        {
                            transform.Rotate(CameraTrans.up, -moveDelta.x, Space.World);
                        }
                    }
                    else
                    {
                        float dot = Vector3.Dot(transform.up, CameraTrans.up);
                        if (dot < 0)
                        {
                            moveDelta.x *= -1;
                        }
                        if (xAxisEnable)
                        {
                            transform.Rotate(CameraTrans.right, moveDelta.y, Space.World);
                        }
                        if (yAxisEnable)
                        {
                            transform.Rotate(transform.up, -moveDelta.x, Space.World);
                        }
                    }
                }
            }
        }

        void OnUp()
        {
            canDrag = false;
            if (autoRotate)
            {
                canAutoRotate = true;
            }
        }

        public void StartAutoRotate()
        {
            if (!AttachCamera)
            {
                Debug.LogWarning("TouchRotateByOneFinger >>> 没有指定从属的摄像机 AttachCamrea");
                return;
            }
            autoRotate = true;
            canAutoRotate = true;
            previousPos = AttachCamera.ScreenToViewportPoint(Input.mousePosition);
        }
    }
}