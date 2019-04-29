using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;

namespace BToolkit
{
    public class XMLUtils
    {

        public static void Write<T>(string path, T data)
        {
            FileStream fs = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                serializer.Serialize(fs, data);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        public static T Read<T>(string path)
        {
            FileStream fs = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                return (T)serializer.Deserialize(fs);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return default(T);
        }

        public static void Write(string path, string xmlStr, bool encrypt)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                if (encrypt)
                {
                    sw.Write(AESUtils.Encrypt(xmlStr));
                }
                else
                {
                    sw.Write(xmlStr);
                }
            }
        }

        public static string Read(string path, bool decrypt)
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

        public static string DictionaryToXML(Dictionary<string, string> dic)
        {
            string xmlStr = "<xml>";
            foreach (var item in dic)
            {
                xmlStr += "<" + item.Key + "><![CDATA[" + item.Value + "]]></" + item.Key + ">";
            }
            xmlStr += "</xml>";
            return xmlStr;
        }
    }
}