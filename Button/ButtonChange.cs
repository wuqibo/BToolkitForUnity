using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BToolkit
{
    public class ButtonChange : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Image target;
        public bool scale = true, color, texture;
        public float pressScale = 0.95f;
        public Color changeColor = new Color(0.97f, 0.97f, 0.97f, 1);
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
            }
			if (target)
            {
                spriteDefault = target.sprite;
                colorDefault = target.color;
            }
            button = GetComponent<Button>();
            bButton = GetComponent<BButton>();
            stateButton = GetComponent<StateButton>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsEnable())
            {
				if(target){
                    if (scale)
                    {
                        if (defaultScale == Vector3.zero)
                        {
                            defaultScale = target.transform.localScale;
                        }
                        target.transform.localScale = defaultScale * pressScale;
                    }
                    if (texture)
                    {
                        target.sprite = changeSprite;
                    }
                    if (color)
                    {
                        target.color = changeColor;
                    }
				}
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsEnable())
            {
				if(target){
					if (scale)
					{
						target.transform.localScale = defaultScale;
					}
					if (texture)
					{
						target.sprite = spriteDefault;
					}
					if (color)
					{
						target.color = colorDefault;
					}
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