using UnityEngine;
using Vuforia;

namespace BToolkit
{
    public class CloudRecognition : MonoBehaviour, IObjectRecoEventHandler
    {
        CloudRecoBehaviour m_CloudRecoBehaviour;
        ObjectTracker m_ObjectTracker;
        TargetFinder m_TargetFinder;
        CloudImageTarget cloudImageTarget;

        void Start()
        {
            m_CloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
            if (m_CloudRecoBehaviour)
            {
                m_CloudRecoBehaviour.RegisterEventHandler(this);
            }
            RestartScan();
        }

        #region Vuforia调用
        public void OnInitialized(TargetFinder targetFinder)
        {
            Debug.Log("Cloud Reco 初始化成功");
            m_ObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            m_TargetFinder = targetFinder;
        }
        public void OnInitError(TargetFinder.InitState initError) { }
        public void OnUpdateError(TargetFinder.UpdateState updateError) { }
        public void OnStateChanged(bool scanning)
        {
            if (scanning)
            {
                m_TargetFinder.ClearTrackables(false);
            }
        }
        public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
        {
            TargetFinder.CloudRecoSearchResult cloudRecoResult = (TargetFinder.CloudRecoSearchResult)targetSearchResult;
            if (cloudRecoResult.MetaData == null)
            {
                Debug.Log("Target metadata not available.");
            }
            else
            {
                Debug.Log("MetaData: " + cloudRecoResult.MetaData);
                Debug.Log("TargetName: " + cloudRecoResult.TargetName);
                Debug.Log("Pointer: " + cloudRecoResult.TargetSearchResultPtr);
                Debug.Log("TargetSize: " + cloudRecoResult.TargetSize);
                Debug.Log("TrackingRating: " + cloudRecoResult.TrackingRating);
                Debug.Log("UniqueTargetId: " + cloudRecoResult.UniqueTargetId);
            }

            LoadAndCreateContent(cloudRecoResult.UniqueTargetId);

            m_CloudRecoBehaviour.CloudRecoEnabled = false;
            m_TargetFinder.ClearTrackables(false);
            m_TargetFinder.EnableTracking(targetSearchResult, cloudImageTarget.gameObject);
        }
        #endregion

        #region 内部调用
        void LoadAndCreateContent(string targetId)
        {
            CloudServerAPI.Instance.LoadTargetInfo(targetId, (CloudTargetInfo info) =>
            {
                cloudImageTarget.PlayTarget(info);
            });
            StorageManager.Instance.AddCloudARScanedRecord(targetId);
        }
        #endregion

        #region 外部调用
        /// <summary>
        /// 初始化时设置对象关联
        /// </summary>
        public void SetCloudImageTarget(CloudImageTarget cloudImageTarget)
        {
            this.cloudImageTarget = cloudImageTarget;
        }

        /// <summary>
        /// 重新开始扫描
        /// </summary>
        public void RestartScan()
        {
            if (m_TargetFinder != null)
            {
                m_TargetFinder.ClearTrackables(false);
            }
            if (m_CloudRecoBehaviour)
            {
                m_CloudRecoBehaviour.CloudRecoEnabled = true;
            }
        }
        #endregion
    }
}