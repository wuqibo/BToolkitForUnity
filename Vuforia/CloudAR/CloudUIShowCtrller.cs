using UnityEngine;

namespace BToolkit
{
    public class CloudUIShowCtrller : MonoBehaviour
    {
        public BButton btnClose;
        CloudOffCardCtrl showTarget;

        public static void Show(CloudOffCardCtrl showTarget)
        {
            CloudUIShowCtrller ctrller = Instantiate(Resources.Load<CloudUIShowCtrller>("CloudUIShowCtrller"));
            ctrller.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            ctrller.showTarget = showTarget;
        }

        void Awake()
        {
            btnClose.onTrigger.AddListener(()=> {
                Destroy(gameObject);
                if (showTarget)
                {
                    showTarget.CloseFromUI();
                }
            });
        }

    }
}