﻿using UnityEngine;

namespace BToolkit
{
    public class BVideoPlayerUICtrl : MonoBehaviour
    {
        public BButton btnClose;
        public BButton btnDirection;
        public BVideoPlayer videoPlayer { get; private set; }
        GameObject panelDefault;

        public void SetBVideoPlayer(BVideoPlayer videoPlayer)
        {
            this.videoPlayer = videoPlayer;
        }

        void OnDisable()
        {
            if (panelDefault)
            {
                panelDefault.SetActive(true);
            }
        }

        void Awake()
        {
            panelDefault = GameObject.Find("PanelDefault");
            if (panelDefault)
            {
                panelDefault.SetActive(false);
            }
            if (btnClose)
            {
                btnClose.onTrigger.AddListener(() =>
                {
                    Destroy(gameObject);
                    if (videoPlayer)
                    {
                        videoPlayer.ToTrackable();
                        videoPlayer.gameObject.SetActive(false);
                    }
                });
            }
            if (btnDirection)
            {
                btnDirection.onTrigger.AddListener(() =>
                {
                    if (videoPlayer)
                    {
                        videoPlayer.SwitchDirection();
                        SetPlayerTransSize();
                    }
                });
            }
        }

        void SetPlayerTransSize()
        {
            if (videoPlayer)
            {
                if (videoPlayer.transform.localEulerAngles.z == 0)
                {
                    //按钮
                    if (btnClose)
                    {
                        RectTransform btnCloseTrans = btnClose.rectTransform;
                        btnCloseTrans.anchorMin = new Vector2(0, 1);
                        btnCloseTrans.anchorMax = new Vector2(0, 1);
                        btnCloseTrans.anchoredPosition = new Vector2(75, -70);
                        btnCloseTrans.localEulerAngles = new Vector3(0, 0, 0);
                    }
                    if (btnDirection)
                    {
                        RectTransform btnDirectionTrans = btnDirection.rectTransform;
                        btnDirectionTrans.anchorMin = new Vector2(1, 1);
                        btnDirectionTrans.anchorMax = new Vector2(1, 1);
                        btnDirectionTrans.anchoredPosition = new Vector2(-85, -70);
                        btnDirectionTrans.localEulerAngles = new Vector3(0, 0, 0);
                    }
                }
                else
                {
                    //按钮
                    if (btnClose)
                    {
                        RectTransform btnCloseTrans = btnClose.rectTransform;
                        btnCloseTrans.anchorMin = new Vector2(1, 1);
                        btnCloseTrans.anchorMax = new Vector2(1, 1);
                        btnCloseTrans.anchoredPosition = new Vector2(-75, -80);
                        btnCloseTrans.localEulerAngles = new Vector3(0, 0, -90);
                    }
                    if (btnDirection)
                    {
                        RectTransform btnDirectionTrans = btnDirection.rectTransform;
                        btnDirectionTrans.anchorMin = new Vector2(1, 0);
                        btnDirectionTrans.anchorMax = new Vector2(1, 0);
                        btnDirectionTrans.anchoredPosition = new Vector2(-75, 80);
                        btnDirectionTrans.localEulerAngles = new Vector3(0, 0, -90);
                    }
                }
            }
        }
    }
}