using UnityEngine;

namespace BToolkit
{
    public class CloudOffCardCtrl : MonoBehaviour
    {
        protected bool hadToScreen;

        /// <summary>
        /// 切换到全屏(模型不用传任何参数)
        /// </summary>
        public virtual void ToScreen(float videoW = 1, float videoH = 1, bool isAVProPlayer = false) { }

        /// <summary>
        /// 切换到AR跟踪
        /// </summary>
        public virtual void ToTrackable() { }

        /// <summary>
        /// 从UI展示中关闭
        /// </summary>
        public virtual void CloseFromUI()
        {
            ToTrackable();
        }
    }
}