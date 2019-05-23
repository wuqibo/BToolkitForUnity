using System;
using System.Collections.Generic;
using UnityEngine;

namespace BToolkit
{
    public class QuaternionUpdate : MonoBehaviour
    {
        class Params
        {
            public float delay;
            public Quaternion toRotation;
            public float time;
            public bool worldSpace;
            public Tween.EaseType2 method;
            public Action OnEndEvent;
            public TweenEvent OnEndEventWithParam;
        }
        public Tween.EaseType2 method;
        public Action EndEvent;
        public TweenEvent EndEventWithParam;
        Quaternion toRotation;
        float maxDegreesDelta, totalTime, currTime;
        bool worldSpace;
        bool delayFinished;
        List<Params> paramsQueue = new List<Params>();

        void OnDestroy()
        {
            EndEvent = null;
            EndEventWithParam = null;
        }

        void Update()
        {
            for (int i = 0; i < paramsQueue.Count; i++)
            {
                Params mParams = paramsQueue[i];
                mParams.delay -= Time.deltaTime;
                if (mParams.delay <= 0f)
                {
                    RotateQuaternionGo(mParams);
                    paramsQueue.RemoveAt(i);
                }
            }
            if (!delayFinished)
            {
                return;
            }
            bool isFinish = false;
            Quaternion rot = worldSpace ? transform.rotation : transform.localRotation;
            if (method == Tween.EaseType2.Linear)
            {
                rot = Quaternion.RotateTowards(rot, toRotation, maxDegreesDelta);
                if (rot == toRotation)
                {
                    isFinish = true;
                }
            }
            else if (method == Tween.EaseType2.ExpoEaseOut)
            {
                currTime += Time.deltaTime * 0.1f;
                if (currTime > totalTime)
                {
                    currTime = totalTime;
                }
                rot = Quaternion.Slerp(rot, toRotation, currTime / totalTime);
                float includeAngle = Quaternion.Angle(rot, toRotation);
                if (includeAngle < 0.5f)
                {
                    rot = toRotation;
                    isFinish = true;
                }
            }
            if (isFinish)
            {
                if (paramsQueue.Count == 0)
                {
                    this.enabled = false;
                    this.delayFinished = false;
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
            if (worldSpace)
            {
                transform.rotation = rot;
            }
            else
            {
                transform.localRotation = rot;
            }
        }

        public void RotateQuaternion(float delay, Quaternion toRotation, float time, bool worldSpace, Tween.EaseType2 method, Action OnEndEvent, TweenEvent OnEndEventWithParam)
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
            newParams.toRotation = toRotation;
            newParams.time = time;
            newParams.worldSpace = worldSpace;
            newParams.method = method;
            newParams.OnEndEvent = OnEndEvent;
            newParams.OnEndEventWithParam = OnEndEventWithParam;
            if (delay > 0f)
            {
                paramsQueue.Add(newParams);
            }
            else
            {
                RotateQuaternionGo(newParams);
            }
            this.enabled = true;
        }
        void RotateQuaternionGo(Params _params)
        {
            this.delayFinished = true;
            this.worldSpace = _params.worldSpace;
            if (_params.time <= 0f)
            {
                if (worldSpace)
                {
                    transform.rotation = _params.toRotation;
                }
                else
                {
                    transform.localRotation = _params.toRotation;
                }
                if (paramsQueue.Count == 0)
                {
                    this.enabled = false;
                    this.delayFinished = false;
                }
                return;
            }
            this.totalTime = _params.time;
            this.currTime = 0f;
            this.maxDegreesDelta = GetMaxDegreesDelta(_params.time, _params.worldSpace, _params.toRotation);
            this.toRotation = _params.toRotation;
            this.method = _params.method;
            this.EndEvent = _params.OnEndEvent;
            this.EndEventWithParam = _params.OnEndEventWithParam;
        }

        float GetMaxDegreesDelta(float time, bool worldSpace, Quaternion targetRotation)
        {
            float includeAngle = Quaternion.Angle(worldSpace ? transform.rotation : transform.localRotation, targetRotation);
            return includeAngle / (time / Time.deltaTime);
        }

    }
}