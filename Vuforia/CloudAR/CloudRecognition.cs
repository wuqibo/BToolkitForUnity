using UnityEngine;
using Vuforia;

namespace BToolkit
{
    public class CloudRecognition:MonoBehaviour, ICloudRecoEventHandler
    {

        public static CloudRecognition instance;
        CloudRecoBehaviour m_CloudRecoBehaviour;
        TargetFinder m_TargetFinder;
        bool isTargetFinderScanning;
        public delegate void ScanedNewTargetAction(TargetFinder.TargetSearchResult targetSearchResult);
        public static ScanedNewTargetAction OnScanedNewTarget = null;

        void OnDestroy()
        {
            instance = null;
        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            m_CloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
            if(m_CloudRecoBehaviour)
            {
                m_CloudRecoBehaviour.RegisterEventHandler(this);
            }
        }

        public void OnInitialized(TargetFinder targetFinder)
        {
            Debug.Log("Cloud Reco 初始化成功");
            this.m_TargetFinder = targetFinder;
        }

        public void OnInitError(TargetFinder.InitState initError) { }
        public void OnUpdateError(TargetFinder.UpdateState updateError) { }

        public void OnStateChanged(bool scanning)
        {
            isTargetFinderScanning = scanning;
            if(scanning)
            {
                m_TargetFinder.ClearTrackables(false);
            }
        }

        public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
        {
            if(targetSearchResult.MetaData == null)
            {
                Debug.LogError("Target metadata not available.");
            }
            else
            {
                Debug.Log("<color=gray>MetaData: " + targetSearchResult.MetaData + "</color>");
                Debug.Log("<color=gray>TargetName: " + targetSearchResult.TargetName + "</color>");
                Debug.Log("<color=gray>Pointer: " + targetSearchResult.TargetSearchResultPtr + "</color>");
                Debug.Log("<color=gray>TargetSize: " + targetSearchResult.TargetSize + "</color>");
                Debug.Log("<color=gray>TrackingRating: " + targetSearchResult.TrackingRating + "</color>");
                Debug.Log("<color=gray>UniqueTargetId: " + targetSearchResult.UniqueTargetId + "</color>");
            }
            if(OnScanedNewTarget != null)
            {
                OnScanedNewTarget(targetSearchResult);
            }
            m_TargetFinder.ClearTrackables(false);
            m_TargetFinder.EnableTracking(targetSearchResult,CloudImageTarget.instance.gameObject);
        }

        public void TrackingLost()
        {
            TrackableWasFound(false);
        }

        public void TrackingFound()
        {
            TrackableWasFound(true);
        }

        void TrackableWasFound(bool isTrackingFound)
        {
            if(m_TargetFinder != null)
            {
                if(isTrackingFound)
                {
                    isTargetFinderScanning = !m_TargetFinder.Stop();
                }
                else
                {
                    m_TargetFinder.ClearTrackables(false);
                    isTargetFinderScanning = m_TargetFinder.StartRecognition();
                }
            }
        }

        public void ResetRecognition()
        {
            CloudImageTarget.instance.OnTrackableStateChanged(TrackableBehaviour.Status.EXTENDED_TRACKED,TrackableBehaviour.Status.NO_POSE);
            TrackingLost();
        }
    }
}