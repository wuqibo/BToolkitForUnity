using UnityEngine;

namespace BToolkit
{
    public class RotateLoop : MonoBehaviour
    {
        public bool randomStart;
        public Vector3 fromAngle = new Vector3(0, 0, 0);
        public float goDelay = 0f;
        public float goDuration = 1f;
        public Tween.EaseType goEaseType = Tween.EaseType.SineEaseInOut;
        [Space]
        public Vector3 toAngle = new Vector3(0, 0, 45);
        public float backoDelay = 0f;
        public float backDuration = 1f;
        public Tween.EaseType backEaseType = Tween.EaseType.SineEaseInOut;

        void Start()
        {
            Tween.Rotate(transform, fromAngle, false);
            if (randomStart)
            {
                Invoke("Go", Random.Range(0f, goDuration));
            }
            else
            {
                Go();
            }
        }

        void Go()
        {
            Tween.Rotate(goDelay, transform, toAngle, goDuration, false, goEaseType, Back);
        }

        void Back()
        {
            Tween.Rotate(backoDelay, transform, fromAngle, backDuration, false, backEaseType, Go);
        }

    }
}