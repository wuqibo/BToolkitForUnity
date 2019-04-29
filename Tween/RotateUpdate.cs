using System;
using System.Collections.Generic;
using UnityEngine;

namespace BToolkit
{
    public class RotateUpdate : MonoBehaviour
    {
        class Params
        {
            public float delay;
            public Vector3 toAngle;
            public float time;
            public bool worldSpace;
            public Tween.EaseType method;
            public Action OnEndEvent;
            public TweenEvent OnEndEventWithParam;
            public bool xEnable;
            public bool yEnable;
            public bool zEnable;
        }
        public Action EndEvent;
        public TweenEvent EndEventWithParam;
        Tween bTween;
        Vector3 toAngle;
        OneAxis xAxis = new OneAxis();
        OneAxis yAxis = new OneAxis();
        OneAxis zAxis = new OneAxis();
        bool worldSpace;
        float t, d;
        class OneAxis
        {
            public float b, c;
            public bool canGo;
        }
        bool delayFinsh;
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
                    RotateGo(mParams);
                    paramsQueue.RemoveAt(i);
                }
            }
            if (!delayFinsh)
            {
                return;
            }
            if (bTween == null)
            {
                return;
            }
            Vector3 vector = worldSpace ? transform.eulerAngles : transform.localEulerAngles;
            if (xAxis.canGo)
            {
                vector.x = (float)bTween.Ease(t, xAxis.b, xAxis.c, d);
            }
            if (yAxis.canGo)
            {
                vector.y = (float)bTween.Ease(t, yAxis.b, yAxis.c, d);
            }
            if (zAxis.canGo)
            {
                vector.z = (float)bTween.Ease(t, zAxis.b, zAxis.c, d);
            }
            if (worldSpace)
            {
                transform.eulerAngles = vector;
            }
            else
            {
                transform.localEulerAngles = vector;
            }
            if (t < d)
            {
                t += Time.deltaTime;
            }
            else
            {
                if (worldSpace)
                {
                    if (!xAxis.canGo)
                    {
                        toAngle.x = transform.eulerAngles.x;
                    }
                    if (!yAxis.canGo)
                    {
                        toAngle.y = transform.eulerAngles.y;
                    }
                    if (!zAxis.canGo)
                    {
                        toAngle.z = transform.eulerAngles.z;
                    }
                    transform.eulerAngles = toAngle;
                }
                else
                {
                    if (!xAxis.canGo)
                    {
                        toAngle.x = transform.localEulerAngles.x;
                    }
                    if (!yAxis.canGo)
                    {
                        toAngle.y = transform.localEulerAngles.y;
                    }
                    if (!zAxis.canGo)
                    {
                        toAngle.z = transform.localEulerAngles.z;
                    }
                    transform.localEulerAngles = toAngle;
                }
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


        public void Rotate(float delay, Vector3 toAngle, float time, bool worldSpace, Tween.EaseType method, Action OnEndEvent, TweenEvent OnEndEventWithParam, bool xEnable, bool yEnable, bool zEnable)
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
            newParams.toAngle = toAngle;
            newParams.time = time;
            newParams.worldSpace = worldSpace;
            newParams.method = method;
            newParams.OnEndEvent = OnEndEvent;
            newParams.OnEndEventWithParam = OnEndEventWithParam;
            newParams.xEnable = xEnable;
            newParams.yEnable = yEnable;
            newParams.zEnable = zEnable;
            if (delay > 0f)
            {
                paramsQueue.Add(newParams);
            }
            else
            {
                RotateGo(newParams);
            }
            this.enabled = true;
        }
        void RotateGo(Params _params)
        {
            this.delayFinsh = true;
            this.worldSpace = _params.worldSpace;
            if (_params.time <= 0f)
            {
                if (_params.worldSpace)
                {
                    transform.eulerAngles = _params.toAngle;
                }
                else
                {
                    if (!_params.xEnable)
                    {
                        _params.toAngle.x = transform.localEulerAngles.x;
                    }
                    if (!_params.yEnable)
                    {
                        _params.toAngle.y = transform.localEulerAngles.y;
                    }
                    if (!_params.zEnable)
                    {
                        _params.toAngle.z = transform.localEulerAngles.z;
                    }
                    transform.localEulerAngles = _params.toAngle;
                }
                if (paramsQueue.Count == 0)
                {
                    this.enabled = false;
                    this.delayFinsh = false;
                }
                return;
            }
            t = 0f;
            d = _params.time;
            Vector3 transPos = _params.worldSpace ? transform.eulerAngles : transform.localEulerAngles;
            xAxis.canGo = _params.xEnable ? (_params.toAngle.x != transPos.x) : _params.xEnable;
            yAxis.canGo = _params.yEnable ? (_params.toAngle.y != transPos.y) : _params.yEnable;
            zAxis.canGo = _params.zEnable ? (_params.toAngle.z != transPos.z) : _params.zEnable;
            if (xAxis.canGo)
            {
                xAxis.b = transPos.x;
                xAxis.c = _params.toAngle.x - transPos.x;
            }
            if (yAxis.canGo)
            {
                yAxis.b = transPos.y;
                yAxis.c = _params.toAngle.y - transPos.y;
            }
            if (zAxis.canGo)
            {
                zAxis.b = transPos.z;
                zAxis.c = _params.toAngle.z - transPos.z;
            }
            this.toAngle = _params.toAngle;
            switch (_params.method)
            {
                case Tween.EaseType.Linear:
                    bTween = new LinearEase();
                    break;
                case Tween.EaseType.ExpoEaseIn:
                    bTween = new ExpoEaseIn();
                    break;
                case Tween.EaseType.ExpoEaseOut:
                    bTween = new ExpoEaseOut();
                    break;
                case Tween.EaseType.ExpoEaseInOut:
                    bTween = new ExpoEaseInOut();
                    break;
                case Tween.EaseType.SineEaseIn:
                    bTween = new SineEaseIn();
                    break;
                case Tween.EaseType.SineEaseOut:
                    bTween = new SineEaseOut();
                    break;
                case Tween.EaseType.SineEaseInOut:
                    bTween = new SineEaseInOut();
                    break;
                case Tween.EaseType.ElasticEaseIn:
                    bTween = new ElasticEaseIn();
                    break;
                case Tween.EaseType.ElasticEaseOut:
                    bTween = new ElasticEaseOut();
                    break;
                case Tween.EaseType.ElasticEaseInOut:
                    bTween = new ElasticEaseInOut();
                    break;
                case Tween.EaseType.BackEaseIn:
                    bTween = new BackEaseIn();
                    break;
                case Tween.EaseType.BackEaseOut:
                    bTween = new BackEaseOut();
                    break;
                case Tween.EaseType.BackEaseInOut:
                    bTween = new BackEaseInOut();
                    break;
                case Tween.EaseType.BounceEaseIn:
                    bTween = new BounceEaseIn();
                    break;
                case Tween.EaseType.BounceEaseOut:
                    bTween = new BounceEaseOut();
                    break;
                case Tween.EaseType.BounceEaseInOut:
                    bTween = new BounceEaseInOut();
                    break;
                default:
                    bTween = new SineEaseOut();
                    break;
            }
            this.EndEvent = _params.OnEndEvent;
            this.EndEventWithParam = _params.OnEndEventWithParam;
        }

    }
}