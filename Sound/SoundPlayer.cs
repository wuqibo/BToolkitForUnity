using UnityEngine;
using System;

namespace BToolkit
{
    public class SoundPlayer : MonoBehaviour
    {
        static bool canReFindAllPlayers = true;
        static SoundPlayer[] allSoundPlayers;
        static int allSoundPlayersCount;
        const string SAVE_KEY = "SOUND_SAVE_KEY";
        AudioSource audioSource;
        float destroyTimer;
        Action OnStopCallback;
        /// <summary>
        /// 读取或设置全场使用SoundPlayer播放的所有音频的开关
        /// </summary>
        public static bool IsSoundOn
        {
            get { return PlayerPrefs.GetInt(SAVE_KEY, 1) == 1; }
            set
            {
                PlayerPrefs.SetInt(SAVE_KEY, value ? 1 : 0);
                if (canReFindAllPlayers)
                {
                    allSoundPlayers = GameObject.FindObjectsOfType<SoundPlayer>();
                    allSoundPlayersCount = allSoundPlayers.Length;
                    canReFindAllPlayers = false;
                }
                for (int i = 0; i < allSoundPlayersCount; i++)
                {
                    allSoundPlayers[i].audioSource.enabled = value;
                }
            }
        }

        static float _volume = 1;
        /// <summary>
        /// 读取或设置全场使用SoundPlayer播放的所有音频的音量
        /// </summary>
        public static float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                if (canReFindAllPlayers)
                {
                    allSoundPlayers = GameObject.FindObjectsOfType<SoundPlayer>();
                    allSoundPlayersCount = allSoundPlayers.Length;
                    canReFindAllPlayers = false;
                }
                for (int i = 0; i < allSoundPlayersCount; i++)
                {
                    allSoundPlayers[i].audioSource.volume = value;
                }
            }
        }

        public static SoundPlayer PlayAndDestroy(float delay, AudioClip clip, Action OnStopCallback = null)
        {
            return PlayAndDestroy(delay, clip, 1f, OnStopCallback);
        }

        public static SoundPlayer PlayAndDestroy(float delay, AudioClip clip, float volume, Action OnStopCallback = null)
        {
            if (!IsSoundOn || !clip)
            {
                return null;
            }
            SoundPlayer soundPlayer = CreatePlayer();
            soundPlayer.destroyTimer = delay + clip.length;
            soundPlayer.Play(delay, clip, volume, OnStopCallback);
            return soundPlayer;
        }

        /// <summary>
        /// 创建一个声音播放器，播完不会自动销毁，需调用Destroy()方法销毁
        /// </summary>
        /// <returns></returns>
        public static SoundPlayer CreatePlayer()
        {
            GameObject go = new GameObject("SoundPlayer");
            DontDestroyOnLoad(go);
            SoundPlayer soundPlayer = go.AddComponent<SoundPlayer>();
            soundPlayer.audioSource = go.AddComponent<AudioSource>();
            return soundPlayer;
        }

        void OnDestroy()
        {
            canReFindAllPlayers = true;
        }

        void Awake()
        {
            canReFindAllPlayers = true;
        }

        public void Play(float delay, AudioClip clip, Action OnStopCallback = null)
        {
            Play(delay, clip, 1f, OnStopCallback);
        }

        public void Play(float delay, AudioClip clip, float volume, Action OnStopCallback = null)
        {
            if (!IsSoundOn || !clip)
            {
                return;
            }
            audioSource.clip = clip;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = volume;
            this.OnStopCallback = OnStopCallback;
            audioSource.PlayDelayed(delay);
        }

        /// <summary>
        /// 停止播放并销毁
        /// </summary>
        public void Destroy()
        {
            Destroy(gameObject);
            if (OnStopCallback != null)
            {
                OnStopCallback();
            }
        }

        void Update()
        {
            if (destroyTimer > 0f)
            {
                destroyTimer -= Time.deltaTime;
                if (destroyTimer <= 0f)
                {
                    Destroy();
                }
            }
        }
    }
}