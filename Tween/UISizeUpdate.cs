using System;
using System.Collections.Generic;
using UnityEngine;

namespace BToolkit
{
    public class UISizeUpdate : MonoBehaviour
    {
        class Params
        {
            public float delay;
            public Vector2 toSize;
            public float time;
            public Tween.EaseType method;
            public Action OnEndEvent;
            public TweenEvent OnEndEventWithParam;
        }
        public Action EndEvent;
        public TweenEvent EndEventWithParam;
        Tween Tween;
        Vector2 toSize;
        OneAxis xAxis = new OneAxis();
        OneAxis yAxis = new OneAxis();
        float t, d;
        class OneAxis
        {
            public float b, c;
            public bool canGo;
        }
        bool delayFinished;
        List<Params> paramsQueue = new List<Params>();
        RectTransform rt;
        RectTransform rectTransform { get { return rt ?? (rt = transform as RectTransform); } }

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
            if (!delayFinished)
            {
                return;
            }
            if (Tween == null)
            {
                return;
            }
            Vector2 vector = rectTransform.sizeDelta;
            if (xAxis.canGo)
            {
                vector.x = (float)Tween.Ease(t, xAxis.b, xAxis.c, d);
            }
            if (yAxis.canGo)
            {
                vector.y = (float)Tween.Ease(t, yAxis.b, yAxis.c, d);
            }
            rectTransform.sizeDelta = vector;
            if (t < d)
            {
                t += Time.deltaTime;
            }
            else
            {
                rectTransform.sizeDelta = toSize;
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
        }

        public void UISize(float delay, Vector2 toScale, float time, Tween.EaseType method, Action OnEndEvent, TweenEvent OnEndEventWithParam)
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
            newParams.toSize = toScale;
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
            this.delayFinished = true;
            if (_params.time <= 0f)
            {
                rectTransform.sizeDelta = _params.toSize;
                if (paramsQueue.Count == 0)
                {
                    this.enabled = false;
                    this.delayFinished = false;
                }
                return;
            }
            t = 0f;
            d = _params.time;
            xAxis.canGo = (_params.toSize.x != rectTransform.sizeDelta.x);
            yAxis.canGo = (_params.toSize.y != rectTransform.sizeDelta.y);
            if (xAxis.canGo)
            {
                xAxis.b = rectTransform.sizeDelta.x;
                xAxis.c = _params.toSize.x - rectTransform.sizeDelta.x;
            }
            if (yAxis.canGo)
            {
                yAxis.b = rectTransform.sizeDelta.y;
                yAxis.c = _params.toSize.y - rectTransform.sizeDelta.y;
            }
            this.toSize = _params.toSize;
            switch (_params.method)
            {
                case Tween.EaseType.Linear:
                    Tween = new LinearEase();
                    break;
                case Tween.EaseType.ExpoEaseIn:
                    Tween = new ExpoEaseIn();
                    break;
                case Tween.EaseType.ExpoEaseOut:
                    Tween = new ExpoEaseOut();
                    break;
                case Tween.EaseType.ExpoEaseInOut:
                    Tween = new ExpoEaseInOut();
                    break;
                case Tween.EaseType.SineEaseIn:
                    Tween = new SineEaseIn();
                    break;
                case Tween.EaseType.SineEaseOut:
                    Tween = new SineEaseOut();
                    break;
                case Tween.EaseType.SineEaseInOut:
                    Tween = new SineEaseInOut();
                    break;
                case Tween.EaseType.ElasticEaseIn:
                    Tween = new ElasticEaseIn();
                    break;
                case Tween.EaseType.ElasticEaseOut:
                    Tween = new ElasticEaseOut();
                    break;
                case Tween.EaseType.ElasticEaseInOut:
                    Tween = new ElasticEaseInOut();
                    break;
                case Tween.EaseType.BackEaseIn:
                    Tween = new BackEaseIn();
                    break;
                case Tween.EaseType.BackEaseOut:
                    Tween = new BackEaseOut();
                    break;
                case Tween.EaseType.BackEaseInOut:
                    Tween = new BackEaseInOut();
                    break;
                case Tween.EaseType.BounceEaseIn:
                    Tween = new BounceEaseIn();
                    break;
                case Tween.EaseType.BounceEaseOut:
                    Tween = new BounceEaseOut();
                    break;
                case Tween.EaseType.BounceEaseInOut:
                    Tween = new BounceEaseInOut();
                    break;
                default:
                    Tween = new SineEaseOut();
                    break;
            }
            this.EndEvent = _params.OnEndEvent;
            this.EndEventWithParam = _params.OnEndEventWithParam;
        }

    }
}