using System;
using UnityEngine;

namespace BToolkit
{
    public class TimeCheck : MonoBehaviour
    {
        public enum Time { Before, Pass }
        public Time destroyTime;
        private static DateTime targetDateTime = new DateTime(2019, 5, 25);

        public static bool HasPassedTime
        {
            get
            {
                if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    DateTime timeNow = DateTime.Now;
                    DateTime timeTarget = targetDateTime;
                    return DateTime.Compare(timeNow, timeTarget) >= 0;
                }
                return true;
            }
        }

        void Awake()
        {
            if (destroyTime == Time.Before)
            {
                if (!HasPassedTime)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                if (HasPassedTime)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}