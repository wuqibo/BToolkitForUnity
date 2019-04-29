using System;
using UnityEngine;

namespace BToolkit
{
    public class TimeCheck : MonoBehaviour
    {
        public enum Time
        {
            Before,
            Pass
        }
        public enum Action
        {
            Hide,
            Show,
            Destroy
        }

        public Time time;
        public Action action;
        static DateTime targetDateTime = new DateTime(2018, 5, 19);

        public static bool HasPassedTime
        {
            get
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
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
            if(time == Time.Before)
            {
                if (!HasPassedTime)
                {
                    ExecuteAction();
                }
            }
            else
            {
                if (HasPassedTime)
                {
                    ExecuteAction();
                }
            }
        }

        void ExecuteAction()
        {
            switch (action)
            {
                case Action.Show:
                    gameObject.SetActive(true);
                    break;
                case Action.Hide:
                    gameObject.SetActive(false);
                    break;
                case Action.Destroy:
                    Destroy(gameObject);
                    break;
            }
        }

    }
}