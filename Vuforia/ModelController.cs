using UnityEngine;

namespace BToolkit
{
    public class ModelController : BImageTarget
    {
        public Model prefab;
        Transform arCamera;
        public Model model { get; private set; }
        ModelUIViewer uiModelViewer;
        public static ModelController proviousModelController;

        void Awake()
        {
            GetComponent<MeshRenderer>().enabled = false;
        }

        protected override void OnTrackingFound()
        {
            //关闭视频
            if (VideoCtrl.proviousVideoCtrl)
            {
                for (int i = 0; i < VideoCtrl.proviousVideoCtrl.videos.Length; i++)
                {
                    VideoCtrl.proviousVideoCtrl.videos[i].player.ToTrackable();
                    VideoCtrl.proviousVideoCtrl.videos[i].player.gameObject.SetActive(false);
                }
            }
            //关闭上一个模型
            if (proviousModelController && proviousModelController != this)
            {
                proviousModelController.ToTracking();
                if (proviousModelController.model)
                {
                    proviousModelController.model.gameObject.SetActive(false);
                }
            }
            proviousModelController = this;

            if (!model)
            {
                model = Instantiate(prefab);
                model.transform.SetParent(transform, false);
                model.arParent = transform;
            }
            model.gameObject.SetActive(true);
            ToTracking();
            if (uiModelViewer)
            {
                Destroy(uiModelViewer.gameObject);
            }
        }

        protected override void OnTrackingLost()
        {
            if (hadFoundOnce)
            {
                if (StorageManager.Instance.IsARHideWhenOffCard)
                {
                    if (model)
                    {
                        model.gameObject.SetActive(false);
                    }
                }
                else
                {
                    ToScreen();
                    uiModelViewer = ModelUIViewer.ShowWhenAROffCard(this);
                }
            }
        }

        /// <summary>
        /// 转到屏幕中心
        /// </summary>
        public void ToScreen()
        {
            if (!arCamera)
            {
                arCamera = GameObject.Find("ARCamera").transform;
            }
            if (model)
            {
                model.transform.SetParent(arCamera, true);
                Tween.Move(0, model.transform, model.screenPos, 0.5f, false, Tween.EaseType.ExpoEaseOut);
                model.transform.localEulerAngles = model.screenAngle;
                //旋转
                TouchRotate3DByOneFinger rotate = model.GetComponent<TouchRotate3DByOneFinger>();
                if (!rotate)
                {
                    rotate = model.gameObject.AddComponent<TouchRotate3DByOneFinger>();
                }
                rotate.attachCamera = arCamera.GetComponent<Camera>();
                rotate.autoRotate = false;
                //缩放
                TouchScaleByTwoFingers scale = model.GetComponent<TouchScaleByTwoFingers>();
                if (!scale)
                {
                    model.gameObject.AddComponent<TouchScaleByTwoFingers>();
                }
                //移动
                TouchMove3DByTwoFingers move = model.GetComponent<TouchMove3DByTwoFingers>();
                if (!move)
                {
                    move = model.gameObject.AddComponent<TouchMove3DByTwoFingers>();
                }
                move.attachCamera = arCamera.GetComponent<Camera>();
            }
        }

        /// <summary>
        /// 回到AR跟踪位置
        /// </summary>
        public void ToTracking()
        {
            if (uiModelViewer)
            {
                Destroy(uiModelViewer.gameObject);
            }
            if (model)
            {
                model.transform.SetParent(model.arParent);
                model.transform.localPosition = model.arPos;
                model.transform.localEulerAngles = model.arAngle;
                if (!isTracking)
                {
                    model.gameObject.SetActive(false);
                }
                //旋转
                TouchRotate3DByOneFinger rotate = model.GetComponent<TouchRotate3DByOneFinger>();
                if (rotate)
                {
                    Destroy(rotate);
                }
                //缩放
                TouchScaleByTwoFingers scale = model.GetComponent<TouchScaleByTwoFingers>();
                if (scale)
                {
                    Destroy(scale);
                }
                //移动
                TouchMove3DByTwoFingers move = model.GetComponent<TouchMove3DByTwoFingers>();
                if (move)
                {
                    Destroy(move);
                }
            }
        }
    }
}