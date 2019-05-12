using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BToolkit
{
    public class ButtonChange : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Image target;
        public bool scale = true, texture;
        public float pressScale = 0.97f;
        public Sprite changeSprite;
        public AudioClip sound;
        public bool canPlayDefaultSound = true;
        Sprite spriteDefault;
        Vector3 defaultScale;
        Button button;
        BButton bButton;
        StateButton stateButton;
        static AudioClip btn_common_sound;

        void Awake()
        {
            if (!target)
            {
                target = GetComponent<Image>();
                if (target)
                {
                    spriteDefault = target.sprite;
                }
            }
            button = GetComponent<Button>();
            bButton = GetComponent<BButton>();
            stateButton = GetComponent<StateButton>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsEnable())
            {
                if (scale)
                {
                    if (defaultScale == Vector3.zero)
                    {
                        defaultScale = transform.localScale;
                    }
                    transform.localScale = defaultScale * pressScale;
                }
                if (texture && target)
                {
                    target.sprite = changeSprite;
                }
                if (sound)
                {
                    SoundPlayer.Play(0, sound);
                }
                else
                {
                    if (canPlayDefaultSound)
                    {
                        if (!btn_common_sound)
                        {
                            btn_common_sound = Resources.Load<AudioClip>("Sounds/btn_common");
                        }
                        SoundPlayer.Play(0, btn_common_sound);
                    }
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsEnable())
            {
                if (scale)
                {
                    transform.localScale = defaultScale;
                }
                if (texture && target)
                {
                    target.sprite = spriteDefault;
                }
            }
        }

        bool IsEnable()
        {
            if (!this.enabled)
            {
                return false;
            }
            else
            {
                if (button)
                {
                    return button.enabled;
                }
                else if (bButton)
                {
                    return bButton.enabled;
                }
                else if (stateButton)
                {
                    return stateButton.enabled;
                }
            }
            return true;
        }
    }
}