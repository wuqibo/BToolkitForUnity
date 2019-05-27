using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;

namespace BToolkit
{
    public class Scroll : MonoBehaviour
    {

        public enum Type
        {
            FreeHorizontal,
            FreeVertical,
            Page
        }
        public Type type = Type.FreeHorizontal;
        public Vector2 itemSize = new Vector2(100, 100);
        public float colSpace = 10f;
        public float rowSpace = 10f;
        public float pageSpace = 20f;
        public int rowCount = 1;
        public int colCount = 1;
        public int pageCount = 1;
        public float endLimit;
        public float dragSpeed = 1f;
        Mask mask = null;
        Vector2 maskSize, pageSize;
        bool isTouching;
        Vector3 crrTouchDelta, downPos;
        Vector3 previousPos;
        float offset = 300f;
        float backSpeed = 0.35f;
        int currPageIndex = 0;
        float dragLengByScreenScale = 0.05f;
        public delegate void OnPageChangeDetegate(int targetPageIndex);
        public OnPageChangeDetegate OnPageChangeEvent = null;

        void OnEnable()
        {
            crrTouchDelta = Vector3.zero;
        }

        void FindAllButtonAndAddTouchEvent(ref Transform[] items)
        {
            int count = items.Length;
            for (int i = 0; i < count; i++)
            {
                Button[] buttons = items[i].GetComponentsInChildren<Button>();
                for (int j = 0; j < buttons.Length; j++)
                {
                    UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(delegate
                    {
                        OnMouseOrTouchDown();
                    });
                    AddTiggerDownToTarget(buttons[j].gameObject, callback);
                }
            }
        }

        void AddTiggerDownToTarget(GameObject target, UnityAction<BaseEventData> callback)
        {
            EventTrigger eventTrigger = target.gameObject.GetComponent<EventTrigger>();
            if (!eventTrigger)
            {
                eventTrigger = target.gameObject.AddComponent<EventTrigger>();
            }
#if UNITY_5
            eventTrigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(callback);
            eventTrigger.triggers.Add(entry);
#else
        eventTrigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(callback);
        eventTrigger.triggers.Add(entry);
#endif
        }

        public void SetPos(Transform[] items, Vector3 itemScale)
        {
            FindAllButtonAndAddTouchEvent(ref items);
            if (maskSize == Vector2.zero)
            {
                mask = transform.parent.GetComponent<Mask>();
                if (mask)
                {
                    RectTransform rectTransform = mask.GetComponent<RectTransform>();
                    maskSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * rectTransform.localScale.x;
                    UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(delegate
                    {
                        OnMouseOrTouchDown();
                    });
                    AddTiggerDownToTarget(mask.gameObject, callback);
                }
                else
                {
                    Debug.LogError("BScroll's parent must add 'Mask' component");
                }
            }
            Vector2 posLeftTop = Vector2.zero;
            posLeftTop.x = -maskSize.x * 0.5f + itemSize.x * itemScale.x * 0.5f + colSpace;
            posLeftTop.y = maskSize.y * 0.5f - itemSize.y * itemScale.y * 0.5f - rowSpace;
            if (type == Type.FreeHorizontal)
            {
                endLimit = -(itemSize.x * itemScale.x + colSpace) * items.Length + maskSize.x - colSpace;
                for (int colI = 0; colI < items.Length; colI++)
                {
                    float posX = posLeftTop.x + colI * (itemSize.x * itemScale.x + colSpace);
                    float posY = posLeftTop.y;
                    items[colI].SetParent(transform);
                    items[colI].localScale = itemScale;
                    items[colI].localPosition = new Vector3(posX, posY, 0);
                }
            }
            else if (type == Type.FreeVertical)
            {
                rowCount = Mathf.CeilToInt((float)items.Length / (float)colCount);
                endLimit = (itemSize.y * itemScale.y + rowSpace) * rowCount - maskSize.y + colSpace;
                int i = 0;
                pageSize.x = (itemSize.x * itemScale.x + colSpace) * colCount + pageSpace;
                int itemsLength = items.Length;
                for (int rowI = 0; rowI < rowCount; rowI++)
                {
                    for (int colI = 0; colI < colCount; colI++)
                    {
                        if (i < itemsLength)
                        {
                            float posX = posLeftTop.x + colI * (itemSize.x * itemScale.x + colSpace);
                            float posY = posLeftTop.y - rowI * (itemSize.y * itemScale.y + rowSpace);
                            items[i].SetParent(transform);
                            items[i].localScale = itemScale;
                            items[i].localPosition = new Vector3(posX, posY, 0);
                            i++;
                        }
                    }
                }
            }
            else if (type == Type.Page)
            {
                int i = 0;
                pageSize.x = (itemSize.x * itemScale.x + colSpace) * colCount + pageSpace;
                for (int pageI = 0; pageI < pageCount; pageI++)
                {
                    for (int rowI = 0; rowI < rowCount; rowI++)
                    {
                        for (int colI = 0; colI < colCount; colI++)
                        {
                            float posX = posLeftTop.x + colI * (itemSize.x * itemScale.x + colSpace);
                            float posY = posLeftTop.y - rowI * (itemSize.y * itemScale.y + rowSpace);
                            items[i].SetParent(transform);
                            items[i].localScale = itemScale;
                            items[i].localPosition = new Vector3(posX, posY, 0);
                            i++;
                        }
                    }
                    posLeftTop.x += (itemSize.x * itemScale.x + colSpace) * colCount + pageSpace;
                }
            }
            if (OnPageChangeEvent != null)
            {
                OnPageChangeEvent(currPageIndex);
            }
        }

