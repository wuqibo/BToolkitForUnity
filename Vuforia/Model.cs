using UnityEngine;

namespace BToolkit
{
    public class Model : MonoBehaviour
    {
        public Transform arParent;
        public Vector3 arPos = Vector3.zero;
        public Vector3 arAngle = Vector3.zero;
        public Vector3 arScale = Vector3.one;

        public Vector3 screenPos = new Vector3(0, -0.2f, 2);
        public Vector3 screenAngle = new Vector3(-10, 0, 0);
        public Vector3 screenScale = Vector3.one;
    }
}