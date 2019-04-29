using UnityEngine;
using System;
using UnityEngine.UI;

namespace BToolkit
{
    public class DialogAlert : MonoBehaviour
    {
        Action CloseEvent = null;
        public RectTransform dialog;
        public Text texTitle, texContent;
        public Button btnClose;
        public static bool isShowing { private set; get; }
        static bool autoDestroy;
        const string prefabPath = "Prefabs/UI/DialogAlert";

        public static DialogAlert Show(string content, RectTransform prefab = null)
        {
            return Show(null, content, 0f, null, prefab);
        }

        public static DialogAlert Show(string content, Action OnClose, RectTransform prefab = null)
        {
            return Show(null, content, 0f, OnClose, prefab);
        }

        public static DialogAlert Show(string content, float time, RectTransform prefab = null)
        {
            return Show(null, content, time, null, prefab);
        }

        public static DialogAlert Show(string content, float time, Action OnClose, RectTransform prefab = null)
        {
            return Show(null, content, time, OnClose, prefab);
        }

        public static DialogAlert Show(string title, string content, RectTransform prefab = null)
        {
            return Show(title, content, 0f, null, prefab);
        }

        public static DialogAlert Show(string title, string content, float time, RectTransform prefab = null)
        {
            return Show(title, content, time, null, prefab);
        }

        public static DialogAlert Show(string title, string content, Action OnClose, RectTransform prefab = null)
        {
            return Show(title, content, 0f, OnClose, prefab);
        }

        public static DialogAlert Show(string title, string content, float time, Action OnClose, RectTransform prefab = null)
        {
            autoDestroy = (time > 0f);
            if (!prefab)
            {
                prefab = Resources.Load<RectTransform>(prefabPath);
                if (!prefab)
                {
                    Debuger.LogError("目录 " + prefabPath + " 下找不到预设体");
                    return null;
                }
            }
            RectTransform trans = Instantiate(prefab);
            trans.SetParent(BUtils.GetTopCanvas(), false);
            DialogAlert dialogAlert = trans.GetComponent<DialogAlert>();
            dialogAlert.CloseEvent = OnClose;
            if (!dialogAlert.dialog)
            {
                dialogAlert.dialog = trans;
            }
            if (dialogAlert.texTitle)
            {
                if (string.IsNullOrEmpty(title))
                {
                    dialogAlert.texTitle.gameObject.SetActive(false);
                }
                else
                {
                    dialogAlert.texTitle.gameObject.SetActive(true);
                    dialogAlert.texTitle.text = title;
                }
            }
            dialogAlert.texContent.text = content;
            Tween.Alpha(trans, 0, false);
            Tween.Alpha(0, trans, 0.5f, 0.3f, Tween.EaseType.SineEaseOut, false);
            Vector2 pos = (dialogAlert.dialog as RectTransform).anchoredPosition;
            Tween.Scale(dialogAlert.dialog, Vector3.zero);
            Tween.Scale(0, dialogAlert.dialog, Vector3.one, 0.3f, Tween.EaseType.BackEaseOut);
            if (time > 0f)
            {
                dialogAlert.Invoke("Out", time);
            }
            isShowing = true;
            return dialogAlert;
        }

        void OnDestroy()
        {
            isShowing = false;
        }

        void Start()
        {
            if (btnClose)
            {
                btnClose.onClick.AddListener(() =>
                {
                    Out();
                });
            }
        }

        void Update()
        {
            if (!autoDestroy)
            {
                if (!btnClose)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        Out();
                    }
                }
            }
        }

        void Out()
        {
            if (CloseEvent != null)
            {
                CloseEvent();
                CloseEvent = null;
            }
            Destroy(gameObject);
        }

    }
}