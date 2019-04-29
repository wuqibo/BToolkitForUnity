using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BToolkit
{
    public class GetTextureFromCamera : MonoBehaviour
    {

        public delegate void DeleCaptureCallback(Texture2D texture);
        private DeleCaptureCallback CaptureCallback;
        private Camera targetCamera;
        private Vector2 getTextureSize;
        private bool canListenCapture;
        private static GetTextureFromCamera instance;
        public static GetTextureFromCamera Instance
        {
            get
            {
                if (!instance)
                {
                    GameObject go = new GameObject("GetTextureFromCamera");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<GetTextureFromCamera>();
                }
                return instance;
            }
        }

        void OnDestroy()
        {
            instance = null;
        }

        public void GetTexture(Camera targetCamera, Vector2 getTextureSize, DeleCaptureCallback CaptureCallback)
        {
            this.targetCamera = targetCamera;
            this.getTextureSize = getTextureSize;
            this.CaptureCallback = CaptureCallback;
            canListenCapture = true;

        }

        void Update()
        {
            if (canListenCapture)
            {
                StartCoroutine(CaptureCamera());
                canListenCapture = false;
            }
        }

        IEnumerator CaptureCamera()
        {
            yield return new WaitForEndOfFrame();
            Rect rect = new Rect(0f, 0f, getTextureSize.x, getTextureSize.y);
            RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
            targetCamera.targetTexture = rt;
            targetCamera.Render();
            RenderTexture.active = rt;
            Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            tex.ReadPixels(rect, 0, 0);
            tex.Apply();
            if (CaptureCallback != null)
            {
                CaptureCallback(tex);
            }
            //还原
            targetCamera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(rt);
        }
    }
}