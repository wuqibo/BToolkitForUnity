using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BToolkit
{
    public class FramesPlayer : MonoBehaviour
    {

        public Sprite[] sprites;
        public bool autoPlay = true;
        public bool loop = true;
        public int fps = 12;
        public bool randomFirstFrame;
        public AudioClip sound = null;
        public float soundVolume = 1;
        SpriteRenderer spriteRenderer;
        Image image;
        float timer, time;
        int currIndex;
        AudioSource player;
        public delegate void Delegate();
        public Delegate OnPlayEnd;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            time = 1f / (float)fps;
            if (autoPlay)
            {
                if (randomFirstFrame)
                {
                    Play(Random.Range(0, sprites.Length));
                }
                else
                {
                    Play();
                }
            }
            else
            {
                Stop();
            }
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= time)
            {
                timer = 0f;
                currIndex++;
                if (currIndex > sprites.Length - 1)
                {
                    if (!loop)
                    {
                        Stop();
                        if (OnPlayEnd != null)
                        {
                            OnPlayEnd();
                        }
                        return;
                    }
                    else
                    {
                        currIndex = 0;
                    }
                }
                if (spriteRenderer)
                {
                    spriteRenderer.sprite = sprites[currIndex];
                }
                else if (image)
                {
                    image.sprite = sprites[currIndex];
                }
            }
        }

        public void Pause()
        {
            this.enabled = false;
        }

        public void Stop()
        {
            this.enabled = false;
            timer = 0f;
        }

        public void Play()
        {
            Play(null);
        }

        public void Play(Delegate OnPlayEnd)
        {
            this.OnPlayEnd = OnPlayEnd;
            Play(0);
        }

        public void Play(int startFrameIndex)
        {
            currIndex = startFrameIndex;
            if (spriteRenderer)
            {
                spriteRenderer.sprite = sprites[currIndex];
                spriteRenderer.enabled = true;
            }
            else if (image)
            {
                image.sprite = sprites[currIndex];
                image.enabled = true;
            }
            timer = 0f;
            this.enabled = true;
            PlaySound();
        }

        public bool IsPlaying
        {
            get
            {
                return this.enabled;
            }
        }

        void PlaySound()
        {
            if (sound)
            {
                player = GetComponent<AudioSource>();
                if (!player)
                {
                    player = gameObject.AddComponent<AudioSource>();
                    player.loop = false;
                    player.playOnAwake = false;
                }
                if (player.clip != sound)
                {
                    player.clip = sound;
                }
                player.volume = soundVolume;
                player.Play();
            }
        }

        public void SetAlpha(float alpha)
        {
            if (spriteRenderer)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
            else if (image)
            {
                Color color = image.color;
                color.a = alpha;
                image.color = color;
            }
        }
    }
}