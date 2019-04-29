#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace BToolkit.UGUIParticle
{
    public class Particle:MonoBehaviour
    {
        public bool active { private set; get; }
        RectTransform trans;
        Image img;
        UGUIParticleSystem uiParticleSystem;
        float lifetimeTotal, lifetime, radian, speed, g;
        Vector2 wind;
        Vector3 startScale;
        float angularVelovity;
        int frameIndex, frameCount;
        float frameTimer;
        float frameInterval;

        void Delete()
        {
#if UNITY_EDITOR
            EditorApplication.update -= UpdateOnEditMode;
#endif
            uiParticleSystem.ClearEvent -= OnClear;
            if(uiParticleSystem.particles.Contains(this))
            {
                uiParticleSystem.particles.Remove(this);
                uiParticleSystem.particlesCount = uiParticleSystem.particles.Count;
            }
            try
            {
                if(gameObject)
                {
                    if(Application.isPlaying)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        DestroyImmediate(gameObject);
                    }
                }
            }
            catch { }
        }

        public void OnSpawn(UGUIParticleSystem particleSystem)
        {
            this.uiParticleSystem = particleSystem;
            this.uiParticleSystem.ClearEvent += OnClear;
#if UNITY_EDITOR
            EditorApplication.update += UpdateOnEditMode;
#endif
            img = gameObject.AddComponent<Image>();
            img.raycastTarget = false;
            trans = transform as RectTransform;
            Init();
        }

        void OnClear()
        {
            if(!active)
            {
                Delete();
            }
        }

        public void Init()
        {
            //位置
            if(uiParticleSystem.space == Space.Local)
            {
                if(trans.parent != uiParticleSystem.transform)
                {
                    trans.SetParent(uiParticleSystem.transform,false);
                }
                if(!uiParticleSystem.shape.isUse)
                {
                    trans.anchoredPosition = Vector2.zero;
                    radian = 0f;
                }
                else
                {
                    if(uiParticleSystem.shape.type == ShapeType.Circle)
                    {
                        float r = uiParticleSystem.shape.radius;
                        trans.anchoredPosition = new Vector2(Random.Range(-r,r),Random.Range(-r,r));
                        float radianHalf = uiParticleSystem.shape.arc * 0.5f;
                        radian = Random.Range(-radianHalf,radianHalf) * Mathf.Deg2Rad;
                    }
                    else
                    {
                        Vector2 size = uiParticleSystem.shape.size * 0.5f;
                        trans.anchoredPosition = new Vector2(Random.Range(-size.x,size.x),Random.Range(-size.y,size.y));
                    }
                }
            }
            else
            {
                if(trans.parent != uiParticleSystem.transform.parent)
                {
                    trans.SetParent(uiParticleSystem.transform.parent,false);
                }
                if(!uiParticleSystem.shape.isUse)
                {
                    trans.anchoredPosition3D = uiParticleSystem.trans.anchoredPosition3D;
                    radian = 0f;
                }
                else
                {
                    if(uiParticleSystem.shape.type == ShapeType.Circle)
                    {
                        float r = uiParticleSystem.shape.radius;
                        trans.anchoredPosition3D = uiParticleSystem.trans.anchoredPosition3D + new Vector3(Random.Range(-r,r),Random.Range(-r,r));
                        float radianHalf = uiParticleSystem.shape.arc * 0.5f;
                        radian = Random.Range(-radianHalf,radianHalf) * Mathf.Deg2Rad - uiParticleSystem.trans.localEulerAngles.z * Mathf.Deg2Rad;
                    }
                    else
                    {
                        radian = uiParticleSystem.trans.localEulerAngles.z * Mathf.Deg2Rad;
                        Vector2 size = uiParticleSystem.shape.size * 0.5f;
                        //未旋转的点
                        Vector3 pos = uiParticleSystem.trans.anchoredPosition3D + new Vector3(Random.Range(-size.x,size.x),Random.Range(-size.y,size.y));
                        //旋转后的点
                        float dis = Vector2.Distance(pos,uiParticleSystem.trans.anchoredPosition3D);
                        float originRadian = Mathf.Atan2(pos.y - uiParticleSystem.trans.anchoredPosition3D.y,pos.x - uiParticleSystem.trans.anchoredPosition3D.x);
                        float newRadian = originRadian + radian;
                        pos.x = uiParticleSystem.trans.anchoredPosition3D.x + Mathf.Cos(newRadian) * dis;
                        pos.y = uiParticleSystem.trans.anchoredPosition3D.y + Mathf.Sin(newRadian) * dis;
                        trans.anchoredPosition3D = pos;
                        radian += Mathf.PI * 0.5f;
                    }
                }
            }
            speed = uiParticleSystem.startSpeed.curr;
            //缩放
            trans.localScale = startScale = Vector3.one * uiParticleSystem.startScale.curr;
            if(uiParticleSystem.scaleOverLifetime.isUse)
            {
                trans.localScale = startScale * uiParticleSystem.scaleOverLifetime.curve.Evaluate(0);
            }
            //初始角度
            trans.localEulerAngles = new Vector3(0,0,uiParticleSystem.startRotation.curr);
            //颜色
            img.color = uiParticleSystem.startColor.curr;
            if(uiParticleSystem.colorOverLifetime.isUse)
            {
                img.color = uiParticleSystem.colorOverLifetime.gradient.Evaluate(0);
            }
            //重力
            g = 0;
            //风
            wind = uiParticleSystem.wind.curr;
            //旋转
            angularVelovity = uiParticleSystem.rotationOverLifetime.curr;
            //渲染
            if(!uiParticleSystem.mRenderer.textureSheetAnimation)
            {
                if(img.sprite != uiParticleSystem.mRenderer.sprite)
                {
                    img.sprite = uiParticleSystem.mRenderer.sprite;
                    img.SetNativeSize();
                }
            }
            else
            {
                frameCount = uiParticleSystem.mRenderer.sprites.Length;
                frameIndex = Random.Range(0,frameCount);
                frameTimer = 0;
                frameInterval = 1 / (float)uiParticleSystem.mRenderer.fps;
                img.sprite = uiParticleSystem.mRenderer.sprites[frameIndex];
                img.SetNativeSize();
            }
            if(img.material != uiParticleSystem.mRenderer.material)
            {
                img.material = uiParticleSystem.mRenderer.material;
            }
            //生命周期
            lifetimeTotal = lifetime = uiParticleSystem.startLifetime.curr;
            img.enabled = true;
            active = true;
        }

        void UpdateOnEditMode()
        {
            if(!Application.isPlaying)
            {
                Update();
            }
        }

        void Update()
        {
            if(!uiParticleSystem)
            {
                Delete();
            }
            if(!img)
            {
#if UNITY_EDITOR
                EditorApplication.update -= UpdateOnEditMode;
#endif
                return;
            }
            if(active)
            {
                float speedDeltaTime = uiParticleSystem.Time_deltaTime * 60;
                float timeProgress = (lifetimeTotal - lifetime) / lifetimeTotal;
                //位置
                if(speed != 0)
                {
                    Vector2 pos = (uiParticleSystem.space == Space.Local) ? trans.anchoredPosition : new Vector2(trans.anchoredPosition3D.x,trans.anchoredPosition3D.y);
                    if(uiParticleSystem.shape.type == ShapeType.Circle)
                    {
                        pos.x += Mathf.Sin(radian) * speed * speedDeltaTime;
                        pos.y += Mathf.Cos(radian) * speed * speedDeltaTime;
                    }
                    else
                    {
                        if(uiParticleSystem.space == Space.Local)
                        {
                            pos.y += speed * speedDeltaTime;
                        }
                        else
                        {
                            pos.x += Mathf.Cos(radian) * speed * speedDeltaTime;
                            pos.y += Mathf.Sin(radian) * speed * speedDeltaTime;
                        }
                    }
                    if(uiParticleSystem.space == Space.Local)
                    {
                        trans.anchoredPosition = pos;
                    }
                    else
                    {
                        trans.anchoredPosition3D = pos;
                    }
                }
                //重力
                if(uiParticleSystem.gravityModifier != 0)
                {
                    Vector2 pos = trans.anchoredPosition3D;
                    g += uiParticleSystem.gravityModifier * uiParticleSystem.Time_deltaTime;
                    pos.y -= g;
                    trans.anchoredPosition3D = pos;
                }
                //风
                if(uiParticleSystem.wind.isUse)
                {
                    Vector2 pos = trans.anchoredPosition3D;
                    pos += wind * speedDeltaTime;
                    trans.anchoredPosition3D = pos;
                }
                //颜色
                if(uiParticleSystem.colorOverLifetime.isUse)
                {
                    if(uiParticleSystem.colorOverLifetime.gradient == null)
                    {
                        uiParticleSystem.colorOverLifetime.gradient = new Gradient();
                    }
                    img.color = uiParticleSystem.colorOverLifetime.gradient.Evaluate(timeProgress);
                }
                //缩放
                if(uiParticleSystem.scaleOverLifetime.isUse)
                {
                    trans.localScale = startScale * uiParticleSystem.scaleOverLifetime.curve.Evaluate(timeProgress);
                }
                //旋转
                if(uiParticleSystem.rotationOverLifetime.isUse)
                {
                    Vector3 angle = trans.localEulerAngles;
                    angle.z += angularVelovity * uiParticleSystem.Time_deltaTime;
                    trans.localEulerAngles = angle;
                }
                //渲染
                if(uiParticleSystem.mRenderer.textureSheetAnimation && frameCount > 0)
                {
                    frameTimer += uiParticleSystem.Time_deltaTime;
                    if(frameTimer >= frameInterval)
                    {
                        frameTimer = 0f;
                        frameIndex++;
                        if(frameIndex > frameCount - 1)
                        {
                            frameIndex = 0;
                        }
                        if(frameIndex > uiParticleSystem.mRenderer.sprites.Length - 1)
                        {
                            Delete();
                            return;
                        }
                        img.sprite = uiParticleSystem.mRenderer.sprites[frameIndex];
                        img.SetNativeSize();
                    }
                }
                //生命周期
                lifetime -= uiParticleSystem.Time_deltaTime;
                if(lifetime <= 0f)
                {
                    img.enabled = false;
                    active = false;
                    if(!uiParticleSystem.isPlaying)
                    {
                        Delete();
                        if(uiParticleSystem.ClearEvent != null)
                        {
                            uiParticleSystem.ClearEvent();
                        }
                    }
                }
            }
        }
    }
}