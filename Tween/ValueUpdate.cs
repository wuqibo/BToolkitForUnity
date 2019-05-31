using System;
using UnityEngine;

namespace BToolkit
{
    public class ValueUpdate : MonoBehaviour
    {
        public Action<float> UpdateEvent, FinishEvent;
        Tween Tween;
        float delay = 0, t = 0, d = 0, b = 0, c = 0;
        float toValue;

        public void Destroy()
        {
            Destroy(gameObject);
        }

        void Update()
        {
            if (delay > 0)
            {
                delay -= Time.deltaTime;
                return;
            }
            if (Tween == null)
            {
                return;
            }
            SetValue(t, d);
            if (t < d)
            {
                t += Time.deltaTime;
            }
            else
            {
                SetValue(0, 0, toValue);
                if (FinishEvent != null)
                {
                    FinishEvent(toValue);
                }
                Destroy(gameObject);
            }
        }

        void SetValue(float t, float d, float toValue = -1f)
        {
            if (toValue == -1f)
            {
                UpdateEvent((float)Tween.Ease(t, b, c, d));
            }
            else
            {
                UpdateEvent(toValue);
            }
        }

        public void Value(float delay, float startValue, float toValue, float time, Tween.EaseType method, Action<float> updateEvent, Action<float> finishEvent)
        {
            this.delay = delay;
            this.toValue = toValue;
            t = 0f;
            b = startValue;
            c = this.toValue - b;
            d = time;
            this.UpdateEvent = updateEvent;
            this.FinishEvent = finishEvent;
            switch (method)
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