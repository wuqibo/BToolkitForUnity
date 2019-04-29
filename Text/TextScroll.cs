using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class TextScroll : MonoBehaviour
    {

        public Text text;
        public float limitMin = 0, limitMax = 200;
        bool canDrag;
        float previoursY, deltaY;

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                OnTouchMove(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnTouchUp();
            }
            if (!canDrag)
            {
                if (text.transform.localPosition.y < limitMin)
                {
                    text.transform.localPosition += (new Vector3(0, limitMin + 0.1f, 0) - text.transform.localPosition) * 0.5f;
                }
                else if (text.transform.localPosition.y > limitMax)
                {
                    text.transform.localPosition += (new Vector3(0, limitMax - 0.1f, 0) - text.transform.localPosition) * 0.5f;
                }
            }
        }

        internal void InitPos()
        {
            text.transform.localPosition = new Vector3(0, limitMin, 0);
        }

        public void OnTextTouchDown()
        {
            previoursY = Input.mousePosition.y;
            canDrag = true;
        }

        void OnTouchMove(Vector3 pos)
        {
            if (canDrag)
            {
                deltaY = pos.y - previoursY;
                previoursY = pos.y;
                text.transform.localPosition += new Vector3(0, deltaY, 0);
            }
        }
        void OnTouchUp()
        {
            canDrag = false;
        }
    }
}