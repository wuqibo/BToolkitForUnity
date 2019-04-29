using System;
using System.Collections.Generic;
using UnityEngine;

namespace BToolkit
{
    public class ScaleUpdate : MonoBehaviour
    {
        class Params
        {
            public float delay;
            public Vector3 toScale;
            public float time;
            public Tween.EaseType method;
            public Action OnEndEvent;
            public TweenEvent OnEndEventWithParam;
        }
        public Action EndEvent;
        public TweenEvent EndEventWithParam;
        Tween bTween;
        Vector3 toScale;
        OneAxis xAxis = new OneAxis();
        OneAxis yAxis = new OneAxis();
        OneAxis zAxis = new OneAxis();
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
                    ScaleGo(mParams);
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
            Vector3 vector = transform.localScale;
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
            transform.localScale = vector;
            if (t < d)
            {
                t += Time.deltaTime;
            }
            else
            {
                transform.localScale = toScale;
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

        public void Scale(float delay, Vector3 toScale, float time, Tween.EaseType method, Action OnEndEvent, TweenEvent OnEndEventWithParam)
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
            newParams.toScale = toScale;
            newParams.time = time;
            newParams.method = method;
            newParams.OnEndEvent = OnEndEvent;
            newParams.OnEndEventWithParam = OnEndEventWithParam;
            if (delay > 0f)
            {
                paramsQueue.Add(newParams);
            }
            else
            {
                ScaleGo(newParams);
            }
            this.enabled = true;
        }
        void ScaleGo(Params _params)
        {
            this.delayFinsh = true;
            if (_params.time <= 0f)
            {
                transform.localScale = _params.toScale;
                if (paramsQueue.Count == 0)
                {
                    this.enabled = false;
                    this.delayFinsh = false;
                }
                return;
            }
            t = 0f;
            d = _params.time;
            xAxis.canGo = (_params.toScale.x != transform.localScale.x);
            yAxis.canGo = (_params.toScale.y != transform.localScale.y);
            zAxis.canGo = (_params.toScale.z != transform.localScale.z);
            if (xAxis.canGo)
            {
                xAxis.b = transform.localScale.x;
                xAxis.c = _params.toScale.x - transform.localScale.x;
            }
            if (yAxis.canGo)
            {
                yAxis.b = transform.localScale.y;
                yAxis.c = _params.toScale.y - transform.localScale.y;
            }
            if (zAxis.canGo)
            {
                zAxis.b = transform.localScale.z;
                zAxis.c = _params.toScale.z - transform.localScale.z;
            }
            this.toScale = _params.toScale;
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