using System.IO;
using UnityEngine;

namespace BToolkit
{
    public class Storage : MonoBehaviour
    {
        /// <summary>
        /// 保存字符串到文件
        /// </summary>
        public static void SaveStringToFile(string fullPath, string content)
        {
            string path = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamWriter streamWriter = File.CreateText(fullPath))
            {
                streamWriter.Write(content);
            }
        }

        /// <summary>
        /// 读取字符串文件
        /// </summary>
        public static string ReadFileToString(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                string readedStr = "";
                using (StreamReader streamReader = File.OpenText(fullPath))
                {
                    readedStr = streamReader.ReadToEnd();
                }
                return readedStr;
            }
            return null;
        }

        /// <summary>
        /// 保存base64到文件
        /// </summary>
        public static void SaveBase64ToFile(string base64, string fullPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(base64))
                {
                    byte[] bytes = System.Convert.FromBase64String(base64);
                    SaveBytesToFile(bytes, fullPath);
                }
            }
            catch { }
        }

        /// <summary>
        /// 保存byte[]到文件
        /// </summary>
        public static void SaveBytesToFile(byte[] bytes, string fullPath)
        {
            string path = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllBytes(fullPath, bytes);
        }
    }
}