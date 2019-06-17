using UnityEngine;

namespace BToolkit
{
    public class ImageTarget_Model : BImageTarget
    {
        public OffCardController_Model prefab;
        public OffCardController_Model model { get; private set; }
        public static ImageTarget_Model proviousModelController;

        void Awake()
        {
            GetComponent<MeshRenderer>().enabled = false;
        }

        protected override void OnTrackingFound()
        {
            //关闭视频
            if (ImageTarget_Video.proviousVideoCtrl)
            {
                for (int i = 0; i < ImageTarget_Video.proviousVideoCtrl.videos.Length; i++)
                {
                    ImageTarget_Video.proviousVideoCtrl.videos[i].player.offCardController.ToTracking();
                    ImageTarget_Video.proviousVideoCtrl.videos[i].player.gameObject.SetActive(false);
                }
            }
            //关闭上一个模型
            if (proviousModelController && proviousModelController != this)
            {
                if (proviousModelController.model)
                {
                    proviousModelController.model.ToTracking();
                    proviousModelController.model.gameObject.SetActive(false);
                }
            }
            proviousModelController = this;

            if (!model)
            {
                model = Instantiate(prefab);
                model.transform.SetParent(transform, false);
                model.transform.localPosition = model.arPos;
                model.transform.localEulerAngles = model.arAngle;
                model.transform.localScale = model.arScale;
                model.arParent = transform;
            }
            model.ToTracking();
            model.gameObject.SetActive(true);
            UIController_Model.Destroy();
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
                    model.ToScreen(true);
                }
            }
        }

    }
}