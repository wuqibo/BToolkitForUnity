using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace BToolkit
{
    public class LayerMovingUGUI : MonoBehaviour
    {

        public bool isMainOfCurrLayer;
        public int layerIndex = 0;
        public LayerMovingUGUI backLayerMainObj;
        public bool closely, randomPos, randomScale;
        public float randomPosXMin = 0f;
        public float randomPosXMax = 1f;
        public float randomScaleMin = 0.8f;
        public float randomScaleMax = 1.2f;
        public float currSpeed;
        public float backLayerSpeed = 0.5f;
        LayerMovingUGUI[] currLayerAllObjs;
        LayerMovingUGUI currLayerMainObj;
        RectTransform rectTrans;
        float posPrevious, posPreviousForLocal;
        float screenWHalf;
        float wHalf;
        static CanvasScaler canvasScaler;
        Vector2 defaultPos;
        Vector3 defaultScale;

        void Awake()
        {
            rectTrans = GetComponent<RectTransform>();
            defaultPos = rectTrans.anchoredPosition;
            defaultScale = rectTrans.localScale;
            LayerMovingUGUI[] allLayers = GameObject.FindObjectsOfType<LayerMovingUGUI>();
            List<LayerMovingUGUI> allLayersSameLayer = new List<LayerMovingUGUI>();
            for (int i = 0; i < allLayers.Length; i++)
            {
                if (allLayers[i].layerIndex == layerIndex)
                {
                    allLayersSameLayer.Add(allLayers[i]);
                }
            }
            currLayerAllObjs = allLayersSameLayer.ToArray();
        }

        void Start()
        {
            if (!canvasScaler)
            {
                canvasScaler = GameObject.FindObjectOfType<CanvasScaler>();
            }
            screenWHalf = 0.5f * canvasScaler.referenceResolution.y * (float)Screen.width / (float)Screen.height;
            wHalf = rectTrans.sizeDelta.x * 0.5f * rectTrans.localScale.x;
            if (isMainOfCurrLayer)
            {
                posPrevious = rectTrans.anchoredPosition.x;
                for (int i = 0; i < currLayerAllObjs.Length; i++)
                {
                    currLayerAllObjs[i].closely = closely;
                    currLayerAllObjs[i].randomPos = randomPos;
                    currLayerAllObjs[i].randomScale = randomScale;
                    currLayerAllObjs[i].randomPosXMin = randomPosXMin;
                    currLayerAllObjs[i].randomPosXMax = randomPosXMax;
                    currLayerAllObjs[i].randomScaleMin = randomScaleMin;
                    currLayerAllObjs[i].randomScaleMax = randomScaleMax;
                    currLayerAllObjs[i].currLayerMainObj = this;
                }
            }
            posPreviousForLocal = rectTrans.anchoredPosition.x;
        }

        void Update()
        {
            //主对象移动/带动本层其他对象/带动后层主对象
            if (isMainOfCurrLayer)
            {
                if (posPrevious != rectTrans.anchoredPosition.x)
                {
                    currSpeed = rectTrans.anchoredPosition.x - posPrevious;
                    posPrevious = rectTrans.anchoredPosition.x;
                    if (Mathf.Abs(currSpeed) < screenWHalf)
                    {
                        //带动本层其他对象移动
                        for (int i = 0; i < currLayerAllObjs.Length; i++)
                        {
                            LayerMovingUGUI obj = currLayerAllObjs[i];
                            obj.currSpeed = currSpeed;
                            if (!obj.isMainOfCurrLayer)
                            {
                                obj.rectTrans.anchoredPosition += new Vector2(obj.currSpeed, 0f);
                            }
                        }
                        //带动后层移动
                        if (backLayerMainObj)
                        {
                            backLayerMainObj.rectTrans.anchoredPosition += new Vector2(currSpeed * backLayerSpeed, 0f);
                        }
                    }
                }
            }
            //每个自己变换位置
            if (posPreviousForLocal != rectTrans.anchoredPosition.x)
            {
                if (rectTrans.anchoredPosition.x < posPreviousForLocal)
                {
                    //向左移并超出屏幕时
                    if (rectTrans.anchoredPosition.x < -screenWHalf - wHalf)
                    {
                        if (Mathf.Abs(posPreviousForLocal - rectTrans.anchoredPosition.x) < screenWHalf)
                        {
                            ChangeToScreenRightSide();
                        }
                    }
                }
                else
                {
                    //向右移并超出屏幕时
                    if (rectTrans.anchoredPosition.x > screenWHalf + wHalf)
                    {
                        if (Mathf.Abs(posPreviousForLocal - rectTrans.anchoredPosition.x) < screenWHalf)
                        {
                            ChangeToScreenLeftSide();
                        }
                    }
                }
                posPreviousForLocal = rectTrans.anchoredPosition.x;
            }
        }

        void ChangeToScreenRightSide()
        {
            float newX = 0f;
            if (closely)
            {
                LayerMovingUGUI rightObj = GetRightObj();
                newX = rightObj.rectTrans.anchoredPosition.x + rightObj.wHalf + wHalf - 2f;
            }
            else
            {
                newX = screenWHalf + wHalf;
                if (randomPos)
                {
                    newX = screenWHalf + wHalf + Random.Range(randomPosXMin, randomPosXMax);
                }
                if (randomScale)
                {
                    float scale = Random.Range(randomScaleMin, randomScaleMax);
                    rectTrans.localScale = new Vector3(scale, scale, 0f);
                }
            }
            rectTrans.anchoredPosition = new Vector2(newX, rectTrans.anchoredPosition.y);
        }

        void ChangeToScreenLeftSide()
        {
            float newX = 0f;
            if (closely)
            {
                LayerMovingUGUI leftObj = GetLeftObj();
                newX = leftObj.rectTrans.anchoredPosition.x - leftObj.wHalf - wHalf + 2f;
            }
            else
            {
                newX = -screenWHalf - wHalf;
                if (randomPos)
                {
                    newX = -screenWHalf - wHalf - Random.Range(randomPosXMin, randomPosXMax);
                }
                if (randomScale)
                {
                    float scale = Random.Range(randomScaleMin, randomScaleMax);
                    rectTrans.localScale = new Vector3(scale, scale, 0f);
                }
            }
            rectTrans.anchoredPosition = new Vector2(newX, rectTrans.anchoredPosition.y);
        }

        LayerMovingUGUI GetLeftObj()
        {
            LayerMovingUGUI uiSceneryLayerMove = currLayerMainObj.currLayerAllObjs[0];
            float tempX = uiSceneryLayerMove.rectTrans.anchoredPosition.x;
            int count = currLayerMainObj.currLayerAllObjs.Length;
            for (int i = 0; i < count; i++)
            {
                LayerMovingUGUI _uiSceneryLayerMove = currLayerMainObj.currLayerAllObjs[i];
                if (_uiSceneryLayerMove.rectTrans.anchoredPosition.x < tempX)
                {
                    tempX = _uiSceneryLayerMove.rectTrans.anchoredPosition.x;
                    uiSceneryLayerMove = _uiSceneryLayerMove;
                }
            }
            return uiSceneryLayerMove;
        }

        LayerMovingUGUI GetRightObj()
        {
            LayerMovingUGUI uiSceneryLayerMove = currLayerMainObj.currLayerAllObjs[0];
            float tempX = uiSceneryLayerMove.rectTrans.anchoredPosition.x;
            int count = currLayerMainObj.currLayerAllObjs.Length;
            for (int i = 0; i < count; i++)
            {
                LayerMovingUGUI _uiSceneryLayerMove = currLayerMainObj.currLayerAllObjs[i];
                if (_uiSceneryLayerMove.rectTrans.anchoredPosition.x > tempX)
                {
                    tempX = _uiSceneryLayerMove.rectTrans.anchoredPosition.x;
                    uiSceneryLayerMove = _uiSceneryLayerMove;
                }
            }
            return uiSceneryLayerMove;
        }

        public void Init()
        {
            rectTrans.anchoredPosition = defaultPos;
            posPrevious = posPreviousForLocal = rectTrans.anchoredPosition.x;
            rectTrans.localScale = defaultScale;
        }
    }
}