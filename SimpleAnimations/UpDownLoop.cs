using UnityEngine;
using System.Collections;

namespace BToolkit
{
    public class UpDownLoop : MonoBehaviour
    {

        public float heightMax = 1f;
        public float heightMin = 0f;
        public float speed = 1.5f;
        float radian;
        KB kb;

        void Awake()
        {
            kb = new KB(-1f, heightMin, 1f, heightMax);
        }

        void Update()
        {
            radian += speed * Time.deltaTime;
            Vector3 pos = new Vector3(0f, kb.k * Mathf.Sin(radian) + kb.b, 0f);
            transform.localPosition = pos;
        }

        public class KB
        {
            public float k, b;
            public KB(float x1, float y1, float x2, float y2)
            {
                k = (y2 - y1) / (x2 - x1);
                b = y1 - k * x1;
            }
        }
    }
}