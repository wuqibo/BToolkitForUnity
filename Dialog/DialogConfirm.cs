using UnityEngine;
using System;
using UnityEngine.UI;

namespace BToolkit
{
    public class DialogConfirm:MonoBehaviour
    {

        public RectTransform dialog;
        public Text text;
        public BButton btnYes, btnNo;
        Action ConfirmEvent, CancelEvent;
        public static bool isShowing { private set; get; }
        const string prefabPath = "DialogConfirm";

        public static DialogConfirm Show(string content,Action OnConfirm,RectTransform prefab = null)
        {
            return Show(content,OnConfirm,null,prefab);
        }

        public static DialogConfirm Show(string content,Action OnConfirm,Action OnCancel,RectTransform prefab = null)
        {
            if(!prefab)
            {
                prefab = Resources.Load<RectTransform>(prefabPath);
                if(!prefab)
                {
                    Debuger.LogError("目录 " + prefabPath + " 下找不到预设体");
                    return null;
                }
            }
            RectTransform trans = Instantiate(prefab);
            trans.SetParent(FindObjectOfType<Canvas>().transform, false);
            DialogConfirm dialogConfirm = trans.GetComponent<DialogConfirm>();
            dialogConfirm.text.text = content;
            dialogConfirm.ConfirmEvent = OnConfirm;
            dialogConfirm.CancelEvent = OnCancel;
            Tween.Alpha(trans,0,false);
            Tween.Alpha(0,trans,0.5f,0.2f,Tween.EaseType.SineEaseOut,false);
            Vector2 pos = (dialogConfirm.dialog as RectTransform).anchoredPosition;
            Tween.Scale(dialogConfirm.dialog, Vector3.zero);
            Tween.Scale(0, dialogConfirm.dialog, Vector3.one, 0.2f, Tween.EaseType.BackEaseOut);
            isShowing = true;
            return dialogConfirm;
        }

        protected virtual void OnDestroy()
        {
            ConfirmEvent = null;
            CancelEvent = null;
            isShowing = false;
        }

        protected virtual void Start()
        {
            btnYes.onTrigger.AddListener(() =>
            {
                OnYesCli();
            });
            btnNo.onTrigger.AddListener(() =>
            {
                OnNoCli();
            });
        }

        protected virtual void Update()
        {
            //遥控器和手机的返回键
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                OnNoCli();
            }
            //遥控器的确认键
            if(Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                OnYesCli();
            }
        }

        public virtual void OnYesCli()
        {
            Out();
            if(ConfirmEvent != null)
            {
                ConfirmEvent();
                ConfirmEvent = null;
            }
        }

        public virtual void OnNoCli()
        {
            Out();
            if(CancelEvent != null)
            {
                CancelEvent();
                CancelEvent = null;
            }
        }

        void Out()
        {
            Destroy(gameObject);
        }
    }
}