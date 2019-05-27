using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

public class FileDownloader : MonoBehaviour
{

    public class ShowingFile
    {
        public enum Type
        {
            Video, Model
        }
        public Type type = Type.Video;
        public string LocalPath
        {
            get { return Application.persistentDataPath + "/" + fileName; }
        }
        public float DownloadProgress
        {
            get
            {
                if (request == null)
                {
                    return 0f;
                }
                return request.downloadProgress;
            }
        }
        bool HasLoaded
        {
            get { return File.Exists(LocalPath); }
        }
        Vector3 Scale
        {
            get
            {
                Vector3 scale = Vector3.one;
                if (type == Type.Video)
                {
                    string[] videoSizeArr = videoSize.Split('X');
                    float scaleX = float.Parse(videoSizeArr[0]);
                    float scaleY = float.Parse(videoSizeArr[1]);
                    if (scaleX > scaleY)
                    {
                        scale.y = scaleY / scaleX;
                    }
                    else
                    {
                        scale.x = scaleX / scaleY;
                    }
                }
                else if (type == Type.Model)
                {
                    scale = Vector3.one;
                }
                scale *= showPercent / 100f;
                return scale;
            }
        }
        float LeftRadto { get { return float.Parse(videoRect.Split('|')[0]); } }
        float TopRadto { get { return float.Parse(videoRect.Split('|')[1]); } }
        float WidthRadto { get { return float.Parse(videoRect.Split('|')[2]); } }
        float HeightRadto { get { return float.Parse(videoRect.Split('|')[3]); } }
        UnityWebRequest request;
        string url, fileName, alphaType;
        string videoRect, videoSize;
        float showPercent;
        public ShowingFile(string url, string videoRect, string videoSize, float showPercent, string alphaType)
        {
            this.url = url;
            this.videoRect = videoRect;
            this.videoSize = videoSize;
            this.showPercent = showPercent;
            this.alphaType = alphaType;
            string[] strArr = url.Split('/');
            fileName = strArr[strArr.Length - 1];
            strArr = url.Split('.');
            if ("u3d".Equals(strArr[strArr.Length - 1]))
            {
                type = Type.Model;
            }
            else
            {
                type = Type.Video;
            }
            if (HasLoaded)
            {
                Debug.Log("<color=yellow>CurrLoadedCallback</color>");
                FileDownloader.Instance.CurrLoadedCallback(type, LocalPath, LeftRadto, TopRadto, WidthRadto, HeightRadto, Scale, alphaType, true);
            }
            else
            {
                if (type == Type.Video)
                {
                    Debug.Log("<color=yellow>CurrLoadedCallback</color>");
                    FileDownloader.Instance.CurrLoadedCallback(type, url, LeftRadto, TopRadto, WidthRadto, HeightRadto, Scale, alphaType, false);
                }
                FileDownloader.Instance.StartCoroutine(_Load());
            }
        }
        IEnumerator _Load()
        {
            if (request == null)
            {
                request = UnityWebRequest.Get(url);
                request.chunkedTransfer = false;
                yield return request.SendWebRequest();
                if (request.isNetworkError)
                {
                    Debug.LogError(url + ":" + request.error);
                }
                else
                {
                    File.WriteAllBytes(LocalPath, request.downloadHandler.data);
                    Debug.Log("<color=yellow>CurrLoadedCallback</color>");
                    FileDownloader.Instance.CurrLoadedCallback(type, LocalPath, LeftRadto, TopRadto, WidthRadto, HeightRadto, Scale, alphaType, true);
                }
            }
        }
        public void TryCallback()
        {
            if (HasLoaded)
            {
                Debug.Log("<color=yellow>CurrLoadedCallback</color>");
                FileDownloader.Instance.CurrLoadedCallback(type, LocalPath, LeftRadto, TopRadto, WidthRadto, HeightRadto, Scale, alphaType, true);
            }
            else
            {
                if (type == Type.Video)
                {
                    Debug.Log("<color=yellow>CurrLoadedCallback</color>");
                    FileDownloader.Instance.CurrLoadedCallback(type, url, LeftRadto, TopRadto, WidthRadto, HeightRadto, Scale, alphaType, false);
                }
            }
        }
        public void ResetValues(float showPercent)
        {
            this.showPercent = showPercent;
        }
    }
    static FileDownloader instance;
    Dictionary<string, ShowingFile> videos = new Dictionary<string, ShowingFile>();
    public delegate void LoadedEvent(ShowingFile.Type type, string localPath, float leftRatio, float topRatio, float wRatio, float hRatio, Vector3 scale, string alphaType, bool readFromLocal);
    public LoadedEvent CurrLoadedCallback;
    public static FileDownloader Instance
    {
        get
        {
            if (!instance)
            {
                GameObject go = new GameObject("FileDownloader");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<FileDownloader>();
            }
            return instance;
        }
    }

    void OnDestroy()
    {
        instance = null;
    }

    public ShowingFile DownloadShowingFile(string fielURL, string videoRect, string videoSize, float showPercent, string alphaType, LoadedEvent LoadedCallback)
    {
        this.CurrLoadedCallback = LoadedCallback;
        ShowingFile showingFile = null;
        if (!videos.ContainsKey(fielURL))
        {
            showingFile = new ShowingFile(fielURL, videoRect, videoSize, showPercent, alphaType);
            videos.Add(fielURL, showingFile);
        }
        else
        {
            showingFile = videos[fielURL];
            showingFile.ResetValues(showPercent);
            showingFile.TryCallback();
        }
        return showingFile;
    }

}
