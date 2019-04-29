using System;
using UnityEngine;

namespace BToolkit
{
    public class CloudLoading : MonoBehaviour
    {
        public GameObject rotateIcon;
        public TextMesh text;
        static CloudLoading instance;

        void OnDestroy()
        {
            instance = null;
            CloudImageTarget.OnTrackingFoundLost -= OnTrackingFoundLost;
        }

        void Awake()
        {
            instance = this;
            CloudImageTarget.OnTrackingFoundLost += OnTrackingFoundLost;
        }

        private void OnTrackingFoundLost(bool isFound)
        {
            if (isFound)
            {
                transform.SetParent(CloudImageTarget.instance.transform, false);
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                transform.SetParent(CloudContentManager.instance.CameraTrans, false);
                transform.localPosition = new Vector3(0, 0, CloudVideoPlayer.DistanceFromCamera);
                transform.localEulerAngles = new Vector3(-90, 0, 0);
            }
        }

        public static void Show(bool b, string fileSize = "0M")
        {
            if (b)
            {
                if (!instance)
                {
                    instance = Instantiate(CloudContentManager.instance.loadingPrefab);
                }
                instance.gameObject.SetActive(true);
                instance.text.text = "SIZE:" + fileSize;
                instance.OnTrackingFoundLost(CloudImageTarget.isFound);
            }
            else
            {
                if (instance)
                {
                    Destroy(instance.gameObject);
                }
            }
        }

        void Update()
        {
            rotateIcon.transform.Rotate(0, 0, -300 * Time.deltaTime);
        }
    }
}