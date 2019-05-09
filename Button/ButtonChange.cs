using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BToolkit
{
    [AddComponentMenu("BToolkit/ButtonChange")]
    public class ButtonChange : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Image target;
        public bool scale = true, texture;
        public float changeScale = 0.97f;
        public Sprite changeSprite;
        public AudioClip sound;
        public bool listenKeyBack;
        public bool canPlayGlobalSound = true;
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
            spriteDefault = target.sprite;
            button = GetComponent<Button>();
            bButton = GetComponent<BButton>();
            stateButton = GetComponent<StateButton>();
        }

        void Update()
        {
            if (listenKeyBack)
            {
                if (IsEnable())
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (button)
                        {
                            button.onClick.Invoke();
                        }
                        else if (bButton)
                        {
                            bButton.onTrigger.Invoke();
                        }
                        else if (stateButton)
                        {
                            stateButton.IsOn = !stateButton.IsOn;
                        }
                    }
                }
            }
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
                    transform.localScale = defaultScale * changeScale;
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
                    if (canPlayGlobalSound)
                    {
                        //if (Lobby.UI.ResSounds.instance)
                        //{
                        //    SoundPlayer.Play(0, Lobby.UI.ResSounds.instance.btnUp);
                        //}
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