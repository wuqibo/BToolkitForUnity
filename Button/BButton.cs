using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BToolkit
{
    [AddComponentMenu("BToolkit/BButton")]
    public class BButton : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
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

        void OnDisable()
        {
            if (OnTouchUp != null)
            {
                OnTouchUp(index);
            }
        }

        void Start() { }

        void Update()
        {
            if (canTouchTimer > 0f)
            {
                canTouchTimer -= Time.deltaTime;
            }
            if (doubleTimer > 0f)
            {
                doubleTimer -= Time.deltaTime;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (OnTouchUp != null)
                {
                    OnTouchUp(index);
                }
            }
            if (listenKeyBack)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    onTrigger.Invoke();
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
                        if (sound)
                        {
                            SoundPlayer.Play(0, sound);
                        }
                        else
                        {
                            if (useCommonSound)
                            {
                                if (!btnCommonSound)
                                {
                                    btnCommonSound = Resources.Load<AudioClip>("Sounds/btn_common");
                                }
                                SoundPlayer.Play(0, btnCommonSound);
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
                            if (sound)
                            {
                                SoundPlayer.Play(0, sound);
                            }
                            else
                            {
                                if (useCommonSound)
                                {
                                    if (!btnCommonSound)
                                    {
                                        btnCommonSound = Resources.Load<AudioClip>("Sounds/btn_common");
                                    }
                                    SoundPlayer.Play(0, btnCommonSound);
                                }
                            }
                        }
                    }
                }
                if (OnTouchDown != null)
                {
                    OnTouchDown(index);
                }
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