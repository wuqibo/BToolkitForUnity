using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BToolkit
{
    public class DrawCamera : MonoBehaviour
    {

        public bool autoStart = true;
        public RawImage image;
        public float textureSizeHeight = 720;
        public int fps = 30;
        WebCamTexture cameraTexture;
        Texture imageDefaultTexture;
        Color imageDefaultColor = Color.white;
        Vector2 textureSize;
        RectTransform imageTrans;

        void Start()
        {
            //RawImage的尺寸
            CanvasScaler canvasScaler = GameObject.FindObjectOfType<CanvasScaler>();
            Vector2 screenUISize = Vector2.zero;
            if (canvasScaler.matchWidthOrHeight > 0.5f)
            {
                screenUISize = new Vector2(canvasScaler.referenceResolution.y * (float)Screen.width / (float)Screen.height, canvasScaler.referenceResolution.y);
            }
            else
            {
                screenUISize = new Vector2(canvasScaler.referenceResolution.x, canvasScaler.referenceResolution.x * (float)Screen.height / (float)Screen.width);
            }
            Vector2 imageSize = Vector2.zero;
            if (Screen.width > Screen.height)
            {
                //横屏
                imageSize.x = screenUISize.x;
                imageSize.y = screenUISize.x * 3f / 4f;
            }
            else
            {
                //竖屏
                imageSize.x = screenUISize.y * 4f / 3f;
                imageSize.y = screenUISize.y;
            }
            imageTrans = image.GetComponent<RectTransform>();
            imageTrans.sizeDelta = imageSize;
            //贴图分辨率
            textureSize.x = textureSizeHeight * 4f / 3f;
            textureSize.y = textureSizeHeight;
            //自动启动
            if (autoStart)
            {
                StartCamera();
            }
        }

        public void StartCamera()
        {
            if (!cameraTexture)
            {
                StartCoroutine(StartCameraIEnumerator());
            }
        }

        public void StopCamera(bool clearPhoto)
        {
            if (cameraTexture)
            {
                cameraTexture.Stop();
                cameraTexture = null;
                if (clearPhoto)
                {
                    image.texture = imageDefaultTexture;
                    image.color = imageDefaultColor;
                }
            }
        }

        public bool IsPlaying
        {
            get
            {
                return cameraTexture != null;
            }
        }

        IEnumerator StartCameraIEnumerator()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.LogError("摄影机权限被禁用");
            }
            else
            {
                WebCamDevice[] devices = WebCamTexture.devices;
                string cameraName = devices[0].name;
                cameraTexture = new WebCamTexture(cameraName, (int)textureSize.x, (int)textureSize.y, fps);
                cameraTexture.Play();
                imageDefaultTexture = image.texture;
                imageDefaultColor = image.color;
                image.color = Color.white;
                image.texture = cameraTexture;
            }
        }
    }
}