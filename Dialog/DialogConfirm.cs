using UnityEngine;
using System;
using UnityEngine.UI;

namespace BToolkit
{
    public class DialogConfirm:MonoBehaviour
    {

        public RectTransform dialog;
        public Text text;
        public Button btnYes, btnNo;
        Action ConfirmEvent, CancelEvent;
        const string prefabPath = "Prefabs/UI/DialogConfirm";

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
            trans.SetParent(BUtils.GetTopCanvas(),false);
            DialogConfirm dialogConfirm = trans.GetComponent<DialogConfirm>();
            dialogConfirm.text.text = content;
            dialogConfirm.ConfirmEvent = OnConfirm;
            dialogConfirm.CancelEvent = OnCancel;
            Tween.Alpha(trans,0,false);
            Tween.Alpha(0,trans,0.5f,0.3f,Tween.EaseType.SineEaseOut,false);
            Vector2 pos = (dialogConfirm.dialog as RectTransform).anchoredPosition;
            Tween.Scale(dialogConfirm.dialog, Vector3.zero);
            Tween.Scale(0, dialogConfirm.dialog, Vector3.one, 0.3f, Tween.EaseType.BackEaseOut);
            return dialogConfirm;
        }

        protected virtual void OnDestroy()
        {
            ConfirmEvent = null;
            CancelEvent = null;
        }

        protected virtual void Start()
        {
            btnYes.onClick.AddListener(() =>
            {
                OnYesCli();
            });
            btnNo.onClick.AddListener(() =>
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