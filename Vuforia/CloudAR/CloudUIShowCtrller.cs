using UnityEngine;

namespace BToolkit
{
    public class CloudUIShowCtrller : MonoBehaviour
    {
        public BButton btnClose;
        CloudOffCardCtrl showTarget;
        public static CloudUIShowCtrller instance;

        public static void Show(CloudOffCardCtrl showTarget)
        {
            if (!instance)
            {
                instance = Instantiate(Resources.Load<CloudUIShowCtrller>("CloudUIShowCtrller"));
            }
            instance.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            instance.showTarget = showTarget;
        }

        public static void Destroy()
        {
            if (instance)
            {
                Destroy(instance.gameObject); ;
            }
        }

        void Awake()
        {
            btnClose.onTrigger.AddListener(()=> {
                Destroy();
                if (showTarget)
                {
                    showTarget.CloseFromUI();
                }
            });
        }

    }
}