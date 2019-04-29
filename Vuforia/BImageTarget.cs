using UnityEngine;
using Vuforia;

namespace BToolkit
{
    public class BImageTarget : MonoBehaviour, ITrackableEventHandler
    {

        public bool disableRenderOnStart = true;
        [HideInInspector]
        public ImageTargetBehaviour imageTargetBehaviour;
        public delegate void OnTrackingDelegate(BImageTarget bImageTarget);
        protected TrackableBehaviour myTrackableBehaviour;
        [HideInInspector]
        public bool isTracking;
        private static OnTrackingDelegate TrackingFoundEvent, TrackingLostEvent;

        void Awake()
        {
            imageTargetBehaviour = GetComponent<ImageTargetBehaviour>();
        }

        /// <summary>
        /// 应在销毁的函数里同时注销事件: UnregisterHanlder()
        /// </summary>
        public static void RegisterHanlder(bool register, OnTrackingDelegate OnTrackingFound = null, OnTrackingDelegate OnTrackingLost = null)
        {
            if (register)
            {
                if (OnTrackingFound != null)
                {
                    TrackingFoundEvent += OnTrackingFound;
                }
                if (OnTrackingLost != null)
                {
                    TrackingLostEvent += OnTrackingLost;
                }
            }
            else
            {
                if (OnTrackingFound != null)
                {
                    TrackingFoundEvent -= OnTrackingFound;
                }
                else
                {
                    TrackingFoundEvent = null;
                }
                if (OnTrackingLost != null)
                {
                    TrackingLostEvent -= OnTrackingLost;
                }
                else
                {
                    TrackingLostEvent = null;
                }
            }

        }

        protected virtual void Start()
        {
            if (disableRenderOnStart)
            {
                MeshRenderer mMeshRenderer = GetComponent<MeshRenderer>();
                if (mMeshRenderer)
                {
                    mMeshRenderer.enabled = false;
                }
            }
            myTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (myTrackableBehaviour)
            {
                myTrackableBehaviour.RegisterTrackableEventHandler(this);
            }
        }

        public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
        }

        protected virtual void OnTrackingFound()
        {
            isTracking = true;
            if (TrackingFoundEvent != null)
            {
                TrackingFoundEvent(this);
            }
        }


        protected virtual void OnTrackingLost()
        {
            isTracking = false;
            if (TrackingLostEvent != null)
            {
                TrackingLostEvent(this);
            }
        }
    }
}