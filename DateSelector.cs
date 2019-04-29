using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BToolkit
{
    public class DateSelector : MonoBehaviour
    {

        public Text textYear, textMonth, textDay;
        public float space = 90;
        public AudioClip sound;
        public Scoll yearScroll, monthScroll, dayScroll;
        AudioSource audioSource;

        void Awake()
        {
            yearScroll = new Scoll(this, Scoll.Type.Year);
            monthScroll = new Scoll(this, Scoll.Type.Month);
            dayScroll = new Scoll(this, Scoll.Type.Day);
            yearScroll.SetCurr(2015);
            monthScroll.SetCurr(10);
            dayScroll.SetCurr(29);
        }

        void Update()
        {
            yearScroll.Update();
            monthScroll.Update();
            dayScroll.Update();
        }

        public void OnYearPress()
        {
            yearScroll.OnTouchDown();
        }

        public void OnMonthPress()
        {
            monthScroll.OnTouchDown();
        }

        public void OnDayPress()
        {
            dayScroll.OnTouchDown();
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public class Scoll
        {
            public enum Type { Year, Month, Day }
            public Text[] texts;
            Text prefab, textPrevious;
            string name;
            int count;
            float[] poses;
            int currPosIndex;
            float previousPos = float.MinValue;
            Type type;
            float dragSpeedScale;
            DateSelector dateSelector;
            public Scoll(DateSelector dateSelector, Type type)
            {
                this.dateSelector = dateSelector;
                this.type = type;
                int amount = 0;
                if (type == Type.Year)
                {
                    count = 21;
                    name = "year";
                    amount = 2000;
                    this.prefab = dateSelector.textYear;
                }
                else if (type == Type.Month)
                {
                    count = 12;
                    name = "month";
                    amount = 1;
                    this.prefab = dateSelector.textMonth;
                }
                else if (type == Type.Day)
                {
                    count = 31;
                    name = "day";
                    amount = 1;
                    this.prefab = dateSelector.textDay;
                }
                texts = new Text[count];
                poses = new float[count];
                for (int i = 0; i < count; i++)
                {
                    if (i == 0)
                    {
                        texts[i] = prefab;
                    }
                    else
                    {
                        texts[i] = (Instantiate(prefab.gameObject) as GameObject).GetComponent<Text>();
                    }
                    texts[i].name = name + amount;
                    texts[i].text = "" + amount;
                    amount++;
                    texts[i].transform.SetParent(prefab.transform.parent);
                    texts[i].transform.localPosition = new Vector3(prefab.transform.localPosition.x, -i * dateSelector.space, 0);
                    texts[i].transform.localScale = prefab.transform.localScale;
                    poses[i] = i * dateSelector.space;
                }
                CanvasScaler canvasScaler = GameObject.FindObjectOfType<CanvasScaler>();
                dragSpeedScale = 1f * canvasScaler.referenceResolution.y / (float)Screen.height;
            }
            public void SetCurr(int value)
            {
                if (type == Type.Year)
                {
                    currPosIndex = value - 1 - 1999;
                }
                else
                {
                    currPosIndex = value - 1;
                }
                if (currPosIndex < 0)
                {
                    currPosIndex = 0;
                }
                else if (currPosIndex > count - 1)
                {
                    currPosIndex = count - 1;
                }
                Vector3 pos = texts[0].transform.localPosition;
                pos.y = poses[currPosIndex];
                texts[0].transform.localPosition = pos;
                if (type == Type.Day)
                {
                    RefreshDays(dateSelector.yearScroll.GetCurr(), dateSelector.monthScroll.GetCurr());
                }
            }
            public int GetCurr()
            {
                return int.Parse(texts[currPosIndex].text);
            }
            public void OnTouchDown()
            {
                previousPos = Input.mousePosition.y;
            }
            public void Update()
            {
                if (previousPos != float.MinValue)
                {
                    float delta = (Input.mousePosition.y - previousPos) * dragSpeedScale;
                    previousPos = Input.mousePosition.y;
                    Vector3 pos = texts[0].transform.localPosition;
                    pos.y += delta;
                    texts[0].transform.localPosition = pos;
                }
                else
                {
                    Vector3 pos = texts[0].transform.localPosition;
                    pos.y = poses[currPosIndex];
                    texts[0].transform.localPosition = pos;
                }
                for (int i = 1; i < count; i++)
                {
                    Vector3 pos = texts[i].transform.localPosition;
                    pos.y = texts[i - 1].transform.localPosition.y - dateSelector.space;
                    texts[i].transform.localPosition = pos;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (previousPos != float.MinValue)
                    {
                        previousPos = float.MinValue;
                        currPosIndex = GetNearestIndex();
                        if (type == Type.Year || type == Type.Month)
                        {
                            dateSelector.dayScroll.RefreshDays(int.Parse(dateSelector.yearScroll.texts[dateSelector.yearScroll.currPosIndex].text), int.Parse(dateSelector.monthScroll.texts[dateSelector.monthScroll.currPosIndex].text));
                        }
                    }
                }
                RefreshTextStyle();
            }
            int GetNearestIndex()
            {
                float dis = float.MaxValue;
                int index = 0;
                int dayCount = count;
                if (type == Type.Day)
                {
                    dayCount = GetCurrDaysCount(dateSelector.yearScroll.GetCurr(), dateSelector.monthScroll.GetCurr());
                }
                for (int i = 0; i < dayCount; i++)
                {
                    float newDis = Mathf.Abs(texts[i].transform.localPosition.y);
                    if (newDis < dis)
                    {
                        dis = newDis;
                        index = i;
                    }
                }
                return index;
            }
            void RefreshTextStyle()
            {
                for (int i = 0; i < count; i++)
                {
                    float newDis = Mathf.Abs(texts[i].transform.localPosition.y);
                    if (newDis < dateSelector.space * 3f)
                    {
                        texts[i].enabled = true;
                        if (newDis < dateSelector.space * 0.5f)
                        {
                            texts[i].transform.localScale = new Vector3(1, 1, 1);
                            texts[i].color = new Color(0.9f, 0.6f, 0.1f);
                            if (previousPos != float.MinValue && textPrevious != texts[i])
                            {
                                textPrevious = texts[i];
                                if (!dateSelector.audioSource)
                                {
                                    dateSelector.audioSource = dateSelector.GetComponent<AudioSource>();
                                    if (!dateSelector.audioSource)
                                    {
                                        dateSelector.audioSource = dateSelector.gameObject.AddComponent<AudioSource>();
                                    }
                                    dateSelector.audioSource.playOnAwake = false;
                                    dateSelector.audioSource.loop = false;
                                    dateSelector.audioSource.clip = dateSelector.sound;
                                }
                                dateSelector.audioSource.Play();
                            }
                        }
                        else
                        {
                            texts[i].transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                            if (newDis < dateSelector.space * 1.5f)
                            {
                                texts[i].color = new Color(0.5f, 0.5f, 0.5f);
                            }
                            else
                            {
                                texts[i].color = new Color(0.8f, 0.8f, 0.8f);
                            }
                        }
                    }
                    else
                    {
                        texts[i].enabled = false;
                    }
                }
            }
            void RefreshDays(int year, int month)
            {
                int dayCount = GetCurrDaysCount(year, month);
                for (int i = 0; i < count; i++)
                {
                    if (i < dayCount)
                    {
                        texts[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        texts[i].gameObject.SetActive(false);
                    }
                }
                if (currPosIndex > dayCount - 1)
                {
                    currPosIndex = dayCount - 1;
                    Vector3 pos = texts[0].transform.localPosition;
                    pos.y = poses[currPosIndex];
                    texts[0].transform.localPosition = pos;
                }
            }

            int GetCurrDaysCount(int year, int month)
            {
                int dayCount = 31;
                if (month == 2)
                {
                    if (year % 4 == 0)
                    {
                        dayCount = 29;
                    }
                    else
                    {
                        dayCount = 28;
                    }
                }
                else if (month == 4 || month == 6 || month == 9 || month == 11)
                {
                    dayCount = 30;
                }
                return dayCount;
            }
        }
    }
}