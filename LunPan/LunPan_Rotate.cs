using System;
using UnityEngine;

namespace BToolkit
{
    public class LunPan_Rotate : MonoBehaviour
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }
        enum State
        {
            Stop,
            Accelerate,
            ConstantSpeed,
            Deceleration,
            Correct
        }
        public Axis axis = Axis.Z;
        State currState = State.Stop;
        Action StopedEvent;
        public float maxSpeed = 300;
        public float accelTime = 1;
        public float constTime = 1;
        public int decelRoundsCount = 1;
        public int stopAngle;
        float timer, constTimer, currSpeed, decelA;
        float previousAngle;

        public void StartRotate(int stopAngle = int.MinValue, Action OnStopedCallback = null)
        {
            this.StopedEvent = OnStopedCallback;
            if (stopAngle != int.MinValue)
            {
                this.stopAngle = stopAngle;
            }
            if (decelRoundsCount < 0)
            {
                decelRoundsCount = 0;
            }
            timer = 0;
            previousAngle = float.MinValue;
            currState = State.Accelerate;
        }

        void Update()
        {
            switch (currState)
            {
                case State.Accelerate:
                    timer += Time.deltaTime;
                    if (timer > accelTime)
                    {
                        timer = accelTime;
                        constTimer = 0;
                        currState = State.ConstantSpeed;
                    }
                    AddAngle(SineIn(timer, 0, maxSpeed, accelTime) * Time.deltaTime);
                    break;
                case State.ConstantSpeed:
                    AddAngle(maxSpeed * Time.deltaTime);
                    constTimer += Time.deltaTime;
                    if (constTimer >= constTime)
                    {
                        if (decelRoundsCount == 0)
                        {
                            SetAngle(stopAngle);
                            currState = State.Stop;
                            return;
                        }
                        if (previousAngle == float.MinValue)
                        {
                            previousAngle = GetCurrAngle();
                        }
                        else
                        {
                            float currAngle = GetCurrAngle();
                            float currAngleSpeed = Mathf.Abs(currAngle - previousAngle);
                            previousAngle = currAngle;
                            if (currAngleSpeed < 180)
                            {
                                if (GetToTargetIncludedAngle() > -currAngleSpeed && GetToTargetIncludedAngle() < currAngleSpeed)
                                {
                                    timer = 0;
                                    float s = GetToTargetIncludedAngle() + decelRoundsCount * 360;//停止所用圈数
                                    currSpeed = maxSpeed;
                                    decelA = Mathf.Pow(currSpeed, 2) / (2f * s);
                                    if (maxSpeed > 0)
                                    {
                                        AddAngle(2f);
                                    }
                                    else
                                    {
                                        AddAngle(-2f);
                                    }
                                    currState = State.Deceleration;
                                }
                            }
                        }
                    }
                    break;
                case State.Deceleration:
                    if (maxSpeed > 0)
                    {
                        currSpeed -= decelA * Time.deltaTime;
                        if (currSpeed < 0)
                        {
                            currSpeed = 0;
                            timer = 0;
                            currState = State.Correct;
                            if (StopedEvent != null)
                            {
                                StopedEvent();
                            }
                        }
                    }
                    else
                    {
                        currSpeed += decelA * Time.deltaTime;
                        if (currSpeed > 0)
                        {
                            currSpeed = 0;
                            timer = 0;
                            currState = State.Correct;
                            if (StopedEvent != null)
                            {
                                StopedEvent();
                            }
                        }
                    }
                    AddAngle(currSpeed * Time.deltaTime);
                    break;
                case State.Correct:
                    SetAngle(Mathf.LerpAngle(GetCurrAngle(), stopAngle, Time.deltaTime));
                    timer += Time.deltaTime;
                    if (timer > 0.5f)
                    {
                        currState = State.Stop;
                    }
                    break;
            }
        }

        public void StopAtAngle(int stopAtKeyAngle)
        {
            currState = State.Stop;
            SetAngle(stopAtKeyAngle);
        }

        void AddAngle(float value)
        {
            Vector3 angle = transform.localEulerAngles;
            if (axis == Axis.Z)
            {
                angle.z += value;
            }
            else if (axis == Axis.Y)
            {
                angle.y += value;
            }
            else
            {
                angle.x += value;
            }
            transform.localEulerAngles = angle;
        }

        void SetAngle(float value)
        {
            Vector3 angle = transform.localEulerAngles;
            if (axis == Axis.Z)
            {
                angle.z = value;
            }
            else if (axis == Axis.Y)
            {
                angle.y = value;
            }
            else
            {
                angle.x = value;
            }
            transform.localEulerAngles = angle;
        }

        float GetToTargetIncludedAngle()
        {
            float angle = 0;
            if (maxSpeed > 0)
            {
                float currAngle = GetCurrAngle();
                while (stopAngle < currAngle)
                {
                    currAngle -= 360;
                }
                angle = stopAngle - currAngle;
            }
            else
            {
                float currAngle = GetCurrAngle();
                while (currAngle < stopAngle)
                {
                    currAngle += 360;
                }
                angle = currAngle - stopAngle;
            }
            while (angle > 360)
            {
                angle -= 360;
            }
            return angle;
        }

        float GetCurrAngle()
        {
            if (axis == Axis.Z)
            {
                float angle = transform.localEulerAngles.z;
                while (angle < 0)
                {
                    angle += 360;
                }
                return angle;
            }
            else if (axis == Axis.Y)
            {
                float angle = transform.localEulerAngles.y;
                while (angle < 0)
                {
                    angle += 360;
                }
                return angle;
            }
            else
            {
                float angle = transform.localEulerAngles.x;
                while (angle < 0)
                {
                    angle += 360;
                }
                return angle;
            }
        }

        float SineOut(double t, double b, double c, double d)
        {
            return (float)(c * Math.Sin(t / d * (Math.PI / 2)) + b);
        }

        float SineIn(double t, double b, double c, double d)
        {
            return (float)(-c * Math.Cos(t / d * (Math.PI / 2)) + c + b);
        }
    }
}