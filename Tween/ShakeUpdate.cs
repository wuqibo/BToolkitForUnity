using System;
using System.Collections.Generic;
using UnityEngine;

namespace BToolkit
{
    public class ShakeUpdate : MonoBehaviour
    {
        class Params
        {
            public float delay;
            public float offset;
            public float time;
            public float scale;
            public Action OnEndEvent;
            public TweenEvent OnEndEventWithParam;
        }
        public Action EndEvent;
        public TweenEvent EndEventWithParam;
        Vector3 defaultPos, defaultScale;
        float shakerTimer;
        Vector3 shakerOffset;
        float offset, scale;
        bool delayFinsh;
        List<Params> paramsQueue = new List<Params>();

        void OnDestroy()
        {
            EndEvent = null;
            EndEventWithParam = null;
        }

        void Awake()
        {
            defaultPos = transform.localPosition;
        }

        void Update()
        {
            for (int i = 0; i < paramsQueue.Count; i++)
            {
                Params mParams = paramsQueue[i];
                mParams.delay -= Time.deltaTime;
                if (mParams.delay <= 0f)
                {
                    ShakeGo(mParams);
                    paramsQueue.RemoveAt(i);
                }
            }
            if (!delayFinsh)
            {
                return;
            }
            if (shakerTimer > 0f)
            {
                shakerTimer -= Time.deltaTime;
                shakerOffset = new Vector3(UnityEngine.Random.Range(-offset, offset), UnityEngine.Random.Range(-offset, offset), UnityEngine.Random.Range(-offset, offset));
                transform.localPosition = defaultPos + shakerOffset;
                if (shakerTimer <= 0f)
                {
                    transform.localPosition = defaultPos;
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
                    if (scale != 1f)
                    {
                        Tween.Scale(0, transform, defaultScale, 0.2f, Tween.EaseType.SineEaseOut);
                    }
                }
            }
        }

        public void Shake(float delay, float offset, float time, float scale, Action OnEndEvent, TweenEvent OnEndEventWithParam)
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
            newParams.offset = offset;
            newParams.time = time;
            newParams.scale = scale;
            newParams.OnEndEvent = OnEndEvent;
            newParams.OnEndEventWithParam = OnEndEventWithParam;
            if (delay > 0f)
            {
                paramsQueue.Add(newParams);
            }
            else
            {
                ShakeGo(newParams);
            }
            this.enabled = true;
        }
        void ShakeGo(Params _params)
        {
            this.delayFinsh = true;
            this.offset = _params.offset * Screen.height * 0.002f;
            shakerTimer = _params.time;
            this.scale = _params.scale;
            this.EndEvent = _params.OnEndEvent;
            this.EndEventWithParam = _params.OnEndEventWithParam;
            if (this.scale != 1f)
            {
                if (defaultScale == Vector3.zero)
                {
                    defaultScale = transform.localScale;
                }
                Tween.Scale(0, transform, defaultScale * _params.scale, 0.1f, Tween.EaseType.SineEaseOut);
            }
        }
    }
}