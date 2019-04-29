using UnityEngine;

namespace BToolkit
{
    public class SwingLoop : MonoBehaviour
    {

        public float range = 3, speed = 1;
        float r;
        float defaultAngle;

        void Awake()
        {
            defaultAngle = transform.localEulerAngles.z;
            r = Random.Range(0f, Mathf.PI * 2f);
        }

        void Update()
        {
            Vector3 angle = transform.localEulerAngles;
            angle.z = defaultAngle + Mathf.Sin(r) * range;
            r += speed * Time.deltaTime;
            transform.localEulerAngles = angle;
        }
    }
}