using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BToolkit
{
    [AddComponentMenu("BToolkit/StateButton")]
    [RequireComponent(typeof(Image))]
    public class StateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public int index;
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
                isOn = !isOn;
                RefreshState();
                SoundPlayer.PlayAndDestroy(0, sound);
                if (onValueChanged != null)
                {
                    onValueChanged.Invoke(isOn);
                }
            }
        }

        void RefreshState()
        {
            if (onState)
            {
                onState.SetActive(isOn);
            }
            if (offState)
            {
                offState.SetActive(!isOn);
            }
        }
    }
}