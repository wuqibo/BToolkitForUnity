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
    }
}