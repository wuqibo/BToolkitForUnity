using UnityEngine;

namespace BToolkit
{
    public class CardCurler_Method_Bottom:CardCurler_Method
    {
        public CardCurler_Method_Bottom(CardCurler cardCurler) : base(cardCurler,Side.Bottom) { }

        public override void SetTargetsPivot(Vector2 touchPos)
        {
            cardCurler.mask.pivot = new Vector2(1,0.35f);
            cardCurler.contentClone.pivot = new Vector2(1 - (touchPos.x - cardCurler.bottomLeft.x) / cardCurler.cardWidth,0);
            if(cardCurler.IsTransverse)
            {
                cardCurler.contentClone.pivot = new Vector2(1,1 - (touchPos.x - cardCurler.bottomLeft.x) / cardCurler.cardWidth);
            }
            cardCurler.symmetryPoint = new Vector2(touchPos.x,cardCurler.bottomLeft.y);
            Update();
        }

        public override void Update()
        {
            switch(currAnimState)
            {
                case AnimState.Drag:
                    Vector2 touchPos = cardCurler.hadAnalogTouchDown ? cardCurler.analogDragPos : cardCurler.TouchPos();
                    dragPos = touchPos;
                    if(dragPos.y < cardCurler.bottomLeft.y + 1)
                    {
                        dragPos.y = cardCurler.bottomLeft.y + 1;
                    }
                    bool canLimitTopLeft = false;
                    if(touchPos.y < cardCurler.topLeft.y)
                    {
                        canLimitTopLeft = (touchPos.x < cardCurler.topLeft.x);
                    }
                    else
                    {
                        canLimitTopLeft = (touchPos.x < cardCurler.topLeft.x + cardCurler.cardWidth * 0.5f);
                    }
                    if(canLimitTopLeft)
                    {
                        if(Vector2.Distance(touchPos,cardCurler.topLeft) > (cardCurler.cardHeight - 50))
                        {
                            float r = Mathf.Atan2(touchPos.y - cardCurler.topLeft.y,touchPos.x - cardCurler.topLeft.x);
                            dragPos.x = cardCurler.topLeft.x + Mathf.Cos(r) * (cardCurler.cardHeight - 50);
                            dragPos.y = cardCurler.topLeft.y + Mathf.Sin(r) * (cardCurler.cardHeight - 50);
                        }
                    }
                    bool canLimitTopRight = false;
                    if(touchPos.y < cardCurler.topLeft.y)
                    {
                        canLimitTopRight = (touchPos.x > cardCurler.topRight.x);
                    }
                    else
                    {
                        canLimitTopRight = (touchPos.x > cardCurler.topLeft.x + cardCurler.cardWidth * 0.5f);
                    }
                    if(canLimitTopRight)
                    {
                        if(Vector2.Distance(touchPos,cardCurler.topRight) > (cardCurler.cardHeight - 50))
                        {
                            float r = Mathf.Atan2(touchPos.y - cardCurler.topRight.y,touchPos.x - cardCurler.topRight.x);
                            dragPos.x = cardCurler.topRight.x + Mathf.Cos(r) * (cardCurler.cardHeight - 50);
                            dragPos.y = cardCurler.topRight.y + Mathf.Sin(r) * (cardCurler.cardHeight - 50);
                        }
                    }
                    break;
                case AnimState.Resume:
                    dragPos = Vector2.MoveTowards(dragPos,cardCurler.symmetryPoint,cardCurler.cardWidth * 8f * Time.deltaTime);
                    if(Vector2.Distance(dragPos,cardCurler.symmetryPoint) == 0)
                    {
                        cardCurler.Resume();
                    }
                    break;
            }
            if(currAnimState != AnimState.TurnOver)
            {
                Vector2 centerLinePoint = BMath.GetCenterBetween2Points(dragPos,cardCurler.symmetryPoint);
                float centerLineRadian = Mathf.Atan2(cardCurler.symmetryPoint.y - dragPos.y,cardCurler.symmetryPoint.x - dragPos.x) - Mathf.PI * 0.5f;
                BMath.Line centerLine = new BMath.Line(centerLinePoint,centerLineRadian);
                leftPoint = BMath.GetCrossPointOf2Line(centerLine,cardCurler.lineL);
                rightPoint = BMath.GetCrossPointOf2Line(centerLine,cardCurler.lineR);

                if(leftPoint.y <= cardCurler.bottomLeft.y && rightPoint.y > cardCurler.bottomRight.y)
                {
                    leftPoint = BMath.GetCrossPointOf2Line(centerLine,cardCurler.lineB);
                }
                else if(leftPoint.y > cardCurler.bottomLeft.y && rightPoint.y <= cardCurler.bottomRight.y)
                {
                    rightPoint = BMath.GetCrossPointOf2Line(centerLine,cardCurler.lineB);
                }

                if(leftPoint.y >= cardCurler.topLeft.y && rightPoint.y < cardCurler.topRight.y)
                {
                    leftPoint = BMath.GetCrossPointOf2Line(centerLine,cardCurler.lineT);
                }
                else if(leftPoint.y < cardCurler.topLeft.y && rightPoint.y >= cardCurler.topRight.y)
                {
                    rightPoint = BMath.GetCrossPointOf2Line(centerLine,cardCurler.lineT);
                }

                if(cardCurler.mask)
                {
                    cardCurler.mask.anchoredPosition = leftPoint;
                    cardCurler.mask.localEulerAngles = new Vector3(0,0,Mathf.Atan2(rightPoint.y - leftPoint.y,rightPoint.x - leftPoint.x) * Mathf.Rad2Deg - 90);
                }
                if(cardCurler.backClone)
                {
                    cardCurler.backClone.SetParent(cardCurler.rectTrans.parent);
                    cardCurler.backClone.anchoredPosition = cardCurler.rectTrans.anchoredPosition;
                    cardCurler.backClone.localEulerAngles = cardCurler.rectTrans.localEulerAngles;
                    cardCurler.backClone.SetParent(cardCurler.mask,true);
                }
                if(cardCurler.contentClone)
                {
                    cardCurler.contentClone.SetParent(cardCurler.rectTrans.parent);
                    cardCurler.contentClone.anchoredPosition = dragPos;
                    cardCurler.contentClone.localEulerAngles = new Vector3(0,0,cardCurler.IsTransverse ? centerLine.radian * Mathf.Rad2Deg * 2f + 90 : centerLine.radian * Mathf.Rad2Deg * 2f + 180);
                    cardCurler.contentClone.SetParent(cardCurler.mask,true);
                }
                if(cardCurler.shadow)
                {
                    cardCurler.shadow.SetParent(cardCurler.rectTrans.parent);
                    Vector2 shadowPos = BMath.GetCenterBetween2Points(leftPoint,rightPoint);
                    cardCurler.shadow.anchoredPosition = shadowPos;
                    cardCurler.shadow.localEulerAngles = new Vector3(0,0,centerLine.radian * Mathf.Rad2Deg);
                    cardCurler.shadow.SetParent(cardCurler.contentClone,true);
                }
                bool canStop = false;
                if(cardCurler.hadAnalogTouchDown)
                {
                    canStop = !cardCurler.isAnalogDraging;
                }
                else
                {
                    canStop = Input.GetMouseButtonUp(0);
                }
                if(canStop)
                {
                    if(dragPos.y > cardCurler.topLeft.y)
                    {
                        cardCurler.contentClone.SetParent(cardCurler.rectTrans.parent,true);
                        //将锚点移回中心并确保卡片位置不变
                        ChangePivotToCenter(cardCurler.contentClone,dragPos,cardCurler.IsTransverse);
                        if(cardCurler.mask)
                        {
                            GameObject.Destroy(cardCurler.mask.gameObject);
                        }
                        if(cardCurler.shadow)
                        {
                            GameObject.Destroy(cardCurler.shadow.gameObject);
                        }
                        currAnimState = AnimState.TurnOver;
                        SoundPlayer.Play(0,cardCurler.turnOverSound);
                    }
                    else
                    {
                        currAnimState = AnimState.Resume;
                    }
                    if(cardCurler.OnTouchUpCurl != null)
                    {
                        cardCurler.OnTouchUpCurl();
                    }
                    cardCurler.isCurling = false;
                    cardCurler.hadAnalogTouchDown = false;
                }
            }
            else
            {
                Vector3 toAngle = new Vector3(0,0,cardCurler.IsTransverse ? 90 : 0);
                if(cardCurler.alwaysTurnToVertical)
                {
                    toAngle = Vector3.zero;
                }
                cardCurler.contentClone.localRotation = Quaternion.Lerp(cardCurler.contentClone.localRotation,Quaternion.Euler(toAngle),cardCurler.cardWidth * turnOverSpeed * Time.deltaTime);
                if(Vector2.Distance(cardCurler.contentClone.anchoredPosition,cardCurler.rectTrans.anchoredPosition) > 0.1f)
                {
                    cardCurler.contentClone.anchoredPosition = Vector2.Lerp(cardCurler.contentClone.anchoredPosition,cardCurler.rectTrans.anchoredPosition,cardCurler.cardWidth * turnOverSpeed * Time.deltaTime);
                }
                else
                {
                    cardCurler.TurnOverFinish();
                }
            }
        }

