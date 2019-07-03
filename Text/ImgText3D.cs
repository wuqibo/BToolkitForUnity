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
            public Transform trans;
            public GameObject go;
        }
        [System.Serializable]
        public class TextureConfig
        {
            public bool active { get; set; }
            public float meshscale = 1f;
            public float texscale = 1f;
            public float offset = 0f;
        }
        public float space = 1f;
        public bool center = true;
        public string colorProperty = "_TintColor";
        public string texProperty = "_MainTex";
        public TextureConfig[] zeroToNine = new TextureConfig[10];
        public List<Extra> extras = new List<Extra>();
        List<OneText> textPool = new List<OneText>();
        float[] charPoses = new float[20];
        MeshRenderer meshRenderer;
        string currText;
        GameObject prefab;
        float offsetX = 0, previousScale = 0;
        public string text
        {
            get
            {
                return currText;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                currText = value.Trim();
                int textLeng = currText.Length;
                int poolLength = textPool.Count;
                offsetX = 0;
                previousScale = 0;
                for (int i = 0; i < textLeng; i++)
                {
                    OneText oneText = GetOneTextFromPool(i);
                    if (!oneText.go.activeInHierarchy)
                    {
                        oneText.go.SetActive(true);
                    }
                    bool isExtra = false;
                    string str = currText.Substring(i, 1);
                    int extrasCount = extras.Count;
                    for (int j = 0; j < extrasCount; j++)
                    {
                        Extra extra = extras[j];
                        if (str.Equals(extra.text))
                        {
                            float x = GetCharPos(i, extra.config);
                            charPoses[i] = x;
                            oneText.trans.localPosition = new Vector3(x, 0, 0);
                            Vector3 meshScale = oneText.trans.localScale;
                            meshScale.x = extra.config.meshscale;
                            oneText.trans.localScale = meshScale;
                            oneText.material.SetTextureScale(texProperty, new Vector2(extra.config.texscale * 0.1f, 1));
                            oneText.material.SetTextureOffset(texProperty, new Vector2(extra.config.offset * 0.1f, 0));
                            isExtra = true;
                            break;
                        }
                    }
                    if (!isExtra)
                    {
                        TextureConfig textureConfig = zeroToNine[int.Parse(str)];
                        float x = GetCharPos(i, textureConfig);
                        charPoses[i] = x;
                        oneText.trans.localPosition = new Vector3(x, 0, 0);
                        Vector3 meshScale = oneText.trans.localScale;
                        meshScale.x = textureConfig.meshscale;
                        oneText.trans.localScale = meshScale;
                        oneText.material.SetTextureScale(texProperty, new Vector2(textureConfig.texscale * 0.1f, 1));
                        oneText.material.SetTextureOffset(texProperty, new Vector2(textureConfig.offset * 0.1f, 0));
                    }
                    oneText.material.SetColor(colorProperty, color);
                }
                if (center)
                {
                    float halfWidth = offsetX * 0.5f;
                    for (int i = 0; i < textLeng; i++)
                    {
                        OneText oneText = GetOneTextFromPool(i);
                        oneText.trans.localPosition -= new Vector3(halfWidth, 0, 0);
                    }
                }
                if (textLeng < poolLength)
                {
                    for (int i = textLeng; i < poolLength; i++)
                    {
                        OneText oneText = textPool[i];
                        if (oneText.go.activeInHierarchy)
                        {
                            oneText.go.SetActive(false);
                        }
                    }
                }
            }
        }

        float GetCharPos(int index, TextureConfig textureConfig)
        {
            if (index > 0)
            {
                offsetX += (previousScale * 0.5f + textureConfig.meshscale * 0.5f + space);
            }
            previousScale = textureConfig.meshscale;
            return offsetX;
        }

        public Color color
        {
            get
            {
                if (!meshRenderer)
                {
                    meshRenderer = GetComponent<MeshRenderer>();
                }
                return meshRenderer.material.GetColor(colorProperty);
            }
            set
            {
                meshRenderer.material.SetColor(colorProperty, value);
                if (textPool != null)
                {
                    int poolLength = textPool.Count;
                    for (int i = 0; i < poolLength; i++)
                    {
                        textPool[i].material.SetColor(colorProperty, value);
                    }
                }
            }
        }

        void OnDestroy()
        {
            zeroToNine = null;
            extras = null;
        }

        void Awake()
        {
            if (!meshRenderer)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }
            meshRenderer.enabled = false;
            Vector3 scale = transform.localScale;
            scale.x = scale.y;
            transform.localScale = scale;
        }

        OneText GetOneTextFromPool(int index)
        {
            OneText oneText = null;
            if (index < textPool.Count)
            {
                oneText = textPool[index];
            }
            else
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
                Destroy(oneText.go.GetComponent<ImgText3D>());
                oneText.trans = oneText.go.transform;
                oneText.trans.SetParent(transform, false);
                oneText.trans.localScale = Vector3.one;
                MeshRenderer meshRenderer = oneText.go.GetComponent<MeshRenderer>();
                meshRenderer.enabled = true;
                oneText.material = meshRenderer.material;
                textPool.Add(oneText);
            }
            return oneText;
        }
    }
}