using System;
using UnityEngine;
using Vuforia;

namespace BToolkit
{
    public class CloudImageTarget : MonoBehaviour, ITrackableEventHandler
    {
        public CloudVideoPlayerManager videoPlayerManager;
        public GameObject loading;

        CloudRecognition cloudRecognition;
        TrackableBehaviour mTrackableBehaviour;
        bool isFirstLost = true;

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
                loading.transform.Rotate(0, 0, -300 * Time.deltaTime);
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
            isFirstLost = false;
            loading.SetActive(true);
        }

        void OnTrackingLost()
        {
            if (isFirstLost)
            {
                videoPlayerManager.Show(false);
            }
            else
            {
                StorageManager.Instance.IsARHideWhenOffCard = false;
                if (StorageManager.Instance.IsARHideWhenOffCard)
                {
                    videoPlayerManager.Show(false);
                    cloudRecognition.RestartScan();
                }
                else
                {
                    VuforiaHelper.StopTracker();
                    float videoW = videoPlayerManager.CurrPlayer.videoW;
                    float videoH = videoPlayerManager.CurrPlayer.videoH;
                    bool isAVProPlayer = videoPlayerManager.CurrPlayer.isAVProPlayer;
                    videoPlayerManager.GetComponent<CloudOffCardCtrl>().ToFullScreen(videoW, videoH, isAVProPlayer);
                }
            }
        }

        /// <summary>
        /// 云识别后调用
        /// </summary>
        public void PlayVideo(MoJingTargetInfo info)
        {
            videoPlayerManager.Play(this, info);
        }
    }
}