using System;
using UnityEngine;
using Vuforia;

namespace BToolkit
{
    public class CloudImageTarget : MonoBehaviour, ITrackableEventHandler
    {
        public CloudShowTarget videoPlayerManager;
        public CloudShowTarget modelViewer;
        public GameObject loading;

        CloudRecognition cloudRecognition;
        TrackableBehaviour mTrackableBehaviour;
        bool hadFoundOnce;

        void Awake()
        {
            loading.SetActive(false);
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
            loading.SetActive(true);
        }

        void OnTrackingLost()
        {
            if (!hadFoundOnce)
            {
                videoPlayerManager.Show(false);
                modelViewer.Show(false);
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
        public void PlayTarget(MoJingTargetInfo info)
        {
            if ("video".Equals(info.showType))
            {
                modelViewer.Show(false);
                videoPlayerManager.PlayTarget(this, info);
            }
            else
            {
                videoPlayerManager.Show(false);
                modelViewer.PlayTarget(this, info);
            }
        }
        
    }
}