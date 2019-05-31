using UnityEngine;

namespace BToolkit
{
    public class CloudUIShowCtrller : MonoBehaviour
    {
        public static CloudUIShowCtrller instance;
        public BButton btnClose;
        CloudOffCardCtrl showTarget;
        GameObject panelDefault;

        public static void Show(CloudImageTarget cloudImageTarget, CloudOffCardCtrl showTarget)
        {
            if (!instance)
            {
                instance = Instantiate(cloudImageTarget.cloudUIShowCtrllerPrefab);
            }
            instance.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            instance.showTarget = showTarget;
        }

        public static void Destroy()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
            }
        }

        void OnDestroy()
        {
            if (showTarget)
            {
                showTarget.OnUICtrllerDestroy();
            }
            if (panelDefault)
            {
                panelDefault.SetActive(true);
            }
        }

        void Awake()
        {
            btnClose.onTrigger.AddListener(()=> {
                Destroy(gameObject);
            });
            panelDefault = GameObject.Find("PanelDefault");
            if (panelDefault)
            {
                panelDefault.SetActive(false);
            }
        }

    }
}