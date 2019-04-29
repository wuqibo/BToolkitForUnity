using UnityEngine;
using System.Collections.Generic;
using System.IO;
using LitJson;

namespace BToolkit
{
    public class BPlayerPrefs
    {

        string fileName;
        string _filePath;
        string FilePath
        {
            get
            {
                if (_filePath == null)
                {
                    _filePath = Application.persistentDataPath + "/" + fileName;
                }
                return _filePath;
            }
        }

        public BPlayerPrefs(string fileName)
        {
            this.fileName = fileName;
        }

        #region All
        public Dictionary<string, object> GetAllDatas()
        {
            if (File.Exists(FilePath))
            {
                return Read<Dictionary<string, object>>(FilePath);
            }
            return null;
        }
        public Dictionary<string, string> GetAllStringDatas()
        {
            Dictionary<string, string> stringData = new Dictionary<string, string>();
            if (File.Exists(FilePath))
            {
                Dictionary<string, object> data = Read<Dictionary<string, object>>(FilePath);
                foreach (KeyValuePair<string, object> dic in data)
                {
                    if (dic.Value is string)
                    {
                        stringData.Add(dic.Key, (string)dic.Value);
                    }
                }
                data = null;
            }
            return stringData;
        }
        public Dictionary<string, int> GetAllIntDatas()
        {
            Dictionary<string, int> intData = new Dictionary<string, int>();
            if (File.Exists(FilePath))
            {
                Dictionary<string, object> data = Read<Dictionary<string, object>>(FilePath);
                foreach (KeyValuePair<string, object> dic in data)
                {
                    if (dic.Value is int)
                    {
                        intData.Add(dic.Key, (int)dic.Value);
                    }
                }
                data = null;
            }
            return intData;
        }
        public Dictionary<string, float> GetAllFloatDatas()
        {
            Dictionary<string, float> floatData = new Dictionary<string, float>();
            if (File.Exists(FilePath))
            {
                Dictionary<string, object> data = Read<Dictionary<string, object>>(FilePath);
                foreach (KeyValuePair<string, object> dic in data)
                {
                    if (dic.Value is float)
                    {
                        floatData.Add(dic.Key, (float)dic.Value);
                    }
                }
                data = null;
            }
            return floatData;
        }
        public Dictionary<string, bool> GetAllBoolDatas()
        {
            Dictionary<string, bool> boolData = new Dictionary<string, bool>();
            if (File.Exists(FilePath))
            {
                Dictionary<string, object> data = Read<Dictionary<string, object>>(FilePath);
                foreach (KeyValuePair<string, object> dic in data)
                {
                    if (dic.Value is bool)
                    {
                        boolData.Add(dic.Key, (bool)dic.Value);
                    }
                }
                data = null;
            }
            return boolData;
        }
        #endregion

        #region Bool
        public bool GetBool(string key)
        {
            return GetBool(key, false);
        }
        public bool GetBool(string key, bool defaultValue)
        {
            return (bool)GetObject(key, defaultValue);
        }
        public void SetBool(string key, bool value)
        {
            SetObject(key, value);
        }
        #endregion

        #region Float
        public float GetFloat(string key)
        {
            return GetFloat(key, 0);
        }
        public float GetFloat(string key, float defaultValue)
        {
            return (float)GetObject(key, defaultValue);
        }
        public void SetFloat(string key, float value)
        {
            SetObject(key, value);
        }
        #endregion

        #region Int
        public int GetInt(string key)
        {
            return GetInt(key, 0);
        }
        public int GetInt(string key, int defaultValue)
        {
            return (int)GetObject(key, defaultValue);
        }
        public void SetInt(string key, int value)
        {
            SetObject(key, value);
        }
        #endregion

        #region String
        public string GetString(string key)
        {
            return GetString(key, null);
        }
        public string GetString(string key, string defaultValue)
        {
            return (string)GetObject(key, defaultValue);
        }
        public void SetString(string key, string value)
        {
            SetObject(key, value);
        }
        #endregion

        #region Util
        public object GetObject(string key, object defaultValue)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (File.Exists(FilePath))
                {
                    Dictionary<string, object> data = Read<Dictionary<string, object>>(FilePath);
                    if (data.ContainsKey(key))
                    {
                        return data[key];
                    }
                }
            }
            return defaultValue;
        }
        public void SetObject(string key, object value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                Dictionary<string, object> data = null;
                if (!File.Exists(FilePath))
                {
                    data = new Dictionary<string, object>();
                }
                else
                {
                    data = Read<Dictionary<string, object>>(FilePath);
                }
                data[key] = value;
                Write(FilePath, data);
            }
        }
        T Read<T>(string path)
        {
            string readedStr = "";
            using (StreamReader sr = File.OpenText(path))
            {
                readedStr = sr.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(readedStr))
            {
                try
                {
                    return JsonMapper.ToObject<T>(readedStr);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
            return default(T);
        }
        void Write(string path, object data)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string str = JsonMapper.ToJson(data);
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(str);
            }
        }
        #endregion

    }
}