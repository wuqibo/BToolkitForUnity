using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class UIDropLoop : MonoBehaviour
    {
        public Vector2 topMaxPos = new Vector2(100, 110);
        public Vector2 topMinPos = new Vector2(-100, 100);
        public float floorY = 0;
        public float g = 0.5f;
        public float minDelay = 0;
        public float maxDelay = 1;
        public float maxRotateSpeed = 10;
        float timer, v, rv;
        RectTransform trans;
        Image img;

        void Start()
        {
            trans = transform as RectTransform;
            img = GetComponent<Image>();
            if (minDelay < 0)
            {
                minDelay = 0;
            }
            if (maxDelay < 0)
            {
                maxDelay = 0;
            }
            Drop();
        }

        void Drop()
        {
            img.enabled = false;
            trans.anchoredPosition = new Vector2(Random.Range(topMinPos.x, topMaxPos.x), Random.Range(topMinPos.y, topMaxPos.y));
            timer = Random.Range(minDelay, maxDelay);
            rv = Random.Range(-maxRotateSpeed, maxRotateSpeed) * 100;
        }

        void Update()
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    img.enabled = true;
                    v = 0;
                }
            }
            else
            {
                trans.localEulerAngles += new Vector3(0, 0, rv * Time.deltaTime);

                v += g;
                Vector2 pos = trans.anchoredPosition;
                pos.y -= v;
                trans.anchoredPosition = pos;
                if (pos.y <= floorY)
                {
                    Drop();
                }
            }
        }
    }
}