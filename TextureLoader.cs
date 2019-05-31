using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;

namespace BToolkit
{
    public class TextureLoader
    {
        public Texture texture { get; private set; }
        public RawImage image { get; private set; }
        UnityWebRequest requestLocal, requestServer;
        const string PhotoFolderName = "PhotoLoader";
        const string Suffix = ".jpg";
        public delegate void FinishEvent(bool success, int width, int height);
        public string savedFileName;
        public bool hasLoaded { get; private set; }
        public string LocalFullPath
        {
            get { return Application.persistentDataPath + "/" + PhotoFolderName + "/" + savedFileName + Suffix; }
        }

        public void Destroy()
        {
            if (requestLocal != null)
            {
                requestLocal.Dispose();
                requestLocal = null;
            }
            if (requestServer != null)
            {
                requestServer.Dispose();
                requestServer = null;
            }
            if (image)
            {
                if (image.texture = texture)
                {
                    image.texture = null;
                }
            }
            Resources.UnloadUnusedAssets();
            hasLoaded = false;
        }

        /// <summary>
        /// limitWidth或limitHeight哪个为零则不参与修改;
        /// <param>增加saveName参数，以支持同一图片保存两个不同尺寸的需求，savleName为null时，自动将url的MD5做saveName</param>
        /// </summary>
        public void LoadAndSetPhoto(RawImage image, string url, int limitWidth = 0, int limitHeight = 0, bool canSave = false, string saveName = null, FinishEvent OnFinishCallback = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(image.name + " : url为空");
                return;
            }
            savedFileName = string.IsNullOrEmpty(saveName) ? GetMD5_32(url) : saveName;
            if (IsLocalExist(savedFileName))
            {
                image.StartCoroutine(LoadFromLocal(image, savedFileName, OnFinishCallback));
            }
            else
            {
                image.StartCoroutine(LoadFromServer(image, url, limitWidth, limitHeight, canSave, savedFileName, OnFinishCallback));
            }
        }
        IEnumerator LoadFromLocal(RawImage image, string fileName, FinishEvent OnFinishCallback)
        {
            this.image = image;
            string url = "file://" + Application.persistentDataPath + "/" + PhotoFolderName + "/" + fileName + Suffix;
            //Debug.Log("<<<<<<<<<<<<<<<<<<<<<<" + url);
            requestLocal = UnityWebRequestTexture.GetTexture(url);
            yield return requestLocal.SendWebRequest();
            if (requestLocal.isNetworkError)
            {
                Debug.LogError(string.IsNullOrEmpty(url) ? "url为空" : requestLocal.error + ": " + url);
                if (OnFinishCallback != null)
                {
                    OnFinishCallback(false, 0, 0);
                }
                yield break;
            }
            Texture2D texture = (requestLocal.downloadHandler as DownloadHandlerTexture).texture;
            image.texture = this.texture = texture;
            hasLoaded = true;
            if (OnFinishCallback != null)
            {
                OnFinishCallback(true, texture.width, texture.height);
            }
            texture = null;
        }
        IEnumerator LoadFromServer(RawImage image, string url, int limitWidth, int limitHeight, bool canSave, string fileName, FinishEvent OnFinishCallback)
        {
            this.image = image;
            if (url.Contains("file:////"))
            {
                url = url.Replace("file:////", "file:///");
            }
            requestServer = UnityWebRequestTexture.GetTexture(url);
            yield return requestServer.SendWebRequest();
            if (requestServer.error != null)
            {
                Debug.LogError(string.IsNullOrEmpty(url) ? "url为空" : requestServer.error + ": " + url);
                if (OnFinishCallback != null)
                {
                    OnFinishCallback(false, 0, 0);
                }
                yield break;
            }
            Texture2D texture = (requestServer.downloadHandler as DownloadHandlerTexture).texture;
            if (texture)
            {
                int texW = texture.width;
                int texH = texture.height;
                if (limitWidth > 0 && limitHeight == 0)
                {
                    if (texW > limitWidth)
                    {
                        int newWidth = limitWidth;
                        int newHeight = (int)(newWidth * texH / (float)texW);
                        if (texW != newWidth || texH != newHeight)
                        {
                            texture = TextureUtils.ResizeTexture(texture, newWidth, newHeight);
                        }
                    }
                }
                else if (limitWidth == 0 && limitHeight > 0)
                {
                    if (texH > limitHeight)
                    {
                        int newHeight = limitHeight;
                        int newWidth = (int)(limitHeight * texW / (float)texH);
                        if (texW != newWidth || texH != newHeight)
                        {
                            texture = TextureUtils.ResizeTexture(texture, newWidth, newHeight);
                        }
                    }
                }
                else if (limitWidth > 0 && limitHeight > 0)
                {
                    texture = TextureUtils.CutTextureToSize(texture, limitWidth, limitHeight);
                }
                if (canSave)
                {
                    byte[] imgData = texture.EncodeToPNG();
                    if (!Directory.Exists(Application.persistentDataPath + "/" + PhotoFolderName))
                    {
                        Directory.CreateDirectory(Application.persistentDataPath + "/" + PhotoFolderName);
                    }
                    File.WriteAllBytes(Application.persistentDataPath + "/" + PhotoFolderName + "/" + fileName + Suffix, imgData);
                }
                image.texture = this.texture = texture;
                hasLoaded = true;
                if (OnFinishCallback != null)
                {
                    OnFinishCallback(true, texture.width, texture.height);
                }
                texture = null;
            }
        }

        bool IsLocalExist(string fileName)
        {
            return File.Exists(Application.persistentDataPath + "/" + PhotoFolderName + "/" + fileName + Suffix);
        }

        string GetMD5_32(string inputStr)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputStr);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}