using UnityEngine;
using Vuforia;

namespace BToolkit
{
    public class CloudImageTarget : MonoBehaviour, ITrackableEventHandler
    {
        protected TrackableBehaviour mTrackableBehaviour;

        public static CloudImageTarget instance;
        public delegate void TrackingFoundLostAction(bool isFound);
        public static TrackingFoundLostAction OnTrackingFoundLost = null;
        public static bool isFound;

        void OnDestroy()
        {
            instance = null;
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.UnregisterTrackableEventHandler(this);
            }
        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }
        }

        public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
                OnTrackingFound();
            }
            else if (previousStatus == TrackableBehaviour.Status.TRACKED && newStatus == TrackableBehaviour.Status.NO_POSE)
            {
                Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
                OnTrackingLost();
            }
            else
            {
                OnTrackingLost();
            }
        }

        void OnTrackingFound()
        {
            Debug.Log("<color=yellow>OnTrackingFound()</color>");
            isFound = true;
            CloudRecognition.instance.TrackingFound();
            if (OnTrackingFoundLost != null)
            {
                OnTrackingFoundLost(true);
            }
        }

        void OnTrackingLost()
        {
            Debug.Log("<color=yellow>OnTrackingLost()</color>");
            isFound = false;
            CloudRecognition.instance.TrackingLost();
            if (OnTrackingFoundLost != null)
            {
                OnTrackingFoundLost(false);
            }
        }
    }
}