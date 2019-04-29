using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    [RequireComponent(typeof(Image))]
    public class ImgText : MonoBehaviour
    {

        [System.Serializable]
        public class Extra
        {
            public string text;
            public Sprite sprite;
        }
        class OneText
        {
            public Image image;
            public RectTransform rectTrans;
            public GameObject go;
            public float x;
        }
        const string FontName = "text";
        public float space;
        public bool center = true;
        public Sprite[] zeroToNine = new Sprite[10];
        public Extra[] extras = null;
        [Header("长度大于某个值则自动缩小")]
        public bool useAutoScale;
        public int scaleOneLeng = 8;
        public int anotherLeng = 10;
        public float anotherScale = 0.8f;
        public RectTransform rectTransform { get { return transform as RectTransform; } }
        Vector3 defaultScale;
        List<OneText> textPool = new List<OneText>();
        int extrasCount;
        string currText;
        Image currImage;
        KB kb;
        bool hasFindOutExistFont;
        public Material material
        {
            get
            {
                if (textPool.Count > 0)
                {
                    return textPool[0].image.material;
                }
                return null;
            }
            set
            {
                int poolLength = textPool.Count;
                for (int i = 0; i < poolLength; i++)
                {
                    OneText oneText = textPool[i];
                    if (oneText.image.material != value)
                    {
                        oneText.image.material = value;
                    }
                }
            }
        }
        public string text
        {
            get
            {
                return currText;
            }
            set
            {
                if (!hasFindOutExistFont)
                {
                    foreach (Transform child in transform)
                    {
                        if (FontName.Equals(child.name))
                        {
                            OneText ot = new OneText();
                            ot.image = child.GetComponent<Image>();
                            ot.rectTrans = child as RectTransform;
                            ot.go = child.gameObject;
                            textPool.Add(ot);
                        }
                    }
                    hasFindOutExistFont = true;
                }
                if (!currImage)
                {
                    currImage = GetComponent<Image>();
                    currImage.enabled = false;
                }
                int poolLength = textPool.Count;
                for (int i = 0; i < poolLength; i++)
                {
                    textPool[i].go.SetActive(false);
                }
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                currText = value.Trim();
                int textLeng = currText.Length;
                if (useAutoScale)
                {
                    if (kb == null)
                    {
                        defaultScale = transform.localScale;
                        kb = new KB(scaleOneLeng, 1f, anotherLeng, anotherScale);
                    }
                    transform.localScale = defaultScale * (textLeng >= scaleOneLeng ? (kb.k * textLeng + kb.b) : 1);
                }
                extrasCount = extras.Length;
                OneText prevousOneText = null;
                float totalWidth = 0;
                float firstOneTextHalfW = 0;
                for (int i = 0; i < textLeng; i++)
                {
                    OneText oneText = GetOneTextFromPool();
                    oneText.go.SetActive(true);
                    bool isExtra = false;
                    string str = currText.Substring(i, 1);
                    for (int j = 0; j < extrasCount; j++)
                    {
                        if (str.Equals(extras[j].text))
                        {
                            oneText.image.sprite = extras[j].sprite;
                            isExtra = true;
                            break;
                        }
                    }
                    if (!isExtra)
                    {
                        try
                        {
                            oneText.image.sprite = zeroToNine[int.Parse(str)];
                        }
                        catch { }
                    }
                    oneText.image.SetNativeSize();
                    oneText.x = 0;
                    if (prevousOneText == null)
                    {
                        firstOneTextHalfW = oneText.rectTrans.sizeDelta.x * 0.5f;
                    }
                    else
                    {
                        oneText.x = prevousOneText.x + prevousOneText.rectTrans.sizeDelta.x * 0.5f + space + oneText.rectTrans.sizeDelta.x * 0.5f;
                    }
                    prevousOneText = oneText;
                    oneText.image.color = currColor;
                    totalWidth += oneText.rectTrans.sizeDelta.x;
                    if (i > 0)
                    {
                        totalWidth += space;
                    }
                }
                float offset = -totalWidth * 0.5f + firstOneTextHalfW;
                poolLength = textPool.Count;
                for (int i = 0; i < poolLength; i++)
                {
                    OneText oneText = textPool[i];
                    if (center)
                    {
                        oneText.x += offset;
                    }
                    oneText.rectTrans.anchoredPosition = new Vector2(oneText.x, 0);
                }
            }
        }
        Color currColor = Color.white;
        public Color color
        {
            get
            {
                return currColor;
            }
            set
            {
                currColor = value;
                if (textPool != null)
                {
                    int poolLength = textPool.Count;
                    for (int i = 0; i < poolLength; i++)
                    {
                        textPool[i].image.color = currColor;
                    }
                }
            }
        }

        void OnDestroy()
        {
            zeroToNine = null;
            extras = null;
        }

        OneText GetOneTextFromPool()
        {
            OneText oneText = null;
            int poolLength = textPool.Count;
            for (int i = 0; i < poolLength; i++)
            {
                OneText _oneText = textPool[i];
                if (!_oneText.go.activeSelf)
                {
                    oneText = _oneText;
                }
            }
            if (oneText == null)
            {
                oneText = new OneText();
                oneText.go = new GameObject("text");
                oneText.rectTrans = oneText.go.AddComponent<RectTransform>();
                oneText.rectTrans.SetParent(transform, false);
                oneText.image = oneText.go.AddComponent<Image>();
                textPool.Add(oneText);
            }
            return oneText;
        }

        // 求一元一次方程的k,b
        class KB
        {
            public float k, b;
            public KB(float x1, float y1, float x2, float y2)
            {
                k = (y2 - y1) / (x2 - x1);
                b = y1 - k * x1;
            }
        }
    }
}