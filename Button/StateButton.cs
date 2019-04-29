using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BToolkit
{
    [AddComponentMenu("BToolkit/StateButton")]
    [RequireComponent(typeof(Image))]
    public class StateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public int index;
        public bool autoChange = true;
        public bool useAlpha = false;
        public Image target;
        public Sprite onSprite, offSprite;
        public AudioClip sound;
        public RectTransform rectTransform { get { return transform as RectTransform; } }
        [System.Serializable]
        public class ValueEvent : UnityEvent<bool> { }
        public ValueEvent onValueChanged;
        bool isOn;

        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                SetSprite();
            }
        }

        void Awake()
        {
            if (!target)
            {
                target = GetComponent<Image>();
            }
            SetSprite();
        }

        //为了能显示enable的勾
        void Start() { }

        public void OnPointerDown(PointerEventData eventData) { }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this.enabled)
            {
                if (autoChange)
                {
                    isOn = !isOn;
                    SetSprite();
                }
                SoundPlayer.Play(0, sound);
                if (onValueChanged != null)
                {
                    onValueChanged.Invoke(isOn);
                }
            }
        }

        void SetSprite()
        {
            if (target)
            {
                if (isOn)
                {
                    if (useAlpha)
                    {
                        target.color = Color.white;
                    }
                    else
                    {
                        target.color = Color.white;
                        target.sprite = onSprite;
                    }
                }
                else
                {
                    if (useAlpha)
                    {
                        target.color = new Color(0, 0, 0, 0);
                    }
                    else
                    {
                        target.color = Color.white;
                        target.sprite = offSprite;
                    }
                }
            }
        }
    }
}