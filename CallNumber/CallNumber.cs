using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class CallNumber
    {

        public static void Call(string number)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidUtils.CallAndroidStaticFunction("cn.btoolkit.callnumber.CallNumber", "call", number);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _CallNum(number);
            }
        }

        [DllImport("__Internal")]
        static extern void _CallNum(string number);
    }
}