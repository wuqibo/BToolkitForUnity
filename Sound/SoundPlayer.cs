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

        /// <summary>
        /// 连续调用该方法时，如果playerId一样，则isCover表示之前声音未播放完成时是否覆盖播放
        /// </summary>
        public static SoundPlayer Play(float delay, AudioClip clip, Action OnStopCallback = null)
        {
            return Play(delay, clip, 1f, OnStopCallback);
        }

        /// <summary>
        /// 连续调用该方法时，如果playerId一样，则isCover表示之前声音未播放完成时是否覆盖播放
        /// </summary>
        public static SoundPlayer Play(float delay, AudioClip clip, float volume, Action OnStopCallback = null)
        {
            if (!IsSoundOn || !clip)
            {
                return null;
            }
            GameObject go = new GameObject("SoundPlayer");
            DontDestroyOnLoad(go);
            SoundPlayer soundPlayer = go.AddComponent<SoundPlayer>();
            soundPlayer.Play2(delay, clip, volume, OnStopCallback);
            return soundPlayer;
        }

        public void Play2(float delay, AudioClip clip, Action OnStopCallback = null)
        {
            Play2(delay, clip, 1f, OnStopCallback);
        }

        public void Play2(float delay, AudioClip clip, float volume, Action OnStopCallback = null)
        {
            if (!IsSoundOn || !clip)
            {
                return;
            }
            destroyTimer = delay + clip.length;
            audioSource.clip = clip;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = volume;
            this.OnStopCallback = OnStopCallback;
            audioSource.PlayDelayed(delay);
        }

        /// <summary>
        /// 停止播放用ID标记过的声音
        /// </summary>
        public void Stop()
        {
            Destroy(gameObject);
            if (OnStopCallback != null)
            {
                OnStopCallback();
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                allSoundPlayers = new SoundPlayer[0];
                allSoundPlayersCount = 0;
                canReFindAllPlayers = true;
            }
            if (destroyTimer > 0f)
            {
                destroyTimer -= Time.deltaTime;
                if (destroyTimer <= 0f)
                {
                    Stop();
                }
            }
        }
    }
}