using UnityEngine;
using UnityEngine.EventSystems;

namespace BToolkit
{
    public class DragUGUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool xEnable = true;
        public bool yEnable = true;
        public bool useLimit = false;
        public int minX = -960;
        public int maxX = 960;
        public int minY = -470;
        public int maxY = 470;
        protected bool isDraging;
        Vector2 _previousPos;
        RectTransform rectTrans;
        int _fingerId;
        Vector2 _screenPos, _currPos, _delta;

        protected virtual void Update()
        {
            if (isDraging)
            {
                if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
                {
                    _screenPos = Input.mousePosition;
                }
                else
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.fingerId == _fingerId)
                        {
                            _screenPos = touch.position;
                        }
                    }
                }
                Vector2 currPos = BUtils.ScreenToUGUIPoint(_screenPos);
                Vector2 delta = currPos - _previousPos;
                _previousPos = currPos;
                if (!rectTrans)
                {
                    rectTrans = transform as RectTransform;
                }
                Vector2 pos = rectTrans.anchoredPosition;
                if (xEnable)
                {
                    pos.x += delta.x;
                }
                if (yEnable)
                {
                    pos.y += delta.y;
                }
                if (useLimit)
                {
                    if (pos.x < minX)
                    {
                        pos.x = minX;
                    }
                    if (pos.x > maxX)
                    {
                        pos.x = maxX;
                    }
                    if (pos.y < minY)
                    {
                        pos.y = minY;
                    }
                    if (pos.y > maxY)
                    {
                        pos.y = maxY;
                    }
                }
                rectTrans.anchoredPosition = pos;
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            _previousPos = BUtils.ScreenToUGUIPoint(eventData.position);
            isDraging = true;
            _fingerId = eventData.pointerId;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            isDraging = false;
        }


    }
}