        /// <summary>
        /// 将锚点移回中心并确保卡片位置不变
        /// </summary>
        void ChangePivotToCenter(RectTransform target,Vector2 currDragPos,bool isTransverse)
        {
            if(!isTransverse)
            {
                Vector2 pivotPos = currDragPos;
                float r = target.localEulerAngles.z * Mathf.Deg2Rad;
                float l = (target.pivot.x - 0.5f) * cardCurler.cardWidth;
                pivotPos.x -= Mathf.Cos(r) * l;
                pivotPos.y -= Mathf.Sin(r) * l;
                r -= Mathf.PI * 0.5f;
                l = cardCurler.cardHeight * 0.5f;
                pivotPos.x -= Mathf.Cos(r) * l;
                pivotPos.y -= Mathf.Sin(r) * l;
                target.pivot = new Vector2(0.5f,0.5f);
                target.anchoredPosition = pivotPos;
                target.localEulerAngles += new Vector3(0,0,180);
            }
            else
            {
                Vector2 pivotPos = currDragPos;
                float r = (target.localEulerAngles.z + 90) * Mathf.Deg2Rad;
                float l = (target.pivot.y - 0.5f) * cardCurler.cardWidth;
                pivotPos.x -= Mathf.Cos(r) * l;
                pivotPos.y -= Mathf.Sin(r) * l;
                r -= Mathf.PI * 0.5f;
                l = cardCurler.cardHeight * 0.5f;
                pivotPos.x -= Mathf.Cos(r) * l;
                pivotPos.y -= Mathf.Sin(r) * l;
                target.pivot = new Vector2(0.5f,0.5f);
                target.anchoredPosition = pivotPos;
            }
        }
    }
}