using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BToolkit
{
    public class PhotosScroll : MonoBehaviour
    {

        class Item
        {
            public RectTransform trans;
            public Item(Object obj, Vector2 size)
            {
                if (obj.GetType() == typeof(Sprite))
                {
                    GameObject go = new GameObject("Image");
                    trans = go.AddComponent<RectTransform>();
                    Image image = go.AddComponent<Image>();
                    image.sprite = obj as Sprite;
                    if (size.x != 0 && size.y == 0)
                    {
                        Vector2 sizeDelta = size;
                        sizeDelta.y = size.x * image.sprite.rect.height / image.sprite.rect.width;
                        trans.sizeDelta = sizeDelta;
                    }
                    else if (size.x == 0 && size.y != 0)
                    {
                        Vector2 sizeDelta = size;
                        sizeDelta.x = size.y * image.sprite.rect.width / image.sprite.rect.height;
                        trans.sizeDelta = sizeDelta;
                    }
                    else if (size.x != 0 && size.y != 0)
                    {
                        trans.sizeDelta = size;
                    }
                    else
                    {
                        image.SetNativeSize();
                    }
                }
                else
                {
                    trans = (obj as Image).transform as RectTransform;
                }
            }
        }
        public string photosPath = "";
        public Image[] photos;
        public float itemSpace = 10;
        public Vector2 itemSize;
        public float threshold = 150f;
        public float dragSpeed = 1;
        public float resetSpeed = 10;
        public bool loop;
        public bool triggerAnyWhere = true;
        public System.Action<int> OnIndexChange;
        int previousIndex;
        Item[] items;
        Vector2[] currItemSizes;
        bool isLoadItems;
        [HideInInspector]
        public int itemsCount, currIndex;
        float previousX, previousXRemember;
        float[] poses;
        bool hasLoaded, isDraging;
        static CanvasScaler canvasScaler;

        void Awake()
        {
            if (!string.IsNullOrEmpty(photosPath))
            {
                StartCoroutine(LoadPhoto());
            }
            else if (photos.Length > 0)
            {
                Object[] objs = new Object[photos.Length];
                for (int i = 0; i < objs.Length; i++)
                {
                    objs[i] = photos[i];
                }
                SetPhotosPos(objs);
            }
            if (!triggerAnyWhere)
            {
                if (!GetComponent<UnityEngine.EventSystems.EventTrigger>())
                {
                    Debug.LogError("对象 " + gameObject.name + " 未勾选 triggerAnyWhere 属性，请添加 EventTrigger 组件并关联 OnTrigger 事件");
                }
            }
        }

        void Update()
        {
            if (hasLoaded)
            {
                if (!isDraging && items != null)
                {
                    items[0].trans.localPosition += (new Vector3(poses[currIndex], 0, 0) - items[0].trans.localPosition) * resetSpeed * Time.deltaTime;
                    ItemUpdate();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (triggerAnyWhere)
                    {
                        OnTouchDown(Input.mousePosition);
                    }
                }
                if (isDraging)
                {
                    if (Input.GetMouseButton(0))
                    {
                        OnTouchMove(Input.mousePosition);
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        OnTouchUp(Input.mousePosition);
                    }
                }
            }
        }

        IEnumerator LoadPhoto()
        {
            isLoadItems = true;
            Object[] objs = Resources.LoadAll(photosPath, typeof(Sprite));
            yield return 0;
            SetPhotosPos(objs);
        }

        public void SetPhotos(Image[] images)
        {
            Object[] objs = new Object[images.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i] = images[i];
            }
            SetPhotosPos(objs);
        }

        void SetPhotosPos(Object[] objs)
        {
            hasLoaded = false;
            if (objs != null)
            {
                if (objs.Length > 0)
                {
                    itemsCount = objs.Length;
                    poses = new float[itemsCount];
                    items = new Item[itemsCount];
                    currItemSizes = new Vector2[itemsCount];
                    for (int i = 0; i < itemsCount; i++)
                    {
                        items[i] = new Item(objs[i], itemSize);
                        if (isLoadItems)
                        {
                            items[i].trans.SetParent(transform, false);
                        }
                        currItemSizes[i] = items[i].trans.GetComponent<RectTransform>().sizeDelta;
                        items[i].trans.localPosition = new Vector3(i * (currItemSizes[i].x + itemSpace), 0, 0);
                        poses[i] = -items[i].trans.localPosition.x;
                    }
                    hasLoaded = true;
                    UpdateLayer();
                }
            }
        }

        /// <summary>
        /// 切换到下一张
        /// </summary>
        public void Next()
        {
            previousIndex = currIndex;
            currIndex++;
            if (currIndex > itemsCount - 1)
            {
                currIndex = itemsCount - 1;
            }
            if (currIndex != previousIndex && OnIndexChange != null)
            {
                previousIndex = currIndex;
                OnIndexChange(currIndex);
            }
            UpdateLayer();
        }

        /// <summary>
        /// 切换到上一张
        /// </summary>
        public void Prevoius()
        {
            previousIndex = currIndex;
            currIndex--;
            if (currIndex < 0)
            {
                currIndex = 0;
            }
            if (currIndex != previousIndex && OnIndexChange != null)
            {
                previousIndex = currIndex;
                OnIndexChange(currIndex);
            }
            UpdateLayer();
        }

        /// <summary>
        /// 直接跳到某张图
        /// </summary>
        public void TurnToIndex(int index, bool useAnim = true)
        {
            previousIndex = currIndex;
            currIndex = index;
            if (currIndex > itemsCount - 1)
            {
                currIndex = itemsCount - 1;
            }
            if (currIndex < 0)
            {
                currIndex = 0;
            }
            if (!useAnim)
            {
                items[0].trans.localPosition = new Vector3(poses[currIndex], 0, 0);
                ItemUpdate();
            }
            if (currIndex != previousIndex && OnIndexChange != null)
            {
                previousIndex = currIndex;
                OnIndexChange(currIndex);
            }
            UpdateLayer();
        }

        public void OnTrigger()
        {
            if (!triggerAnyWhere)
            {
                OnTouchDown(Input.mousePosition);
            }
        }

        void OnTouchDown(Vector3 pos)
        {
            if (hasLoaded)
            {
                isDraging = true;
                previousX = previousXRemember = ScreenToUGUIPoint(pos).x;
            }
        }

        void OnTouchMove(Vector3 pos)
        {
            if (hasLoaded)
            {
                Vector2 uiPos = ScreenToUGUIPoint(pos);
                float delta = (uiPos.x - previousX) * dragSpeed;
                previousX = uiPos.x;
                items[0].trans.localPosition += new Vector3(delta, 0, 0);
                ItemUpdate();
            }
        }

        void OnTouchUp(Vector3 pos)
        {
            if (isDraging)
            {
                bool canPrevoius = false, canNext = false;
                Vector2 uiPos = ScreenToUGUIPoint(pos);
                if (previousXRemember - uiPos.x > threshold)
                {
                    canNext = true;
                }
                else if (uiPos.x - previousXRemember > threshold)
                {
                    canPrevoius = true;
                }
                if (canNext)
                {
                    previousIndex = currIndex;
                    currIndex++;
                    if (currIndex > itemsCount - 1)
                    {
                        if (loop)
                        {
                            currIndex = 0;
                        }
                        else
                        {
                            currIndex = itemsCount - 1;
                        }
                    }
                    if (currIndex != previousIndex && OnIndexChange != null)
                    {
                        previousIndex = currIndex;
                        OnIndexChange(currIndex);
                    }
                    UpdateLayer();
                }
                else if (canPrevoius)
                {
                    previousIndex = currIndex;
                    currIndex--;
                    if (currIndex < 0)
                    {
                        if (loop)
                        {
                            currIndex = itemsCount - 1;
                        }
                        else
                        {
                            currIndex = 0;
                        }
                    }
                    if (currIndex != previousIndex && OnIndexChange != null)
                    {
                        previousIndex = currIndex;
                        OnIndexChange(currIndex);
                    }
                    UpdateLayer();
                }
                isDraging = false;
            }
        }

        void ItemUpdate()
        {
            for (int i = 1; i < itemsCount; i++)
            {
                Vector3 pos = items[i].trans.localPosition;
                pos.x = items[0].trans.localPosition.x + i * (currItemSizes[i].x + itemSpace);
                items[i].trans.localPosition = pos;
            }
        }

        void UpdateLayer()
        {
            if (currIndex >= 0 && currIndex < items.Length)
            {
                items[currIndex].trans.SetSiblingIndex(itemsCount);
            }
        }

        Vector2 ScreenToUGUIPoint(Vector3 screenPosition)
        {
            Vector2 screenUISize;
            if (!canvasScaler)
            {
                canvasScaler = GameObject.FindObjectOfType<CanvasScaler>();
            }
            if (canvasScaler.matchWidthOrHeight > 0.5f)
            {
                screenUISize = new Vector2(canvasScaler.referenceResolution.y * (float)Screen.width / (float)Screen.height, canvasScaler.referenceResolution.y);
            }
            else
            {
                screenUISize = new Vector2(canvasScaler.referenceResolution.x, canvasScaler.referenceResolution.x * (float)Screen.height / (float)Screen.width);
            }
            Vector2 p;
            p.x = screenUISize.x * (screenPosition.x - Screen.width * 0.5f) / (float)Screen.width;
            p.y = screenUISize.y * (screenPosition.y - Screen.height * 0.5f) / (float)Screen.height;
            return p;
        }
    }
}