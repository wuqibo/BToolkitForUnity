using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BToolkit
{
    [AddComponentMenu("BToolkit/SwitchButton")]
    [RequireComponent(typeof(Image))]
    public class SwitchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        public RectTransform bar;
        public float offset = 50;
        public Shader grayShader;//Shader不支持打包AssetBundle
        public AudioClip sound;
        [System.Serializable]
        public class ValueEvent : UnityEvent<bool> { }
        public ValueEvent onValueChanged;
        Material grayMaterial;
        Image bgImage, barImage;
        float barCurrX;
        bool isOn;

        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                SetBarButtonState(isOn, true);
            }
        }

        void Awake()
        {
            if(grayShader)
            {
                grayMaterial = new Material(grayShader);
            }
            bgImage = GetComponent<Image>();
            barImage = bar.GetComponent<Image>();
            SetBarButtonState(isOn, false);
        }

        void Update()
        {
            Vector2 pos = bar.anchoredPosition;
            pos.x += (barCurrX - pos.x) * 0.7f;
            bar.anchoredPosition = pos;
        }

        public void OnPointerDown(PointerEventData eventData) { }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (this.enabled)
            {
                SetBarButtonState(!isOn, true);
                SoundPlayer.Play(0,sound);
                if (onValueChanged != null)
                {
                    onValueChanged.Invoke(isOn);
                }
            }
        }

        void SetBarButtonState(bool on, bool useFade)
        {
            isOn = on;
            barCurrX = isOn ? offset : -offset;
            if (!useFade)
            {
                Vector2 pos = bar.anchoredPosition;
                pos.x = barCurrX;
                bar.anchoredPosition = pos;
            }
            if (grayMaterial)
            {
				if (!bgImage)
                {
                    bgImage = GetComponent<Image>();
                }
                if (!barImage)
                {
                    barImage = bar.GetComponent<Image>();
                }
                bgImage.material = barImage.material = isOn ? null : grayMaterial;
            }
        }
    }
}