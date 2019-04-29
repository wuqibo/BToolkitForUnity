using System;
using System.Collections.Generic;
using UnityEngine;

namespace BToolkit
{
    public class MoveUpdate : MonoBehaviour
    {
        class Params
        {
            public float delay;
            public Vector3 toPos;
            public float time;
            public bool worldSpace;
            public Tween.EaseType method;
            public Action OnEndEvent;
            public TweenEvent OnEndEventWithParam;
            public bool xEnable;
            public bool yEnable;
            public bool zEnable;
        }
        Action EndEvent;
        TweenEvent EndEventWidthParam;
        Tween bTween;
        Vector3 toPos;
        RectTransform rectTrans;
        OneAxis xAxis = new OneAxis();
        OneAxis yAxis = new OneAxis();
        OneAxis zAxis = new OneAxis();
        bool useRectTrans, worldSpace;
        float t, d;
        class OneAxis
        {
            public float b, c;
            public bool canGo;
        }
        Vector3 pos;
        bool delayFinsh;
        List<Params> paramsQueue = new List<Params>();

        void OnDestroy()
        {
            EndEvent = null;
            EndEventWidthParam = null;
        }

        void Awake()
        {
            rectTrans = GetComponent<RectTransform>();
            useRectTrans = rectTrans;
        }

        void Update()
        {
            for (int i = 0; i < paramsQueue.Count; i++)
            {
                Params mParams = paramsQueue[i];
                mParams.delay -= Time.deltaTime;
                if (mParams.delay <= 0f)
                {
                    MoveGo(mParams);
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
            if (useRectTrans)
            {
                pos = rectTrans.anchoredPosition3D;
            }
            else
            {
                pos = worldSpace ? transform.position : transform.localPosition;
            }
            if (xAxis.canGo)
            {
                pos.x = (float)bTween.Ease(t, xAxis.b, xAxis.c, d);
            }
            if (yAxis.canGo)
            {
                pos.y = (float)bTween.Ease(t, yAxis.b, yAxis.c, d);
            }
            if (zAxis.canGo)
            {
                pos.z = (float)bTween.Ease(t, zAxis.b, zAxis.c, d);
            }
            if (useRectTrans)
            {
                rectTrans.anchoredPosition3D = pos;
            }
            else
            {
                if (worldSpace)
                {
                    transform.position = pos;
                }
                else
                {
                    transform.localPosition = pos;
                }
            }
            if (t < d)
            {
                t += Time.deltaTime;
            }
            else
            {
                if (useRectTrans)
                {
                    if (!xAxis.canGo)
                    {
                        toPos.x = rectTrans.anchoredPosition3D.x;
                    }
                    if (!yAxis.canGo)
                    {
                        toPos.y = rectTrans.anchoredPosition3D.y;
                    }
                    if (!zAxis.canGo)
                    {
                        toPos.z = rectTrans.anchoredPosition3D.z;
                    }
                    rectTrans.anchoredPosition3D = toPos;
                }
                else
                {
                    if (worldSpace)
                    {
                        if (!xAxis.canGo)
                        {
                            toPos.x = transform.position.x;
                        }
                        if (!yAxis.canGo)
                        {
                            toPos.y = transform.position.y;
                        }
                        if (!zAxis.canGo)
                        {
                            toPos.z = transform.position.z;
                        }
                        transform.position = toPos;
                    }
                    else
                    {
                        if (!xAxis.canGo)
                        {
                            toPos.x = transform.localPosition.x;
                        }
                        if (!yAxis.canGo)
                        {
                            toPos.y = transform.localPosition.y;
                        }
                        if (!zAxis.canGo)
                        {
                            toPos.z = transform.localPosition.z;
                        }
                        transform.localPosition = toPos;
                    }
                }
                if (paramsQueue.Count == 0)
                {
                    this.enabled = false;
                    this.delayFinsh = false;
                    if (EndEvent != null)
                    {
                        EndEvent();
                    }
                    if (EndEventWidthParam != null)
                    {
                        EndEventWidthParam(transform);
                    }
                }
            }
        }

        public void Move(float delay, Vector3 toPos, float time, bool worldSpace, Tween.EaseType method, Action OnEndEvent, TweenEvent OnEndEventWithParam, bool xEnable, bool yEnable, bool zEnable)
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
            newParams.toPos = toPos;
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
                MoveGo(newParams);
            }
            this.enabled = true;
        }
        void MoveGo(Params _params)
        {
            this.delayFinsh = true;
            this.worldSpace = _params.worldSpace;
            rectTrans = GetComponent<RectTransform>();
            useRectTrans = rectTrans;
            if (_params.time <= 0f)
            {
                if (useRectTrans)
                {
                    rectTrans.anchoredPosition3D = _params.toPos;
                }
                else
                {
                    if (worldSpace)
                    {
                        transform.position = _params.toPos;
                    }
                    else
                    {
                        if (!_params.xEnable)
                        {
                            _params.toPos.x = transform.localPosition.x;
                        }
                        if (!_params.yEnable)
                        {
                            _params.toPos.y = transform.localPosition.y;
                        }
                        if (!_params.zEnable)
                        {
                            _params.toPos.z = transform.localPosition.z;
                        }

                        transform.localPosition = _params.toPos;
                    }
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
            Vector3 transPos = worldSpace ? transform.position : transform.localPosition;
            if (useRectTrans)
            {
                transPos = rectTrans.anchoredPosition3D;
            }
            xAxis.canGo = _params.xEnable ? (_params.toPos.x != transPos.x) : _params.xEnable;
            yAxis.canGo = _params.yEnable ? (_params.toPos.y != transPos.y) : _params.yEnable;
            zAxis.canGo = _params.zEnable ? (_params.toPos.z != transPos.z) : _params.zEnable;
            if (xAxis.canGo)
            {
                xAxis.b = transPos.x;
                xAxis.c = _params.toPos.x - transPos.x;
            }
            if (yAxis.canGo)
            {
                yAxis.b = transPos.y;
                yAxis.c = _params.toPos.y - transPos.y;
            }
            if (zAxis.canGo)
            {
                zAxis.b = transPos.z;
                zAxis.c = _params.toPos.z - transPos.z;
            }
            this.toPos = _params.toPos;
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
            this.EndEventWidthParam = _params.OnEndEventWithParam;
        }

    }
}