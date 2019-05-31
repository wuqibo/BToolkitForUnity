using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class TouchShowBtnsCtrl : MonoBehaviour
    {
        public Transform[] buttons;
        private float[] alphas;
        private const float ShowDuration = 2;
        private bool isShow;
        TouchShowBtnsCtrl previousTouchShowBtnsCtrl;
        static TouchShowBtnsCtrl CurrTopTouchShowBtnsCtrl;

        void OnDestroy()
        {
            if(CurrTopTouchShowBtnsCtrl == this)
            {
                CurrTopTouchShowBtnsCtrl = previousTouchShowBtnsCtrl;
            }
        }

        void Awake()
        {
            alphas = new float[buttons.Length];
            for (int i = 0; i < alphas.Length; i++)
            {
                if (buttons[i].GetComponent<Image>())
                {
                    alphas[i] = buttons[i].GetComponent<Image>().color.a;
                }
                else
                {
                    alphas[i] = buttons[i].GetComponent<RawImage>().color.a;
                }
            }
            previousTouchShowBtnsCtrl = CurrTopTouchShowBtnsCtrl;
            CurrTopTouchShowBtnsCtrl = this;
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(ShowDuration);
            isShow = false;
            for (int i = 0; i < buttons.Length; i++)
            {
                Tween.Alpha(0, buttons[i], 0, 2, Tween.EaseType.ExpoEaseOut, (Transform target) =>
                {
                    target.gameObject.SetActive(false);
                });
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (CurrTopTouchShowBtnsCtrl != this)
                {
                    return;
                }
                if (!isShow)
                {
                    isShow = true;
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        Transform btn = buttons[i];
                        btn.gameObject.SetActive(true);
                        Tween.StopAlpha(btn);
                        Tween.Alpha(btn, alphas[i]);
                    }
                    StartCoroutine(Start());
                }
            }
        }
    }
}