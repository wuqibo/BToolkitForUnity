using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace BToolkit
{
    public class ChangeCsvFileToUTF8
    {
        [MenuItem("BToolkit/Change To/TextFile To UTF-8")]
        static void Execute()
        {
            Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            if (objs.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请选择文件夹或贴图", "确定");
                return;
            }
            for (int i = 0; i < objs.Length; i++)
            {
                Object obj = objs[i];
                if (obj is DefaultAsset)
                {
                    //如果选择的是文件夹
                    string dirPath = AssetDatabase.GetAssetPath(obj);
                    foreach (string childPath in Directory.GetFiles(dirPath))
                    {
                        SetFileProperties(childPath);
                    }
                }
                else
                {
                    SetFileProperties(AssetDatabase.GetAssetPath(obj));
                }
            }
        }

        static void SetFileProperties(string unityPath)
        {
            string fullPath = GetFullPath(unityPath);
            string content = ReadFileToString(fullPath);
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            byte[] encodedBytes = utf8Encoding.GetBytes(content);
            string utf8String = utf8Encoding.GetString(encodedBytes);
            File.WriteAllText(fullPath, utf8String, Encoding.UTF8);
            Debug.Log(fullPath + " 转换完成");
        }

        static string GetFullPath(string unityPath)
        {
            unityPath = unityPath.Remove(0, 6);
            return Application.dataPath + unityPath;
        }

        static string ReadFileToString(string fullPath)
        {
            string readedStr = "";
            using (StreamReader streamReader = new StreamReader(fullPath, Encoding.Default))
            {
                readedStr = streamReader.ReadToEnd();
            }
            return readedStr;
        }
    }
}