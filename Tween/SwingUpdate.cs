using System;
using System.Collections.Generic;
using UnityEngine;

namespace BToolkit
{
    public class SwingUpdate : MonoBehaviour
    {
        class Params
        {
            public float delay;
            public float offsetAngle;
            public float speed;
            public float time;
            public bool useDamping;
            public Action OnEndEvent;
            public TweenEvent OnEndEventWithParam;
        }
        public Action EndEvent;
        public TweenEvent EndEventWithParam;
        Vector3 defaultAngle, addAngle;
        float r, speed;
        float swingTime;
        bool useDamping;
        float offsetAngle;
        bool delayFinsh;
        List<Params> paramsQueue = new List<Params>();

        void OnDestroy()
        {
            EndEvent = null;
            EndEventWithParam = null;
        }

        void Awake()
        {
            defaultAngle = transform.localEulerAngles;
        }

        void Update()
        {
            for (int i = 0; i < paramsQueue.Count; i++)
            {
                Params mParams = paramsQueue[i];
                mParams.delay -= Time.deltaTime;
                if (mParams.delay <= 0f)
                {
                    SwingGo(mParams);
                    paramsQueue.RemoveAt(i);
                }
            }
            if (!delayFinsh)
            {
                return;
            }
            if (swingTime > 0f)
            {
                swingTime -= Time.deltaTime;
                r += speed * Time.deltaTime;
                addAngle.z = Mathf.Sin(r) * offsetAngle;
                if (useDamping)
                {
                    addAngle.z *= swingTime;
                }
                transform.localEulerAngles = defaultAngle + addAngle;
                if (swingTime <= 0f)
                {
                    transform.localEulerAngles = defaultAngle;
                    if (paramsQueue.Count == 0)
                    {
                        this.enabled = false;
                        this.delayFinsh = false;
                        if (EndEvent != null)
                        {
                            EndEvent();
                        }
                        if (EndEventWithParam != null)
                        {
                            EndEventWithParam(transform);
                        }
                    }
                }
            }
        }

        public void Swing(float delay, float offsetAngle, float speed, float time, bool useDamping, Action OnEndEvent, TweenEvent OnEndEventWithParam)
        {
            for (int i = 0; i < paramsQueue.Count; i++)
            {
                if (delay <= paramsQueue[i].delay)
                {
                    paramsQueue.RemoveAt(i);
                }
            }
            Params newParams = new Params();
            newParams.delay = delay;
            newParams.offsetAngle = offsetAngle;
            newParams.speed = speed;
            newParams.time = time;
            newParams.useDamping = useDamping;
            newParams.OnEndEvent = OnEndEvent;
            newParams.OnEndEventWithParam = OnEndEventWithParam;
            if (delay > 0f)
            {
                paramsQueue.Add(newParams);
            }
            else
            {
                SwingGo(newParams);
            }
            this.enabled = true;
        }
        void SwingGo(Params _params)
        {
            if (_params.time <= 0f)
            {
                return;
            }
            this.delayFinsh = true;
            this.r = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            this.offsetAngle = _params.offsetAngle / _params.time;
            this.speed = _params.speed;
            this.useDamping = _params.useDamping;
            swingTime = _params.time;
            this.enabled = true;
            this.EndEvent = _params.OnEndEvent;
            this.EndEventWithParam = _params.OnEndEventWithParam;
        }
       
    }
}