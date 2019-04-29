using UnityEngine;

namespace BToolkit
{
    public class CameraWalk : MonoBehaviour
    {

        public enum Method
        {
            HorizontalWalk,
            FreeFly,
            RigidbodyWalk,
            KeyboardAndRightMouse
        }
        public Method method = Method.HorizontalWalk;
        public float beginHeight = 1.8f;
        public float moveSpeed = 2;
        public float rotateSpeed = 150;
        float defautY;
        UnityEngine.CharacterController mCharacterController;

        void Awake()
        {
            if (method == Method.RigidbodyWalk)
            {
                mCharacterController = GetComponent<UnityEngine.CharacterController>();
                if (!mCharacterController)
                {
                    Debug.LogError("请给摄像机绑定 CharacterController");
                }
            }
            if (!Application.isEditor
                && Application.platform != RuntimePlatform.WindowsPlayer
                && Application.platform != RuntimePlatform.WebGLPlayer
                && Application.platform != RuntimePlatform.OSXPlayer)
            {
                Destroy(this);
            }
            else
            {
                Vector3 pos = transform.localPosition;
                pos.y = beginHeight;
                transform.localPosition = pos;
            }
        }

        void Update()
        {
            switch (method)
            {
                case Method.HorizontalWalk:
                    HorizontalWalk();
                    break;
                case Method.FreeFly:
                    FreeFly();
                    break;
                case Method.RigidbodyWalk:
                    RigidbodyWalk();
                    break;
                case Method.KeyboardAndRightMouse:
                    KeyboardAndRightMouse();
                    break;
            }
        }

        void HorizontalWalk()
        {
            //移动
            if (Input.GetKey(KeyCode.W))
            {
                Vector3 pos = transform.position;
                defautY = pos.y;
                pos += transform.forward * moveSpeed * Time.deltaTime;
                pos.y = defautY;
                transform.position = pos;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                Vector3 pos = transform.position;
                defautY = pos.y;
                pos -= transform.forward * moveSpeed * Time.deltaTime;
                pos.y = defautY;
                transform.position = pos;
            }
            if (Input.GetKey(KeyCode.A))
            {
                Vector3 pos = transform.position;
                defautY = pos.y;
                pos -= transform.right * moveSpeed * Time.deltaTime;
                pos.y = defautY;
                transform.position = pos;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Vector3 pos = transform.position;
                defautY = pos.y;
                pos += transform.right * moveSpeed * Time.deltaTime;
                pos.y = defautY;
                transform.position = pos;
            }
            //左右看
            transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0, Space.World);
            //上下看
            transform.Rotate(-Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime, 0, 0, Space.Self);
        }

        void FreeFly()
        {
            //移动
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            }
            //左右看
            transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0, Space.World);
            //上下看
            transform.Rotate(-Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime, 0, 0, Space.Self);
        }

        void RigidbodyWalk()
        {
            //移动
            if (mCharacterController)
            {
                Vector3 currSpeed = Vector3.zero;
                if (Input.GetKey(KeyCode.W))
                {
                    currSpeed = transform.TransformDirection(Vector3.forward) * moveSpeed;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    currSpeed = transform.TransformDirection(Vector3.back) * moveSpeed;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    currSpeed = transform.TransformDirection(Vector3.left) * moveSpeed;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    currSpeed = transform.TransformDirection(Vector3.right) * moveSpeed;
                }
                mCharacterController.SimpleMove(currSpeed);
            }
            //左右看
            transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0, Space.World);
            //上下看
            transform.Rotate(-Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime, 0, 0, Space.Self);
        }

        void KeyboardAndRightMouse()
        {
            //移动
            if (Input.GetKey(KeyCode.W))
            {
                Vector3 pos = transform.position;
                defautY = pos.y;
                pos += transform.forward * moveSpeed * Time.deltaTime;
                pos.y = defautY;
                transform.position = pos;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                Vector3 pos = transform.position;
                defautY = pos.y;
                pos -= transform.forward * moveSpeed * Time.deltaTime;
                pos.y = defautY;
                transform.position = pos;
            }
            //方向
            if (Input.GetKey(KeyCode.A))
            {
                Vector3 pos = transform.position;
                defautY = pos.y;
                pos -= transform.right * moveSpeed * Time.deltaTime;
                pos.y = defautY;
                transform.position = pos;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Vector3 pos = transform.position;
                defautY = pos.y;
                pos += transform.right * moveSpeed * Time.deltaTime;
                pos.y = defautY;
                transform.position = pos;
            }
            if (Input.GetMouseButton(1))
            {
                //左右看
                transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0, Space.World);
                //上下看
                transform.Rotate(-Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime, 0, 0, Space.Self);
            }
        }

    }
}