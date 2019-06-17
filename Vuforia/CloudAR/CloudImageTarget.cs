using System;
using UnityEngine;
using Vuforia;

namespace BToolkit
{
    public class CloudImageTarget : MonoBehaviour, ITrackableEventHandler
    {
        public static CloudImageTarget instance;
        public CloudVideoPlayerManager videoPlayerManager;
        public CloudModelViewer modelViewer;
        public GameObject loading;

        CloudRecognition cloudRecognition;
        TrackableBehaviour mTrackableBehaviour;
        bool hadFoundOnce;

        void OnDestroy()
        {
            VuforiaHelper.LoadingActiveAction -= SetLoadingActive;
        }

        void Awake()
        {
            instance = this;
            VuforiaHelper.LoadingActiveAction += SetLoadingActive;
            VuforiaHelper.LoadingActiveAction(false);
        }

        void Start()
        {
            cloudRecognition = FindObjectOfType<CloudRecognition>();
            if (!cloudRecognition)
            {
                Debug.LogError("未将CloudRecognition对象拖入场景");
            }
            cloudRecognition.SetCloudImageTarget(this);
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }
        }


        void Update()
        {
            if (loading.activeInHierarchy)
            {
                loading.transform.Rotate(0, 0, 300 * Time.deltaTime);
            }
        }

        void SetLoadingActive(bool b)
        {
            VuforiaHelper.idLoadingActive = b;
            loading.SetActive(b);
        }

        public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                Debug.Log("<color=yellow>Trackable " + mTrackableBehaviour.TrackableName + " found</color>");
                OnTrackingFound();
            }
            else if (previousStatus == TrackableBehaviour.Status.TRACKED && newStatus == TrackableBehaviour.Status.NO_POSE)
            {
                Debug.Log("<color=yellow>Trackable " + mTrackableBehaviour.TrackableName + " lost</color>");
                OnTrackingLost();
            }
            else
            {
                OnTrackingLost();
            }
        }

        void OnTrackingFound()
        {
            hadFoundOnce = true;
            videoPlayerManager.OnTrackingFound();
            modelViewer.OnTrackingFound();
            VuforiaHelper.LoadingActiveAction(true);
        }

        void OnTrackingLost()
        {
            if (!hadFoundOnce)
            {
                videoPlayerManager.gameObject.SetActive(false);
                modelViewer.gameObject.SetActive(false);
            }
            else
            {
                videoPlayerManager.OnTrackingLost();
                modelViewer.OnTrackingLost();
                cloudRecognition.RestartScan();
            }
        }

        /// <summary>
        /// 云识别后调用
        /// </summary>
        public void PlayTarget(CloudTargetInfo info)
        {
            if ("video".Equals(info.showType))
            {
                modelViewer.gameObject.SetActive(false);
                videoPlayerManager.gameObject.SetActive(true);
                videoPlayerManager.PlayTarget(info);
            }
            else
            {
                videoPlayerManager.gameObject.SetActive(false);
                modelViewer.gameObject.SetActive(true);
                modelViewer.PlayTarget(info);
            }
        }
        
    }
}