using System;
using UnityEngine;

namespace BToolkit
{
    public class LunPan : MonoBehaviour
    {
        enum State
        {
            /// <summary>
            /// 停止
            /// </summary>
            Stop,
            /// <summary>
            /// 加速
            /// </summary>
            Accelerate,
            /// <summary>
            /// 匀速
            /// </summary>
            ConstantSpeed,
            /// <summary>
            /// 减速
            /// </summary>
            Deceleration
        }
        public RectTransform zhuanpan;
        public RectTransform finger;
        public AudioClip sound;
        public LunPan_Cell textPoint;
        int cellCount = 16;
        [HideInInspector]
        public LunPan_Cell[] cells;
        State currState = State.Stop;
        float constantSpeedTime = 0.5f, constantSpeedTimer;
        float repeatRate = 0.02f;
        //旋转数据
        [Header("转盘")]
        public Sprite[] goldIconLevels = new Sprite[4];
        public float zhuanpanOffset;
        float accelerate = 0.2f;//加速
        float deceleration = 0.05f;//减速
        int subTimes = 200;//减速次数
        float accelerateAngle, decelerationAngle;
        float accelerateSpeed, decelerationSpeed;
        int stopIndex;
        Action<int> StopEvent;
        //指针数据
        [Header("指针")]
        public float fingerMaxAngle = 45;
        public float dropRadio = 0.5f;//0-1选择
        public int fingerOffset = 0;
        float cellAngle;
        float fingerPreviousAngle;
        bool hasPlaySound;
        BMath.Line fingerAngleKB;

        void Start()
        {
            finger.localEulerAngles = new Vector3(0, 0, 0);
        }

        public void CreateCells(int[] goldValues)
        {
            if (dropRadio < 0f)
            {
                dropRadio = 0f;
            }
            if (dropRadio > 1f)
            {
                dropRadio = 1f;
            }
            textPoint.gameObject.SetActive(false);
            cellCount = goldValues.Length;
            cells = new LunPan_Cell[cellCount];
            cellAngle = 360 / (float)cellCount;
            float angle = cellAngle * 0.5f;
            for (int i = 0; i < cells.Length; i++)
            {
                LunPan_Cell cell = Instantiate(textPoint);
                cell.transform.SetParent(textPoint.transform.parent, false);
                cell.gameObject.SetActive(true);
                cell.SetContent(this, goldValues[i]);
                Vector2 pos;
                float radius = textPoint.rectTransform.anchoredPosition.y;
                float radian = angle * Mathf.PI / 180f;
                pos.x = Mathf.Sin(radian) * radius;
                pos.y = Mathf.Cos(radian) * radius;
                angle += cellAngle;
                cell.rectTransform.anchoredPosition = pos;
                cell.rectTransform.localEulerAngles = new Vector3(0, 0, -i * cellAngle - cellAngle * 0.5f);
                cells[i] = cell;
            }
            fingerAngleKB = new BMath.Line(0, 0, 360 * dropRadio, fingerMaxAngle);
        }

        /// <summary>
        /// 开始转动轮盘 参数：指定停止的角度索引和回调，该索引在轮盘停止时的回调传回
        /// </summary>
        public void StartRotate(int stopIndex, Action<int> OnStop)
        {
            this.stopIndex = stopIndex;
            this.StopEvent = OnStop;
            //加速初始配置
            accelerateSpeed = 0;
            accelerateAngle = zhuanpan.localEulerAngles.z;
            //减速初始配置
            decelerationSpeed = 0;
            decelerationAngle = 360 * stopIndex / (float)cellCount;
            for (int i = 0; i < subTimes; i++)
            {
                decelerationSpeed += deceleration;
                decelerationAngle += decelerationSpeed;
            }
            currState = State.Accelerate;
            InvokeRepeating("RotateUpdate", 0, repeatRate);
        }

        void RotateUpdate()
        {
            switch (currState)
            {
                case State.Accelerate:
                    //加速
                    accelerateSpeed += accelerate;
                    accelerateAngle -= accelerateSpeed;
                    if (accelerateSpeed >= decelerationSpeed)
                    {
                        constantSpeedTimer = constantSpeedTime;
                        currState = State.ConstantSpeed;
                    }
                    zhuanpan.localEulerAngles = new Vector3(0, 0, accelerateAngle);
                    UpdateZhizhenAngle(zhuanpan.localEulerAngles.z);
                    break;
                case State.ConstantSpeed:
                    //匀速
                    constantSpeedTimer -= repeatRate;
                    if (constantSpeedTimer <= 0f)
                    {
                        currState = State.Deceleration;
                    }
                    zhuanpan.localEulerAngles -= new Vector3(0, 0, accelerateSpeed);
                    UpdateZhizhenAngle(zhuanpan.localEulerAngles.z);
                    break;
                case State.Deceleration:
                    //减速
                    decelerationAngle -= decelerationSpeed;
                    decelerationSpeed -= deceleration;
                    if (decelerationSpeed < 0f)
                    {
                        decelerationSpeed = 0;
                        CancelInvoke("RotateUpdate");
                        if (StopEvent != null)
                        {
                            StopEvent(stopIndex);
                        }
                        currState = State.Stop;
                    }
                    zhuanpan.localEulerAngles = new Vector3(0, 0, decelerationAngle + zhuanpanOffset);
                    UpdateZhizhenAngle(zhuanpan.localEulerAngles.z);
                    break;
            }
        }

        void UpdateZhizhenAngle(float zhuanpanAngle)
        {
            zhuanpanAngle += fingerOffset;
            float zhuanpanPositiveAngle = 360 - PositiveAngle(zhuanpanAngle * cellCount);
            float fingerAngle = fingerAngleKB.k * zhuanpanPositiveAngle + fingerAngleKB.b;
            if (zhuanpanPositiveAngle < 360 * dropRadio)
            {
                finger.localEulerAngles = new Vector3(0, 0, fingerAngle);
            }
            else
            {
                finger.localEulerAngles = Vector3.zero;
            }
            if (fingerAngle < fingerPreviousAngle)
            {
                if (!hasPlaySound)
                {
                    SoundPlayer.Play(0, sound);
                    hasPlaySound = true;
                }
            }
            else
            {
                hasPlaySound = false;
            }
            fingerPreviousAngle = fingerAngle;
        }

        float CurrPointAngle(float angle)
        {
            if (angle > 0)
            {
                angle -= 360;
            }
            return angle;
        }

        int GetNearestPointIndex(float zhuanpanAngle)
        {
            int index = 0;
            float minAngle = float.MinValue;
            for (int i = 0; i < cellCount; i++)
            {
                float subAngle = PositiveAngle(zhuanpanAngle + i * cellAngle);
                if (subAngle < minAngle)
                {
                    index = i;
                    minAngle = subAngle;
                }
            }
            return index;
        }

        float PositiveAngle(float angle)
        {
            while (angle < 0)
            {
                angle += 360;
            }
            while (angle > 360)
            {
                angle -= 360;
            }
            return angle;
        }

    }
}