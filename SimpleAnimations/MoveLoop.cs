using UnityEngine;

namespace BToolkit
{
    public class MoveLoop : MonoBehaviour
    {
        public Vector3 fromPos = new Vector3(0, 0, 0);
        public float goDelay = 0f;
        public float goDuration = 1f;
        public Tween.EaseType goEaseType = Tween.EaseType.Linear;
        [Space]
        public Vector3 toPos = new Vector3(0, 100, 0);
        public float backoDelay = 0f;
        public float backDuration = 1f;
        public Tween.EaseType backEaseType = Tween.EaseType.Linear;

        void Start()
        {
            Tween.Move(transform, fromPos, false);
            Go();
        }

        void Go()
        {
            Tween.Move(goDelay, transform, toPos, goDuration, false, goEaseType, Back);
        }

        void Back()
        {
            Tween.Move(backoDelay, transform, fromPos, backDuration, false, backEaseType, Go);
        }

    }
}