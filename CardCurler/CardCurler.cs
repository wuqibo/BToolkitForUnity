using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    [RequireComponent(typeof(BButton))]
    public class CardCurler:MonoBehaviour
    {
        public bool alwaysTurnToVertical = true;
        public Sprite backSprite, contentSprite, shadowSprite;
        public BButton btnRotate;
        public AudioClip turnOverSound;
        internal RectTransform rectTrans, currParnet;
        internal RectTransform mask, backClone, contentClone, shadow;
        internal Vector2 topLeft, bottomLeft, topRight, bottomRight;
        internal Vector2 symmetryPoint;
        internal float cardWidth, cardHeight;
        internal BMath.Line lineL, lineR, lineT, lineB;
        internal Rect rectB, rectL, rectR, rectT;
        internal bool IsTransverse { get { return transform.localEulerAngles.z == 90 || transform.localEulerAngles.z == 270; } }
        internal Action OnBtnRotateClick, OnRotateFinished;
        internal Action OnStartTurnOver, OnTouchDownCurl, OnCurlingUpdate, OnTouchUpCurl, OnTurnOverSucceed;
        internal bool hadAnalogTouchDown, isAnalogDraging;
        internal Vector2 analogDragPos;
        internal CardCurler_Method currMethod;
        internal bool isCurling;
        Image maskImg, backCloneImg, contentImg, shadowImg;
        Transform btnRotateParent;
        bool canTouchCurl, hadTriggerStartTurnOverEvent, canTriggerOnStartCurlEvent, hadTurnOverSucceed;

        void Awake()
        {
            rectTrans = transform as RectTransform;
            BButton bButton = GetComponent<BButton>();
            bButton.triggerMethod = BButton.TriggerMethod.Down;
            bButton.canTriggerInterval = 0;
            bButton.onTrigger.AddListener(OnTouchDown);
            if(btnRotate)
            {
                btnRotateParent = btnRotate.transform.parent;
                btnRotate.canTriggerInterval = 0f;
                btnRotate.onTrigger.AddListener(() =>
                {
                    StartCoroutine(RotateCardTo(rectTrans));
                    if(OnBtnRotateClick != null)
                    {
                        OnBtnRotateClick();
                    }
                });
            }
            Init(true);
        }

        void Update()
        {
            if(currMethod != null)
            {
                currMethod.Update();
                if(isCurling)
                {
                    if(OnCurlingUpdate != null)
                    {
                        OnCurlingUpdate();
                    }
                    if(canTriggerOnStartCurlEvent)
                    {
                        if(OnTouchDownCurl != null)
                        {
                            OnTouchDownCurl();
                        }
                        canTriggerOnStartCurlEvent = false;
                    }
                }
            }
        }

        /// <summary>
        /// 外部拖拽翻牌（代码模拟而非触控或鼠标）,isDrag为false时表示已放开
        /// </summary>
        public void Drag(bool isDrag,Vector2 dragPos)
        {
            if(isDrag)
            {
                isAnalogDraging = true;
                this.analogDragPos = dragPos;
                if(!hadAnalogTouchDown)
                {
                    OnTouchDown(dragPos);
                    hadAnalogTouchDown = true;
                }
            }
            else
            {
                isAnalogDraging = false;
            }
        }

        /// <summary>
        /// 初始化卡片的各个位置关系,参数为true表示可触摸翻牌，false表示不可触摸，一般用于调用Drag()方法进行模拟自动播放
        /// </summary>
        public void Init(bool canTouchCurl)
        {
            this.canTouchCurl = canTouchCurl;
            hadTriggerStartTurnOverEvent = false;
            GetComponent<BButton>().enabled = canTouchCurl;
            GetComponent<Image>().sprite = backSprite;
            currParnet = rectTrans.parent as RectTransform;
            if(!IsTransverse)
            {
                topLeft = rectTrans.anchoredPosition + new Vector2(-rectTrans.sizeDelta.x * 0.5f,rectTrans.sizeDelta.y * 0.5f);
                topRight = rectTrans.anchoredPosition + new Vector2(rectTrans.sizeDelta.x * 0.5f,rectTrans.sizeDelta.y * 0.5f);
                bottomLeft = rectTrans.anchoredPosition + new Vector2(-rectTrans.sizeDelta.x * 0.5f,-rectTrans.sizeDelta.y * 0.5f);
                bottomRight = rectTrans.anchoredPosition + new Vector2(rectTrans.sizeDelta.x * 0.5f,-rectTrans.sizeDelta.y * 0.5f);
            }
            else
            {
                topLeft = rectTrans.anchoredPosition + new Vector2(-rectTrans.sizeDelta.y * 0.5f,rectTrans.sizeDelta.x * 0.5f);
                topRight = rectTrans.anchoredPosition + new Vector2(rectTrans.sizeDelta.y * 0.5f,rectTrans.sizeDelta.x * 0.5f);
                bottomLeft = rectTrans.anchoredPosition + new Vector2(-rectTrans.sizeDelta.y * 0.5f,-rectTrans.sizeDelta.x * 0.5f);
                bottomRight = rectTrans.anchoredPosition + new Vector2(rectTrans.sizeDelta.y * 0.5f,-rectTrans.sizeDelta.x * 0.5f);
            }
            cardWidth = bottomRight.x - bottomLeft.x;
            cardHeight = topLeft.y - bottomLeft.y;
            lineL = new BMath.Line(topLeft,bottomLeft);
            lineR = new BMath.Line(topRight,bottomRight);
            lineT = new BMath.Line(topLeft,topRight);
            lineB = new BMath.Line(bottomLeft,bottomRight);
            float rectSize = IsTransverse ? cardHeight * 0.35f : cardWidth * 0.35f;
            rectB = new Rect(bottomLeft.x,bottomLeft.y + rectSize,cardWidth,rectSize);
            rectL = new Rect(topLeft.x,topLeft.y - rectSize,rectSize,cardHeight - rectSize * 2);
            rectR = new Rect(topRight.x - rectSize,topRight.y - rectSize,rectSize,cardHeight - rectSize * 2);
            rectT = new Rect(topLeft.x,topLeft.y,cardWidth,rectSize);
        }

        //还原成盖着的状态
        internal void Resume()
        {
            GetComponent<Image>().enabled = true;
            if(btnRotate)
            {
                btnRotate.transform.SetParent(btnRotateParent,false);
                btnRotate.rectTransform.anchoredPosition = Vector2.zero;
                btnRotate.enabled = true;
            }
            if(mask)
            {
                Destroy(mask.gameObject);
            }
            currMethod = null;
        }

        //翻开成功
        public void TurnOverFinish()
        {
            DestroyClone();
            Image originCard = GetComponent<Image>();
            originCard.sprite = contentSprite;
            hadTurnOverSucceed = true;
            GetComponent<BButton>().enabled = false;
            if(OnTurnOverSucceed != null)
            {
                OnTurnOverSucceed();
            }
        }

        //翻开成功
        public void DestroyClone()
        {
            if(contentClone)
            {
                Destroy(contentClone.gameObject);
            }
            if(mask)
            {
                Destroy(mask.gameObject);
            }
            if(alwaysTurnToVertical)
            {
                rectTrans.localEulerAngles = Vector3.zero;
            }
            Image originCard = GetComponent<Image>();
            originCard.enabled = true;
            currMethod = null;
            GetComponent<BButton>().enabled = canTouchCurl;
        }

        //按下时执行一次(来自触控或鼠标)
        void OnTouchDown()
        {
            OnTouchDown(TouchPos());
        }

        //按下时执行一次
        void OnTouchDown(Vector2 touchPos)
        {
            if(hadTurnOverSucceed)
            {
                return;
            }
            if(TouchInRect(touchPos,rectB))
            {
                currMethod = new CardCurler_Method_Bottom(this);
            }
            else if(TouchInRect(touchPos,rectL))
            {
                currMethod = new CardCurler_Method_Left(this);
            }
            else if(TouchInRect(touchPos,rectR))
            {
                currMethod = new CardCurler_Method_Right(this);
            }
            else if(TouchInRect(touchPos,rectT))
            {
                currMethod = new CardCurler_Method_Top(this);
            }
            if(currMethod != null)
            {
                if(!mask)
                {
                    mask = new GameObject("Mask",typeof(Image),typeof(Mask)).transform as RectTransform;
                    mask.SetParent(transform.parent,false);
                    mask.GetComponent<Mask>().showMaskGraphic = false;
                    maskImg = mask.GetComponent<Image>();
                    maskImg.raycastTarget = false;
                    mask.sizeDelta = rectTrans.sizeDelta * 2f;
                    maskImg.enabled = false;

                    backClone = new GameObject("Back_Clone",typeof(Image)).transform as RectTransform;
                    backClone.SetParent(mask,false);
                    backClone.sizeDelta = rectTrans.sizeDelta;
                    backCloneImg = backClone.GetComponent<Image>();
                    backCloneImg.sprite = GetComponent<Image>().sprite;
                    backCloneImg.raycastTarget = false;
                    backCloneImg.enabled = false;

                    contentClone = new GameObject("Content_Clone",typeof(Image),typeof(Mask)).transform as RectTransform;
                    contentClone.SetParent(mask,false);
                    contentClone.GetComponent<Mask>().showMaskGraphic = true;
                    contentImg = contentClone.GetComponent<Image>();
                    contentImg.sprite = contentSprite;
                    contentImg.raycastTarget = false;
                    contentImg.enabled = false;
                    contentClone.sizeDelta = rectTrans.sizeDelta;

                    shadow = new GameObject("Shadow",typeof(Image)).transform as RectTransform;
                    shadow.SetParent(contentClone,false);
                    shadowImg = shadow.GetComponent<Image>();
                    shadowImg.sprite = shadowSprite;
                    shadowImg.raycastTarget = false;
                    shadowImg.enabled = false;
                    shadow.sizeDelta = new Vector2(cardHeight * 2f,100f);
                    shadow.pivot = new Vector2(0.5f,0.8f);
                }

                if(gameObject.activeInHierarchy)
                {
                    StartCoroutine(ShowContentDelay(maskImg,backCloneImg,contentImg,shadowImg));
                }

                if(btnRotate)
                {
                    btnRotate.transform.SetParent(backClone,false);
                    btnRotate.rectTransform.anchoredPosition = Vector2.zero;
                    btnRotate.enabled = false;
                }

                currMethod.SetTargetsPivot(touchPos);
                if(!hadTriggerStartTurnOverEvent)
                {
                    if(OnStartTurnOver != null)
                    {
                        OnStartTurnOver();
                    }
                    hadTriggerStartTurnOverEvent = true;
                }
                isCurling = true;
                canTriggerOnStartCurlEvent = true;
            }
        }

        //获取当前点击的位置
        internal Vector2 TouchPos()
        {
            return BUtils.ScreenToUGUIPoint(Input.mousePosition) - currParnet.anchoredPosition;
        }

        //判断当前点击在卡片哪一个部位，以此判断翻卡的方向
        bool TouchInRect(Vector2 touchPos,Rect rect)
        {
            if(touchPos.x > rect.x && touchPos.x < rect.x + rect.width)
            {
                if(touchPos.y < rect.y && touchPos.y > rect.y - rect.height)
                {
                    return true;
                }
            }
            return false;
        }

        //旋转到某个角度
        IEnumerator RotateCardTo(Transform target)
        {
            Vector3 toAngle = Vector3.zero;
            if(target.localEulerAngles.z == 0f)
            {
                toAngle = new Vector3(0,0,90f);
            }
            else if(target.localEulerAngles.z == 90f)
            {
                toAngle = new Vector3(0,0,180f);
            }
            else if(target.localEulerAngles.z == 180f)
            {
                toAngle = new Vector3(0,0,270f);
            }
            else if(target.localEulerAngles.z == 270f)
            {
                toAngle = new Vector3(0,0,360f);
            }
            if(btnRotate)
            {
                btnRotate.transform.localEulerAngles = new Vector3(0,0,-toAngle.z);
            }
            while(true)
            {
                target.localEulerAngles = Vector3.Lerp(target.localEulerAngles,toAngle,0.5f);
                if(Vector3.Distance(target.localEulerAngles,toAngle) < 0.1f)
                {
                    target.localEulerAngles = toAngle;
                    Init(canTouchCurl);
                    if(OnRotateFinished != null)
                    {
                        OnRotateFinished();
                    }
                    yield break;
                }
                yield return new WaitForSeconds(0.01f);
            }
        }

        //延迟两帧显示
        IEnumerator ShowContentDelay(Image maskImg,Image backCloneImg,Image contentImg,Image shadowImg)
        {
            yield return 1;
            GetComponent<Image>().enabled = false;
            maskImg.enabled = true;
            backCloneImg.enabled = true;
            contentImg.enabled = true;
            shadowImg.enabled = true;
        }
    }
}