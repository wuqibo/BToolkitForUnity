using UnityEngine;

namespace BToolkit
{
    public class TouchScaleByTwoFingers : MonoBehaviour
    {

        public float speed = 1f;
        public float min = 0.2f;
        public float max = 3f;
        public bool testInEditor;
        int testDownNum;
        float previousDis;
        Vector3 firstPoint, secondPoint;

        void OnDisable()
        {
            if (Application.isEditor)
            {
                if (testInEditor)
                {
                    OnePointUp();
                    firstPoint = Vector3.zero;
                    secondPoint = Vector3.zero;
                }
            }
        }

        void Update()
        {
            if (Application.isEditor)
            {
                if (testInEditor)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        if (testDownNum == 0)
                        {
                            firstPoint = Input.mousePosition;
                        }
                    }
                    if (Input.GetMouseButton(1))
                    {
                        if (testDownNum > 0)
                        {
                            secondPoint = Input.mousePosition;
                            TwoPointUpdate(firstPoint, secondPoint, false);
                        }
                    }
                    if (Input.GetMouseButtonUp(1))
                    {
                        if (testDownNum == 0)
                        {
                            testDownNum++;
                        }
                        else
                        {
                            OnePointUp();
                            firstPoint = Vector3.zero;
                            secondPoint = Vector3.zero;
                            testDownNum = 0;
                        }
                    }
                }
            }
            else
            {
                if (Input.touchCount == 1)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        OnePointUp();
                    }
                }
                else if (Input.touchCount >= 2)
                {
                    TwoPointUpdate(Input.GetTouch(0).position, Input.GetTouch(1).position, Input.GetTouch(1).phase == TouchPhase.Ended);
                }
            }
        }

        void OnGUI()
        {
            if (Application.isEditor)
            {
                if (firstPoint != Vector3.zero)
                {
                    GUI.Box(new Rect(firstPoint.x - 25, Screen.height - firstPoint.y - 25, 50, 50), "");
                }
                if (secondPoint != Vector3.zero)
                {
                    GUI.Box(new Rect(secondPoint.x - 25, Screen.height - secondPoint.y - 25, 50, 50), "");
                }
            }
        }

        void OnePointUp()
        {
            previousDis = 0f;
        }

        void TwoPointUpdate(Vector3 firstPos, Vector3 secondPos, bool secondFingerUp)
        {
            Vector3 fromPos = new Vector3(firstPos.x / (float)Screen.width, firstPos.y / (float)Screen.height, 0);
            Vector3 toPos = new Vector3(secondPos.x / (float)Screen.width, secondPos.y / (float)Screen.height, 0);
            if (previousDis == 0f)
            {
                previousDis = (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)).magnitude;
            }
            else
            {
                float currDis = (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)).magnitude;
                float deltaScale = (currDis - previousDis) * 5f * speed;
                previousDis = currDis;
                transform.localScale += new Vector3(deltaScale, deltaScale, deltaScale);
                if (transform.localScale.x < min)
                {
                    transform.localScale = Vector3.one * min;
                }
                else if (transform.localScale.x > max)
                {
                    transform.localScale = Vector3.one * max;
                }
                if (secondFingerUp)
                {
                    previousDis = 0f;
                }
            }
        }
    }
}