        public void PriviousPage()
        {
            currPageIndex--;
            if (currPageIndex < 0)
            {
                currPageIndex = pageCount - 1;
            }
            if (OnPageChangeEvent != null)
            {
                OnPageChangeEvent(currPageIndex);
            }
        }

        public void NextPage()
        {
            currPageIndex++;
            if (currPageIndex > pageCount - 1)
            {
                currPageIndex = 0;
            }
            if (OnPageChangeEvent != null)
            {
                OnPageChangeEvent(currPageIndex);
            }
        }

        public void SetPageIndex(int index)
        {
            this.currPageIndex = index;
            if (OnPageChangeEvent != null)
            {
                OnPageChangeEvent(currPageIndex);
            }
        }

        public void LocateToItemIndex(int index)
        {
            //
        }

        public void LocateToPoxX(float x)
        {
            crrTouchDelta = Vector3.zero;
            Vector3 pos = transform.localPosition;
            pos.x = x;
            transform.localPosition = pos;
        }

        public void LocateToPoxY(float y)
        {
            crrTouchDelta = Vector3.zero;
            Vector3 pos = transform.localPosition;
            pos.y = y;
            transform.localPosition = pos;
        }

        void Update()
        {
            if (Application.isEditor)
            {
                if (Input.GetMouseButton(0))
                {
                    OnMove();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    OnUp();
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        OnMove();
                    }
                    if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        OnUp();
                    }
                }
            }
            if (type == Type.FreeHorizontal)
            {
                if (!isTouching)
                {
                    crrTouchDelta += (Vector3.zero - crrTouchDelta) * 0.2f;
                    Vector3 pos = transform.localPosition;
                    pos.x = ScrollUpdate(pos.x, crrTouchDelta.x, endLimit, 0f, offset, isTouching, backSpeed, maskSize.x);
                    transform.localPosition = pos;
                }
            }
            else if (type == Type.FreeVertical)
            {
                if (!isTouching)
                {
                    crrTouchDelta += (Vector3.zero - crrTouchDelta) * 0.2f;
                    Vector3 pos = transform.localPosition;
                    pos.y = ScrollUpdate(pos.y, crrTouchDelta.y, endLimit, 0f, offset, isTouching, backSpeed, maskSize.y);
                    transform.localPosition = pos;
                }
            }
            else if (type == Type.Page)
            {
                if (!isTouching)
                {
                    Vector3 pos = transform.localPosition;
                    pos.x += (-currPageIndex * pageSize.x - pos.x) * backSpeed;
                    transform.localPosition = pos;
                }
            }
        }

        void OnMouseOrTouchDown()
        {
            OnDown(Input.mousePosition);
        }

        void OnDown(Vector2 pos)
        {
            isTouching = true;
            crrTouchDelta = Vector3.zero;
            previousPos = downPos = Input.mousePosition;
        }

        void OnMove()
        {
            if (isTouching)
            {
                if (type == Type.FreeHorizontal)
                {
                    crrTouchDelta = (Input.mousePosition - previousPos) * dragSpeed;
                    previousPos = Input.mousePosition;
                    Vector3 pos = transform.localPosition;
                    pos.x = ScrollUpdate(pos.x, crrTouchDelta.x, endLimit, 0f, offset, isTouching, backSpeed, maskSize.x);
                    transform.localPosition = pos;
                }
                else if (type == Type.FreeVertical)
                {
                    crrTouchDelta = (Input.mousePosition - previousPos) * dragSpeed;
                    previousPos = Input.mousePosition;
                    Vector3 pos = transform.localPosition;
                    pos.y = ScrollUpdate(pos.y, crrTouchDelta.y, endLimit, 0f, offset, isTouching, backSpeed, maskSize.y);
                    transform.localPosition = pos;
                }
                else if (type == Type.Page)
                {
                    crrTouchDelta = (Input.mousePosition - previousPos) * dragSpeed;
                    previousPos = Input.mousePosition;
                    Vector3 pos = transform.localPosition;
                    pos.x += crrTouchDelta.x;
                    transform.localPosition = pos;
                }
            }
        }

        void OnUp()
        {
            if (isTouching)
            {
                isTouching = false;
                if (type == Type.Page)
                {
                    if (Input.mousePosition.x - downPos.x > dragLengByScreenScale * Screen.width)
                    {
                        if (currPageIndex > 0)
                        {
                            currPageIndex--;
                            if (OnPageChangeEvent != null)
                            {
                                OnPageChangeEvent(currPageIndex);
                            }
                        }
                    }
                    else if (downPos.x - Input.mousePosition.x > dragLengByScreenScale * Screen.width)
                    {
                        if (currPageIndex < pageCount - 1)
                        {
                            currPageIndex++;
                            if (OnPageChangeEvent != null)
                            {
                                OnPageChangeEvent(currPageIndex);
                            }
                        }
                    }
                }
            }
        }

        float ScrollUpdate(float currPos, float moveDelta, float limitBegin, float limitEnd, float offset, bool isTouching, float backSpeed, float maskWidth)
        {
            if (!isTouching)
            {
                if (type == Type.FreeHorizontal)
                {
                    bool overBegin = (currPos > limitEnd);
                    bool overEnd = (currPos < limitBegin);
                    if (overBegin)
                    {
                        currPos += (limitEnd - currPos) * backSpeed;
                    }
                    else if (overEnd)
                    {
                        if (limitBegin > 0f)
                        {
                            currPos += (0f - currPos) * backSpeed;
                        }
                        else
                        {
                            currPos += (limitBegin - currPos) * backSpeed;
                        }
                    }
                }
                else if (type == Type.FreeVertical)
                {
                    bool overBegin = (currPos < limitEnd);
                    bool overEnd = (currPos > limitBegin);
                    if (overBegin)
                    {
                        currPos += (limitEnd - currPos) * backSpeed;
                    }
                    else if (overEnd)
                    {
                        if (limitBegin > 0f)
                        {
                            currPos += (limitBegin - currPos) * backSpeed;
                        }
                        else
                        {
                            currPos += (0f - currPos) * backSpeed;
                        }
                    }
                }
            }
            currPos += moveDelta;
            return currPos;
        }
    }
}