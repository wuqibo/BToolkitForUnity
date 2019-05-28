using UnityEngine;

namespace BToolkit
{
    public class CloudShowTarget : MonoBehaviour
    {
        protected CloudImageTarget cloudImageTarget;

        public void Show(bool b)
        {
            gameObject.SetActive(b);
        }

        public virtual void PlayTarget(CloudImageTarget cloudImageTarget, MoJingTargetInfo info)
        {
            Show(true);
            this.cloudImageTarget = cloudImageTarget;
        }

        public virtual void OnTrackingLost() { }
    }
}