using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class CloudFileDownloader : MonoBehaviour
{
    private static List<string> loadingList = new List<string>();

    /// <summary>
    /// 换成本地路径，如果本地文件不存在则返回原http域名
    /// </summary>
    public static string ParsePath(string httpUrl)
    {
        string[] arr = httpUrl.Split('/');
        string fileName = arr[arr.Length - 1];
        string path = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(path))
        {
            if (path.Contains(".u3d"))
            {
                path = "file:///" + path;
            }
            return path;
        }
        return httpUrl;
    }

    /// <summary>
    /// 缓存
    /// </summary>
    public static void Save(string httpUrl)
    {
        if (loadingList.Contains(httpUrl))
        {
            return;
        }
        string[] arr = httpUrl.Split('/');
        string fileName = arr[arr.Length - 1];
        string localPath = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(localPath))
        {
            return;
        }
        loadingList.Add(httpUrl);
        GameObject go = new GameObject("FileDownloader");
        DontDestroyOnLoad(go);
        CloudFileDownloader instance = go.AddComponent<CloudFileDownloader>();
        instance.StartCoroutine(instance.Load(httpUrl, localPath));
    }

    IEnumerator Load(string httpUrl, string savePath)
    {
        UnityWebRequest request = UnityWebRequest.Get(httpUrl);
        request.chunkedTransfer = false;
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debuger.LogError("FileDownloader:" + request.error);
        }
        File.WriteAllBytes(savePath, request.downloadHandler.data);
        if (loadingList.Contains(httpUrl))
        {
            loadingList.Remove(httpUrl);
        }
        Destroy(gameObject);
    }

}
