using System;
using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class TVButton:MonoBehaviour
    {

        public enum ListenKey
        {
            TVTelecontroller,
            Back,
            Enter
        }
        public bool canScale = true;
        public float selectedScale = 1.1f;
        public Sprite normal, select;
        public AudioClip selectSound;
        public float selectSoundVolume = 0.2f;
        public AudioClip clickSound;
        public float clickSoundVolume = 0.2f;
        public float excuteDelay = 0;
        public ListenKey listenKey = ListenKey.TVTelecontroller;
        Image image;
        Vector3 defaultScale;
        Button button;

        void Awake()
        {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
            defaultScale = image.transform.localScale;
        }

        public void OnSelected(bool b)
        {
            if(select)
            {
                image.sprite = b ? select : normal;
            }
            if(canScale)
            {
                image.transform.localScale = b ? defaultScale * selectedScale : defaultScale;
            }
            if(b)
            {
                SoundPlayer.PlayAndDestroy(0,selectSound,selectSoundVolume);
            }
        }

        void Update()
        {
            if(listenKey == ListenKey.Back)
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    OnPress();
                }
                else if(Input.GetKeyUp(KeyCode.Escape))
                {
                    OnClick();
                }
            }
            else if(listenKey == ListenKey.Enter)
            {
                if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown((KeyCode)10))
                {
                    OnPress();
                }
                else if(Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.JoystickButton0) || Input.GetKeyUp((KeyCode)10))
                {
                    OnClick();
                }
            }
        }

        public void OnPress()
        {
            image.transform.localScale = defaultScale;
        }

        public void OnClick()
        {
            image.transform.localScale = defaultScale * selectedScale;
            if(excuteDelay <= 0f)
            {
                ExcuteClick();
            }
            else
            {
                Invoke("ExcuteClick",excuteDelay);
            }
            SoundPlayer.PlayAndDestroy(0,clickSound,clickSoundVolume);
        }

        void ExcuteClick()
        {
            button.onClick.Invoke();
        }
    }
}