using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BToolkit
{
    [AddComponentMenu("BToolkit/BButton")]
    public class BButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public enum TriggerMethod
        {
            Up, Down, Double
        }
        public int index;
        public TriggerMethod triggerMethod = TriggerMethod.Up;
        public float canTriggerInterval = 1;
        public bool listenKeyBack;
        public AudioClip sound;
        public bool useCommonSound = true;
        public UnityEvent onTrigger;
        public UnityAction<int> OnTouchDown, OnTouchClick, OnTouchUp;
        public RectTransform rectTransform { get { return transform as RectTransform; } }
        float doubleTimer;
        float canTouchTimer;
        static AudioClip btnCommonSound;
        //此两个属性用于存储多个监听Back键的按钮同时存在场景中时，按照创建的先后倒序执行
        BButton previousBButton;
        static BButton lastCreateBButton;
        ButtonChange buttonChange;
        int currPointerId = -1;

        void OnDestroy()
        {
            if (listenKeyBack)
            {
                if (lastCreateBButton == this)
                {
                    lastCreateBButton = previousBButton;
                }
            }
        }

        void OnDisable()
        {
            if (OnTouchUp != null)
            {
                OnTouchUp(index);
            }
        }

        void Awake()
        {
            buttonChange = GetComponent<ButtonChange>();
            if (listenKeyBack)
            {
                previousBButton = lastCreateBButton;
                lastCreateBButton = this;
            }
        }

        void Start() { }

        void Update()
        {
            if (canTouchTimer > 0f)
            {
                canTouchTimer -= Time.deltaTime;
                if (canTouchTimer <= 0f)
                {
                    if (buttonChange)
                    {
                        buttonChange.enabled = true;
                    }
                }
            }
            if (doubleTimer > 0f)
            {
                doubleTimer -= Time.deltaTime;
            }
            if (listenKeyBack)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (DialogAlert.isShowing || DialogConfirm.isShowing)
                    {
                        return;
                    }
                    if (lastCreateBButton == this)
                    {
                        onTrigger.Invoke();
                    }
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (enabled)
            {
                if (triggerMethod == TriggerMethod.Down)
                {
                    if (canTouchTimer <= 0f)
                    {
                        onTrigger.Invoke();
                        canTouchTimer = canTriggerInterval;
                        if (buttonChange && canTouchTimer > 0)
                        {
                            buttonChange.enabled = false;
                        }
                        if (sound)
                        {
                            SoundPlayer.PlayAndDestroy(0, sound);
                        }
                        else
                        {
                            if (useCommonSound)
                            {
                                if (!btnCommonSound)
                                {
                                    btnCommonSound = Resources.Load<AudioClip>("Sounds/btn_common");
                                }
                                SoundPlayer.PlayAndDestroy(0, btnCommonSound);
                            }
                        }
                    }
                }
                else if (triggerMethod == TriggerMethod.Double)
                {
                    if (doubleTimer <= 0f)
                    {
                        doubleTimer = 0.5f;
                    }
                    else
                    {
                        if (canTouchTimer <= 0f)
                        {
                            onTrigger.Invoke();
                            doubleTimer = 0f;
                            canTouchTimer = canTriggerInterval;
                            if (buttonChange && canTouchTimer > 0)
                            {
                                buttonChange.enabled = false;
                            }
                            if (sound)
                            {
                                SoundPlayer.PlayAndDestroy(0, sound);
                            }
                            else
                            {
                                if (useCommonSound)
                                {
                                    if (!btnCommonSound)
                                    {
                                        btnCommonSound = Resources.Load<AudioClip>("Sounds/btn_common");
                                    }
                                    SoundPlayer.PlayAndDestroy(0, btnCommonSound);
                                }
                            }
                        }
                    }
                }
                if (OnTouchDown != null)
                {
                    OnTouchDown(index);
                }
                currPointerId = eventData.pointerId;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            bool canExecUp = true;
            if (!Application.isEditor && Application.platform != RuntimePlatform.WindowsPlayer)
            {
                canExecUp = (currPointerId == eventData.pointerId);
            }
            if (canExecUp)
            {
                if (OnTouchUp != null)
                {
                    OnTouchUp(index);
                }
                currPointerId = -1;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (enabled)
            {
                if (triggerMethod == TriggerMethod.Up)
                {
                    if (canTouchTimer <= 0f)
                    {
                        onTrigger.Invoke();
                        canTouchTimer = canTriggerInterval;
                        if (buttonChange && canTouchTimer > 0)
                        {
                            buttonChange.enabled = false;
                        }
                    }
                }
                if (OnTouchClick != null)
                {
                    OnTouchClick(index);
                }
            }
        }
    }
}