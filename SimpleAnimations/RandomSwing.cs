using UnityEngine;

namespace BToolkit
{
    public class RandomSwing : MonoBehaviour
    {
        public float offsetAngle = 10;
        public float speed = 10;
        public float duration = 1;
        public float ranMin = 3, ranMax = 8;
        float timer, swingTime, r;

        void Awake()
        {
            this.enabled = false;
            timer = Random.Range(ranMin, ranMax);
            Invoke("Go", timer);

        }

        void Go()
        {
            r = 0f;
            swingTime = duration;
            this.enabled = true;
            timer = Random.Range(ranMin, ranMax);
            Invoke("Go", timer);
        }

        void Update()
        {
            if (swingTime > 0f)
            {
                swingTime -= Time.deltaTime;
                r += speed * Time.deltaTime;
                Vector3 angle = transform.localEulerAngles;
                angle.z = Mathf.Sin(r) * offsetAngle * swingTime;
                transform.localEulerAngles = angle;
                if (swingTime <= 0f)
                {
                    transform.localEulerAngles = Vector3.zero;
                    this.enabled = false;
                }
            }
        }

    }
}