using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.Video;

namespace BToolkit {
    public class CloudVideoPlayer:MonoBehaviour {

        public VideoPlayer unityPlayer;
        public MediaPlayer avProPlayer;
        public bool isPlaying { private set; get; }
        bool canListenFrame;
        public const float DistanceFromCamera = 1.8f;

        void Awake() {
            unityPlayer.GetComponent<MeshRenderer>().enabled = false;
            avProPlayer.GetComponent<MeshRenderer>().enabled = false;
        }

        void Update() {
            if(canListenFrame) {
                bool hadVideoPlay = false;
                if(unityPlayer.gameObject.activeInHierarchy) {
                    hadVideoPlay = (unityPlayer.frame > 10);
                }
                if(avProPlayer.gameObject.activeInHierarchy) {
                    hadVideoPlay = (avProPlayer.Control.GetCurrentTimeMs() > 1000);
                }
                if(hadVideoPlay) {
                    CloudLoading.Show(false);
                    canListenFrame = false;
                    unityPlayer.GetComponent<MeshRenderer>().enabled = true;
                    avProPlayer.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }

        public void SetVideoAndPlay(string url,float leftRatio,float topRatio,float wRatio,float hRatio,Vector3 showScale,string alphaType) {
            if(!"mask".Equals(alphaType)) {
                avProPlayer.gameObject.SetActive(false);
                unityPlayer.gameObject.SetActive(true);
                SetTranform(unityPlayer.transform,leftRatio,topRatio,wRatio,hRatio,showScale);
                unityPlayer.source = VideoSource.Url;
                unityPlayer.url = url;
                unityPlayer.Play();
            } else {
                unityPlayer.gameObject.SetActive(false);
                avProPlayer.gameObject.SetActive(true);
                SetTranform(avProPlayer.transform,leftRatio,topRatio,wRatio,hRatio,showScale);
                avProPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL,url);
            }
            canListenFrame = true;
            isPlaying = true;
        }

        public void Pause() {
            if(unityPlayer.gameObject.activeInHierarchy) {
                unityPlayer.Pause();
            }
            if(avProPlayer.gameObject.activeInHierarchy && avProPlayer.Control != null) {
                avProPlayer.Control.Pause();
            }
            isPlaying = false;
        }

        public void Play() {
            if(unityPlayer.gameObject.activeInHierarchy) {
                unityPlayer.Play();
            }
            if(avProPlayer.gameObject.activeInHierarchy && avProPlayer.Control != null) {
                avProPlayer.Control.Play();
            }
            isPlaying = true;
        }

        public void Stop() {
            if(unityPlayer.gameObject.activeInHierarchy) {
                unityPlayer.Stop();
            }
            if(avProPlayer.gameObject.activeInHierarchy && avProPlayer.Control != null) {
                avProPlayer.Control.Stop();
            }
            isPlaying = false;
        }

        void SetTranform(Transform target,float leftRatio,float topRatio,float wRatio,float hRatio,Vector3 showScale) {
            Vector3 scale = target.localScale;
            scale = showScale * 1.01f;
            scale.x *= wRatio;
            scale.y *= hRatio;
            target.localScale = scale;
            Vector3 pos = target.localPosition;
            pos.x += target.localScale.x * leftRatio;
            pos.z -= target.localScale.y * topRatio;
            target.localPosition = pos;
        }
    }
}