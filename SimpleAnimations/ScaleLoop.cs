using UnityEngine;

namespace BToolkit
{
    public class ScaleLoop : MonoBehaviour
    {
        public bool randomStart;
        public Vector3 minScale = new Vector3(0.9f, 0.9f, 0.9f);
        public Vector3 maxScale = new Vector3(1.1f, 1.1f, 1.1f);
        public float speed = 5;
        float scaleX, scaleY, scaleZ, r;

        void Awake()
        {
            if (randomStart)
            {
                r = Random.Range(0f, Mathf.PI * 2f);
            }
        }

        void Update()
        {
            r += speed * Time.deltaTime;
            float sin = Mathf.Sin(r) * 0.5f + 0.5f;
            scaleX = minScale.x + sin * (maxScale.x - minScale.x);
            scaleY = minScale.y + sin * (maxScale.y - minScale.y);
            scaleZ = minScale.z + sin * (maxScale.z - minScale.z);
            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }
    }
}