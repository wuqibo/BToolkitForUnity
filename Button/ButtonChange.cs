using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BToolkit
{
    public class ButtonChange : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Image target;
        public bool scale = true, color, texture;
        public float pressScale = 0.97f;
        public Color changeColor = new Color(1, 1, 1, 1);
        public Sprite changeSprite;

        Color colorDefault;
        Sprite spriteDefault;
        Vector3 defaultScale;
        Button button;
        BButton bButton;
        StateButton stateButton;

        void Awake()
        {
            if (!target)
            {
                target = GetComponent<Image>();
                if (target)
                {
                    spriteDefault = target.sprite;
                    colorDefault = target.color;
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
                if (color && target)
                {
                    target.color = changeColor;
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
                if (color && target)
                {
                    target.color = colorDefault;
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