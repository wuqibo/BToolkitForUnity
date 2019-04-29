using UnityEngine;

namespace BToolkit
{
    public class AndroidUtils
    {

        static AndroidJavaObject jo;
        /// <summary>
        /// 调用Java(Android)层的非静态函数
        /// </summary>
        public static void CallAndroidFunction(string functionName, params object[] args)
        {
            if (jo == null)
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            }
            jo.Call(functionName, args);
        }
        public static T CallAndroidFunction<T>(string functionName, params object[] args)
        {
            if (jo == null)
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return jo.Call<T>(functionName, args);
        }
        /// <summary>
        /// 调用Java(Android)层的静态函数，无返回值
        /// </summary>
        public static void CallAndroidStaticFunction(string classPath, string methodName, params object[] args)
        {
            AndroidJavaClass jc = new AndroidJavaClass(classPath);
            jc.CallStatic(methodName, args);
        }
        /// <summary>
        /// 调用Java(Android)层的静态函数，有返回值
        /// </summary>
        public static T CallAndroidStaticFunction<T>(string classPath, string methodName, params object[] args)
        {
            AndroidJavaClass jc = new AndroidJavaClass(classPath);
            return jc.CallStatic<T>(methodName, args);
        }

    }
}