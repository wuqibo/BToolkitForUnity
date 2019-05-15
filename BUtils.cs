using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BToolkit
{
    public class BUtils : MonoBehaviour
    {
        private static BUtils instance;
        public static BUtils Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<BUtils>();
                    if (!instance)
                    {
                        GameObject go = new GameObject("BUtils");
                        DontDestroyOnLoad(go);
                        instance = go.AddComponent<BUtils>();
                    }
                }
                return instance;
            }
        }
        public static Action<Texture2D> FinishCallbackEvent = null;
        private static CanvasScaler canvasScaler;
        private static Vector2 screenUISize;
        public static Vector2 ScreenUISize
        {
            get
            {
                if (screenUISize == Vector2.zero)
                {
                    if (!canvasScaler)
                    {
                        canvasScaler = GameObject.FindObjectOfType<CanvasScaler>();
                    }
                }
                if (canvasScaler.matchWidthOrHeight > 0.5f)
                {
                    screenUISize = new Vector2(canvasScaler.referenceResolution.y * (float)Screen.width / (float)Screen.height, canvasScaler.referenceResolution.y);
                }
                else
                {
                    screenUISize = new Vector2(canvasScaler.referenceResolution.x, canvasScaler.referenceResolution.x * (float)Screen.height / (float)Screen.width);
                }
                return screenUISize;
            }
        }

        /// <summary>
        /// 截取屏幕并返回
        /// </summary>
        public static void ScreenShot(Action<Texture2D> FinishCallback)
        {
            FinishCallbackEvent = FinishCallback;
            Instance.StartCoroutine(DoScreenShot());
        }
        static IEnumerator DoScreenShot()
        {
            Texture2D cutImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            yield return new WaitForEndOfFrame();
            cutImage.ReadPixels(rect, 0, 0, true);
            cutImage.Apply();
            yield return cutImage;
            if (FinishCallbackEvent != null)
            {
                FinishCallbackEvent(cutImage);
            }
        }
        
        /// <summary>
        /// 判断字符串是否可以转为整型
        /// </summary>
        public static bool IsInt(string value)
        {
            int result;
            return int.TryParse(value, out result);
        }
        /// <summary>
        /// 判断字符串是否可以转为浮点型
        /// </summary>
        public static bool IsFloat(string value)
        {
            float result;
            return float.TryParse(value, out result);
        }

        /// <summary>
        /// 播放Animator动画
        /// </summary>
        public static void PlayAnim(Animator animator, string clipName, float transitionDuration)
        {
            if (animator)
            {
                if (transitionDuration <= 0)
                {
                    animator.Play(clipName, 0, 0f);
                }
                else
                {
                    if (animator.GetNextAnimatorStateInfo(0).fullPathHash == 0)
                    {
                        animator.CrossFade(clipName, transitionDuration);
                    }
                    else
                    {
                        animator.Play(clipName, 0, 0f);
                    }
                }
            }
        }

        /// <summary>
        /// 获取经纬度
        /// </summary>
        /*
        public delegate void GPSDelegate(float latitude,float longitude);
        static GPSDelegate GPSCallback;
        public static void GetGPS(float desiredAccuracyInMeters,float updateDistanceInMeters,GPSDelegate Callback) {
            GPSCallback = Callback;
            Instance.StartCoroutine(StartGPS(desiredAccuracyInMeters,updateDistanceInMeters));
        }
        static IEnumerator StartGPS(float desiredAccuracyInMeters,float updateDistanceInMeters) {
            if(!Input.location.isEnabledByUser) {
                Debuger.Log("isEnabledByUser value is:" + Input.location.isEnabledByUser.ToString() + " Please turn on the GPS");
                yield break;
            }
            Input.location.Start(desiredAccuracyInMeters,updateDistanceInMeters);//第一个参数设置位置精确度，比如500则可不用打开GPS，用流量即可获取到，第二个参数设置设备横向移动500米位置才会更新)
            int maxWait = 20;
            while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
                yield return new WaitForSeconds(1);
                maxWait--;
            }
            if(maxWait < 1) {
                Debuger.Log("Init GPS service time out");
                yield break;
            }
            if(Input.location.status == LocationServiceStatus.Failed) {
                Debuger.Log("Unable to determine device location");
                yield break;
            } else {
                float latitude = Input.location.lastData.longitude;
                float longitude = Input.location.lastData.latitude;
                GPSCallback(latitude,longitude);
            }
        }
        */

        /// <summary>
        /// 手机震动
        /// </summary>
        public static void MobileVibrate()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
        }

        /// <summary>
        /// 从SD卡里安装APK包
        /// </summary>
        public static void InstallAPK(string path)
        {
            //确保已导入InstallAPK.jar包
            AndroidJavaClass jc = new AndroidJavaClass("cn.wqb.tools.InstallAPK");
            jc.CallStatic("install", path);
        }

        /// <summary>
        /// 屏幕转换到UGUI坐标
        /// </summary>
        public static Vector2 ScreenToUGUIPoint(Vector3 screenPosition)
        {
            Vector2 p;
            p.x = ScreenUISize.x * (screenPosition.x - Screen.width * 0.5f) / (float)Screen.width;
            p.y = ScreenUISize.y * (screenPosition.y - Screen.height * 0.5f) / (float)Screen.height;
            return p;
        }

        /// <summary>
        /// UGUI转换到屏幕坐标
        /// </summary>
        public static Vector3 UGUIToScreenPoint(Vector2 uiPosition)
        {
            Vector3 p;
            p.x = Screen.width * (uiPosition.x + ScreenUISize.x * 0.5f) / ScreenUISize.x;
            p.y = Screen.height * (uiPosition.y + ScreenUISize.y * 0.5f) / ScreenUISize.y;
            p.z = 0;
            return p;
        }

        /// <summary>
        /// 判断是否点击在UI上
        /// </summary>
        public static bool IsTouchOnUGUI
        {
            get
            {
                if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
                {
                    return EventSystem.current.IsPointerOverGameObject();
                }
                else
                {
                    return EventSystem.current.IsPointerOverGameObject(0);
                }
            }
        }

        /// <summary>
        /// Application.streamingAssets方法分平台处理，否则移动平台上可能无法下载数据(不带file:///)
        /// </summary>
        /// <returns></returns>
        public static string streamingAssetsPath
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                return Application.dataPath + "/StreamingAssets";
#elif UNITY_ANDROID && !UNITY_EDITOR
                return Application.dataPath + "!/assets";
#elif UNITY_IOS && !UNITY_EDITOR
                return Application.dataPath + "/Raw";
#endif
            }
        }

        /// <summary>
        /// Application.streamingAssets方法分平台处理，否则移动平台上可能无法下载数据(自带file:///)
        /// </summary>
        /// <returns></returns>
        public static string streamingAssetsPathForWebRequest
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                return "file:///" + streamingAssetsPath;
#elif UNITY_ANDROID && !UNITY_EDITOR
                return "jar:file:///"+streamingAssetsPath;
#elif UNITY_IOS && !UNITY_EDITOR
                return "file:///"+streamingAssetsPath;
#endif
            }
        }

    }
}