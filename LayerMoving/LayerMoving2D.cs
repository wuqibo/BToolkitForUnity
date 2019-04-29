using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace BToolkit
{
    [ExecuteInEditMode]
    public class LayerMoving2D : MonoBehaviour
    {

        public bool isMainOfCurrLayer;
        public int layerIndex = 0;
        public LayerMoving2D backLayerMainObj;
        public bool closely, randomPos, randomScale;
        public float randomPosXMin = 0f;
        public float randomPosXMax = 1f;
        public float randomScaleMin = 0.8f;
        public float randomScaleMax = 1.2f;
        public float currSpeed;
        public float backLayerSpeed = 0.5f;
        LayerMoving2D[] currLayerAllObjs;
        LayerMoving2D currLayerMainObj;
        float posPrevious, posPreviousForLocal;
        float screenWHalf;
        float wHalf;
        Vector2 defaultPos;
        Vector3 defaultScale;

        void Awake()
        {
            defaultPos = transform.localPosition;
            defaultScale = transform.localScale;
            LayerMoving2D[] allLayers = GameObject.FindObjectsOfType<LayerMoving2D>();
            List<LayerMoving2D> allLayersSameLayer = new List<LayerMoving2D>();
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
            screenWHalf = ScreenWorldSides[2].x;
            float screenWorldRatio = ScreenWorldSides[1].y * 2f / 1000f;
            wHalf = GetComponent<SpriteRenderer>().sprite.textureRect.width * 0.5f * transform.localScale.x * screenWorldRatio;
            if (isMainOfCurrLayer)
            {
                posPrevious = transform.localPosition.x;
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
            posPreviousForLocal = transform.localPosition.x;
        }

        void Update()
        {
            //主对象移动/带动本层其他对象/带动后层主对象
            if (isMainOfCurrLayer)
            {
                if (posPrevious != transform.localPosition.x)
                {
                    currSpeed = transform.localPosition.x - posPrevious;
                    posPrevious = transform.localPosition.x;
                    if (Mathf.Abs(currSpeed) < screenWHalf)
                    {
                        //带动本层其他对象移动
                        for (int i = 0; i < currLayerAllObjs.Length; i++)
                        {
                            LayerMoving2D obj = currLayerAllObjs[i];
                            obj.currSpeed = currSpeed;
                            if (!obj.isMainOfCurrLayer)
                            {
                                obj.transform.localPosition += new Vector3(obj.currSpeed, 0f, 0f);
                            }
                        }
                        //带动后层移动
                        if (backLayerMainObj)
                        {
                            backLayerMainObj.transform.localPosition += new Vector3(currSpeed * backLayerSpeed, 0f, 0f);
                        }
                    }
                }
            }
            //每个自己变换位置
            if (posPreviousForLocal != transform.localPosition.x)
            {
                if (transform.localPosition.x < posPreviousForLocal)
                {
                    //向左移并超出屏幕时
                    if (transform.localPosition.x < -screenWHalf - wHalf)
                    {
                        if (Mathf.Abs(posPreviousForLocal - transform.localPosition.x) < screenWHalf)
                        {
                            ChangeToScreenRightSide();
                        }
                    }
                }
                else
                {
                    //向右移并超出屏幕时
                    if (transform.localPosition.x > screenWHalf + wHalf)
                    {
                        if (Mathf.Abs(posPreviousForLocal - transform.localPosition.x) < screenWHalf)
                        {
                            ChangeToScreenLeftSide();
                        }
                    }
                }
                posPreviousForLocal = transform.localPosition.x;
            }
#if UNITY_EDITOR
            //设置摄影机正交
            if (!Camera.main)
            {
                Debug.LogError("Must add a Camera with tag:'MainCamera'");
            }
            else
            {
                Camera.main.orthographic = true;
                Camera.main.orthographicSize = 5;
            }
#endif
        }

        void ChangeToScreenRightSide()
        {
            float newX = 0f;
            if (closely)
            {
                LayerMoving2D rightObj = GetRightObj();
                newX = rightObj.transform.localPosition.x + rightObj.wHalf + wHalf;
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
                    transform.localScale = new Vector3(scale, scale, 0f);
                }
            }
            transform.localPosition = new Vector2(newX, transform.localPosition.y);
        }

        void ChangeToScreenLeftSide()
        {
            float newX = 0f;
            if (closely)
            {
                LayerMoving2D leftObj = GetLeftObj();
                newX = leftObj.transform.localPosition.x - leftObj.wHalf - wHalf;
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
                    transform.localScale = new Vector3(scale, scale, 0f);
                }
            }
            transform.localPosition = new Vector2(newX, transform.localPosition.y);
        }

        LayerMoving2D GetLeftObj()
        {
            LayerMoving2D uiSceneryLayerMove = currLayerMainObj.currLayerAllObjs[0];
            float tempX = uiSceneryLayerMove.transform.localPosition.x;
            int count = currLayerMainObj.currLayerAllObjs.Length;
            for (int i = 0; i < count; i++)
            {
                LayerMoving2D _uiSceneryLayerMove = currLayerMainObj.currLayerAllObjs[i];
                if (_uiSceneryLayerMove.transform.localPosition.x < tempX)
                {
                    tempX = _uiSceneryLayerMove.transform.localPosition.x;
                    uiSceneryLayerMove = _uiSceneryLayerMove;
                }
            }
            return uiSceneryLayerMove;
        }

        LayerMoving2D GetRightObj()
        {
            LayerMoving2D uiSceneryLayerMove = currLayerMainObj.currLayerAllObjs[0];
            float tempX = uiSceneryLayerMove.transform.localPosition.x;
            int count = currLayerMainObj.currLayerAllObjs.Length;
            for (int i = 0; i < count; i++)
            {
                LayerMoving2D _uiSceneryLayerMove = currLayerMainObj.currLayerAllObjs[i];
                if (_uiSceneryLayerMove.transform.localPosition.x > tempX)
                {
                    tempX = _uiSceneryLayerMove.transform.localPosition.x;
                    uiSceneryLayerMove = _uiSceneryLayerMove;
                }
            }
            return uiSceneryLayerMove;
        }

        public void Init()
        {
            transform.localPosition = defaultPos;
            posPrevious = posPreviousForLocal = transform.localPosition.x;
            transform.localScale = defaultScale;
        }

        Vector3[] ScreenWorldSides
        {
            get
            {
                Vector3[] mSides = new Vector3[4];
                if (!Camera.main)
                {
                    Debug.LogError("Must be create Camera.main");
                    return mSides;
                }
                mSides[0] = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
                mSides[1] = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f));
                mSides[2] = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
                mSides[3] = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f));
                return mSides;
            }
        }
    }
}