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
            if (CurrTopTouchShowBtnsCtrl == this)
            {
                CurrTopTouchShowBtnsCtrl = previousTouchShowBtnsCtrl;
            }
        }

        void Awake()
        {
            alphas = new float[buttons.Length];
            for (int i = 0; i < alphas.Length; i++)
            {
                Transform btns = buttons[i];
                if (btns)
                {
                    if (btns.GetComponent<Image>())
                    {
                        alphas[i] = btns.GetComponent<Image>().color.a;
                    }
                    else
                    {
                        alphas[i] = btns.GetComponent<RawImage>().color.a;
                    }
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
                Transform btns = buttons[i];
                if (btns)
                {
                    Tween.Alpha(0, btns, 0, 2, Tween.EaseType.ExpoEaseOut, (Transform target) =>
                    {
                        if (target)
                        {
                            target.gameObject.SetActive(false);
                        }
                    });
                }
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
                        if (btn)
                        {
                            btn.gameObject.SetActive(true);
                            Tween.StopAlpha(btn);
                            Tween.Alpha(btn, alphas[i]);
                        }
                    }
                    StartCoroutine(Start());
                }
            }
        }
    }
}