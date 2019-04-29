using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BToolkit
{
    [RequireComponent(typeof(RectTransform))]
    public class UIParticle : MonoBehaviour
    {

        public bool playOnAwake = true;
        public int count = 20;
        public float lifeTimeMin = 0.2f, lifeTimeMax = 0.5f;
        public float startSpeedMin = 0f, startSpeedMax = 50f;
        public float gravity = 3;
        public int startAngleMin = 0, startAngleMax = 360;
        public float angleAddMin, angleAddMax;
        public float startScaleMin = 1f, startScaleMax = 1f;
        public float scaleAddMin, scaleAddMax;
        public Color startColor = Color.white;
        public Color endColor = Color.white;
        public bool loop = true;
        public Sprite sprite;
        public Material material;

        Item[] items = null;
        int currCount;

        class Item
        {
            public Image image;
            public RectTransform trans;
            float radian, speed, gv, angle, angleAdd, timer;
            Vector3 scaleAdd;
            UIParticle uiParticle;
            BMath.Line colorKB;
            bool active;
            public Item(UIParticle uiParticle)
            {
                this.uiParticle = uiParticle;
            }
            public void Show()
            {
                if (!trans.gameObject.activeInHierarchy)
                {
                    trans.gameObject.SetActive(true);
                }
            }
            public void Play()
            {
                trans.anchoredPosition = Vector3.zero;
                trans.localScale = Vector3.one * Random.Range(uiParticle.startScaleMin, uiParticle.startScaleMax);
                scaleAdd = Vector3.one * Random.Range(uiParticle.scaleAddMin, uiParticle.scaleAddMax);
                radian = Random.Range(0f, Mathf.PI * 2f);
                speed = Random.Range(uiParticle.startSpeedMin, uiParticle.startSpeedMax);
                gv = 0;
                timer = Random.Range(uiParticle.lifeTimeMin, uiParticle.lifeTimeMax);
                angle = Random.Range(uiParticle.startAngleMin, uiParticle.startAngleMax);
                trans.localEulerAngles = new Vector3(0, 0, angle);
                angleAdd = Random.Range(uiParticle.angleAddMin, uiParticle.angleAddMax);
                image.color = uiParticle.startColor;
                image.raycastTarget = false;
                colorKB = new BMath.Line(timer, 0f, 0f, 1f);
                active = true;
            }
            public void Update()
            {
                if (active)
                {
                    float deltaTime = Time.deltaTime * 30f;
                    Vector3 pos = trans.anchoredPosition;
                    pos.x += Mathf.Cos(radian) * speed * deltaTime;
                    pos.y += Mathf.Sin(radian) * speed * deltaTime;
                    pos.y -= gv * deltaTime;
                    gv += uiParticle.gravity * deltaTime;
                    trans.anchoredPosition = pos;
                    trans.localScale += scaleAdd * deltaTime;
                    Vector3 angle = trans.localEulerAngles;
                    angle.z += angleAdd * deltaTime;
                    trans.localEulerAngles = angle;
                    image.color = Color.Lerp(uiParticle.startColor, uiParticle.endColor, colorKB.k * timer + colorKB.b);
                    if (timer > 0f)
                    {
                        timer -= Time.deltaTime;
                        if (timer <= 0f)
                        {
                            if (uiParticle.loop)
                            {
                                Play();
                            }
                            else
                            {
                                active = false;
                                trans.gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }

        void OnEnable()
        {
            if (playOnAwake)
            {
                Play();
            }
        }

        public void Play()
        {
            if (items == null)
            {
                this.currCount = count;
                items = new Item[currCount];
                for (int i = 0; i < currCount; i++)
                {
                    items[i] = new Item(this);
                    items[i].trans = new GameObject("item", typeof(RectTransform)).GetComponent<RectTransform>();
                    items[i].trans.gameObject.AddComponent<CanvasRenderer>();
                    items[i].image = items[i].trans.gameObject.AddComponent<Image>();
                    if (material)
                    {
                        items[i].image.material = material;
                    }
                    else
                    {
                        items[i].image.sprite = sprite;
                    }
                    items[i].trans.SetParent(transform, false);
                }
            }
            for (int i = 0; i < currCount; i++)
            {
                items[i].Show();
                items[i].Play();
            }
        }

        void Update()
        {
            for (int i = 0; i < currCount; i++)
            {
                items[i].Update();
            }
        }

        void DestroyAllItems()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null && items[i].trans)
                {
                    Destroy(items[i].trans.gameObject);
                }
            }
        }
    }
}