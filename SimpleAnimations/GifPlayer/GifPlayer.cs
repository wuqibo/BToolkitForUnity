using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BToolkit
{
    public enum GifPlayerState
    {
        PreProcessing,
        Loading,
        Stopped,
        Playing,
        Error,
        Disabled,
    }


    [AddComponentMenu("BToolkit/GIF Player")]
    [RequireComponent(typeof(RawImage))]
    public class GifPlayer : MonoBehaviour
    {

        #region Public Fields
        public bool Loop = true;
        public bool AutoPlay = true;
        public string filePath = "";
        public int width { get { return _gifDecoder == null ? 0 : _gifDecoder.GetFrameWidth(); } }
        public int height { get { return _gifDecoder == null ? 0 : _gifDecoder.GetFrameHeight(); } }
        public float timeScale = 1;
        public GifPlayerState state { get; set; }
        public RectTransform rectTransform { get { return transform as RectTransform; } }
        #endregion

        #region Internal fields
        Texture2D gifTexture;
        GifDecoder _gifDecoder; // The GIF decoder
        bool _hasFirstFrameBeenShown; // Has the first frame of the GIF already been shown
        float _secondsTillNextFrame; // Seconds till next frame
        List<GifDecoder.GifFrame> _cachedFrames; // Cache of all frames that have been decoded
        GifDecoder.GifFrame CurrentFrame { get; set; } // The current frame that is being displayed
        int CurrentFrameNumber { get; set; } // The current frame we are at

        Thread _decodeThread;
        readonly EventWaitHandle _wh = new AutoResetEvent(false);
        bool _threadIsCanceled;
        bool _frameIsReady;
        readonly object _locker = new object();
        float _editorPreviousUpdateTime; // Time of previous update in editor
        RawImage img;
        #endregion

        #region Unity Events
        void Awake()
        {
            if (state == GifPlayerState.PreProcessing || state == GifPlayerState.Disabled || state == GifPlayerState.Error) Init();
        }
        void OnEnable()
        {
            if (state == GifPlayerState.PreProcessing || state == GifPlayerState.Disabled || state == GifPlayerState.Error) Init();
        }
        void Update()
        {
            CheckFrameChange();
        }
        void OnApplicationQuit()
        {
            EndDecodeThread();
        }
        #endregion

        #region Public API
        public void Init(bool hideOnFinished = false)
        {
#if UNITY_WEBGL
        Debuger.LogWarning("Animated GIF Player: Threaded Decoder is not available in WebGL");
        reutrn;
#endif
            _cachedFrames = new List<GifDecoder.GifFrame>();
            _gifDecoder = new GifDecoder();
            CurrentFrameNumber = 0;
            _hasFirstFrameBeenShown = false;
            _frameIsReady = false;
            state = GifPlayerState.Disabled;
            StartDecodeThread();
            if (filePath.Length <= 0) return;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(Load(hideOnFinished));
            }
        }

        public void Play()
        {
            if (state != GifPlayerState.Stopped)
            {
                Debuger.LogWarning("Can't play GIF playback. State is: " + state);
                return;
            }
            state = GifPlayerState.Playing;
        }

        /// <summary>
        /// Pause playback.
        /// </summary>
        public void Pause()
        {
            if (state != GifPlayerState.Playing)
            {
                Debuger.LogWarning("Can't pause GIF is not playing. State is: " + state);
                return;
            }
            state = GifPlayerState.Stopped;
        }

        public int GetFramesCount()
        {
            return _gifDecoder == null ? 0 : _gifDecoder.GetFrameCount();
        }
        #endregion

        #region Methods
        IEnumerator Load(bool hideOnFinished)
        {
            if (filePath.Length == 0)
            {
                Debuger.LogWarning("File name not set");
                yield break;
            }
            state = GifPlayerState.Loading;
            string url;
            if (filePath.Substring(0, 4) == "http")
            {
                url = filePath;
            }
            else
            {
                url = BUtils.streamingAssetsPathForWebRequest + "/" + filePath;
            }
#if UNITY_2018
            using (var www = new UnityWebRequest(url))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    Debuger.LogWarning("File load error.\n" + www.error);
                    state = GifPlayerState.Error;
                }
                else
                {
                    lock (_locker)
                    {
                        if (_gifDecoder.Read(new System.IO.MemoryStream(www.downloadHandler.data)) == GifDecoder.Status.StatusOk)
                        {
                            state = GifPlayerState.PreProcessing;
                            CreateTargetTexture();
                            StartDecoder(hideOnFinished);
                        }
                        else
                        {
                            Debuger.LogWarning("Error loading gif");
                            state = GifPlayerState.Error;
                        }
                    }
                }
            }
#else
            using (var www = new WWW(url))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error) == false)
                {
                    Debuger.LogWarning("File load error.\n" + www.error);
                    state = GifPlayerState.Error;
                }
                else
                {
                    lock (_locker)
                    {
                        if (_gifDecoder.Read(new System.IO.MemoryStream(www.bytes)) == GifDecoder.Status.StatusOk)
                        {
                            state = GifPlayerState.PreProcessing;
                            CreateTargetTexture();
                            StartDecoder(hideOnFinished);
                        }
                        else
                        {
                            Debuger.LogWarning("Error loading gif");
                            state = GifPlayerState.Error;
                        }
                    }
                }
            }
