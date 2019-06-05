using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace BToolkit
{
    public class UGUIJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {

        public RectTransform bar;
        public float bgActiveAlpha = 1f;
        public float barActiveAlpha = 1f;
        public bool canKeyboardCtrl = true;
        public delegate void JoystickDelegate(bool isActive, float radian);
        public static UGUIJoystick instance;
        static JoystickDelegate OnJoystickDragEvent;
        RectTransform trans;
        bool isLock, isActive;
        float maxDistance;
        public bool Lock
        {
            get { return isLock; }
            set
            {
                isLock = value;
                if (isLock)
                {
                    Init();
                }
            }
        }
        Vector2 defaultPos;
        float defaultAlpha, barDefaultAlpha;
        Image image, barImage;
        RawImage rawImage, barRawImage;
        float screenSizeRate, screenUIWidth;
        int joyEndListen;

        void Awake()
        {
            instance = this;
            trans = GetComponent<RectTransform>();
            if (bar)
            {
                Button barBtn = bar.GetComponent<Button>();
                if (barBtn)
                {
                    Destroy(barBtn);
                }
            }
            else
            {
                Debug.LogError("Joystick Bar is null");
            }
            maxDistance = trans.sizeDelta.x * 0.35f * trans.localScale.x;
            GetDefaultInfo();
        }

        void Start()
        {
            CanvasScaler canvasScaler = GameObject.FindObjectOfType<CanvasScaler>();
            screenSizeRate = canvasScaler.referenceResolution.y / (float)Screen.height;
            screenUIWidth = canvasScaler.referenceResolution.y * (float)Screen.width / (float)Screen.height;
        }

        void OnEnable()
        {
            Init();
        }

        void OnDisable()
        {
            Init();
        }

        void OnDestroy()
        {
            Init();
            instance = null;
        }

        void OnApplicationFocus(bool focusStatus)
        {
            Init();
        }

        public void Init()
        {
            isActive = false;
            SetBorder(defaultPos, defaultAlpha);
            SetBar(Vector2.zero, barDefaultAlpha);
            if (OnJoystickDragEvent != null)
            {
                OnJoystickDragEvent(false, 0);
            }
        }

        void Update()
        {
            if (isActive)
            {
                if (OnJoystickDragEvent != null)
                {
                    if (bar.anchoredPosition.x != 0 || bar.anchoredPosition.y != 0)
                    {
                        float radian = Mathf.Atan2(bar.anchoredPosition.y, bar.anchoredPosition.x);
                        OnJoystickDragEvent(true, radian);
                    }
                }
            }
            if (canKeyboardCtrl)
            {
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    bar.anchoredPosition += new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * 50f;
                    float dis = Vector2.Distance(bar.anchoredPosition, Vector2.zero);
                    if (dis >= maxDistance)
                    {
                        Vector2 vec = bar.anchoredPosition * maxDistance / dis;
                        bar.anchoredPosition = vec;
                    }
                    if (OnJoystickDragEvent != null)
                    {
                        float radian = Mathf.Atan2(bar.anchoredPosition.y, bar.anchoredPosition.x);
                        OnJoystickDragEvent(true, radian);
                    }
                    joyEndListen = 2;
                }
                if (joyEndListen > 0)
                {
                    joyEndListen -= 1;
                    if (joyEndListen <= 0)
                    {
                        Init();
                    }
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (isLock)
            {
                return;
            }
            Vector2 touchPos = eventData.position * screenSizeRate;
            if (defaultPos.x < 0)
            {
                touchPos.x -= screenUIWidth;
            }
            SetBorder(touchPos, bgActiveAlpha);
            SetBar(Vector2.zero, barActiveAlpha);
            isActive = true;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (isLock)
            {
                return;
            }
            Vector2 touchPos = eventData.position * screenSizeRate;
            if (defaultPos.x < 0)
            {
                touchPos.x -= screenUIWidth;
            }
            bar.anchoredPosition = touchPos - trans.anchoredPosition;
            float dis = Vector2.Distance(bar.anchoredPosition, Vector2.zero);
            if (dis >= maxDistance)
            {
                Vector2 vec = bar.anchoredPosition * maxDistance / dis;
                bar.anchoredPosition = vec;
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Init();
        }

        public static void RegisterEvent(JoystickDelegate OnJoystickDrag)
        {
            OnJoystickDragEvent = OnJoystickDrag;
        }
        public static void UnregisterEvent()
        {
            RegisterEvent(null);
        }

        void GetDefaultInfo()
        {
            defaultPos = trans.anchoredPosition;
            image = GetComponent<Image>();
            barImage = bar.GetComponent<Image>();
            rawImage = GetComponent<RawImage>();
            barRawImage = bar.GetComponent<RawImage>();
            defaultAlpha = image ? image.color.a : rawImage.color.a;
            barDefaultAlpha = barImage ? barImage.color.a : barRawImage.color.a;
        }
        void SetBorder(Vector2 pos, float alpha)
        {
            trans.anchoredPosition = pos;
            if (image)
            {
                Color color = image.color;
                color.a = alpha;
                image.color = color;
            }
            else
            {
                Color color = rawImage.color;
                color.a = alpha;
                rawImage.color = color;
            }
        }
        void SetBar(Vector2 pos, float alpha)
        {
            bar.anchoredPosition = pos;
            if (barImage)
            {
                Color color = barImage.color;
                color.a = alpha;
                barImage.color = color;
            }
            else
            {
                Color color = barRawImage.color;
                color.a = alpha;
                barRawImage.color = color;
            }
        }
    }
}