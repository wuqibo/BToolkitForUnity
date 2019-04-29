using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class TouchMoveUIByOneFinger : MonoBehaviour
    {

        public bool testInEditor;
        RectTransform trans;
        Vector2 previousPos;
        Vector2 uiToScreenRatio;

        void Awake()
        {
            trans = transform as RectTransform;
            CanvasScaler canvasScaler = FindObjectOfType<CanvasScaler>();
            if (canvasScaler.matchWidthOrHeight > 0.5f)
            {
                Vector2 screenUISize = new Vector2(canvasScaler.referenceResolution.y * (float)Screen.width / (float)Screen.height, canvasScaler.referenceResolution.y);
                uiToScreenRatio = new Vector2(screenUISize.x / (float)Screen.width, screenUISize.y / (float)Screen.height);
            }
            else
            {
                Vector2 screenUISize = new Vector2(canvasScaler.referenceResolution.x, canvasScaler.referenceResolution.x * (float)Screen.height / (float)Screen.width);
                uiToScreenRatio = new Vector2(screenUISize.x / (float)Screen.width, screenUISize.y / (float)Screen.height);
            }
        }

        void Update()
        {
            if (Application.isEditor)
            {
                if (testInEditor)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        OnDown(Input.mousePosition);
                    }
                    if (Input.GetMouseButton(0))
                    {
                        OnDrag(Input.mousePosition);
                    }
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        OnDown(touch.position);
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        OnDrag(touch.position);
                    }
                }
            }
        }

        void OnDown(Vector2 screenPos)
        {
            previousPos = screenPos;
        }

        void OnDrag(Vector2 screenPos)
        {
            Vector2 deltaPos = screenPos - previousPos;
            previousPos = screenPos;
            trans.anchoredPosition += new Vector2(deltaPos.x * uiToScreenRatio.x, deltaPos.y * uiToScreenRatio.y);
        }
    }
}