using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class CopyAndPaste
    {
        /// <summary>
        /// 复制到系统剪贴板
        /// </summary>
        public static void Copy(string text)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.copyandpaste.CopyAndPaste", "copy", text);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _Copy(text);
            }
            else
            {
                GUIUtility.systemCopyBuffer = text;
            }
        }

        /// <summary>
        /// 从系统剪贴板获取复制的内同
        /// </summary>
        public static string GetPaste()
        {
            string text = "";
            if (Application.platform == RuntimePlatform.Android)
            {
                text = AndroidHelper.CallAndroidStaticFunction<string>("cn.btoolkit.copyandpaste.CopyAndPaste", "getPaste");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                text = _GetPaste();
            }
            else
            {
                text = GUIUtility.systemCopyBuffer;
            }
            return text;
        }

        [DllImport("__Internal")]
        static extern void _Copy(string text);

        [DllImport("__Internal")]
        static extern string _GetPaste();
    }
}