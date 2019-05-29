using UnityEngine;
using UnityEngine.Video;

namespace BToolkit
{
    public class VideoCtrl : BImageTarget
    {
        [System.Serializable]
        public class Video
        {
            public BVideoPlayer player;
            public string path;
        }
        public Video[] videos;
        public static VideoCtrl proviousVideoCtrl { get; private set; }

        void Awake()
        {
            for (int i = 0; i < videos.Length; i++)
            {
                videos[i].player.source = VideoSource.Url;
                videos[i].player.playOnAwake = false;
                videos[i].player.url = Application.streamingAssetsPath + "/" + videos[i].path;
            }
            GetComponent<MeshRenderer>().enabled = false;
        }

        protected override void OnTrackingFound()
        {
            //关闭模型
            if (ModelController.proviousModelController)
            {
                ModelController.proviousModelController.ToTracking();
                if (ModelController.proviousModelController.model)
                {
                    ModelController.proviousModelController.model.gameObject.SetActive(false);
                }
            }
            //关闭上一个视频
            if (proviousVideoCtrl && proviousVideoCtrl != this)
            {
                for (int i = 0; i < proviousVideoCtrl.videos.Length; i++)
                {
                    proviousVideoCtrl.videos[i].player.ToTrackable();
                    proviousVideoCtrl.videos[i].player.gameObject.SetActive(false);
                }
            }
            proviousVideoCtrl = this;

            videos[0].player.ToTrackable();
            videos[0].player.Play();
            if (videos.Length > 1)
            {
                videos[1].player.gameObject.SetActive(true);
                videos[1].player.Play();
            }
        }

        protected override void OnTrackingLost()
        {
            if (hadFoundOnce)
            {
                if (videos.Length > 1)
                {
                    videos[1].player.gameObject.SetActive(false);
                }
                if (!StorageManager.Instance.IsARHideWhenOffCard)
                {
                    videos[0].player.ToScreen();
                }
            }
        }
    }
}