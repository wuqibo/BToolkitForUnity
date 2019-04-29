using UnityEngine;

namespace BToolkit
{
    public class Hinge2D : MonoBehaviour
    {

        public bool isHead;
        public Hinge2D target;
        public float guidOffset;
        public bool useLimit;
        public float angleLimit = 180;
        Hinge2D parent;
        RectTransform rectTrans;
        float radius1, radius2;
        float leng1, leng2;
        Vector2 drawPoint1, drawPoint2, point1, point2;

        void Awake()
        {
            Vector2 pos = GetPos();
            point1 = drawPoint1 = pos;
            point2 = drawPoint2 = pos + new Vector2(0, -guidOffset);
            leng1 = (pos - pos + new Vector2(0, -guidOffset)).magnitude;
            if (target)
            {
                leng2 = (pos + new Vector2(0, -guidOffset) - target.GetPos()).magnitude;
            }
        }

        Vector2 GetPos()
        {
            GetTrans();
            if (rectTrans)
            {
                return rectTrans.anchoredPosition;
            }
            return transform.position;
        }

        void GetTrans()
        {
            rectTrans = GetComponent<RectTransform>();
            if (target)
            {
                if (!target.transform)
                {
                    target.rectTrans = target.GetComponent<RectTransform>();
                }
                if (!target.parent)
                {
                    target.parent = this;
                }
            }
            radius1 = (transform.localScale.x + transform.localScale.y + transform.localScale.z) * 0.008f;
            radius2 = (transform.localScale.x + transform.localScale.y + transform.localScale.z) * 0.005f;
            if (rectTrans)
            {
                radius1 *= 100;
                radius2 *= 100;
            }
            if (guidOffset == 0)
            {
                if (rectTrans)
                {
                    guidOffset = rectTrans.sizeDelta.y * 0.4f * transform.localScale.y;
                }
                else
                {
                    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                    if (spriteRenderer && spriteRenderer.sprite)
                    {
                        guidOffset = 0.005f * spriteRenderer.sprite.texture.height * 0.4f * transform.localScale.y;
                    }
                    else
                    {
                        guidOffset = 0.005f * transform.localScale.y;
                    }
                }
            }
        }

        void Update()
        {
            if (isHead)
            {
                point1 = GetPos();
            }
            point2 = Follow(leng1, point1, point2, 1);
            if (target)
            {
                target.point1 = Follow(leng2, point2, target.point1, 2);
            }
            if (rectTrans)
            {
                rectTrans.anchoredPosition = point1;
            }
            else
            {
                transform.position = point1;
            }
            float angleZ = Mathf.Atan2(point2.y - point1.y, point2.x - point1.x) * 180f / Mathf.PI + 90f;
            transform.eulerAngles = new Vector3(0, 0, angleZ);
        }

        void OnDrawGizmos()
        {
            GetTrans();
            Gizmos.color = Color.red;
            if (!Application.isPlaying)
            {
                drawPoint1 = GetPos();
                drawPoint2 = GetPos() + new Vector2(0, -guidOffset);
            }
            else
            {
                drawPoint1 = point1;
                drawPoint2 = point2;
            }
            Vector2 rectTransOffset = Vector2.zero;
            float canvasScale = 1f;
            if (rectTrans)
            {
                rectTransOffset = new Vector2(BUtils.ScreenUISize.x * 0.5f, BUtils.ScreenUISize.y * 0.5f);
                Canvas canvas = GameObject.FindObjectOfType<Canvas>();
                if (canvas)
                {
                    canvasScale = canvas.transform.localScale.x;
                }
            }
            Gizmos.DrawSphere((drawPoint1 + rectTransOffset) * canvasScale, radius1);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere((drawPoint2 + rectTransOffset) * canvasScale, radius2);
            //连线
            if (target)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f);
                Gizmos.DrawLine((drawPoint2 + rectTransOffset) * canvasScale, (target.drawPoint1 + rectTransOffset) * canvasScale);
            }
        }

        Vector2 Follow(float maxDis, Vector2 _point1, Vector2 _point2, int pointNum)
        {
            if (Vector2.Distance(_point1, _point2) != maxDis)
            {
                float radian = Mathf.Atan2(_point2.y - _point1.y, _point2.x - _point1.x);
                if (useLimit)
                {
                    if (pointNum == 1)
                    {
                        if (parent)
                        {
                            Vector3 dir1 = point1 - parent.point2;
                            Vector3 dir2 = point2 - point1;
                            Vector3 dir3 = point2 - parent.point2;
                            if (Vector3.Angle(dir1, dir2) > angleLimit)
                            {
                                if (Vector3.Cross(dir1, dir3).z > 0f)
                                {
                                    radian = Mathf.Atan2(point1.y - parent.point2.y, point1.x - parent.point2.x) + angleLimit * Mathf.PI / 180f;
                                }
                                else
                                {
                                    radian = Mathf.Atan2(point1.y - parent.point2.y, point1.x - parent.point2.x) - angleLimit * Mathf.PI / 180f;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (target)
                        {
                            Vector3 dir1 = point2 - point1;
                            Vector3 dir2 = target.point1 - point2;
                            Vector3 dir3 = target.point1 - point1;
                            if (Vector3.Angle(dir1, dir2) > angleLimit)
                            {
                                if (Vector3.Cross(dir1, dir3).z > 0f)
                                {
                                    radian = Mathf.Atan2(point2.y - point1.y, point2.x - point1.x) + angleLimit * Mathf.PI / 180f;
                                }
                                else
                                {
                                    radian = Mathf.Atan2(point2.y - point1.y, point2.x - point1.x) - angleLimit * Mathf.PI / 180f;
                                }
                            }
                        }
                    }
                }
                _point2.x = _point1.x + Mathf.Cos(radian) * maxDis;
                _point2.y = _point1.y + Mathf.Sin(radian) * maxDis;
            }
            return _point2;
        }
    }
}