#endif
        }

        void CreateTargetTexture()
        {
            if (gifTexture != null && _gifDecoder != null && gifTexture.width == _gifDecoder.GetFrameWidth() && gifTexture.height == _gifDecoder.GetFrameWidth()) return; // Target texture already set

            if (_gifDecoder == null || _gifDecoder.GetFrameWidth() == 0 || _gifDecoder.GetFrameWidth() == 0)
            {
                gifTexture = Texture2D.blackTexture;
                return;
            }

            gifTexture = new Texture2D(_gifDecoder.GetFrameWidth(), _gifDecoder.GetFrameHeight(), TextureFormat.RGBA32, false);
            gifTexture.hideFlags = HideFlags.HideAndDontSave;
        }

        void SetTexture()
        {
            if (!img)
            {
                img = GetComponent<RawImage>();
            }
            img.texture = gifTexture;
            if (gifTexture)
            {
                if(img.color!= Color.white)
                {
                    img.color = Color.white;
                }
            }
        }

        void AddFrameToCache(GifDecoder.GifFrame frame)
        {
            var copyOfImage = new byte[frame.Image.Length];
            Buffer.BlockCopy(frame.Image, 0, copyOfImage, 0, frame.Image.Length);
            frame.Image = copyOfImage;
            lock (_cachedFrames) _cachedFrames.Add(frame);
        }

        void StartDecoder(bool hideOnFinished)
        {
            _wh.Set();
            state = GifPlayerState.Stopped;
            //Gif准备完成
            if (hideOnFinished)
            {
                gameObject.SetActive(false);
                return;
            }
            if (AutoPlay) Play();
        }

        // Check if the next frame should be shown
        void UpdateFrameTime()
        {
            if (state != GifPlayerState.Playing) return; // Not playing

            if (!Application.isPlaying)
            {
                _secondsTillNextFrame -= (Time.realtimeSinceStartup - _editorPreviousUpdateTime) * timeScale;
                _editorPreviousUpdateTime = Time.realtimeSinceStartup;
                return;
            }

            // Calculate seconds till next gif frame
            _secondsTillNextFrame -= Time.deltaTime;
        }

        // Update the frame
        void UpdateFrame()
        {
            if (_gifDecoder.NumberOfFrames > 0 && _gifDecoder.NumberOfFrames == CurrentFrameNumber)
            {
                // Set frame number to 0 if we are at the last one
                CurrentFrameNumber = 0;
                if (!Loop)
                {
                    // Stop playback if not looping
                    Pause();
                    return;
                }
            }

            lock (_cachedFrames)
            {
                CurrentFrame = _cachedFrames.Count > CurrentFrameNumber
                    ? _cachedFrames[CurrentFrameNumber]
                    : _gifDecoder.GetCurrentFrame();
            }

            // Prepare the next frame 
            if (!_gifDecoder.AllFramesRead)
            {
                // Not all frames are read yet. Prepare the next frame
                StartReadFrame();
            }

            // Update the target texture with the new frame
            gifTexture.LoadRawTextureData(CurrentFrame.Image);
            gifTexture.Apply();

            // Set next frame time
            _secondsTillNextFrame = CurrentFrame.Delay;

            // Move to next frame
            CurrentFrameNumber++;
        }


        // Check if the frame needs to be updated
        void CheckFrameChange()
        {
            if (state != GifPlayerState.Playing && _hasFirstFrameBeenShown || !_frameIsReady) return;


            if (!_hasFirstFrameBeenShown)
            {
                // Show the first frame
                SetTexture();
                lock (_locker) UpdateFrame();
                _hasFirstFrameBeenShown = true;
                return;
            }

            UpdateFrameTime();

            if (_secondsTillNextFrame > 0) return;

            // Time to change the frame
            lock (_locker) UpdateFrame();
        }

        // Starts reading a frame
        void StartReadFrame()
        {
            _frameIsReady = false;
            _wh.Set();
        }

        // Start the decode thread
        void StartDecodeThread()
        {
            if (_decodeThread != null && _decodeThread.IsAlive) return;

            _threadIsCanceled = false;
            _decodeThread = new Thread(() => FrameDataThread(false, true));
            _decodeThread.Name = "gifDecoder" + _decodeThread.ManagedThreadId;
            _decodeThread.IsBackground = true;
            _decodeThread.Start();
        }

        // Ends the decode thread
        void EndDecodeThread()
        {
            if (_threadIsCanceled) return;
            _threadIsCanceled = true;
            _wh.Set();
        }

        void FrameDataThread(bool loopDecoder, bool readAllFrames)
        {
            _wh.WaitOne();
            while (!_threadIsCanceled)
            {
                lock (_locker)
                {
                    // Read the next frame
                    _gifDecoder.ReadNextFrame(loopDecoder);

                    if (_gifDecoder.AllFramesRead)
                    {
                        _frameIsReady = true;
                        break;
                    }

                    AddFrameToCache(_gifDecoder.GetCurrentFrame());

                    if (readAllFrames)
                    {
                        if (_gifDecoder.AllFramesRead)
                        {
                            _frameIsReady = true;
                            break;
                        }
                        continue;
                    }
                    _frameIsReady = true;
                }
                _wh.WaitOne(); // Wait for signal that frame must be read
            }
            _threadIsCanceled = true;
            _wh.Close();
        }
#endregion
    }
}