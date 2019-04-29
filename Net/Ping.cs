using UnityEngine;
using System.Collections;
using System;

namespace BToolkit
{
    public class Ping : SingletonMonoBehaviour<Ping>
    {
        public bool isIdle { private set; get; }

        protected override void Awake()
        {
            base.Awake();
            isIdle = true;
        }

        public void StartPing(string ip, Action<int> Callback)
        {
            isIdle = false;
            StartCoroutine(DoStartPing(ip, Callback));
        }

        IEnumerator DoStartPing(string ip, Action<int> Callback)
        {
            UnityEngine.Ping ping = new UnityEngine.Ping(ip);
            while (!ping.isDone)
            {
                yield return null;
            }
            if (Callback != null)
            {
                Callback(ping.time);
            }
            isIdle = true;
        }
    }
}