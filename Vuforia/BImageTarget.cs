using UnityEngine;
using Vuforia;

namespace BToolkit
{
    public abstract class BImageTarget : MonoBehaviour, ITrackableEventHandler
    {
        public ImageTargetBehaviour imageTargetBehaviour;

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

        void ITrackableEventHandler.OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                    newStatus == TrackableBehaviour.Status.TRACKED ||
                    newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
        }

        protected abstract void OnTrackingFound();
        protected abstract void OnTrackingLost();
    }
}