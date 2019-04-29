using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class AlphaLoop:MonoBehaviour
    {
        public bool randomStart;
        public float minAlpha = 0f;
        public float maxAlpha = 1f;
        public float speed = 5;
        float alpha, r;
        Material[] materials;
        string[] propertyNames;
        int renderersCount;
        Image[] images;
        int imgCount;

        void Awake()
        {
            MeshRenderer[] mMeshRenderer = GetComponentsInChildren<MeshRenderer>();
            renderersCount = mMeshRenderer.Length;
            if(renderersCount > 0)
            {
                materials = new Material[renderersCount];
                propertyNames = new string[renderersCount];
                for(int i = 0;i < renderersCount;i++)
                {
                    materials[i] = mMeshRenderer[i].material;
                    if(materials[i].HasProperty("_Color"))
                    {
                        propertyNames[i] = "_Color";
                    }
                    else
                    {
                        propertyNames[i] = "_TintColor";
                    }
                }
            }
            images = GetComponentsInChildren<Image>();
            imgCount = images.Length;
            if(randomStart)
            {
                r = Random.Range(0,Mathf.PI * 2f);
            }
        }

        void Update()
        {
            r += speed * Time.deltaTime;
            alpha = minAlpha + (Mathf.Sin(r) * 0.5f + 0.5f) * (maxAlpha - minAlpha);
            SetAlpha(alpha);
        }

        internal void SetStartAlpha(float a)
        {
            if(a == 0)
            {
                r = Mathf.PI * 3f / 2f;
            }
            else
            {
                r = Mathf.PI / 2f;
            }
            SetAlpha(a);
        }

        void SetAlpha(float a)
        {
            if(imgCount > 0)
            {
                for(int i = 0;i < imgCount;i++)
                {
                    Color color = images[i].color;
                    color.a = alpha;
                    images[i].color = color;
                }
            }
            else if(renderersCount > 0)
            {
                for(int i = 0;i < renderersCount;i++)
                {
                    Color color = materials[i].GetColor(propertyNames[i]);
                    color.a = alpha;
                    materials[i].SetColor(propertyNames[i],color);
                }
            }
        }
    }
}