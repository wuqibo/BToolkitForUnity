using UnityEngine;
using Vuforia;

namespace BToolkit
{
    public abstract class BImageTarget : MonoBehaviour, ITrackableEventHandler
    {
        public ImageTargetBehaviour imageTargetBehaviour;
        public bool isTracking { get; private set; }

        protected virtual void Start()
        {
            if (!imageTargetBehaviour)
            {
                imageTargetBehaviour = GetComponent<ImageTargetBehaviour>();
            }
            if (imageTargetBehaviour)
            {
                imageTargetBehaviour.RegisterTrackableEventHandler(this);
            }
        }

        /// <summary>
        /// 设置ImageTargetBehaviour对象
        /// </summary>
        public void SetImageTargetBehaviour(ImageTargetBehaviour imageTargetBehaviour)
        {
            this.imageTargetBehaviour = imageTargetBehaviour;
            if (imageTargetBehaviour)
            {
                imageTargetBehaviour.RegisterTrackableEventHandler(this);
            }
        }

        void ITrackableEventHandler.OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                    newStatus == TrackableBehaviour.Status.TRACKED ||
                    newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                isTracking = true;
                OnTrackingFound();
            }
            else
            {
                isTracking = false;
                OnTrackingLost();
            }
        }

        protected abstract void OnTrackingFound();
        protected abstract void OnTrackingLost();
    }
}