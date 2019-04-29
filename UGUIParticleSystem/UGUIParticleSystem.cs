using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System;

namespace BToolkit.UGUIParticle
{
    [AddComponentMenu("BToolkit/UGUI Particle System")]
    [RequireComponent(typeof(RectTransform))]
    public class UGUIParticleSystem:MonoBehaviour
    {
        /////////////////////////////////////////////////////////
        public bool hadInitSettings;
        public bool isMainOpen = true;
        public float duration = 1;
        public bool looping = true;
        public float startDelay;
        public FloatValue startLifetime, startSpeed, startScale, startRotation;
        public ColorValue startColor;
        public float gravityModifier;
        public Space space;
        public bool playOnAwake = true;
        public int maxParticles = 100;
        public Emission emission;
        public Shape shape;
        public Wind wind;
        public ColorOverLifetime colorOverLifetime;
        public ScaleOverLifetime scaleOverLifetime;
        public RotationOverLifetime rotationOverLifetime;
        public Renderer mRenderer;
        /////////////////////////////////////////////////////////
        public List<Particle> particles = new List<Particle>();
        public int particlesCount;
        public bool isPlaying;
        float lastTime, deltaTime, timer, delay;
        public float Time_deltaTime { get { return (Application.isPlaying ? Time.deltaTime : deltaTime); } }
        RectTransform _trans;
        public RectTransform trans { get { return _trans ?? (_trans = transform as RectTransform); } }
        public Action ClearEvent;
        UGUIParticleSystem parentSystem, childSystem;

        public void Play()
        {
            Clear();
            timer = 0;
            emission.timer = 0;
            delay = startDelay;
            isPlaying = true;
            if (childSystem)
            {
                childSystem.Play();
            }
        }

        public void Stop()
        {
            isPlaying = false;
            if (childSystem)
            {
                childSystem.Stop();
            }
        }

        public void Clear()
        {
            Stop();
            Particle[] allParticles = FindObjectsOfType<Particle>();
            for(int i = 0;i < allParticles.Length;i++)
            {
                DestroyImmediate(allParticles[i].gameObject);
            }
            particles.Clear();
            particlesCount = 0;
            if (childSystem)
            {
                childSystem.Clear();
            }
        }

        void Awake()
        {
            SetParentAndChild();
            if (playOnAwake)
            {
                Play();
            }
        }

        public void SetParentAndChild()
        {
            parentSystem = transform.parent.GetComponent<UGUIParticleSystem>();
            if (parentSystem)
            {
                parentSystem.childSystem = this;
            }
        }

        public void UpdateOnEditMode()
        {
#if UNITY_EDITOR
            try
            {
                if(!Application.isPlaying && Selection.activeGameObject == gameObject)
                {
                    if(gameObject.activeInHierarchy && this.enabled)
                    {
                        SetEditModeDeltaTime();
                        Update();
                    }
                }
            }
            catch { }
#endif
        }

        void Update()
        {
            if(isPlaying)
            {
                if(delay > 0f)
                {
                    delay -= Time_deltaTime;
                    return;
                }
                if(emission.isUse)
                {
                    emission.timer -= Time_deltaTime;
                    if(emission.timer < 0f)
                    {
                        emission.timer = 1 / emission.rate;
                        Particle particle = GetParticleFromPool();
                        if(particle != null)
                        {
                            particle.Init();
                        }
                    }
                }
                timer += Time_deltaTime;
                if(!looping)
                {
                    if(timer >= duration)
                    {
                        isPlaying = false;
                    }
                }
            }
        }

        void SetEditModeDeltaTime()
        {
            deltaTime = 0;
            float currTime = Time.realtimeSinceStartup;
            if(lastTime != 0)
            {
                deltaTime = currTime - lastTime;
            }
            lastTime = currTime;
        }

        Particle GetParticleFromPool()
        {
            for(int i = 0;i < particlesCount;i++)
            {
                Particle p = particles[i];
                if(!p.active)
                {
                    return p;
                }
            }
            if(particlesCount < maxParticles)
            {
                GameObject go = new GameObject("uiparticle");
                Particle newParticle = go.AddComponent<Particle>();
                particles.Add(newParticle);
                particlesCount = particles.Count;
                newParticle.OnSpawn(this);
                return newParticle;
            }
            return null;
        }

    }
}