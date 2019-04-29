using System.Collections.Generic;
using UnityEngine;

namespace BToolkit
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ImgText3D : MonoBehaviour
    {

        [System.Serializable]
        public class Extra
        {
            public string text;
            public TextureConfig config;
        }
        class OneText
        {
            public Material material;
            public Transform transform;
            public GameObject go;
        }
        [System.Serializable]
        public class TextureConfig
        {
            public bool active;
            public float offset = 0f;
        }
        public float space = 1f;
        public bool center = true;
        public string colorProperty = "_TintColor";
        public string texProperty = "_MainTex";
        public float texScale = 1f;
        public TextureConfig[] zeroToNine = new TextureConfig[10];
        public List<Extra> extras = new List<Extra>();
        List<OneText> textPool = new List<OneText>();
        int extrasCount;
        string currText;
        GameObject prefab;
        public string text
        {
            get
            {
                return currText;
            }
            set
            {
                GetComponent<MeshRenderer>().enabled = false;
                currText = value.Trim();
                int poolLength = textPool.Count;
                for (int i = 0; i < poolLength; i++)
                {
                    textPool[i].go.SetActive(false);
                }
                if (string.IsNullOrEmpty(currText))
                {
                    return;
                }
                int textLeng = currText.Length;
                extrasCount = extras.Count;
                for (int i = 0; i < textLeng; i++)
                {
                    OneText oneText = GetOneTextFromPool();
                    oneText.go.SetActive(true);
                    float offsetX = i * space;
                    if (center)
                    {
                        offsetX -= space * textLeng * 0.5f - space * 0.5f;
                    }
                    oneText.transform.localPosition = new Vector3(offsetX, 0f, 0f);
                    bool isExtra = false;
                    string str = currText.Substring(i, 1);
                    for (int j = 0; j < extrasCount; j++)
                    {
                        if (str.Equals(extras[j].text))
                        {
                            oneText.material.SetTextureScale(texProperty, new Vector2(texScale * 0.1f, 1));
                            oneText.material.SetTextureOffset(texProperty, new Vector2(extras[j].config.offset * 0.1f, 0));
                            isExtra = true;
                            break;
                        }
                    }
                    if (!isExtra)
                    {
                        oneText.material.SetTextureScale(texProperty, new Vector2(texScale * 0.1f, 1));
                        oneText.material.SetTextureOffset(texProperty, new Vector2(zeroToNine[int.Parse(str)].offset * 0.1f, 0));
                    }
                    oneText.material.SetColor(colorProperty, currColor);
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
                        textPool[i].material.SetColor(colorProperty, currColor);
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
                if (!prefab)
                {
                    prefab = oneText.go = Instantiate(gameObject);
                }
                else
                {
                    oneText.go = Instantiate(prefab);
                }
                oneText.go.name = "text";
                Destroy(oneText.go.GetComponent<ImgText>());
                oneText.transform = oneText.go.transform;
                oneText.transform.SetParent(transform, false);
                oneText.transform.localScale = Vector3.one;
                MeshRenderer meshRenderer = oneText.go.GetComponent<MeshRenderer>();
                meshRenderer.enabled = true;
                oneText.material = meshRenderer.material;
                textPool.Add(oneText);
            }
            return oneText;
        }
    }
}