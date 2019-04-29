using UnityEngine;

namespace BToolkit
{
    public class TouchRotateUIByTwoFingers : MonoBehaviour
    {

        public bool testInEditor;
        int testDownNum;
        float previousAngle;
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
            if (firstPoint != Vector3.zero)
            {
                GUI.Box(new Rect(firstPoint.x - 25, Screen.height - firstPoint.y - 25, 50, 50), "");
            }
            if (secondPoint != Vector3.zero)
            {
                GUI.Box(new Rect(secondPoint.x - 25, Screen.height - secondPoint.y - 25, 50, 50), "");
            }
        }

        void OnePointUp()
        {
            previousAngle = 0f;
        }

        void TwoPointUpdate(Vector3 firstPos, Vector3 secondPos, bool secondFingerUp)
        {
            if (previousAngle == 0f)
            {
                previousAngle = Mathf.Atan2(secondPos.y - firstPos.y, secondPos.x - firstPos.x) * 180f / Mathf.PI;
            }
            else
            {
                float currAngle = Mathf.Atan2(secondPos.y - firstPos.y, secondPos.x - firstPos.x) * 180f / Mathf.PI;
                float deltaAngle = currAngle - previousAngle;
                previousAngle = currAngle;
                transform.localEulerAngles += new Vector3(0, 0, deltaAngle);
                if (secondFingerUp)
                {
                    previousAngle = 0f;
                }
            }
        }
    }
}