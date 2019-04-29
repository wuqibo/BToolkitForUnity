using UnityEngine;

namespace BToolkit
{
    public class MusicPlayer
    {
        const string SAVE_KEY = "MUSIC_SAVE_KEY";
        static AudioSource player = null;
        static AudioClip[] currClips;
        static bool isPlayingWhenMusicOff;
        static float volume = 1;
        static int currClipIndex;
        static bool currRandomBegin, currRandomNext;
        static float currNextDelay;

        /// <summary>
        /// 读取或设置MusicPlayer播放的音频的开关
        /// </summary>
        public static bool IsMusicOn
        {
            get { return PlayerPrefs.GetInt(SAVE_KEY,1) == 1; }
            set
            {
                PlayerPrefs.SetInt(SAVE_KEY,value ? 1 : 0);
                if(player)
                {
                    player.enabled = value;
                    if(isPlayingWhenMusicOff)
                    {
                        player.Play();
                    }
                }
            }
        }
        /// <summary>
        /// 读取或设置MusicPlayer播放的音频的音量
        /// </summary>
        public static float Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                if(player)
                {
                    player.volume = value;
                }
            }
        }

        public static void Play(float delay,AudioClip[] clips,bool randomBegin,bool randomNext,float nextDelay)
        {
            if(currClips != null)
            {
                currClips = new AudioClip[1];
            }
            currClips = clips;
            currRandomBegin = randomBegin;
            currRandomNext = randomNext;
            currNextDelay = nextDelay;
            currClipIndex = currRandomBegin ? Random.Range(0,currClips.Length) : 0;
            DoPlay(delay,currClips[currClipIndex],true,false);
        }

        public static void Play(float delay,AudioClip clip,bool resume,bool loop)
        {
            currClips = null;
            DoPlay(delay,clip,resume,loop);
        }

        static void DoPlay(float delay,AudioClip clip,bool resume,bool loop)
        {
            if(player)
            {
                GameObject.Destroy(player.gameObject);
            }
            GameObject playerGo = new GameObject("MusicPlayer");
            MonoBehaviour.DontDestroyOnLoad(playerGo);
            player = playerGo.AddComponent<AudioSource>();
            player.playOnAwake = false;
            if(player.clip != clip)
            {
                player.Stop();
                player.clip = clip;
                if(currClips != null && clip != null)
                {
                    string timerId = "MusicPlayer_Timer";
                    BTimer.DestroyByTimerId(timerId);
                    BTimer.Invoke(clip.length + currNextDelay,PlayNextMusic,timerId);
                }
            }
            player.loop = loop;
            if(IsMusicOn)
            {
                if(resume)
                {
                    player.Stop();
                }
                if(player.clip != null && !player.isPlaying)
                {
                    player.PlayDelayed(delay);
                }
            }
            isPlayingWhenMusicOff = true;
        }

        public static void Stop()
        {
            if(player)
            {
                player.Stop();
            }
            isPlayingWhenMusicOff = false;
        }

        public static void Pause()
        {
            if(player && player.clip != null)
            {
                player.Pause();
            }
            isPlayingWhenMusicOff = false;
        }

        public static void Resume()
        {
            if(player && player.clip != null)
            {
                player.Play();
            }
            isPlayingWhenMusicOff = true;
        }

        static void PlayNextMusic()
        {
            if(currClips != null)
            {
                currClipIndex = currRandomNext ? Random.Range(0,currClips.Length) : currClipIndex + 1;
                if(currClipIndex > currClips.Length - 1)
                {
                    currClipIndex = 0;
                }
                DoPlay(0,currClips[currClipIndex],true,false);
            }
        }
    }
}