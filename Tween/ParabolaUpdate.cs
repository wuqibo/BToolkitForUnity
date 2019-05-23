using System;
using System.Collections.Generic;
using UnityEngine;

namespace BToolkit
{
    public class ParabolaUpdate : MonoBehaviour
    {
        class Params
        {
            public float delay;
            public Vector3 toPos;
            public Vector3 middlePos;
            public float delta;
            public bool worldSpace;
            public float elasticity;
            public Action OnCollisionEvent;
            public TweenEvent OnCollisionEventWithParam;
            public Action OnEndEvent;
            public TweenEvent OnEndEventWithParam;
            public bool useHeight;
        }
        public Action CollisionEvent, EndEvent;
        public TweenEvent CollisionEventWithParam, EndEventWithParam;
        Vector3 fromPos, middlePos, toPos;
        float delta, t, elasticity;
        bool worldSpace, useHeight;
        Vector3 previousPos;
        bool delayFinished;
        List<Params> paramsQueue = new List<Params>();

        void OnDestroy()
        {
            CollisionEvent = null;
            CollisionEventWithParam = null;
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
                    ParabolaGo(mParams);
                    paramsQueue.RemoveAt(i);
                }
            }
            if (!delayFinished)
            {
                return;
            }
            Vector3 pos = worldSpace ? transform.position : transform.localPosition;
            t += delta * Time.deltaTime;
            if (t > 1f)
            {
                t = 1f;
            }
            pos = GetBezierPoint(fromPos, middlePos, toPos, t);
            if (useHeight)
            {
                if (IsDowning())
                {
                    if (pos.y < toPos.y)
                    {
                        pos.y = toPos.y;
                    }
                }
            }
            if (worldSpace)
            {
                transform.position = pos;
            }
            else
            {
                transform.localPosition = pos;
            }
            if (t == 1f)
            {
                if (CollisionEvent != null)
                {
                    CollisionEvent();
                }
                if (CollisionEventWithParam != null)
                {
                    CollisionEventWithParam(transform);
                }
                if (elasticity > 0f)
                {
                    if (useHeight)
                    {
                        float nextHeight = (middlePos.y - toPos.y) * elasticity;
                        if (nextHeight > 0.01f)
                        {
                            Vector3 nextPos = toPos + (toPos - fromPos) * elasticity;
                            nextPos.y = toPos.y;
                            float nextDelta = delta / elasticity;
                            Go(0, nextPos, nextHeight, nextDelta, worldSpace, elasticity, null, null, EndEvent, EndEventWithParam);
                        }
                        else
                        {
                            if (EndEvent != null)
                            {
                                EndEvent();
                            }
                            if (EndEventWithParam != null)
                            {
                                EndEventWithParam(transform);
                            }
                            if (paramsQueue.Count == 0)
                            {
                                this.enabled = false;
                                this.delayFinished = false;
                            }
                        }
                    }
                    else
                    {
                        Vector3 nextPos = toPos + (toPos - fromPos) * elasticity;
                        if (Vector3.Distance(toPos, nextPos) > 0.01f)
                        {
                            Vector3 nextMidPos = toPos + (middlePos - fromPos) * elasticity;
                            float nextDelta = delta / elasticity;
                            Go(0, nextPos, nextMidPos, nextDelta, worldSpace, elasticity, null, null, EndEvent, EndEventWithParam);
                        }
                        else
                        {
                            if (EndEvent != null)
                            {
                                EndEvent();
                            }
                            if (EndEventWithParam != null)
                            {
                                EndEventWithParam(transform);
                            }
                            if (paramsQueue.Count == 0)
                            {
                                this.enabled = false;
                                this.delayFinished = false;
                            }
                        }
                    }
                }
                else
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
            }
        }

        bool IsDowning()
        {
            if (previousPos.y > transform.position.y)
            {
                return true;
            }
            previousPos = transform.position;
            return false;
        }

        public void Go(float delay, Vector3 toPos, float height, float delta, bool worldSpace, float elasticity, Action OnCollisionEvent, TweenEvent OnCollisionEventWithParam, Action OnEndEvent, TweenEvent OnEndEventWithParam)
        {
            for (int i = 0; i < paramsQueue.Count; i++)
            {
                if (delay <= paramsQueue[i].delay)
                {
                    paramsQueue.RemoveAt(i);
                }
            }
            Vector3 fromP = worldSpace ? transform.position : transform.localPosition;
            middlePos = new Vector3((fromP.x + toPos.x) * 0.5f, fromP.y + height, (fromP.z + toPos.z) * 0.5f);
            Params newParams = new Params();
            newParams.delay = delay;
            newParams.toPos = toPos;
            newParams.middlePos = middlePos;
            newParams.delta = delta;
            newParams.worldSpace = worldSpace;
            newParams.elasticity = elasticity;
            newParams.OnCollisionEvent = OnCollisionEvent;
            newParams.OnCollisionEventWithParam = OnCollisionEventWithParam;
            newParams.OnEndEvent = OnEndEvent;
            newParams.OnEndEventWithParam = OnEndEventWithParam;
            newParams.useHeight = true;
            if (delay > 0f)
            {
                paramsQueue.Add(newParams);
            }
            else
            {
                ParabolaGo(newParams);
            }
            this.enabled = true;
        }
        public void Go(float delay, Vector3 toPos, Vector3 middlePos, float delta, bool worldSpace, float elasticity, Action OnCollisionEvent, TweenEvent OnCollisionEventWithParam, Action OnEndEvent, TweenEvent OnEndEventWithParam)
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
            newParams.middlePos = middlePos;
            newParams.delta = delta;
            newParams.worldSpace = worldSpace;
            newParams.elasticity = elasticity;
            newParams.OnCollisionEvent = OnCollisionEvent;
            newParams.OnCollisionEventWithParam = OnCollisionEventWithParam;
            newParams.OnEndEvent = OnEndEvent;
            newParams.OnEndEventWithParam = OnEndEventWithParam;
            newParams.useHeight = false;
            if (delay > 0f)
            {
                if (paramsQueue.Count == 0)
                {
                    this.delayFinished = false;
                }
                paramsQueue.Add(newParams);
            }
            else
            {
                ParabolaGo(newParams);
            }
            this.enabled = true;
        }
        void ParabolaGo(Params _params)
        {
            this.delayFinished = true;
            this.worldSpace = _params.worldSpace;
            fromPos = _params.worldSpace ? transform.position : transform.localPosition;
            previousPos = fromPos;
            this.toPos = _params.toPos;
            this.middlePos = _params.middlePos;
            this.delta = _params.delta;
            this.elasticity = _params.elasticity;
            this.CollisionEvent = _params.OnCollisionEvent;
            this.CollisionEventWithParam = _params.OnCollisionEventWithParam;
            this.EndEvent = _params.OnEndEvent;
            this.EndEventWithParam = _params.OnEndEventWithParam;
            t = 0f;
            this.useHeight = _params.useHeight;
        }

        Vector3 GetBezierPoint(Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            //t范围0到1,对应的贝塞尔起点到终点;
            float x = CalculateQuadSpline(p1.x, p2.x, p3.x, t);
            float y = CalculateQuadSpline(p1.y, p2.y, p3.y, t);
            float z = CalculateQuadSpline(p1.z, p2.z, p3.z, t);
            return new Vector3(x, y, z);
        }

        float CalculateQuadSpline(float z0, float z1, float z2, float t)
        {
            float a1 = (float)((1.0 - t) * (1.0 - t) * z0);
            float a2 = (float)(2.0 * t * (1 - t) * z1);
            float a3 = (float)(t * t * z2);
            float a4 = a1 + a2 + a3;
            return a4;
        }
        
    }
}