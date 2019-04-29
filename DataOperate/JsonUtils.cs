using UnityEngine;
using System.IO;
using LitJson;

namespace BToolkit
{
    public class JsonUtils
    {

        /// <summary>
        /// 传入的类结构里的不能含有float型变量
        /// </summary>
        public static void Write(string path, object data, bool encrypt)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string str = JsonMapper.ToJson(data);
            using (StreamWriter sw = File.CreateText(path))
            {
                if (encrypt)
                {
                    sw.Write(AESUtils.Encrypt(str));
                }
                else
                {
                    sw.Write(str);
                }
            }
            Debug.Log("Saved to: " + path);
        }

        public static T Read<T>(string path, bool decrypt)
        {
            if (File.Exists(path))
            {
                string readedStr = "";
                using (StreamReader sr = File.OpenText(path))
                {
                    readedStr = sr.ReadToEnd();
                }
                if (!string.IsNullOrEmpty(readedStr))
                {
                    if (decrypt)
                    {
                        readedStr = AESUtils.Decrypt(readedStr);
                    }
                    try
                    {
                        return JsonMapper.ToObject<T>(readedStr);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
            return default(T);
        }

        public static JsonData Read(string path, bool decrypt)
        {
            if (File.Exists(path))
            {
                string readedStr = "";
                using (StreamReader sr = File.OpenText(path))
                {
                    readedStr = sr.ReadToEnd();
                }
                if (!string.IsNullOrEmpty(readedStr))
                {
                    if (decrypt)
                    {
                        readedStr = AESUtils.Decrypt(readedStr);
                    }
                    try
                    {
                        return JsonMapper.ToObject(readedStr);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
            return null;
        }

        public static T Parse<T>(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    return JsonMapper.ToObject<T>(str);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
            return default(T);
        }

        public static JsonData Parse(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    return JsonMapper.ToObject(str);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
            return null;
        }

        /// <summary>
        /// 传入的类结构里的不能含有float型变量
        /// </summary>
        public static string Parse(object data)
        {
            if (data != null)
            {
                try
                {
                    return JsonMapper.ToJson(data);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
            return null;
        }

        public static void WriteJsonStr(string path, string jsonStr, bool encrypt)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                if (encrypt)
                {
                    sw.Write(AESUtils.Encrypt(jsonStr));
                }
                else
                {
                    sw.Write(jsonStr);
                }
            }
        }

        public static string ReadJsonStr(string path, bool decrypt)
        {
            if (File.Exists(path))
            {
                string readedStr = "";
                using (StreamReader sr = File.OpenText(path))
                {
                    readedStr = sr.ReadToEnd();
                }
                if (!string.IsNullOrEmpty(readedStr))
                {
                    if (decrypt)
                    {
                        return AESUtils.Decrypt(readedStr);
                    }
                    return readedStr;
                }
            }
            return null;
        }
    }
}