using UnityEngine;

namespace BToolkit
{
    public class Float : MonoBehaviour
    {
        public float radius = 100;
        public float time = 5;
        Vector2 centerPos;

        void Start()
        {
            centerPos = (transform as RectTransform).anchoredPosition;
            MoveToRandomPos();
        }

        void MoveToRandomPos()
        {
            Vector2 targetPos = centerPos + new Vector2(Random.Range(-radius, radius), Random.Range(-radius, radius));
            float dis = Vector2.Distance((transform as RectTransform).anchoredPosition, targetPos);
            float t = time * dis / 100f;
            Tween.Move(0, transform, targetPos, t, false, Tween.EaseType.SineEaseInOut, MoveToRandomPos);
        }
    }
}