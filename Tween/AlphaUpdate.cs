using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class AlphaUpdate : MonoBehaviour
    {
        class Params
        {
            public float delay;
            public float toAlpha;
            public float time;
            public Tween.EaseType method;
            public Action OnEndEvent;
            public TweenEvent OnEndEventWithParam;
            public bool withChildren;
        }
        class OneGraphic
        {
            public float alphaRatio;
            public Graphic graphic;
        }
        public Action EndEvent;
        public TweenEvent EndEventWithParam;
        Tween Tween;
        OneGraphic[] graphices = new OneGraphic[0];
        float t = 0, d = 0, b = 0, c = 0;
        float toAlpha;
        bool withChildren;
        bool delayFinished;
        List<Params> paramsQueue = new List<Params>();

        void OnDestroy()
        {
            EndEvent = null;
            EndEventWithParam = null;
        }

        void Awake()
        {
            Graphic[] _graphices = GetComponentsInChildren<Graphic>();
            graphices = new OneGraphic[_graphices.Length];
            for (int i = 0; i < graphices.Length; i++)
            {
                Graphic _graphic = _graphices[i];
                graphices[i] = new OneGraphic() { graphic = _graphic };
                if (i > 0)
                {
                    graphices[i].alphaRatio = _graphic.color.a / graphices[0].graphic.color.a;
                }
            }
        }

        void Update()
        {
            for (int i = 0; i < paramsQueue.Count; i++)
            {
                Params mParams = paramsQueue[i];
                mParams.delay -= Time.deltaTime;
                if (mParams.delay <= 0f)
                {
                    AlphaGo(mParams);
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
            SetGraphicAlpha(t, d);
            if (t < d)
            {
                t += Time.deltaTime;
            }
            else
            {
                SetGraphicAlpha(0, 0, toAlpha);
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

        void SetGraphicAlpha(float t, float d, float a = -1f)
        {
            for (int i = 0; i < graphices.Length; i++)
            {
                OneGraphic oneGraphic = graphices[i];
                if(oneGraphic==null || oneGraphic.graphic == null)
                {
                    continue;
                }
                if (!withChildren)
                {
                    if (oneGraphic.graphic.transform != transform)
                    {
                        break;
                    }
                }
                Color color = oneGraphic.graphic.color;
                if (i == 0)
                {
                    if (a == -1f)
                    {
                        color.a = (float)Tween.Ease(t, b, c, d);
                    }
                    else
                    {
                        color.a = a;
                    }
                }
                else
                {
                    color.a = graphices[0].graphic.color.a * oneGraphic.alphaRatio;
                }
                oneGraphic.graphic.color = color;
            }
        }

        public void Alpha(float delay, float toAlpha, float time, Tween.EaseType method, Action OnEndEvent, TweenEvent OnEndEventWithParam, bool withChildren)
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
            newParams.toAlpha = toAlpha;
            newParams.time = time;
            newParams.method = method;
            newParams.OnEndEvent = OnEndEvent;
            newParams.OnEndEventWithParam = OnEndEventWithParam;
            newParams.withChildren = withChildren;
            if (delay > 0f)
            {
                paramsQueue.Add(newParams);
            }
            else
            {
                AlphaGo(newParams);
            }
            this.enabled = true;
        }
        void AlphaGo(Params _params)
        {
            this.delayFinished = true;
            this.toAlpha = _params.toAlpha;
            this.withChildren = _params.withChildren;
            t = 0f;
            if (graphices.Length > 0)
            {
                b = graphices[0].graphic.color.a;
            }
            if (_params.time <= 0f)
            {
                SetGraphicAlpha(0, 0, this.toAlpha);
                if (paramsQueue.Count == 0)
                {
                    this.enabled = false;
                    this.delayFinished = false;
                }
            }
            else
            {
                c = _params.toAlpha - b;
                d = _params.time;
                this.EndEvent = _params.OnEndEvent;
                this.EndEventWithParam = _params.OnEndEventWithParam;
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
            }
        }
       
    }
}