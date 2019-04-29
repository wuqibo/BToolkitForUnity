using System;
using UnityEngine;

namespace BToolkit
{
    public class Wait : MonoBehaviour
    {

        public Transform icon;
        public float rotateSpeed = -300;
        private static Wait dialogWait;
        private const string prefabPath = "Prefabs/UI/Wait";
        private float currTimeout;
        private Action TimeoutEvent;


        /// <summary>
        /// 参数1：显示或隐藏。参数2：默认10秒后超时自动销毁，如果输入0则永不超时
        /// </summary>
        public static Wait Show(bool b, float timeout = 10f, Action OnTimeoutCallback = null)
        {
            if (b)
            {
                if (!dialogWait)
                {
                    Wait prefab = Resources.Load<Wait>(prefabPath);
                    if (!prefab)
                    {
                        Debug.LogError("目录 " + prefab + " 下找不到预设体");
                        return null;
                    }
                    dialogWait = Instantiate(prefab);
                }
                dialogWait.currTimeout = timeout;
				dialogWait.TimeoutEvent = OnTimeoutCallback;
                dialogWait.transform.SetParent(BUtils.GetTopCanvas(), false);
                return dialogWait;
            }
            else
            {
                if (dialogWait)
                {
                    DestroyImmediate(dialogWait.gameObject);
                }
            }
            return null;
        }

        void Update()
        {
            if (icon)
            {
                icon.Rotate(0, 0, rotateSpeed * Time.deltaTime);
            }
            if (currTimeout > 0f)
            {
                currTimeout -= Time.deltaTime;
                if (currTimeout <= 0f)
                {
                    DestroyImmediate(gameObject);
                    if (TimeoutEvent != null)
                    {
                        TimeoutEvent();
                    }
                }
            }
        }
    }
}