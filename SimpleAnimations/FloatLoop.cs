using UnityEngine;

namespace BToolkit
{
    public class FloatLoop : MonoBehaviour
    {
        public bool randomStart;
        public float minY = 0f;
        public float maxY = 1f;
        public float speed = 5;
        float y, r;
        RectTransform rectTrans;

        void Awake()
        {
            if (GetComponent<RectTransform>())
            {
                rectTrans = transform as RectTransform;
            }
            if (randomStart)
            {
                r = Random.Range(0, Mathf.PI * 2f);
            }
        }

        void Update()
        {
            r += speed * Time.deltaTime;
            y = minY + (Mathf.Sin(r) * 0.5f + 0.5f) * (maxY - minY);
            SetPosition(y);
        }

        void SetPosition(float y)
        {
            if (rectTrans)
            {
                Vector2 pos = rectTrans.anchoredPosition;
                pos.y = y;
                rectTrans.anchoredPosition = pos;
            }
            else
            {
                Vector2 pos = transform.localPosition;
                pos.y = y;
                transform.localPosition = pos;
            }
        }
    }
}