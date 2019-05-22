using Vuforia;
using UnityEngine;
using System.Collections.Generic;

namespace BToolkit
{
    public class VuforiaHelper : MonoBehaviour
    {

        static VuforiaHelper instance;
        public static VuforiaHelper Instance
        {
            get
            {
                if (!instance)
                {
                    GameObject obj = new GameObject("VuforiaHelper");
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<VuforiaHelper>();
                }
                return instance;
            }
        }
        Camera stereoCameraRight;
        ScreenOrientation defaultScreenOrientation;

        void Awake()
        {
            instance = this;
            defaultScreenOrientation = Screen.orientation;
        }

        void OnDestroy()
        {
            instance = null;
        }

        /// <summary>
        /// 开启AR识别并跟踪
        /// </summary>
        public static void StartTracker()
        {
            ObjectTracker mObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            if (mObjectTracker != null)
            {
                mObjectTracker.Start();
            }
        }

        /// <summary>
        /// 关闭AR识别并跟踪
        /// </summary>
        public static void StopTracker()
        {
            ObjectTracker mObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            if (mObjectTracker != null)
            {
                mObjectTracker.Stop();
            }
        }

        /// <summary>
        /// 切换到眼镜模式
        /// </summary>
        public void SwitchToEyewearMode()
        {
            //Debug.Log(">>>>>>>>>>>> SwitchToEyewearMode");
            if (CameraDevice.Instance.Stop() && CameraDevice.Instance.Deinit())
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                IEnumerable<IViewerParameters> viewerParameters = Device.Instance.GetViewerList().GetAllViewers();
                foreach (IViewerParameters vp in viewerParameters)
                {
                    if (vp.GetName().Equals("Cardboard v1"))
                    {
                        MixedRealityController.Instance.SetViewerParameters(vp);
                    }
                }
                MixedRealityController.Instance.SetMode(MixedRealityController.Mode.VIEWER_VR);
                VideoBackgroundManager.Instance.SetVideoBackgroundEnabled(true);
            }
        }

        /// <summary>
        /// 退出眼镜模式
        /// </summary>
        public void QuitEyewearMode()
        {
            //Debug.Log(">>>>>>>>>>>> QuitEyewearMode");
            Screen.orientation = defaultScreenOrientation;
            MixedRealityController.Instance.SetMode(MixedRealityController.Mode.HANDHELD_AR);
            VideoBackgroundManager.Instance.SetVideoBackgroundEnabled(true);
        }

        /// <summary>
        /// 开关闪光灯
        /// </summary>
        public void PutOnFlashlight(bool b)
        {
            CameraDevice.Instance.SetFlashTorchMode(b);
        }

    }
}