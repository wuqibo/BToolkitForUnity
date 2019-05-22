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
            if (!IsSoundOn)
            {
                return null;
            }
            return CreatePlayerAndPlay(delay, clip, volume, OnStopCallback);
        }

        /// <summary>
        /// 停止播放用ID标记过的声音
        /// </summary>
        public void Stop()
        {
            if (OnStopCallback != null)
            {
                OnStopCallback();
            }
            Destroy(gameObject);
        }

        static SoundPlayer CreatePlayerAndPlay(float delay, AudioClip clip, float volume, Action OnStopCallback)
        {
            if (clip)
            {
                GameObject go = new GameObject("SoundPlayer");
                DontDestroyOnLoad(go);
                SoundPlayer soundPlayer = go.AddComponent<SoundPlayer>();
                soundPlayer.destroyTimer = delay + clip.length;
                soundPlayer.audioSource = go.AddComponent<AudioSource>();
                soundPlayer.audioSource.spatialBlend = 0f;
                soundPlayer.audioSource.clip = clip;
                soundPlayer.audioSource.playOnAwake = false;
                soundPlayer.audioSource.loop = false;
                soundPlayer.audioSource.volume = volume;
                soundPlayer.OnStopCallback = OnStopCallback;
                soundPlayer.audioSource.PlayDelayed(delay);
                return soundPlayer;
            }
            return null;
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
                    gameObject.name = "ReadyToDestroy";
                    Destroy(gameObject);
                    if (OnStopCallback != null)
                    {
                        OnStopCallback();
                    }
                }
            }
        }
    }
}