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
        public GameObject onState, offState;
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
                RefreshState();
            }
        }

        void Awake()
        {
            RefreshState();
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
                    RefreshState();
                }
                SoundPlayer.PlayAndDestroy(0, sound);
                if (onValueChanged != null)
                {
                    onValueChanged.Invoke(isOn);
                }
            }
        }

        void RefreshState()
        {
            onState.SetActive(isOn);
            offState.SetActive(!isOn);
        }
    }
}