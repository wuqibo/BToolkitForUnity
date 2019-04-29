using UnityEngine;

namespace BToolkit
{
    public class RotateQuaternionLoop : MonoBehaviour
    {
        public Vector3 fromAngle = new Vector3(0, 0, 0);
        public float goDelay = 0f;
        public float goDuration = 1f;
        public Tween.EaseType2 goEaseType = Tween.EaseType2.Linear;
        [Space]
        public Vector3 toAngle = new Vector3(0, 0, 45);
        public float backoDelay = 0f;
        public float backDuration = 1f;
        public Tween.EaseType2 backEaseType = Tween.EaseType2.Linear;

        void Start()
        {
            Tween.RotateQuaternion(transform, Quaternion.Euler(fromAngle), false);
            Go();
        }

        void Go()
        {
            Tween.RotateQuaternion(goDelay, transform, Quaternion.Euler(toAngle), goDuration, false, goEaseType, Back);
        }

        void Back()
        {
            Tween.RotateQuaternion(backoDelay, transform, Quaternion.Euler(fromAngle), backDuration, false, backEaseType, Go);
        }

    }
}