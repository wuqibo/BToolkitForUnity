using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    [ExecuteInEditMode]
    public class BSlider : MonoBehaviour
    {

        public enum Method
        {
            Move, Fill
        }
        public Method method = Method.Fill;
        public Image bar, barFollow;
        public float value0 = 0f, value1 = 1f;
        public float value;
        float currValue;
        bool isLeftToRight = true;
        float shadowSpeed;
        BMath.Line kb;

        void Awake()
        {
            isLeftToRight = (value0 < value1);
            kb = new BMath.Line(0, value0, 1, value1);
        }

        void Start()
        {
            InitShadow();
        }

        void Update()
        {
            if (bar)
            {
                if (currValue != value)
                {
                    if (value < 0f)
                    {
                        value = 0f;
                    }
                    if (value > 1f)
                    {
                        value = 1f;
                    }
                    if (kb == null)
                    {
                        isLeftToRight = (value0 < value1);
                        kb = new BMath.Line(0, value0, 1, value1);
                    }
                    float x = kb.k * value + kb.b;
                    if (method == Method.Fill)
                    {
                        bar.fillAmount = x;
                        if (barFollow)
                        {
                            shadowSpeed = Mathf.Abs(bar.fillAmount - barFollow.fillAmount) * 2f;
                        }
                    }
                    else
                    {
                        Vector3 pos = bar.transform.localPosition;
                        pos.x = x;
                        bar.transform.localPosition = pos;
                        if (barFollow)
                        {
                            shadowSpeed = Mathf.Abs(bar.transform.localPosition.x - barFollow.transform.localPosition.x) * 2f;
                        }
                    }
                    currValue = value;
                }
            }
            if (barFollow)
            {
                if (method == Method.Fill)
                {
                    if (isLeftToRight)
                    {
                        if (bar.fillAmount > barFollow.fillAmount)
                        {
                            barFollow.fillAmount = bar.fillAmount;
                        }
                        else
                        {
                            barFollow.fillAmount -= shadowSpeed * Time.deltaTime;
                        }
                    }
                    else
                    {
                        if (bar.fillAmount < barFollow.fillAmount)
                        {
                            barFollow.fillAmount = bar.fillAmount;
                        }
                        else
                        {
                            barFollow.fillAmount += shadowSpeed * Time.deltaTime;
                        }
                    }
                }
                else
                {
                    if (isLeftToRight)
                    {
                        if (bar.transform.localPosition.x > barFollow.transform.localPosition.x)
                        {
                            barFollow.transform.localPosition = bar.transform.localPosition;
                        }
                        else
                        {
                            barFollow.transform.localPosition -= new Vector3(shadowSpeed * Time.deltaTime, 0f, 0f);
                        }
                    }
                    else
                    {
                        if (bar.transform.localPosition.x < barFollow.transform.localPosition.x)
                        {
                            barFollow.transform.localPosition = bar.transform.localPosition;
                        }
                        else
                        {
                            barFollow.transform.localPosition += new Vector3(shadowSpeed * Time.deltaTime, 0f, 0f);
                        }
                    }
                }
            }
#if UNITY_EDITOR
            kb = new BMath.Line(0, value0, 1, value1);
#endif
        }

        public void InitShadow()
        {
            if (barFollow)
            {
                if (method == Method.Fill)
                {
                    barFollow.fillAmount = bar.fillAmount;
                }
                else
                {
                    barFollow.transform.localPosition = bar.transform.localPosition;
                }
            }
        }
    }
}